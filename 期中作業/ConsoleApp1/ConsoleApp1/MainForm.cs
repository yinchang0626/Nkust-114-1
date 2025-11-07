using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace JapanBitthViewer
{
    public partial class MainForm : Form
    {
        private DataTable table = new DataTable();

        public MainForm()
        {
            InitializeComponent();
            
            // Initialize chart
            chart1.ChartAreas.Clear();
            chart1.ChartAreas.Add(new ChartArea("Default"));
            
            // Set chart appearance
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chart1.ChartAreas[0].BackColor = System.Drawing.Color.WhiteSmoke;
            
            // Try to load japan_birth.csv if it exists
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "japan_birth.csv");
            if (File.Exists(csvPath))
            {
                LoadCsv(csvPath);
                // Set default columns for plotting
                if (cbX.Items.Contains("year")) cbX.SelectedItem = "year";
                if (cbY.Items.Contains("birth_total")) cbY.SelectedItem = "birth_total";
                btnPlot.PerformClick();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "CSV files|*.csv|All files|*.*";
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (ofd.ShowDialog() != DialogResult.OK) return;
            LoadCsv(ofd.FileName);
        }

        private void LoadCsv(string path)
        {
            table.Clear();
            table.Columns.Clear();
            var lines = File.ReadAllLines(path);
            if (lines.Length == 0) return;
            var headers = lines[0].Split(',');
            foreach (var h in headers) table.Columns.Add(h.Trim());
            for (int i = 1; i < lines.Length; i++)
            {
                var fields = lines[i].Split(',');
                var row = table.NewRow();
                for (int j = 0; j < headers.Length && j < fields.Length; j++)
                    row[j] = fields[j].Trim();
                table.Rows.Add(row);
            }
            dataGridView1.DataSource = table;
            cbX.Items.Clear();
            cbY.Items.Clear();
            foreach (DataColumn col in table.Columns)
            {
                cbX.Items.Add(col.ColumnName);
                cbY.Items.Add(col.ColumnName);
            }
            if (cbX.Items.Count > 0) cbX.SelectedIndex = 0;
            if (cbY.Items.Count > 1) cbY.SelectedIndex = 1;
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            
            // Add chart title
            chart1.Titles.Add(new Title("日本出生人數統計 (男女性別)", Docking.Top));
            
            // Create series for male births
            var maleSeries = new Series
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Int32,
                YValueType = ChartValueType.Double,
                Name = "男性出生數",
                BorderWidth = 2,
                Color = System.Drawing.Color.Blue
            };
            
            // Create series for female births
            var femaleSeries = new Series
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Int32,
                YValueType = ChartValueType.Double,
                Name = "女性出生數",
                BorderWidth = 2,
                Color = System.Drawing.Color.Pink
            };
            
            chart1.Series.Add(maleSeries);
            chart1.Series.Add(femaleSeries);
            
            // Configure chart area
            chart1.ChartAreas[0].AxisX.Title = "年份";
            chart1.ChartAreas[0].AxisY.Title = "出生人數";
            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            
            // Add legend
            chart1.Legends.Clear();
            chart1.Legends.Add(new Legend("Legend"));
            
            // Add data points
            foreach (DataRow row in table.Rows)
            {
                var year = row["year"].ToString();
                var maleBirths = row["birth_male"].ToString();
                var femaleBirths = row["birth_female"].ToString();
                
                if (int.TryParse(year, out var x) && 
                    double.TryParse(maleBirths, out var male) && 
                    double.TryParse(femaleBirths, out var female))
                {
                    maleSeries.Points.AddXY(x, male);
                    femaleSeries.Points.AddXY(x, female);
                }
            }
        }
    }
}
