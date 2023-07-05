using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testC_
{
    public partial class Form2 : Form
    {
        private DataTable mergedDataTable;
        public Form2()
        {
            InitializeComponent();
            
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                directoryTextBox.Text = selectedPath;
                fileSystemWatcher.Path = selectedPath;
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                fileSystemWatcher.Changed += FileSystemWatcher_Changed;

                //DataTable newDataTable = ReadCsvFile(selectedPath);

                // Enable the set button
                //setButton.Enabled = true;
            }
        }

        private void FileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            DataTable newDataTable = ReadCsvFile(filePath);
        }

        private void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            DataTable newDataTable = ReadCsvFile(filePath);

            if (newDataTable != null)
            {
                if (mergedDataTable == null)
                {
                    // If mergedDataTable is null, create a new DataTable using the structure of the new DataTable
                    mergedDataTable = newDataTable.Copy();
                    dataGridView.Invoke((MethodInvoker)delegate
                    {
                        dataGridView.DataSource = mergedDataTable;
                    });
                }
                else
                {
                    // Merge the new DataTable with the existing merged DataTable
                    mergedDataTable.Merge(newDataTable, true, MissingSchemaAction.Add);
                }
            }
        }
        private static DataTable ReadCsvFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string[] headers = reader.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    while (!reader.EndOfStream)
                    {
                        string[] rows = reader.ReadLine().Split(',');
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < rows.Length; i++)
                        {
                            dataRow[i] = rows[i];
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading CSV file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            saveFileDialog.FileName = "merged.csv";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string csvFilePath = saveFileDialog.FileName;

                ExportDataTableToCsv(mergedDataTable, csvFilePath);

                MessageBox.Show("CSV file exported successfully!", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static void ExportDataTableToCsv(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Append the column headers
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            // Append the rows
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            // Write the contents to the CSV file
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
