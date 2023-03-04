namespace WordsPathFinder.AStar;

/// <summary>
/// Граф слов с возможными переходами
/// </summary>
public class WordsGraph : Dictionary<Word, List<Word>>
{
    /// <summary>
    /// Приватный конструктор для использования извне только Generate
    /// </summary>
    private WordsGraph(int capacity) : base(capacity) { }
    /// <summary>
    /// Сгенерировать граф слов
    /// </summary>
    /// <param name="dict">Словарь слов</param>
    /// <returns>Словарь слово-возможные слова для перехода</returns>
    public static WordsGraph Generate(string[] dict)
    {
        // Сразу берём кол-во элементов максимально возможное, чтобы не тратить ресурсы на расширение
        WordsGraph gr = new(dict.Length);
        // Отбираем только строки нужной длины
        // Создаём список узлов графа для всех слов
        var words = Array.ConvertAll(dict, s => new Word(s));
        Parallel.For(0, words.Length, i =>
            {
                // Берём слово из списка
                var w = words[i];
                // Находим все слова, отличающиеся только на одну букву
                // Ищем только среди слов после текущего,т.к. см. ниже
                var steps = words[(i + 1)..].Where(w.IsOnlyOneLetterDiffWith).ToList();
                // Потоки со списком работают по очереди
                lock (gr)
                {
                    // Добавляем слово и его переходы в граф
                    if (gr.TryGetValue(w, out var list))
                        list.AddRange(steps);
                    else
                        gr.Add(w, steps);
                    // Т.к. граф неориентированный, для переходов сразу добавляем 
                    // текущее слово как возможный переход (обратный)
                    foreach (var w2 in steps)
                    {
                        if (gr.TryGetValue(w2, out list))
                            list.Add(w);
                        else
                            gr.Add(w2, new List<Word>() { w });
                    }
                }
            });
        return gr;
    }
}
