using System.Data;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private DataTable dataTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            // 啟動時自動載入 CSV
            LoadCsvData();
        }

        private void LoadCsvData()
        {
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "主要國家老化指數.csv");
            csvPath = Path.GetFullPath(csvPath);

            if (!File.Exists(csvPath))
            {
                MessageBox.Show($"找不到檔案：{csvPath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                dataTable.Clear();
                var lines = File.ReadAllLines(csvPath);

                if (lines.Length < 2)
                {
                    MessageBox.Show("CSV 檔案內容不足", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 解析標題列
                var headers = lines[0].Split(',');
                foreach (var header in headers)
                {
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        dataTable.Columns.Add(header.Trim());
                    }
                }

                // 解析資料列
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(',');
                    if (values.Length >= dataTable.Columns.Count)
                    {
                        var row = dataTable.NewRow();
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            row[j] = values[j].Trim();
                        }
                        dataTable.Rows.Add(row);
                    }
                }

                dataGridView1.DataSource = dataTable;
                MessageBox.Show($"成功載入 {dataTable.Rows.Count} 筆資料", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入 CSV 時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("請先載入資料", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                chart1.Series.Clear();
                chart1.ChartAreas[0].AxisX.Title = "西元年";
                chart1.ChartAreas[0].AxisY.Title = "老化指數";
                chart1.Titles.Clear();
                chart1.Titles.Add("主要國家老化指數趨勢");

                // 按國別分組繪製折線
                var countries = dataTable.AsEnumerable()
                    .Select(r => r["國別"].ToString())
                    .Distinct()
                    .ToList();

                foreach (var country in countries)
                {
                    if (string.IsNullOrWhiteSpace(country)) continue;

                    var series = new Series(country)
                    {
                        ChartType = SeriesChartType.Line,
                        BorderWidth = 2,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 6
                    };

                    var countryData = dataTable.AsEnumerable()
                        .Where(r => r["國別"].ToString() == country)
                        .OrderBy(r => Convert.ToInt32(r["西元年"]))
                        .ToList();

                    foreach (var row in countryData)
                    {
                        int year = Convert.ToInt32(row["西元年"]);
                        double index = Convert.ToDouble(row["老化指數"]);
                        series.Points.AddXY(year, index);
                    }

                    chart1.Series.Add(series);
                }

                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0";
                chart1.ChartAreas[0].RecalculateAxesScale();

                MessageBox.Show($"成功繪製 {countries.Count} 個國家的折線圖", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"繪製圖表時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
