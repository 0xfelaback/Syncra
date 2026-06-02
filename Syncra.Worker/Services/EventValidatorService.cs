using Syncra.Application.Interfaces;
using Syncra.Worker;

public class EventValidatorService : IEventValidatorService
{
    private readonly IAccountSnapshotRepository _accountSnapshotRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly INodeStateRepository _nodeStateRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<Worker> _logger;
    public EventValidatorService(
        IAccountSnapshotRepository accountSnapshotRepository,
        IAccountRepository accountRepository,
        INodeStateRepository nodeStateRepository,
        IEventRepository eventRepository,
        ILogger<Worker> logger)
    {

        _accountSnapshotRepository = accountSnapshotRepository;
        _accountRepository = accountRepository;
        _nodeStateRepository = nodeStateRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }
    public async Task<(decimal snapBalance, long snapSequence)?> LoadStartState(string accountId, long serverSequence)
    {
        AccountSnapshot? lastSnapshot = await _accountSnapshotRepository.GetByLastByAccountIdAsync(accountId);
        if (lastSnapshot is null || serverSequence <= 0)
        {
            return null;
        }
        return (lastSnapshot.balance, lastSnapshot.snapshot_sequence);
    }
    public async Task<List<Event>?> FetchEventstoReplay(long lastServerSequence, long newServerSequence, string accountId) => await _eventRepository.GetEventsSinceLastSnap(lastServerSequence, newServerSequence, accountId);
    public decimal ReplayEachEventinOrder(decimal current_balance, List<Event> events)
    {
        foreach (var even in events)
        {
            _logger.LogInformation($"Applying event {even.event_id}: {even.Type} {even.payload.amount}");
            switch (even.Type)
            {
                case Event.EventType.AccountDebited:
                    current_balance -= even.payload.amount;
                    _logger.LogInformation($"Balance: {current_balance}");
                    break;
                case Event.EventType.AccountCredited:
                    current_balance += even.payload.amount;
                    _logger.LogInformation($"Balance: {current_balance}");
                    break;
                case Event.EventType.CompensatingTransaction:
                    // reverse the original operation
                    if (even.compensates_event != null)
                    {
                        switch (even.compensates_event.Type)
                        {
                            case Event.EventType.AccountDebited:
                                current_balance += even.payload.amount; // Add back
                                _logger.LogInformation($"Compensating Debit: Balance: {current_balance}");
                                break;
                            case Event.EventType.AccountCredited:
                                current_balance -= even.payload.amount; // Remove
                                _logger.LogInformation($"Compensating Credit: Balance: {current_balance}");
                                break;
                            default:
                                string errorMsg2 = $"Unknown event type during replaying compensating event at event sourcing: {even.compensates_event.Type}";
                                _logger.LogError(errorMsg2);
                                throw new ArgumentOutOfRangeException(errorMsg2); // TODO: Resolve this
                        }
                    }
                    break;
                case Event.EventType.RejectedTransaction:
                    _logger.LogInformation($"Rejected transaction: Balance unchanged at {current_balance}");
                    break;
                default:
                    string errorMsg = $"Unknown event type during replaying events at event sourcing: {even.Type}";
                    _logger.LogError(errorMsg);
                    throw new ArgumentOutOfRangeException(errorMsg); // TODO: Resolve this
            }
        }
        return current_balance;
    }
    public ApplyEventValidationResult TestApplyNewEvent(Event new_even, decimal test_balance, long snapSequence, int events_replayed)
    {
        // test on event initiator's account
        decimal pre_event_balance = test_balance;
        switch (new_even.Type)
        {
            case Event.EventType.AccountDebited:
                test_balance -= new_even.payload.amount;
                _logger.LogInformation($"If we apply new event, balance would be: {test_balance}");
                break;
            case Event.EventType.AccountCredited:
                test_balance += new_even.payload.amount;
                _logger.LogInformation($"If we apply new event, balance would be: {test_balance}");
                break;
            case Event.EventType.CompensatingTransaction:
                if (new_even.compensates_event != null)
                {
                    switch (new_even.compensates_event.Type)
                    {
                        case Event.EventType.AccountDebited:
                            test_balance += new_even.payload.amount;
                            _logger.LogInformation($"If we apply new compensating event, balance would be: {test_balance}");
                            break;
                        case Event.EventType.AccountCredited:
                            test_balance -= new_even.payload.amount;
                            _logger.LogInformation($"If we apply new compensating event, balance would be: {test_balance}");
                            break;
                        default:
                            string errorMsg2 = $"Unknown event type during replaying compensating event at event sourcing: {new_even.compensates_event.Type}";
                            _logger.LogError(errorMsg2);
                            return new ApplyEventValidationResult(false, $"Unknown event type during replaying compensating event at event sourcing: {new_even.compensates_event.Type}", pre_event_balance, test_balance, new_even.Type, snapSequence, events_replayed);
                    }
                }
                break;
            case Event.EventType.RejectedTransaction:
                _logger.LogInformation($"Rejected transaction: After applying new event balance will be unchanged at {test_balance}");
                break;
            default:
                string errorMsg = $"Unknown event type during applying new event (test): {new_even.Type}";
                _logger.LogError(errorMsg);
                return new ApplyEventValidationResult(false, $"Unknown event type during applying new event (test): {new_even.Type}", pre_event_balance, test_balance, new_even.Type, snapSequence, events_replayed);
        }

        // check constraints
        if (test_balance < 0)
        {
            return new ApplyEventValidationResult
            (false, $"InsufficientFunds (Debit would make balance negative); new Balance fell below 0: {test_balance}.", pre_event_balance, test_balance, new_even.Type, snapSequence, events_replayed
            );
        }
        if (new_even.payload.amount <= 0)
        {
            return new ApplyEventValidationResult(false, $"InvalidAmount (Event amount is negative or zero) - event payload amount is invalid: {new_even.payload.amount}", pre_event_balance, test_balance, new_even.Type, snapSequence, events_replayed);
        }

        return new ApplyEventValidationResult(true, "Validations passed", pre_event_balance, test_balance, new_even.Type, snapSequence, events_replayed);
    }

