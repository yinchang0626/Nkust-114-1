# 食品營養成分資料庫實作說明

## 專案概述
本專案將 JSON 格式的食品營養成分資料讀取並寫入 SQL Server LocalDB 資料庫。

## 已完成的工作

### 1. 建立實體類別
- **檔案**: `FoodInfoEntity.cs`
- **說明**: 基於 `FoodInfo.cs` 建立的資料庫實體類別
- **特點**:
  - 包含主鍵 `Id`（自動遞增）
  - 所有欄位都使用字串型別（避免 JSON 格式錯誤）
  - 使用 Data Annotations 定義欄位型別和長度
  - 包含 `CreatedAt` 時間戳記欄位

### 2. 建立 DbContext
- **檔案**: `FoodDbContext.cs`
- **說明**: Entity Framework Core 的資料庫上下文類別
- **連線字串**: 
  ```
  Server=(localdb)\mssqllocaldb;
  Database=FoodNutritionDb;
  Trusted_Connection=True;
  MultipleActiveResultSets=true
  ```
- **特點**:
  - 為 `IntegratedNumber`、`SampleName`、`AnalysisCategory` 建立索引以提升查詢效能

### 3. 已安裝的 NuGet 套件
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10)
- `Microsoft.EntityFrameworkCore.Tools` (9.0.10)

### 4. Migration
- **Migration 名稱**: `InitialCreate`
- **建立時間**: 2025-10-31 02:06:15
- **狀態**: 已套用至資料庫

### 5. Program.cs 功能
更新後的程式包含以下功能：
1. 讀取並反序列化 JSON 檔案
2. 顯示前 10 筆資料預覽
3. 連接到資料庫
4. 詢問是否清空現有資料
5. 批次寫入資料（每批 1000 筆）
6. 顯示寫入進度
7. 顯示統計資訊（前 5 大分析項分類）

## 使用方式

### 執行程式
```powershell
cd "C:\Works\11410\Nkust-114-1\Sample\ConsoleApp"
dotnet run
```

### 查看資料庫
可以使用以下工具連接到 LocalDB：
- SQL Server Management Studio (SSMS)
- Azure Data Studio
- Visual Studio 的 SQL Server 物件總管

**連線字串**:
```
Server=(localdb)\mssqllocaldb;Database=FoodNutritionDb;Trusted_Connection=True;
```

### 資料庫操作

#### 查看資料表結構
```sql
USE FoodNutritionDb;
GO

-- 查看資料表結構
EXEC sp_help 'FoodInfos';

-- 查看索引
EXEC sp_helpindex 'FoodInfos';
```

#### 查詢資料範例
```sql
-- 查詢總筆數
SELECT COUNT(*) FROM FoodInfos;

-- 依分析項分類統計
SELECT AnalysisCategory, COUNT(*) AS Count
FROM FoodInfos
GROUP BY AnalysisCategory
ORDER BY Count DESC;

-- 查詢特定食品
SELECT TOP 10 *
FROM FoodInfos
WHERE SampleName LIKE '%米%'
ORDER BY CreatedAt DESC;

-- 查詢特定整合編號的所有分析項
SELECT SampleName, AnalysisItem, ContentPer100g, ContentUnit
FROM FoodInfos
WHERE IntegratedNumber = '您的整合編號'
ORDER BY AnalysisCategory;
```

## 資料表結構

### FoodInfos 資料表
| 欄位名稱 | 型別 | 說明 |
|---------|------|------|
| Id | int (PK) | 主鍵，自動遞增 |
| ContentPerUnitWeight | nvarchar(200) | 每單位重含量 |
| IntegratedNumber | nvarchar(100) | 整合編號（已建立索引） |
| AnalysisCategory | nvarchar(100) | 分析項分類（已建立索引） |
| SampleName | nvarchar(500) | 樣品名稱（已建立索引） |
| ContentPer100g | nvarchar(200) | 每100克含量 |
| ContentPerUnit | nvarchar(200) | 每單位含量 |
| StandardDeviation | nvarchar(200) | 標準差 |
| UnitWeight | nvarchar(200) | 每單位重 |
| ContentUnit | nvarchar(100) | 含量單位 |
| SampleCount | nvarchar(100) | 樣本數 |
| WasteRate | nvarchar(100) | 廢棄率 |
| SampleEnglishName | nvarchar(500) | 樣品英文名稱 |
| DataCategory | nvarchar(100) | 資料類別 |
| AnalysisItem | nvarchar(200) | 分析項 |
| FoodCategory | nvarchar(200) | 食品分類 |
| ContentDescription | nvarchar(max) | 內容物描述 |
| CommonName | nvarchar(500) | 俗名 |
| CreatedAt | datetime2 | 建立時間 |

## 注意事項

1. **資料型別**: 所有欄位都使用字串型別，確保 JSON 資料不會因為格式問題而無法匯入
2. **批次處理**: 程式使用批次處理（1000 筆一批），避免記憶體不足
3. **索引**: 已為常用查詢欄位建立索引，提升查詢效能
4. **錯誤處理**: 包含完整的例外處理機制
5. **資料清空**: 執行前會詢問是否清空現有資料，避免重複匯入

## 進階操作

### 新增 Migration
如果需要修改資料表結構：
```powershell
dotnet ef migrations add [Migration名稱]
dotnet ef database update
```

### 移除最後一個 Migration
```powershell
dotnet ef migrations remove
```

### 重建資料庫
```powershell
dotnet ef database drop
dotnet ef database update
```

## 效能優化建議

1. **批次大小**: 可以調整 `batchSize` 參數（目前為 1000）
2. **索引**: 根據實際查詢需求增加或調整索引
3. **資料型別**: 如果確定資料格式正確，可以將部分欄位改為數值型別
4. **非同步操作**: 可以改用 `async/await` 提升效能

## 檔案清單

- `FoodInfo.cs` - 原始 JSON 反序列化類別
- `FoodInfoEntity.cs` - 資料庫實體類別
- `FoodDbContext.cs` - EF Core DbContext
- `Program.cs` - 主程式（含資料庫寫入邏輯）
- `Migrations/` - EF Core Migration 檔案

## 聯絡資訊
如有問題，請參考專案 README.md 或聯絡開發人員。
