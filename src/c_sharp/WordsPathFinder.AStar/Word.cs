namespace WordsPathFinder.AStar;

/// <summary>
/// Класс, описывающий слово (узел) в графе
/// </summary>
public record struct Word(string Text)
{
    /// <summary>
    /// Рассчитать расстояние между словами.
    /// Расстояние равно кол-ву различающихся букв.
    /// </summary>
    public int DistanceTo(Word word)
    {
        int dist = 0;
        for (int i = 0; i < Text.Length; i++)
        {
            if (Text[i] != word.Text[i])
                dist++;
        }
        return dist;
    }

    /// <summary>
    /// Определяет, разница между словами только в одну букву или нет.
    /// </summary>
    /// <param name="word">Другое слово</param>
    public bool IsOnlyOneLetterDiffWith(Word word)
    {
        int dist = 0;
        for (int i = 0; i < Text.Length; i++)
        {
            if (Text[i] != word.Text[i])
                // Экономия времени за счёт того, что нет необходимости
                // Пробегать всё слово целиком
                if (dist == 1)
                    return false;
                else
                    dist++;
        }
        return dist == 1;
    }
}
