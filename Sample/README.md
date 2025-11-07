# Nkust-114-1

## 專案說明

這是一個 .NET 9.0 控制台應用程式，用於處理地震報告資料。

## 資料來源

本專案需要使用政府開放資料平台提供的顯著有感地震報告資料：

**資料來源**：[顯著有感地震報告 - 政府資料開放平臺](https://data.gov.tw/dataset/6068)

### 資料準備

開發者需要自行從上述連結下載 JSON 格式的資料檔案，並放置在 `ConsoleApp/App_Data/` 目錄下。

**注意**：`App_Data` 資料夾已設定為 Git 忽略，因此您需要手動建立此資料夾並放入資料檔案。

## 專案啟動方式

### 開發環境需求
- .NET 9.0 SDK

### 執行步驟

1. **專案**
   

2. **準備資料檔案**
   - 從 [政府開放資料平台](https://data.gov.tw/dataset/8543) 下載 JSON 格式的食品營養成分資料
   - 在專案根目錄建立 `ConsoleApp/App_Data/` 資料夾（如果不存在）
   - 將下載的 JSON 檔案放入 `ConsoleApp/App_Data/` 目錄

3. **建置專案**
   ```bash
   dotnet build
   ```

4. **執行應用程式**
   ```bash
   dotnet run --project ConsoleApp
   ```

   或者直接進入 ConsoleApp 目錄執行：
   ```bash
   cd ConsoleApp
   dotnet run
   ```

## 專案結構

```
earthquake/
├── ConsoleApp1/
│   ├── ConsoleApp1.csproj
│   ├── Program.cs
│   ├── FoodInfo.cs
│   └── App_Data/          # 資料檔案目錄（需手動建立）
│       └── *.json         # 顯著有感地震資料檔案
├── ConsoleApp1.sln
└── README.md
```
