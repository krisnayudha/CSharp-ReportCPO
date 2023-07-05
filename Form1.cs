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
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        public Form1()
        {
            InitializeComponent();
            //InitializeControls();
        }
        //make button feature by program
        /*private void InitializeControls()
        {
            // Create and configure the Button control
            ButtonLoad = new Button();
            ButtonLoad.Text = "Load CSV";
            ButtonLoad.Click += buttonLoad_Click;
            ButtonLoad.Location = new System.Drawing.Point(12, 12);
            Controls.Add(ButtonLoad);

            // Create and configure the DataGridView control
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new System.Drawing.Point(12, 48);
            Controls.Add(dataGridView);
        }*/

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string csvFilePath = openFileDialog.FileName;

                dataTable = ReadCsvToDataTable(csvFilePath);

                // Set the DataGridView's data source to the populated DataTable
                dataGridView.DataSource = dataTable;
            }
        }

        private DataTable ReadCsvToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();

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
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = rows[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            return dataTable;
        }

    }
}
