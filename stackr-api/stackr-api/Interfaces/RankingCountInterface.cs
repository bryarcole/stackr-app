
public interface IRankingCountServices
{
    List<KeyValuePair<string, double>> CalculateRankScores(List<List<string>> rankings);
}