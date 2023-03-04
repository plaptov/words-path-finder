using WordsPathFinder.Models;

namespace WordsPathFinder.Services;

public interface IPathFinderService
{
    Task<PathResponse?> TryFindWordsPath(string from, string to);
}
