using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace testC_
{
    public partial class CPOS_Apllication : Form
    {
        private DataTable mergeDataTable;

        public CPOS_Apllication()
        {
            InitializeComponent();
            reportLabel.MouseClick += ReportLabel_MouseClick;
            Controls.Add(reportLabel);

            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToOrderColumns = false;

            
        }


        private void ReportLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //MessageBox.Show("Right-click Detected");

                // create menu item
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                ToolStripMenuItem loadMenuItem = new ToolStripMenuItem("load");
                ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("save");
                ToolStripMenuItem exportMenuItem = new ToolStripMenuItem("export");

                // add menu items to context menu strip
                contextMenuStrip.Items.Add(loadMenuItem);
                contextMenuStrip.Items.Add(saveMenuItem);
                contextMenuStrip.Items.Add(exportMenuItem);

                // Attach the context menu strip to the TextBox control
                reportLabel.ContextMenuStrip = contextMenuStrip;

                // handle event from menu item
                loadMenuItem.Click += LoadMenuItem_Click;
                saveMenuItem.Click += SaveMenuItem_Click;
                exportMenuItem.Click += ExportMenuItem_Click;

            }
        }

        private void ExportMenuItem_Click(object sender, EventArgs e)
        {
            
            if (chartProcessing())
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                saveFileDialog.Title = "Save DataGridView Data";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePath = saveFileDialog.FileName;

                    int widthLine = chartLine.Width;
                    int widthNut = chartDoughNut.Width;
                    Bitmap bitmapLine = new Bitmap(widthLine, chartLine.Height);
                    //Bitmap bitmapNut = new Bitmap(widthNut, chartDoughNut.Height);

                    // Render the chart onto the bitmap
                    chartLine.DrawToBitmap(bitmapLine, new Rectangle(0, 0, chartLine.Width, chartLine.Height));
                    //chartDoughNut.DrawToBitmap(bitmapNut, new Rectangle(0, 0, widthNut, chartDoughNut.Height));

                    // Save the image to the specified file path
                    bitmapLine.Save(filePath, ImageFormat.Png);
                    //bitmapNut.Save(filePath, ImageFormat.Png);

                    // Dispose the bitmap object
                    //bitmapNut.Dispose();
                    bitmapLine.Dispose();

                    // Show a message to indicate successful export
                    MessageBox.Show("Chart exported successfully.");
                }
            }
            if (chartProcessing())
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files|*.png";
                saveFileDialog.Title = "Save DataGridView Data";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePath = saveFileDialog.FileName;

                    int widthLine = chartLine.Width;
                    int widthNut = chartDoughNut.Width;
                    //Bitmap bitmapLine = new Bitmap(widthLine, chartLine.Height);
                    Bitmap bitmapNut = new Bitmap(widthNut, chartDoughNut.Height);

                    // Render the chart onto the bitmap
                    //chartLine.DrawToBitmap(bitmapLine, new Rectangle(0, 0, chartLine.Width, chartLine.Height));
                    chartDoughNut.DrawToBitmap(bitmapNut, new Rectangle(0, 0, widthNut, chartDoughNut.Height));

                    // Save the image to the specified file path
                    //bitmapLine.Save(filePath, ImageFormat.Png);
                    bitmapNut.Save(filePath, ImageFormat.Png);

                    // Dispose the bitmap object
                    bitmapNut.Dispose();
                    //bitmapLine.Dispose();

                    // Show a message to indicate successful export
                    MessageBox.Show("Chart exported successfully.");
                }
            }
            else
            {
                MessageBox.Show("No chart available. Create a chart first.");
                return;
            }
            
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (tableProcessing())
            {
                // Create a SaveFileDialog to choose the file location
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files|*.csv";
                saveFileDialog.Title = "Save DataGridView Data";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePath = saveFileDialog.FileName;

                    // Create a StringBuilder to store the data
                    StringBuilder sb = new StringBuilder();

                    // Append the column headers to the StringBuilder
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        sb.Append(column.HeaderText + ",");
                    }

                    sb.AppendLine();

                    // Append the data rows to the StringBuilder
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            sb.Append(cell.Value + ",");
                        }

                        sb.AppendLine();
                    }

                    // Write the data to the file
                    File.WriteAllText(filePath, sb.ToString());

                    MessageBox.Show("Data saved successfully.");
                }
            }
            else
            {
                MessageBox.Show("No Data available.");
                return;
            }
           
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] filePaths = openFileDialog.FileNames;

                mergeDataTable = MergeCsvFiles(filePaths);

                dataGridView.DataSource = null;
                // Set the DataGridView's data source to the populated DataTable
                dataGridView.DataSource = mergeDataTable;

                // set Average Efficiency in label
                double efficiency_AverageValues = summaryEfficiency(mergeDataTable);
                labelEfficiency.Text = efficiency_AverageValues.ToString("0.000");

                // Set Average Power Input in label
                double power_AverageValues = summaryPowerInput(mergeDataTable);
                labelPowerInput.Text = power_AverageValues.ToString("0.000");

                loadDataGraph();
                loadDataGraphDoughNut();
            }
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //Console.WriteLine($"event : {column.SortMode}");
            }
        }

        private bool tableProcessing()
        {
            if(dataGridView.DataSource == null)
            {
                return false;
            }
            return true;
        }
        
        private Chart loadDataGraph()
        {
            chartLine.Series.Clear();
            Series seriesChartLine;
            seriesChartLine = chartLine.Series.Add("Efficiency");
            
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.IsNewRow && row.Cells["Total_Eff_value"].Value != null)
                {
                    
                    double y = Convert.ToDouble(row.Cells["Total_Eff_value"].Value);
                    seriesChartLine.ChartType = SeriesChartType.SplineArea;
                    seriesChartLine.BackGradientStyle = GradientStyle.LeftRight;
                    seriesChartLine.BorderColor = Color.FromArgb(255, 128, 255);
                    seriesChartLine.Color = Color.FromArgb(241, 88, 127);
                    seriesChartLine.BorderWidth = 3;
                    seriesChartLine.BackSecondaryColor = Color.FromArgb(107, 83, 255);
                    seriesChartLine.Points.AddY(y);
                }
            }
            return chartLine;
        }
        private Chart loadDataGraphDoughNut()
        {
            chartDoughNut.Series.Clear();
            Series seriesDoughNut;
            seriesDoughNut = chartDoughNut.Series.Add("Kw Efficiency");

            double totalChillerEff, totalCTEff, totalCWPEff, totalCHWPEff;
            totalChillerEff = 0;
            totalCTEff = 0;
            totalCWPEff = 0;
            totalCHWPEff = 0;
            double count = 0;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.IsNewRow && row.Cells["Chiller_Eff_value"].Value != null)
                {

                    double y = Convert.ToDouble(row.Cells["Chiller_Eff_value"].Value);
                    totalChillerEff += y;
                    count++;
                }
                if (!row.IsNewRow && row.Cells["CT_Eff_value"].Value != null)
                {

                    double y = Convert.ToDouble(row.Cells["CT_Eff_value"].Value);
                    totalCTEff += y; 
                }
                if (!row.IsNewRow && row.Cells["CWP_Eff_value"].Value != null)
                {

                    double y = Convert.ToDouble(row.Cells["CWP_Eff_value"].Value);
                    totalCWPEff+= y;
                }
                if (!row.IsNewRow && row.Cells["CHWP_Eff_value"].Value != null)
                {

                    double y = Convert.ToDouble(row.Cells["CHWP_Eff_value"].Value);
                    totalCHWPEff += y;
                }
            }
            double averageChiller = count > 0 ? totalChillerEff / count : 0;
            double averageCT = count > 0 ? totalCTEff / count : 0;
            double averageCWP = count > 0 ? totalCWPEff / count : 0;
            double averageCHWP = count > 0 ? totalCHWPEff / count : 0;
            seriesDoughNut.ChartType = SeriesChartType.Doughnut;

            double efficiency_AverageValues = summaryEfficiency(mergeDataTable);

            // configure the dougNut display
            seriesDoughNut.BorderColor = Color.FromArgb(42, 45, 86);
            seriesDoughNut.Color = Color.FromArgb(192, 192, 255);
            seriesDoughNut.BackSecondaryColor = Color.FromArgb(255, 192, 120);
            seriesDoughNut.BorderWidth = 5;
            seriesDoughNut.Palette = ChartColorPalette.BrightPastel;
            seriesDoughNut.BackGradientStyle = GradientStyle.DiagonalLeft;
            

            // point Chiller
            seriesDoughNut.Points.AddXY($"{averageChiller.ToString("0.000")}", efficiency_AverageValues - (efficiency_AverageValues - (0.15 + averageChiller)));
            DataPoint dataPointChiller = seriesDoughNut.Points[0];
            dataPointChiller.LegendText = "Chiller";

            // point CT
            seriesDoughNut.Points.AddXY($"{averageCT.ToString("0.000")}", efficiency_AverageValues - (efficiency_AverageValues - (0.15 + averageCT)));
            DataPoint dataPointCT = seriesDoughNut.Points[1];
            dataPointCT.LegendText = "Cooling Tower";

            // point CWP
            seriesDoughNut.Points.AddXY($"{averageCWP.ToString("0.000")}", efficiency_AverageValues - (efficiency_AverageValues - (0.15 + averageCWP)));
            DataPoint dataPointCWP = seriesDoughNut.Points[2];
            dataPointCWP.LegendText = "CWP";

            // point CHWP
            seriesDoughNut.Points.AddXY($"{averageCHWP.ToString("0.000")}", efficiency_AverageValues - (efficiency_AverageValues - (0.15 + averageCHWP)));
            DataPoint dataPointCHWP = seriesDoughNut.Points[3];
            dataPointCHWP.LegendText = "Cooling Tower";

            Console.WriteLine($"averageChiller : {averageChiller}");

            return chartDoughNut;
        }

        private bool chartProcessing()
        {
            foreach (Series series in chartLine.Series)
            {
                if (series.Points.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static DataTable MergeCsvFiles(string[] filePaths)
        {

            DataTable mergedDataTable = new DataTable();
            string columnName_1 = "Chiller_Eff_value";
            string columnName_2 = "Total_Eff_Value";

            // Assume that the CSV files have the same structure (same columns)
            if (filePaths.Length > 0)
            {
                // Read the first CSV file to create the table structure
                using (StreamReader reader = new StreamReader(filePaths[0]))
                {
                    string[] headers = reader.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        mergedDataTable.Columns.Add(header);

                    }
                }

                // Read the remaining CSV files and merge the data
                foreach (string filePath in filePaths)
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        reader.ReadLine(); // Skip the headers

                        while (!reader.EndOfStream)
                        {
                            string[] rows = reader.ReadLine().Split(',');
                            DataRow dataRow = mergedDataTable.NewRow();
                            for (int i = 0; i < rows.Length; i++)
                            {
                                dataRow[i] = rows[i];
                            }
                            mergedDataTable.Rows.Add(dataRow);
                        }
                    }
                }
            }
            // Filter data contain #NAME?
            for (int i = mergedDataTable.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = mergedDataTable.Rows[i];

                // Assuming you want to filter rows that contain "#NAME?"
                if (row.ItemArray.Any(item => item.ToString().Contains("#NAME?")))
                {
                    mergedDataTable.Rows.RemoveAt(i);
                }
            }

            // filter peak data from chiller eff value
            int columnIndex_1 = mergedDataTable.Columns.IndexOf(columnName_1);
            Console.WriteLine($"Column Index : {columnIndex_1}");

            if (columnIndex_1 >= 0)
            {
                //Console.WriteLine("At least one condition is true");
                for (int i = mergedDataTable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = mergedDataTable.Rows[i];
                    decimal cellValue = Convert.ToDecimal(mergedDataTable.Rows[i][columnIndex_1]);
                    //Console.WriteLine($"Rows nomer {i}");
                    if (cellValue > 2 || cellValue < 0)
                    {
                        //Console.WriteLine("At least one condition is true");
                        mergedDataTable.Rows.RemoveAt(i);
                    }
                }
            }

            // filter peak data from total eff value
            int columnIndex_2 = mergedDataTable.Columns.IndexOf(columnName_2);
            Console.WriteLine($"Column Index : {columnIndex_2}");

            if (columnIndex_2 >= 0)
            {
                //Console.WriteLine("At least one condition is true");
                for (int i = mergedDataTable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = mergedDataTable.Rows[i];
                    decimal cellValue = Convert.ToDecimal(mergedDataTable.Rows[i][columnIndex_2]);
                    //Console.WriteLine($"Rows nomer {i}");
                    if (cellValue > 2 || cellValue < 0)
                    {
                        //Console.WriteLine("At least one condition is true");
                        mergedDataTable.Rows.RemoveAt(i);
                    }
                }
            }

            /*
            foreach (DataRow row in mergedDataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write(item + "\t");
                }
                Console.WriteLine();
            }*/
            summaryEfficiency(mergedDataTable);
            summaryPowerInput(mergedDataTable);
            return mergedDataTable;
        }

        private static double summaryEfficiency(DataTable mergedDataTable)
        {
            double efficiencyAverage;
            int columnIndex = mergedDataTable.Columns.IndexOf("Total_Eff_value");
            int rowCount = 0;

            foreach (DataRow row in mergedDataTable.Rows)
            {
                if(!row.IsNull(columnIndex))
                {
                    rowCount++;
                }
            }
            Console.WriteLine($"Rows : {rowCount}");
            List<object> columnValues = new List<object>();

            foreach (DataRow row in mergedDataTable.Rows)
            {
                object value = row[columnIndex]; // or row[columnName]
                columnValues.Add(value);
            }

            // sum the column values
            double sum = columnValues.Sum(value => Convert.ToDouble(value));
            //Console.WriteLine($"sum : {sum}");

            efficiencyAverage = sum / rowCount;
            Console.WriteLine($"Efficiency Average : {efficiencyAverage}");

            return efficiencyAverage;
        }

        private static double summaryPowerInput(DataTable mergedDataTable)
        {
            double powerInputAverage;
            int columnIndex = mergedDataTable.Columns.IndexOf("kW_TOTAL_value");
            int rowCount = 0;

            foreach (DataRow row in mergedDataTable.Rows)
            {
                if (!row.IsNull(columnIndex))
                {
                    rowCount++;
                }
            }
            Console.WriteLine($"Rows : {rowCount}");
            List<object> columnValues = new List<object>();

            foreach (DataRow row in mergedDataTable.Rows)
            {
                object value = row[columnIndex]; // or row[columnName]
                columnValues.Add(value);
            }

            // sum the column values
            double sum = columnValues.Sum(value => Convert.ToDouble(value));
            //Console.WriteLine($"sum : {sum}");

            powerInputAverage = sum / rowCount;
            Console.WriteLine($"Power Input Average : {powerInputAverage}");

            return powerInputAverage;
        }
    }
}
