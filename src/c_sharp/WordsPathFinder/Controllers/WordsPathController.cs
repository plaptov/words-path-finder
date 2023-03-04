using Microsoft.AspNetCore.Mvc;
using WordsPathFinder.Models;
using WordsPathFinder.Services;

namespace WordsPathFinder.Controllers;

[ApiController]
[Route("api/wordsPath")]
public class WordsPathController : ControllerBase
{
    private readonly IPathFinderService _pathFinderService;
    private readonly ILogger<WordsPathController> _logger;

    public WordsPathController(IPathFinderService pathFinderService, ILogger<WordsPathController> logger)
    {
        _pathFinderService = pathFinderService;
        _logger = logger;
    }

    [HttpGet()]
    [ProducesResponseType(typeof(PathResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] string from, [FromQuery] string to)
    {
        _logger.LogInformation("Request from {From} to {To}", from, to);

        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
        {
            return BadRequest("'from' and 'to' query parameters are required");
        }
        if (from.Length != to.Length)
        {
            return BadRequest("'from' and 'to' query parameters must be of same length");
        }
        
        var path = await _pathFinderService.TryFindWordsPath(from, to);
        if (path == null)
        {
            return NotFound();
        }
        return Ok(path);
    }
}
