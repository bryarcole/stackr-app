
public interface IBordaCountService
{
    List<KeyValuePair<string, double>> CalculateBordaScores(List<List<string>> rankings);
}