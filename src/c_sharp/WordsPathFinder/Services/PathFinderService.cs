using Microsoft.EntityFrameworkCore;
using WordsPathFinder.AStar;
using WordsPathFinder.DAL;
using WordsPathFinder.Models;

namespace WordsPathFinder.Services;

public class PathFinderService : IPathFinderService
{
    private readonly WordsPathFinderContext _context;
    private readonly GraphCache _graphCache;

    public PathFinderService(WordsPathFinderContext context, GraphCache graphCache)
    {
        _context = context;
        _graphCache = graphCache;
    }

    public async Task<PathResponse?> TryFindWordsPath(string from, string to)
    {
        var savedPath = await _context.WordsPaths
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.From == from && x.To == to);
        if (savedPath != null)
            return new PathResponse
            {
                Length = savedPath.Steps.Count,
                Steps = savedPath.Steps.ToArray(),
            };

        var graph = await _graphCache.Get(from.Length);
        var path = PathFinder.FindShortestPath(graph, new Word(from), new Word(to));
        if (path == null)
            return null;

        _context.WordsPaths.Add(new WordsPathModel
        {
            From = from,
            To = to,
            Steps = path.Words.ToList(),
        });
        await _context.SaveChangesAsync();
        
        return new PathResponse
        {
            Length = path.Length,
            Steps = path.Words.ToArray(),
        };
    }
}
