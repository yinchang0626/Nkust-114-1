using System.Globalization;
using C111181149.Models;

namespace C111181149.Services;

public static class CsvProcessor
{
    // Stream-based CSV reader for the simple format: Year,Male,Female with optional header
    public static List<MarriageCount> ReadCsv(string path)
    {
        var list = new List<MarriageCount>();
        if (!File.Exists(path)) return list;

        using var sr = new StreamReader(path);
        string? first = null;
        if (!sr.EndOfStream) first = sr.ReadLine();

        if (first is null) return list;
        if (!IsHeaderLine(first))
        {
            // first line contains data, process it
            TryParseLine(first, list);
        }

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            TryParseLine(line, list);
        }

        return list;
    }

    private static bool IsHeaderLine(string line)
    {
        var lower = line.ToLowerInvariant();
        // common heuristics: contains non-digit header names or Chinese 年/男性/女性
        if (lower.Contains("年") || lower.Contains("男性") || lower.Contains("女性") || lower.Contains("year") || lower.Contains("male") || lower.Contains("female"))
            return true;
        // if first token is not an integer, treat as header
        var firstTok = line.Split(',').FirstOrDefault()?.Trim() ?? string.Empty;
        return !int.TryParse(firstTok, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
    }

    private static void TryParseLine(string line, List<MarriageCount> list)
    {
        var parts = line.Split(',');
        if (parts.Length < 3) return;
        if (!int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var year)) return;
        int male = ParseInt(parts[1]);
        int female = ParseInt(parts[2]);
        list.Add(new MarriageCount { Year = year, Male = male, Female = female });
    }

    private static int ParseInt(string s)
    {
        s = s?.Trim() ?? string.Empty;
        if (int.TryParse(s, out var v)) return v;
        var digits = new string(s.Where(c => char.IsDigit(c) || c == '-').ToArray());
        if (int.TryParse(digits, out var v2)) return v2;
        return 0;
    }
}
