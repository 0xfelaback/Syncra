using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Syncra.Application.DTOs;

namespace Syncra.Api.Controllers;

[ApiController]
[Route("api")]
public class EntryController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    public EntryController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    /// <summary>
    ///  Node submits batch of events for processing.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncEndpoint(SyncRequestDto requestDto, CancellationToken cancellationToken)
    {
        var pubEvents = requestDto.events.Select(item => _publishEndpoint.Publish(item, context =>
            {
                context.SetRoutingKey(item.accountId); // set up binding - auto fanout
            }, cancellationToken));
        System.Console.WriteLine($"Sendng events to queue, {requestDto.events}");

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
