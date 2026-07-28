using Gamestore.BLL.DTOs.Ban;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("comments")]
public class CommentController(ICommentService commentService) : ControllerBase
{
    [HttpGet("ban/durations")]
    public IActionResult GetBanDurations()
    {
        var durations = commentService.GetBanDurations();
        return Ok(durations);
    }

    [HttpPost("ban")]
    public async Task<IActionResult> BanUser([FromBody] BanRequestDto request)
    {
        await commentService.BanUserAsync(request);
        return Ok();
    }
}