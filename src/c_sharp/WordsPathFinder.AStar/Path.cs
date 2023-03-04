namespace WordsPathFinder.AStar;

/// <summary>
/// Путь в графе
/// </summary>
public class Path
{
    // Список слов в пути
    private readonly Word[] words;

    /// <summary>
    /// Создать экземпляр пути в графе.
    /// </summary>
    /// <param name="prevPath">Предыдущий путь (может быть null)</param>
    /// <param name="newWord">Новое слово, добавляемое к предыдущему пути</param>
    /// <param name="finish">Конечное слово для вычисления приоритета</param>
    public Path(Path? prevPath, Word newWord, Word finish)
    {
        if (prevPath != null)
        {
            // Если предыдущий путь задан, копируем слова из него и добавляем новое
            words = new Word[prevPath.Length + 1];
            Array.Copy(prevPath.words, words, prevPath.Length);
        }
        else
            // Иначе создаём список из одного слова
            words = new Word[1];
        // Записываем новое слово в конец пути
        words[^1] = newWord;
        // Приоритет равен пройденному расстоянию и 
        // оценочному оставшемуся расстоянию до конечного слова
        // Т.о. при сортировке по возрастанию в начале всегда лучший элемент
        Priority = newWord.DistanceTo(finish) + words.Length;
    }
    public IEnumerable<string> Words => words.Select(w => w.Text);

    /// <summary>
    /// Длина пройденного пути
    /// </summary>
    public int Length => words.Length;
    /// <summary>
    /// Последнее слово в пути
    /// </summary>
    public Word Last => words.Last();
    /// <summary>
    /// Приоритет пути
    /// </summary>
    public int Priority { get; }
    /// <summary>
    /// Строковое представление пути
    /// </summary>
    /// <returns>Список слов через перевод строки</returns>
    public override string ToString() => string.Join('\n', words);
}
