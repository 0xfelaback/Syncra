using FluentValidation;
using Syncra.Application.DTOs;


public class SyncRequestDtoValidation : AbstractValidator<SyncRequestDto>
{
    public SyncRequestDtoValidation()
    {
        RuleFor(s => s.nodeId)
            .NotEmpty().NotNull()
            .WithMessage("Node ID is required.");

        RuleFor(s => s.events)
            .NotEmpty()
            .WithMessage("Events list cannot be empty.")
            .Must(events => events != null && events.Count > 0)
            .WithMessage("At least one event must be provided.");

        RuleFor(s => s.lastKnownServerSequence)
            .GreaterThan(0)
            .WithMessage("Last known server sequence must be non-negative and above 0.");

        RuleForEach(s => s.events)
            .SetValidator(new SyncEventRequestValidation());
    }
}

public class SyncEventRequestValidation : AbstractValidator<SyncEventRequest>
{
    public SyncEventRequestValidation()
    {
        RuleFor(e => e.event_id)
            .NotEmpty().NotNull()
            .WithMessage("Event ID is required.");

        RuleFor(e => e.nodeSequence)
            .GreaterThan(0)
            .WithMessage("Node sequence must be non-negative.");

        RuleFor(e => e.nodeTimestamp)
            .NotEmpty().NotNull()
            .WithMessage("Node timestamp is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Node timestamp cannot be in the future.");

        RuleFor(e => e.eventType)
            .IsInEnum()
            .WithMessage("Event type must be a valid enum value.");

        RuleFor(e => e.accountId)
            .NotEmpty().NotNull()
            .WithMessage("Account ID is required.");

        RuleFor(e => e.payload)
            .NotNull()
            .WithMessage("Payload is required.");

        RuleFor(e => e.payload.amount)
            .GreaterThan(0)
            .WithMessage("Payload amount must be greater than zero.");

        RuleFor(e => e.payload.reason)
            .NotEmpty().NotNull()
            .WithMessage("Payload reason is required.");

        RuleFor(e => e.payload.to_account_id)
            .NotEmpty().NotNull()
            .WithMessage("Payload to_account_id is required.");
    }
}