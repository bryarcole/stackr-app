public class RankingCountService : IRankingCountService
{
    public List<KeyValuePair<string, double>> CalculateRankScores(List<string> rankings)
    {
        var scores = new Dictionary<string, double>();
        double index = 10.0; // Start with the highest decimal score

        foreach (var candidate in rankings)
        {
            if (!scores.ContainsKey(candidate))
            {
                scores[candidate] = 0.0;
            }
            scores[candidate] += index;
            index -= 0.5; // Decrement by 0.5 instead of 1
        }

        return scores.OrderByDescending(kvp => kvp.Value).ToList();
    }
}