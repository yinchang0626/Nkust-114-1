# C111181149 - 期中作業（CSV 處理版）

概述
-- 本專案為期中作業範例：讀取放在 `App_Data/` 或專案根目錄的 CSV（年別、男性、女性），解析為物件，進行簡單分析，並輸出解析後的 JSON 摘要。

目前功能（本次重構/新增）
- CSV 解析（流式逐行解析，容錯 header 與非數字欄位）
- 資料模型：`Models/MarriageCount.cs`（Year/Male/Female/Total）
- 服務：`Services/CsvProcessor.cs`、`Services/DataAnalyzer.cs`
- 主程式 `Program.cs`：自動搜尋 CSV 檔，解析、分析並輸出 `*.summary.json`
- 開發便利性：`GlobalUsings.cs`、已清理 `.gitignore`

檔案清單（重要）
- `Program.cs` — 主流程：檔案搜尋 → 解析 → 分析 → 輸出
- `Models/MarriageCount.cs` — CSV 單列資料模型
- `Services/CsvProcessor.cs` — 流式 CSV 解析器
- `Services/DataAnalyzer.cs` — 簡單分析器（總和、最大、年增率）
- `App_Data/` — 放置來源 CSV（已加入 `.gitignore`）

快速啟動
1. 將 CSV 檔放入：

	C:\Users\jackyjuang\Desktop\Nkust-114-1\期中作業\C111181149\App_Data\

	範例 CSV 格式（含 header）：

	年別,男性,女性
	108,6,6
	109,13,37

2. 在 PowerShell 中執行：

```powershell
cd "C:\Users\jackyjuang\Desktop\Nkust-114-1\期中作業\C111181149"
dotnet build
dotnet run
```

執行結果
- 程式會在檔案目錄輸出 `*.summary.json`（解析後的 JSON），並在主控台列印摘要（總和、最大值、年對年變化）。

注意事項與建議
- 若 CSV 很大，現行程式已以流式讀取，能避免大量記憶體占用。
- 若來源 CSV 欄位命名或順序不同，建議採用 CsvHelper 並使用 header mapping，我可以協助加入並自動對映欄位。
- 若需長期儲存與查詢，建議加入 EF Core（SQLite 為起點）並建立匯入流程。

下一步（可選，請回覆一項）
- "加入 CsvHelper"：使用 CsvHelper 套件提升解析健壯性與欄位對映。
- "加入 EF"：新增 EF Core（SQLite）與匯入範例。
- "新增測試"：建立 xUnit 測試覆蓋 CsvProcessor 與 DataAnalyzer。
- "刪除 nested"：若不需要範例檔，我可完全移除 `ConsoleApp/` 子資料夾以精簡專案。

如需我幫忙執行下一步，回覆對應選項即可。
