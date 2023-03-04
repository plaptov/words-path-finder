namespace WordsPathFinder.AStar;

/// <summary>
/// Класс поиска пути по графу
/// </summary>
public class PathFinder
{
    /// <summary>
    /// Найти кратчайший путь в графе между словами
    /// </summary>
    /// <param name="graph">Граф слов с переходами</param>
    /// <param name="start">Начальное слово</param>
    /// <param name="finish">Конечное слово</param>
    /// <returns>Путь, содержащий переходы от начального слова к конечному.
    /// Либо null, если путь не найден.</returns>
    public static Path? FindShortestPath(
        WordsGraph graph, Word start, Word finish)
    {
        // Если в графе нет начального слова, искать нечего
        if (!graph.ContainsKey(start))
            return null;
        // Создаём очередь обработки вариантов
        // Ключ - приоритет, значение - очередь путей
        // Т.о. нет необходимости каждый раз сортировать очередь по приоритету
        var paths = new Dictionary<int, Queue<Path>>();
        // Текущий рассматриваемый путь (начальное слово)
        var curPath = new Path(null, start, finish);
        // Добавляем путь в хранилище
        AddPathToStorage(paths, curPath);
        // Список использованных слов
        var usedWords = new HashSet<Word>
        {
            // Начальное слово мы уже использовали
            start,
        };
        // Минимальный приоритет - обрабатываем очередь с этим приоритетом
        int minPriority = curPath.Priority;
        // Пока есть варианты...
        while (curPath != null)
        {
            // Если хранилище опустело, решения нет
            if (paths.Count == 0)
                return null;
            // Берём путь из очереди с минимальным приоритетом
            var queue = paths[minPriority];
            curPath = queue.Dequeue();
            // Если последнее слово равно конечному - решение найдено!
            if (curPath.Last == finish)
                return curPath;
            // Если очередь с этим приоритетом опустела, удаляем её из хранилища
            if (queue.Count == 0)
                paths.Remove(minPriority);
            // Получаем список переходов из последнего слова пути
            var steps = graph.GetValueOrDefault(curPath.Last);
            if (steps != null)
                // Для каждого перехода...
                foreach (var word in steps)
                {
                    // Если слово уже использовано, пропускаем
                    // Такой путь будет длиннее, он нам не нужен
                    if (usedWords.Add(word))
                        // Добавляем в очередь путь с новым переходом
                        AddPathToStorage(paths, new Path(curPath, word, finish));
                }
            // Если в очереди что-то осталось, вычисляем лучший приоритет
            if (paths.Count > 0)
                minPriority = paths.Keys.Min();
        }
        return null;
    }
    /// <summary>
    /// Метод добавления пути в очередь
    /// </summary>
    /// <param name="storage">Хранилище очереди</param>
    /// <param name="path">Путь для добавления</param>
    private static void AddPathToStorage(Dictionary<int, Queue<Path>> storage, Path path)
    {
        // Если в хранилище нет такого приоритета, добавляем его с пустой очередью
        if (!storage.TryGetValue(path.Priority, out var queue))
        {
            queue = new Queue<Path>();
            storage.Add(path.Priority, queue);
        }
        // Добавляем путь в очередь в рамках приоритета
        queue.Enqueue(path);
    }
}
