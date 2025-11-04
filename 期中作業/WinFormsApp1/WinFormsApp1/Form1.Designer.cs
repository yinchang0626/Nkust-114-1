using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        // Controls created in designer
        private ComboBox comboBoxSpeedLimit;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCityStats;
        private Label labelSpeedLimit;

        /// <summary>
        /// Initialize form controls (minimal designer implementation).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            this.comboBoxSpeedLimit = new ComboBox();
            this.chartCityStats = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelSpeedLimit = new Label();

            ((ISupportInitialize)(this.chartCityStats)).BeginInit();
            this.SuspendLayout();

            // comboBoxSpeedLimit
            this.comboBoxSpeedLimit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSpeedLimit.FormattingEnabled = true;
            this.comboBoxSpeedLimit.Location = new Point(12, 12);
            this.comboBoxSpeedLimit.Name = "comboBoxSpeedLimit";
            this.comboBoxSpeedLimit.Size = new Size(120, 23);
            this.comboBoxSpeedLimit.TabIndex = 0;
            this.comboBoxSpeedLimit.SelectedIndexChanged += new EventHandler(this.comboBoxSpeedLimit_SelectedIndexChanged);

            // labelSpeedLimit
            this.labelSpeedLimit.AutoSize = true;
            this.labelSpeedLimit.Location = new Point(150, 16);
            this.labelSpeedLimit.Name = "labelSpeedLimit";
            this.labelSpeedLimit.Size = new Size(200, 15);
            this.labelSpeedLimit.TabIndex = 1;
            this.labelSpeedLimit.Text = "速限篩選: 全部 | 總數量: 0";

            // chartCityStats
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartArea.Name = "ChartArea1";
            this.chartCityStats.ChartAreas.Add(chartArea);
            var legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            legend.Name = "Legend1";
            this.chartCityStats.Legends.Add(legend);
            this.chartCityStats.Location = new Point(12, 50);
            this.chartCityStats.Name = "chartCityStats";
            this.chartCityStats.Size = new Size(760, 380);
            this.chartCityStats.TabIndex = 2;
            this.chartCityStats.Text = "chart1";

            // Form1
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 461);
            this.Controls.Add(this.chartCityStats);
            this.Controls.Add(this.labelSpeedLimit);
            this.Controls.Add(this.comboBoxSpeedLimit);
            this.Name = "Form1";
            this.Text = "測速照相機統計";
            this.Load += new EventHandler(this.Form1_Load);

            ((ISupportInitialize)(this.chartCityStats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            InitializeComboBox();
            UpdateChart("全部");
        }

        private void LoadData()
        {
            dataTable = new DataTable();

            // 讀取 CSV 檔案
                // 讀取 CSV 檔案
                // Try to locate the CSV file by searching upward from the app base directory.
                // This avoids problems when the working directory is the bin output folder.
                string csvName = "NPA_TD1.csv";
                string csvPath = FindFileInParents(AppDomain.CurrentDomain.BaseDirectory, csvName) ?? csvName;

            try
            {
                // Open the file allowing other processes to keep it open (shared read/write).
                using (var fs = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    // 讀取標題列
                    string headerLine = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        MessageBox.Show($"CSV 檔案為空或者無法讀取: {csvPath}", "錯誤",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string[] headers = headerLine.Split(',');

                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // 跳過第二行（中文標題）
                    sr.ReadLine();

                    // 讀取資料
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split(',');
                        dataTable.Rows.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取檔案錯誤: {ex.Message}\n嘗試的路徑: {csvPath}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Search up the directory tree from <paramref name="startDir"/> for a file named <paramref name="fileName"/>.
        /// Returns the full path if found, otherwise null.
        /// </summary>
        private string FindFileInParents(string startDir, string fileName)
        {
            try
            {
                var dir = new DirectoryInfo(startDir);
                for (int i = 0; i < 10 && dir != null; i++)
                {
                    string candidate = Path.Combine(dir.FullName, fileName);
                    if (File.Exists(candidate)) return candidate;
                    dir = dir.Parent;
                }
            }
            catch
            {
                // ignore and return null
            }
            return null;
        }

        private void InitializeComboBox()
        {
            // 取得所有不重複的速限值
            var speedLimits = dataTable.AsEnumerable()
                .Select(row => row["limit"].ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => int.TryParse(s, out int val) ? val : 999)
                .ToList();

            comboBoxSpeedLimit.Items.Add("全部");
            foreach (var limit in speedLimits)
            {
                comboBoxSpeedLimit.Items.Add(limit);
            }

            comboBoxSpeedLimit.SelectedIndex = 0;
        }

        private void comboBoxSpeedLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChart(comboBoxSpeedLimit.SelectedItem.ToString());
        }

        private void UpdateChart(string speedLimit)
        {
            // 清空圖表
            chartCityStats.Series.Clear();
            chartCityStats.Titles.Clear();

            // 篩選資料
            var filteredData = dataTable.AsEnumerable();

            if (speedLimit != "全部")
            {
                filteredData = filteredData.Where(row =>
                    row["limit"].ToString() == speedLimit);
            }

            // 統計各縣市數量
            var cityStats = filteredData
                .GroupBy(row => row["CityName"].ToString())
                .Select(g => new
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(20) // 只顯示前20名
                .ToList();

            if (cityStats.Count == 0)
            {
                MessageBox.Show("查無資料", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 建立圖表
            Series series = new Series("測速照相機數量");
            series.ChartType = SeriesChartType.Column;
            // Treat string X values as indexed categories so each city gets its own column
            series.IsXValueIndexed = true;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0}";
            series.Font = new Font("Microsoft JhengHei", 9);

            // 設定顏色
            series.Color = Color.FromArgb(0, 120, 215);
            series.BorderColor = Color.FromArgb(0, 90, 180);
            series.BorderWidth = 1;

            foreach (var stat in cityStats)
            {
                series.Points.AddXY(stat.City, stat.Count);
            }

            // Make columns a bit narrower to reduce overlap when many categories exist.
            // PointWidth is a custom attribute recognized by the WinForms Chart control.
            series.SetCustomProperty("PointWidth", "0.6");

            chartCityStats.Series.Add(series);

            // 設定標題
            string title = speedLimit == "全部"
                ? "各縣市測速照相機數量統計"
                : $"各縣市測速照相機數量統計 (速限: {speedLimit} km/h)";

            chartCityStats.Titles.Add(new Title(title, Docking.Top,
                new Font("Microsoft JhengHei", 14, FontStyle.Bold), Color.Black));

            // 設定圖表區域
            ChartArea chartArea = chartCityStats.ChartAreas[0];
            chartArea.AxisX.Title = "縣市";
            chartArea.AxisX.TitleFont = new Font("Microsoft JhengHei", 11, FontStyle.Bold);
            chartArea.AxisX.LabelStyle.Font = new Font("Microsoft JhengHei", 9);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.LabelStyle.Angle = -45;

            chartArea.AxisY.Title = "數量";
            chartArea.AxisY.TitleFont = new Font("Microsoft JhengHei", 11, FontStyle.Bold);
            chartArea.AxisY.LabelStyle.Font = new Font("Microsoft JhengHei", 9);

            // 設定背景
            chartArea.BackColor = Color.WhiteSmoke;
            chartCityStats.BackColor = Color.White;

            // 顯示圖例
            chartCityStats.Legends[0].Enabled = true;
            chartCityStats.Legends[0].Font = new Font("Microsoft JhengHei", 10);
            chartCityStats.Legends[0].Docking = Docking.Bottom;

            // 統計資訊
            int totalCount = cityStats.Sum(x => x.Count);
            labelSpeedLimit.Text = $"速限篩選: {speedLimit} | 總數量: {totalCount}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}