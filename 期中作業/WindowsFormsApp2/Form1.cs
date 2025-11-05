using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CsvHelper.Configuration.Attributes;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private int stockUp;
        private int stockDown;
        private int stockNoChange;
        private int transactionVolumeOverMillion;
        private int transactionVolumeBelowTenThousand;
        //MessageBox.Show("開始執行 Form1()");
        public Form1()
        {
            //MessageBox.Show("✅ Form1() 開始執行");
            InitializeComponent();
            try
            {
                ProcessStockData();
                Setupchart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("發生錯誤:\n" + ex.Message, "例外訊息");
            }
            //InitializeComponent();
            //ProcessStockData();
            //Setupchart();
        }

        private void Setupchart()
        {
            Chart chart = new Chart();
            ChartArea chartArea = new ChartArea();

            chart.Dock = DockStyle.Fill;
            chartArea.AxisX.Title = "Info";
            chartArea.AxisY.Title = "Num";
            chartArea.BackColor = System.Drawing.Color.LightGray;
            chartArea.AxisX.TitleFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            chartArea.AxisY.TitleFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            chartArea.AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas.Add(chartArea);

            Title chartTitle = new Title("2025/11/03 台灣上市股票資訊", Docking.Top, new System.Drawing.Font("Verdana", 16, System.Drawing.FontStyle.Bold), System.Drawing.Color.Black);
            chart.Titles.Add(chartTitle);

            Series series = new Series("Stock Data")
            {
                ChartType = SeriesChartType.Column,
                BorderWidth = 12,
                Font = new System.Drawing.Font("Arial", 12)
            };

            var dataPoints = new[]
            {
                new { Label = "股價上漲", Value = stockUp, Color = System.Drawing.Color.Red },
                new { Label = "股價下跌", Value = stockDown, Color = System.Drawing.Color.Green },
                new { Label = "股價平盤", Value = stockNoChange, Color = System.Drawing.Color.RoyalBlue },
                new { Label = "成交股數 > 1M", Value = transactionVolumeOverMillion, Color = System.Drawing.Color.Orange },
                new { Label = "成交股數 < 10,000", Value = transactionVolumeBelowTenThousand, Color = System.Drawing.Color.Orange }
            };

            foreach (var dataPoint in dataPoints)
            {
                int pointIndex = series.Points.AddXY(dataPoint.Label, dataPoint.Value);
                series.Points[pointIndex].Color = dataPoint.Color;
            }

            series.IsValueShownAsLabel = true;
            chart.Series.Add(series);
            this.Controls.Add(chart);
        }
        private void ProcessStockData()
        {
            var csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "MI_INDEX_ALL_20251103.csv");

            if (!File.Exists(csvFilePath))
            {
                MessageBox.Show("找不到資料檔案：" + csvFilePath, "錯誤");
                return;
            }

            List<StockData> records = new List<StockData>();
            bool startRead = false;

            try
            {
                using (var reader = new StreamReader(csvFilePath, System.Text.Encoding.GetEncoding("big5")))
                {
                    string line;
                    int lineNum = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineNum++;

                        // 跳過前兩行，直到找到「證券代號」
                        if (!startRead)
                        {
                            if (line.Contains("證券代號") && line.Contains("收盤價"))
                            {
                                MessageBox.Show($"偵測到標題行：第 {lineNum} 行\n內容：{line}");
                                startRead = true;
                            }
                            continue;
                        }

                        // 停止條件（遇到說明文字或空行）
                        if (line.StartsWith("說明") || string.IsNullOrWhiteSpace(line))
                            break;

                        // 拆分欄位
                        var parts = SplitCsvLine(line);

                        if (parts.Length < 8) continue; // 不完整行略過

                        string code = Clean(parts[0]);
                        if (code.Length != 4 || !code.All(char.IsDigit))
                            continue; // 跳過非股票代號

                        string name = Clean(parts[1]);
                        long volume = ParseLong(parts[2]);
                        decimal open = ParseDecimal(parts[5]);
                        decimal close = ParseDecimal(parts[8]);

                        records.Add(new StockData
                        {
                            StockCode = code,
                            StockName = name,
                            TransactionVolume = volume,
                            OpenPrice = open,
                            ClosePrice = close
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取 CSV 發生錯誤:\n" + ex.Message, "錯誤");
                return;
            }

            // 統計
            stockUp = records.Count(r => r.ClosePrice > r.OpenPrice);
            stockDown = records.Count(r => r.ClosePrice < r.OpenPrice);
            stockNoChange = records.Count(r => r.ClosePrice == r.OpenPrice);
            transactionVolumeOverMillion = records.Count(r => r.TransactionVolume > 1000000);
            transactionVolumeBelowTenThousand = records.Count(r => r.TransactionVolume < 10000);

            MessageBox.Show(
                $"成功讀取股票資料 {records.Count} 筆\n" +
                $"上漲: {stockUp}  下跌: {stockDown}  平盤: {stockNoChange}\n" +
                $"成交量 > 1M: {transactionVolumeOverMillion}  < 10,000: {transactionVolumeBelowTenThousand}",
                "資料載入結果");
        }

        private string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            result.Add(current);
            return result.ToArray();
        }

        private string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace(",", "").Replace("\"", "").Trim();
        }

        private decimal ParseDecimal(string s)
        {
            decimal v;
            return decimal.TryParse(Clean(s), out v) ? v : 0;
        }

        private long ParseLong(string s)
        {
            long v;
            return long.TryParse(Clean(s), out v) ? v : 0;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    public class StockData
    {
        [Name("證券代號")]
        public string StockCode { get; set; }

        [Name("證券名稱")]
        public string StockName { get; set; }

        [Name("成交股數")]
        public long TransactionVolume { get; set; }

        [Name("成交金額")]
        public decimal TransactionAmount { get; set; }

        [Name("開盤價")]
        public decimal OpenPrice { get; set; }

        [Name("最高價")]
        public decimal HighPrice { get; set; }

        [Name("最低價")]
        public decimal LowPrice { get; set; }

        [Name("收盤價")]
        public decimal ClosePrice { get; set; }

        [Name("成交筆數")]
        public int TransactionCount { get; set; }
    }
}
