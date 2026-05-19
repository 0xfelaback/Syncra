using Microsoft.AspNetCore.Mvc;

namespace Syncra.Api.Controllers;

[ApiController]
[Route("api")]
public class EntryController : ControllerBase
{
    /// <summary>
    ///  Node submits batch of events for processing.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncEndpoint(SyncRequestDto requestDto)
    {
        return Ok();
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
