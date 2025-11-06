using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using C111181149.Models;
using C111181149.Services;

Console.WriteLine("期中作業（C111181149）- CSV/JSON 資料處理與分析");

// 搜尋 CSV 檔案：從多個候選目錄尋找以避免在不同執行情境找不到檔案
var cwd = Directory.GetCurrentDirectory();
var baseDir = AppContext.BaseDirectory;
// 嘗試從 bin/... 往上推到專案目錄（最多 4 層），若失敗則 fallback 到目前工作目錄
string? projectDir = null;
{
	var dir = new DirectoryInfo(baseDir);
	for (int i = 0; i < 6 && dir != null; i++)
	{
		if (File.Exists(Path.Combine(dir.FullName, "C111181149.csproj")))
		{
			projectDir = dir.FullName;
			break;
		}
		dir = dir.Parent;
	}
	if (projectDir == null) projectDir = cwd;
}

var searchDirs = new List<string> {
	cwd,
	baseDir,
	projectDir,
	Path.Combine(projectDir, "App_Data"),
	Path.Combine(cwd, "App_Data")
}.Distinct().ToList();

var candidates = new List<string>();
foreach (var d in searchDirs)
{
	if (!Directory.Exists(d)) continue;
	try
	{
		candidates.AddRange(Directory.EnumerateFiles(d, "*.csv", SearchOption.TopDirectoryOnly));
	}
	catch { }
}

// fallback: recursive search one level deep under projectDir
if (!candidates.Any())
{
	try
	{
		candidates.AddRange(Directory.EnumerateFiles(projectDir, "*.csv", SearchOption.AllDirectories).Take(50));
	}
	catch { }
}

if (!candidates.Any())
{
	Console.WriteLine("找不到任何 CSV 檔案。搜尋的目錄：");
	foreach (var d in searchDirs) Console.WriteLine("  - " + d);
	Console.WriteLine("請把 CSV 放在專案資料夾或 App_Data/ 中，或指定路徑後再執行。");
	return;
}

foreach (var csv in candidates)
{
	Console.WriteLine($"處理 CSV: {Path.GetFileName(csv)}");
	try
	{
		var rows = CsvProcessor.ReadCsv(csv);
		if (!rows.Any())
		{
			Console.WriteLine("  解析後沒有資料（可能欄位格式不同或檔案空白）");
			continue;
		}

		// 列印前幾筆
		Console.WriteLine($"  讀到 {rows.Count} 筆記錄；示例列印：");
		foreach (var r in rows.Take(5)) Console.WriteLine("    " + r);

		// 分析並列印摘要
		DataAnalyzer.PrintSummary(rows, Console.Out);

		// 寫出分析結果為 JSON（統一輸出到 summaries 子資料夾以避免被再次處理）
		var summaryJson = JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
		var summariesDir = Path.Combine(Path.GetDirectoryName(csv) ?? projectDir, "summaries");
		Directory.CreateDirectory(summariesDir);
		var outPath = Path.Combine(summariesDir, Path.GetFileNameWithoutExtension(csv) + ".summary.json");
		await File.WriteAllTextAsync(outPath, summaryJson);
		Console.WriteLine($"  已輸出解析後 JSON: {outPath}");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"  解析/分析失敗: {ex.Message}");
	}
}

// 處理 JSON 檔案（如果有）
var jsonFiles = new List<string>();
foreach (var d in searchDirs)
{
	if (!Directory.Exists(d)) continue;
	try { jsonFiles.AddRange(Directory.EnumerateFiles(d, "*.json", SearchOption.TopDirectoryOnly)); } catch { }
}
if (!jsonFiles.Any())
{
	// try recursive, but limit
	try { jsonFiles.AddRange(Directory.EnumerateFiles(projectDir, "*.json", SearchOption.AllDirectories).Take(50)); } catch { }
}

foreach (var jf in jsonFiles.Distinct())
{
	var name = Path.GetFileName(jf);
	// 跳過 dotnet metadata 與已產生的 summary 檔，或 summaries 子資料夾內的檔案
	if (name.EndsWith(".deps.json", StringComparison.OrdinalIgnoreCase) ||
		name.EndsWith(".runtimeconfig.json", StringComparison.OrdinalIgnoreCase) ||
		name.IndexOf(".summary", StringComparison.OrdinalIgnoreCase) >= 0 ||
		jf.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(p => string.Equals(p, "summaries", StringComparison.OrdinalIgnoreCase)))
	{
		Console.WriteLine($"  跳過檔案: {name}");
		continue;
	}
	await JsonProcessor2.ProcessJsonAsync(jf, Console.Out);
}

Console.WriteLine("處理完成。");
