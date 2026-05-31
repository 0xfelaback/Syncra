using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Syncra.Application.DTOs;

namespace Syncra.Api.Controllers;

[ApiController]
[Route("api")]
public class EntryController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEntryService _entryService;
    public EntryController(IPublishEndpoint publishEndpoint, IEntryService entryService)
    {
        _publishEndpoint = publishEndpoint;
        _entryService = entryService;
    }
    /// <summary>
    ///  Node submits batch of events for processing.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncEndpoint(SyncRequestDto requestDto, CancellationToken cancellationToken)
    {

        var events = await _entryService.SaveBatchRequests(requestDto.nodeId, requestDto.events, cancellationToken);
        var pubEvents = events.Select(item => _publishEndpoint.Publish(item, context =>
            {
                context.SetRoutingKey(item.aggregateId); // setup distributed lock! - important
            }, cancellationToken));
        Console.WriteLine($"Sendng events to queue, {requestDto.events}");

        await Task.WhenAll(pubEvents);
        return Ok("sent to exchange.");
    }

    /// <summary>
    ///  Retrieve current account state.
    /// </summary>
    [HttpGet("accounts/{accountId}")]
    public async Task<IActionResult> GetAccountState()
    {
        return Ok();
    }


    /// <summary>
    ///  Node polls for missed events (fallback if SignalR disconnected).
    /// </summary>
    [HttpGet("events")]
    public async Task<IActionResult> PollEvents(long since)
    {
        return Ok();
    }

    /// <summary>
    ///  Retrieve event history for an account.
    /// </summary>
    [HttpGet("accounts/{accountId}/history")]
    public async Task<IActionResult> RequestEventHistory()
    {
        return Ok();
    }
}