    public async Task<ApplyEventValidationResult> AccountChecks(Event new_even, string nodeId)
    {

        var account = await _accountRepository.GetByIdAsync(new_even.aggregateId);
        if (account is null)
        {
            _logger.LogInformation($"Account {new_even.aggregateId} not found. Creating placeholder account and state.");
            account = new Account
            {
                account_id = new_even.aggregateId,
                userId = 0,
                user = null!,
                account_state = new AccountState
                {
                    account_id = new_even.aggregateId,
                    balance = 0m,
                    provisional_balance = 0m,
                    version = 0,
                    last_event_id = null,
                    last_event = null!,
                    updated_at = DateTime.UtcNow,
                    account = null!
                }
            };
            account.account_state.account = account;

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();
        }

        if (account.account_state is null)
        {
            _logger.LogInformation($"Account {new_even.aggregateId} exists but has no state. Initializing state.");
            var accountState = new AccountState
            {
                account_id = new_even.aggregateId,
                balance = 0m,
                provisional_balance = 0m,
                version = 0,
                last_event_id = null,
                last_event = null!,
                updated_at = DateTime.UtcNow,
                account = account
            };
            account.account_state = accountState;
            await _accountRepository.UpdateAsync(account);
            await _accountRepository.SaveChangesAsync();
        }

        // TODO: Account frozen check is not yet backed by a dedicated frozen flag in the current model.
        // Once a frozen state exists on Account / AccountState, add that check here.

        var nodeState = await _nodeStateRepository.GetByIdAsync(nodeId);
        if (nodeState is null)
        {
            return new ApplyEventValidationResult(false, $"NodeUnauthorized (Unknown node '{nodeId}')", 0m, 0m, new_even.Type, 0, 0);
        }

        if (!string.Equals(nodeState.status, "active", StringComparison.OrdinalIgnoreCase))
        {
            return new ApplyEventValidationResult(false, $"NodeUnauthorized (Node '{nodeId}' is not active)", 0m, 0m, new_even.Type, 0, 0);
        }

        return new ApplyEventValidationResult(true, "Account checks passed", 0m, 0m, new_even.Type, 0, 0);
    }

}
