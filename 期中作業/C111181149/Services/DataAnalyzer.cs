using C111181149.Models;

namespace C111181149.Services;

public static class DataAnalyzer
{
    public static void PrintSummary(IEnumerable<MarriageCount> data, TextWriter output)
    {
        var list = data.OrderBy(d => d.Year).ToList();
        if (!list.Any())
        {
            output.WriteLine("No data to analyze.");
            return;
        }

        int totalMale = list.Sum(d => d.Male);
        int totalFemale = list.Sum(d => d.Female);
        int total = list.Sum(d => d.Total);

        output.WriteLine($"Records: {list.Count}");
        output.WriteLine($"Total Male: {totalMale}");
        output.WriteLine($"Total Female: {totalFemale}");
        output.WriteLine($"Grand Total: {total}");

        var maxMale = list.OrderByDescending(d => d.Male).First();
        var maxFemale = list.OrderByDescending(d => d.Female).First();
        var maxTotal = list.OrderByDescending(d => d.Total).First();

        output.WriteLine($"Max Male: {maxMale.Year} ({maxMale.Male})");
        output.WriteLine($"Max Female: {maxFemale.Year} ({maxFemale.Female})");
        output.WriteLine($"Max Total: {maxTotal.Year} ({maxTotal.Total})");

        // simple year-over-year growth
        output.WriteLine("Year-over-year changes:");
        for (int i = 1; i < list.Count; i++)
        {
            var prev = list[i - 1];
            var cur = list[i];
            double maleChange = PercentChange(prev.Male, cur.Male);
            double femaleChange = PercentChange(prev.Female, cur.Female);
            double totalChange = PercentChange(prev.Total, cur.Total);
            output.WriteLine($"  {prev.Year}->{cur.Year}: male {maleChange:F1}% female {femaleChange:F1}% total {totalChange:F1}%");
        }
    }

    private static double PercentChange(int prev, int cur)
    {
        if (prev == 0) return cur == 0 ? 0 : 100.0;
        return ((double)(cur - prev) / Math.Abs(prev)) * 100.0;
    }
}
