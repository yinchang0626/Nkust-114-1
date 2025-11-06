using System.Text.Json;
using System.Text.Json.Serialization;

namespace C111181149.Services;

public static class JsonProcessor2
{
    public static async Task ProcessJsonAsync(string path, TextWriter output)
    {
        var name = Path.GetFileName(path);

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            output.WriteLine("檔案不存在，跳過。");
            return;
        }

        // 跳過 dotnet metadata 與已產生的 summary
        if (name.EndsWith(".deps.json", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(".runtimeconfig.json", StringComparison.OrdinalIgnoreCase) ||
            name.IndexOf(".summary", StringComparison.OrdinalIgnoreCase) >= 0 ||
            path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(p => string.Equals(p, "summaries", StringComparison.OrdinalIgnoreCase)))
        {
            return; // 靜默跳過
        }

        try
        {
            using var fs = File.OpenRead(path);
            using var doc = await JsonDocument.ParseAsync(fs);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                var items = root.EnumerateArray().ToList();
                
                // 針對婚姻統計資料的特殊處理
                if (items.Count > 0 && items[0].TryGetProperty("區域別", out _))
                {
                    // 統計各區域的婚姻數據
                    var stats = new Dictionary<string, (int total, int same, int different)>();
                    
                    foreach (var item in items)
                    {
                        if (item.TryGetProperty("區域別", out var areaElement) &&
                            item.TryGetProperty("總計_總計", out var totalElement) &&
                            item.TryGetProperty("相同性別_總計", out var sameElement) &&
                            item.TryGetProperty("不同性別_總計", out var diffElement))
                        {
                            var area = areaElement.GetString() ?? "未知";
                            var total = ParseJsonNumber(totalElement);
                            var same = ParseJsonNumber(sameElement);
                            var diff = ParseJsonNumber(diffElement);
                            stats[area] = (total, same, diff);
                        }
                    }

                    if (stats.Any())
                    {
                        output.WriteLine("\n婚姻統計摘要：");
                        output.WriteLine("區域　　　　　總計　不同性別　相同性別");
                        output.WriteLine("========================================");
                        
                        foreach (var kvp in stats.OrderByDescending(x => x.Value.total))
                        {
                            output.WriteLine($"{kvp.Key,-12} {kvp.Value.total,6} {kvp.Value.different,8} {kvp.Value.same,8}");
                        }

                        // 計算全區總計
                        var grandTotal = stats.Values.Sum(x => x.total);
                        var totalSame = stats.Values.Sum(x => x.same);
                        var totalDiff = stats.Values.Sum(x => x.different);
                        output.WriteLine("----------------------------------------");
                        output.WriteLine($"總計　　　　 {grandTotal,6} {totalDiff,8} {totalSame,8}");

                        // 寫出 summary JSON（更簡潔的格式）
                        var summary = new MarriageStats
                        {
                            Source = Path.GetFileName(path),
                            Total = grandTotal,
                            TotalDifferentGender = totalDiff,
                            TotalSameGender = totalSame,
                            ByArea = stats.Select(kvp => new AreaStats
                            {
                                Area = kvp.Key,
                                Total = kvp.Value.total,
                                SameGender = kvp.Value.same,
                                DifferentGender = kvp.Value.different
                            }).OrderByDescending(x => x.Total).ToList()
                        };

                        var summariesDir = Path.Combine(Path.GetDirectoryName(path) ?? ".", "summaries");
                        Directory.CreateDirectory(summariesDir);
                        var outPath = Path.Combine(summariesDir, 
                            Path.GetFileNameWithoutExtension(path) + ".summary.json");
                        
                        var options = new JsonSerializerOptions 
                        { 
                            WriteIndented = true,
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        };
                        
                        await File.WriteAllTextAsync(outPath, 
                            JsonSerializer.Serialize(summary, options));
                        
                        output.WriteLine($"\n已輸出統計摘要: {outPath}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"  解析失敗: {ex.Message}");
        }
    }

    private static int ParseJsonNumber(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.String &&
            int.TryParse(element.GetString(), out var n))
            return n;
        
        if (element.ValueKind == JsonValueKind.Number &&
            element.TryGetInt32(out var m))
            return m;
        
        return 0;
    }
}