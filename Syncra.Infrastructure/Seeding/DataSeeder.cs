using Bogus;

public class DataSeeder
{
    private static readonly Random random = new();
    private static string GetAlphaLabel(int index)
    {
        var label = string.Empty;
        while (true)
        {
            label = (char)('a' + (index % 26)) + label;
            index = index / 26 - 1;
            if (index < 0)
            {
                break;
            }
        }
        return label;
    }

    public static Dictionary<string, List<object>> GenerateMockRecords(int accountCount = 10000)
    {
        var userCount = Math.Min(1000, Math.Max(1, accountCount / 10));
        var nodeCount = 20;
        var eventCount = Math.Max(accountCount * 2, 1000);
        var snapshotCount = Math.Max(accountCount / 10, 1);
        var conflictCount = Math.Max(accountCount / 20, 1);
        var archiveCount = Math.Max(eventCount / 10, 1);

        var userIds = Enumerable.Range(1, userCount).ToArray();
        var accountIds = Enumerable.Range(1, accountCount).Select(i => $"acc-{i}").ToArray();
        var nodeIds = Enumerable.Range(0, nodeCount).Select(i => $"node-{GetAlphaLabel(i)}").ToArray();

        var userFaker = new Faker<User>()
            .RuleFor(u => u.userId, f => f.IndexFaker + 1)
            .RuleFor(u => u.userName, f => f.Internet.UserName())
            .RuleFor(u => u.passwordhash, f => f.Internet.Password(12, false));
        var users = userFaker.Generate(userCount);

        var accountFaker = new Faker<Account>()
            .RuleFor(a => a.account_id, f => accountIds[f.IndexFaker])
            .RuleFor(a => a.userId, f => f.Random.ArrayElement(userIds))
            .RuleFor(a => a.user, _ => null!)
            .RuleFor(a => a.account_state, _ => null!);
        var accounts = accountFaker.Generate(accountCount);

        var nodeFaker = new Faker<NodeState>()
        .RuleFor(n => n.node_id, f => nodeIds[f.IndexFaker])
        .RuleFor(n => n.status, _ => "active")
        .RuleFor(n => n.local_sequence, f => f.Random.Int(0, 5000))
        .RuleFor(n => n.last_synced_server_sequence, f => f.Random.Long(0, 100000))
        .RuleFor(n => n.pending_events_count, f => f.Random.Int(0, 100))
            .RuleFor(n => n.last_sync_attempt, f => f.Date.Recent(10).ToUniversalTime())
            .RuleFor(n => n.last_successful_sync, f => f.Date.Recent(10).ToUniversalTime())
            .RuleFor(n => n.last_seen, f => f.Date.Recent(1).ToUniversalTime());
        var nodes = nodeFaker.Generate(nodeCount);

        var lorem = new Faker();
        var eventTypes = Enum.GetValues(typeof(Event.EventType)).Cast<Event.EventType>().ToArray();
        var eventStatuses = Enum.GetValues(typeof(Event.EventStatus)).Cast<Event.EventStatus>().ToArray();

        var nodeSequenceCounters = nodeIds.ToDictionary(id => id, _ => 0);
        var events = new List<Event>(eventCount);
        for (var i = 0; i < eventCount; i++)
        {
            var fromAccount = accountIds[random.Next(accountIds.Length)];
            var toAccount = accountIds[random.Next(accountIds.Length)];
            if (toAccount == fromAccount)
            {
                toAccount = accountIds[(random.Next(accountCount - 1) + 1) % accountCount];
            }

            var nodeId = nodeIds[random.Next(nodeIds.Length)];
            var nodeSequence = nodeSequenceCounters[nodeId] + 1;
            nodeSequenceCounters[nodeId] = nodeSequence;
            var idTimestamp = "17155" + random.Next(0, 100000).ToString("D5");

            events.Add(new Event
            {
                event_id = $"event:{nodeId}_{idTimestamp}_{nodeSequence:D3}",
                node_id = nodeId,
                node = null!,
                node_sequence = nodeSequence,
                server_sequence = i + 1L,
                node_timestamp = DateTime.UtcNow.AddSeconds(-random.Next(0, 86400)),
                server_timestamp = DateTime.UtcNow.AddSeconds(-random.Next(0, 86400)),
                Type = eventTypes[random.Next(eventTypes.Length)],
                Status = eventStatuses[random.Next(eventStatuses.Length)],
                payload = new Event.EventPayloadData
                {
                    amount = Math.Round((decimal)random.NextDouble() * 2000m + 1m, 2),
                    reason = lorem.Lorem.Sentence(3),
                    from_account_id = fromAccount,
                    from_account = null!,
                    to_account_id = toAccount,
                    to_account = null!
                },
                created_at = DateTime.UtcNow,
                parent_event_id = null,
                compensates_event_id = null,
                caused_conflict_id = null,
                compensates_conflict_id = null
            });
        }

        var archiveTypes = Enum.GetValues(typeof(EventArchive.EventArchiveType)).Cast<EventArchive.EventArchiveType>().ToArray();
        var archiveStatuses = Enum.GetValues(typeof(EventArchive.EventArchiveStatus)).Cast<EventArchive.EventArchiveStatus>().ToArray();

        var eventArchives = new List<EventArchive>(archiveCount);
        for (var i = 0; i < archiveCount; i++)
        {
            var fromAccount = accountIds[random.Next(accountIds.Length)];
            var toAccount = accountIds[random.Next(accountIds.Length)];
            if (toAccount == fromAccount)
            {
                toAccount = accountIds[(random.Next(accountCount - 1) + 1) % accountCount];
            }

            eventArchives.Add(new EventArchive
            {
                event_id = $"arch-{i:D8}",
                node_id = nodeIds[random.Next(nodeIds.Length)],
                node = null!,
                node_sequence = random.Next(1, 100000),
                server_sequence = i + 1L,
                node_timestamp = DateTime.UtcNow.AddSeconds(-random.Next(86400, 172800)),
                server_timestamp = DateTime.UtcNow.AddSeconds(-random.Next(86400, 172800)),
                Type = archiveTypes[random.Next(archiveTypes.Length)],
                Status = archiveStatuses[random.Next(archiveStatuses.Length)],
                payload = new EventArchive.EventArchivePayloadData
                {
                    amount = Math.Round((decimal)random.NextDouble() * 3000m + 1m, 2),
                    reason = lorem.Lorem.Sentence(3),
                    from_account_id = fromAccount,
                    from_account = null!,
                    to_account_id = toAccount,
                    to_account = null!
                },
                created_at = DateTime.UtcNow.AddDays(-random.Next(30, 90)),
                caused_conflict_id = null,
                compensates_conflict_id = null,
                parent_event_id = null,
                compensates_event_id = null
            });
        }

        var accountStates = accountIds.Select((id, idx) => new AccountState
        {
            account_id = id,
            account = null!,
            balance = Math.Round((decimal)random.NextDouble() * 25000m - 5000m, 2),
            provisional_balance = Math.Round((decimal)random.NextDouble() * 30000m - 5000m, 2),
            version = random.Next(1, 100),
            last_event_id = idx == 0 ? null : events[idx - 1].event_id,
            last_event = null!,
            last_server_sequence = random.Next(1, eventCount + 1),
            updated_at = DateTime.UtcNow.AddMinutes(-random.Next(0, 1440))
        }).ToList();

        var snapshots = Enumerable.Range(1, snapshotCount).Select(i => new AccountSnapshot
        {
            snapshot_id = i,
            account_id = accountIds[random.Next(accountIds.Length)],
            account = null!,
            snapshot_sequence = random.Next(1, eventCount + 1),
            balance = Math.Round((decimal)random.NextDouble() * 25000m - 5000m, 2),
            version = random.Next(1, 100),
            event_count = random.Next(1, 1000),
            created_at = DateTime.UtcNow.AddDays(-random.Next(0, 30))
        }).ToList();

        // Ensure unique event/archive references for conflicts to satisfy unique indexes
        var shuffledEvents = events.OrderBy(_ => random.Next()).ToList();
        var shuffledArchives = eventArchives.OrderBy(_ => random.Next()).ToList();
        // If there are fewer events/archives than needed, we'll cycle the shuffled lists

        var conflicts = new List<Conflict>(conflictCount);
        for (var i = 0; i < conflictCount; i++)
        {
            var originalEvent = shuffledEvents[(2 * i) % shuffledEvents.Count];
            var compensationEvent = shuffledEvents[(2 * i + 1) % shuffledEvents.Count];
            var originalArchive = shuffledArchives[(2 * i) % shuffledArchives.Count];
            var compensationArchive = shuffledArchives[(2 * i + 1) % shuffledArchives.Count];

            conflicts.Add(new Conflict
            {
                conflict_id = i + 1,
                detected_at = DateTime.UtcNow.AddSeconds(-random.Next(0, 86400)),
                account_id = accountIds[random.Next(accountIds.Length)],
                account = null!,
                original_event_id = originalEvent.event_id,
                original_event = null!,
                compensation_event_id = compensationEvent.event_id,
                compensation_event = null!,
                original_event_archive_id = originalArchive.event_id,
                original_event_archive = null!,
                compensation_event_archive_id = compensationArchive.event_id,
                compensation_event_archive = null!,
                Type = Conflict.ConflictType.InsufficientFunds,
                atempted_balance = Math.Round((decimal)random.NextDouble() * 5000m, 2),
                actual_balance = Math.Round((decimal)random.NextDouble() * 5000m, 2),
                resolution = Conflict.Resolution.compensate
            });
        }

        return new Dictionary<string, List<object>>
        {
            ["users"] = users.Cast<object>().ToList(),
            ["nodes"] = nodes.Cast<object>().ToList(),
            ["accounts"] = accounts.Cast<object>().ToList(),
            ["accountStates"] = accountStates.Cast<object>().ToList(),
            ["snapshots"] = snapshots.Cast<object>().ToList(),
            ["events"] = events.Cast<object>().ToList(),
            ["eventArchives"] = eventArchives.Cast<object>().ToList(),
            ["conflicts"] = conflicts.Cast<object>().ToList()
        };
    }
}
