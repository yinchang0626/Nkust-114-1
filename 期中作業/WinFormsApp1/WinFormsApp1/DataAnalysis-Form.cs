using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DataAnalysisApp
{
    public partial class DataAnalysisForm : Form
    {
        private DataTable dataTable = new DataTable();
        private Chart chartControl;

        public DataAnalysisForm()
        {
            InitializeComponent();
            this.Text = "資料分析系統";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            LoadCSVData();
        }

        private void SetupUI()
        {
            // 建立上方面板存放按鈕
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = System.Drawing.Color.LightGray
            };

            // 圓餅圖按鈕
            Button pieChartBtn = new Button
            {
                Text = "圓餅圖",
                Location = new System.Drawing.Point(20, 10),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.SkyBlue
            };
            pieChartBtn.Click += (s, e) => DisplayPieChart();

            // 直方圖按鈕
            Button barChartBtn = new Button
            {
                Text = "直方圖",
                Location = new System.Drawing.Point(130, 10),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.LightCoral
            };
            barChartBtn.Click += (s, e) => DisplayBarChart();

            // 重新整理按鈕
            Button refreshBtn = new Button
            {
                Text = "重新整理",
                Location = new System.Drawing.Point(240, 10),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.LightGreen
            };
            refreshBtn.Click += (s, e) => LoadCSVData();

            buttonPanel.Controls.Add(pieChartBtn);
            buttonPanel.Controls.Add(barChartBtn);
            buttonPanel.Controls.Add(refreshBtn);

            // 建立圖表控制項
            chartControl = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White
            };
            chartControl.ChartAreas.Add(new ChartArea());

            this.Controls.Add(chartControl);
            this.Controls.Add(buttonPanel);
        }

        private void LoadCSVData()
        {
            try
            {
                // 使用檔案對話框選擇 CSV 檔案
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "請選擇 CSV 檔案"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadCSV(openFileDialog.FileName);
                    DisplayPieChart(); // 預設顯示圓餅圖
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCSV(string filePath)
        {
            dataTable.Clear();

            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string[] headers = reader.ReadLine()?.Split(',') ?? new string[] { };
                string[] descriptions = reader.ReadLine()?.Split(',') ?? new string[] { };

                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] values = line.Split(',');
                        dataTable.Rows.Add(values);
                    }
                }
            }

            MessageBox.Show($"成功載入 {dataTable.Rows.Count} 筆資料", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisplayPieChart()
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("請先載入資料", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            chartControl.Series.Clear();
            chartControl.ChartAreas[0].AxisX.LabelStyle.Angle = 0;

            // 統計各縣市的監測點數量
            var cityCounts = new Dictionary<string, int>();
            foreach (DataRow row in dataTable.Rows)
            {
                string city = row[0].ToString();
                if (!cityCounts.ContainsKey(city))
                    cityCounts[city] = 0;
                cityCounts[city]++;
            }

            Series series = new Series
            {
                Name = "監測點數量",
                ChartType = SeriesChartType.Pie
            };

            foreach (var kvp in cityCounts.OrderByDescending(x => x.Value).Take(10))
            {
                series.Points.AddXY(kvp.Key, kvp.Value);
            }

            chartControl.Series.Add(series);
            chartControl.Titles.Clear();
            chartControl.Titles.Add(new Title("各縣市監測點分布 (前10名)"));
        }

        private void DisplayBarChart()
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("請先載入資料", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            chartControl.Series.Clear();

            // 統計各縣市的監測點數量
            var cityCounts = new Dictionary<string, int>();
            foreach (DataRow row in dataTable.Rows)
            {
                string city = row[0].ToString();
                if (!cityCounts.ContainsKey(city))
                    cityCounts[city] = 0;
                cityCounts[city]++;
            }

            Series series = new Series
            {
                Name = "監測點數量",
                ChartType = SeriesChartType.Column
            };

            foreach (var kvp in cityCounts.OrderByDescending(x => x.Value).Take(10))
            {
                series.Points.AddXY(kvp.Key, kvp.Value);
            }

            chartControl.Series.Add(series);

            // 設定軸標籤角度
            chartControl.ChartAreas[0].AxisX.LabelStyle.Angle = 45;

            // 設定 Y 軸
            chartControl.ChartAreas[0].AxisY.Title = "監測點數量";
            chartControl.ChartAreas[0].AxisX.Title = "縣市";

            chartControl.Titles.Clear();
            chartControl.Titles.Add(new Title("各縣市監測點分布直方圖 (前10名)"));
        }
    }

    /*
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DataAnalysisForm());
        }
    }
    */
}
