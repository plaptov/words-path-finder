using System.Collections.Concurrent;
using WordsPathFinder.AStar;

namespace WordsPathFinder.Services;

public class GraphCache
{
    private static readonly ConcurrentDictionary<int, Task<WordsGraph>> _cache = new();
    private readonly IHttpClientFactory _factory;

    public GraphCache(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<WordsGraph> Get(int wordsLength) =>
        await _cache.GetOrAdd(wordsLength, DownloadAndBuild);

    private async Task<WordsGraph> DownloadAndBuild(int wordsLength)
    {
        var client = _factory.CreateClient(nameof(GraphCache));
        var data = await client.GetStringAsync($"{wordsLength}.txt");
        return WordsGraph.Generate(data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
    }
}
