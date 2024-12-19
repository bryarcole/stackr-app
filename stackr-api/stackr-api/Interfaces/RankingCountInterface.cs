
public interface IRankingCountService
{
    List<KeyValuePair<string, double>> CalculateRankScores(List<List<string>> rankings);
}