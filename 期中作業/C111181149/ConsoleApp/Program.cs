using System.Text.Json;
using System.Text.Json.Serialization;
//using C111181149;

// This nested sample ConsoleApp was moved/disabled as part of project cleanup.
// Source is intentionally left non-functional to avoid confusion.
Console.WriteLine("[INFO] Nested ConsoleApp disabled. Use the root C111181149 project instead.");

var appData = Path.Combine(AppContext.BaseDirectory, "App_Data");
if (!Directory.Exists(appData))
{
    Console.WriteLine($"找不到 App_Data 資料夾：{appData}");
    Console.WriteLine("請將下載的 JSON 檔放在 ConsoleApp/App_Data/ 並重新執行。");
    return;
}

var jsonFiles = Directory.GetFiles(appData, "*.json", SearchOption.TopDirectoryOnly);
if (jsonFiles.Length == 0)
{
    Console.WriteLine("在 App_Data 中找不到任何 .json 檔案。請放入食品營養成分 JSON 檔。");
    return;
}

int total = 0;
foreach (var file in jsonFiles)
{
    Console.WriteLine($"讀取: {Path.GetFileName(file)}");
    try
    {
        var text = await File.ReadAllTextAsync(file);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      //  var items = JsonSerializer.Deserialize<List<FoodInfo>>(text, options);
    /*    if (items != null)
        {
            Console.WriteLine($"  解析到 {items.Count} 筆資料 (示例列印前5筆)");
            total += items.Count;
            foreach (var it in items.Take(5))
            {
                Console.WriteLine($"    編號:{it.FoodNo} 名稱:{it.FoodName} 熱量:{it.Energy}kcal 蛋白質:{it.Protein}g");
            }
        }*/
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  解析失敗: {ex.Message}");
    }
}
Console.WriteLine($"總共解析到 {total} 筆資料。\n完成。若需匯入資料庫或更多欄位，請查看 README。");
