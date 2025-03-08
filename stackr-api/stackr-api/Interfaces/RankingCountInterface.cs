namespace Stackr_Api.Models;

public interface IRankingCountService
{
    List<Rank> CalculateRankScores(List<string> rankings);
}