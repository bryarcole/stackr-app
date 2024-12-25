
public interface IRankingCountService
{
    List<KeyValuePair<string, double>> CalculateRankScores(List<string> rankings);
}