using System.Data;


namespace AbsorbanceConvert
{
    public partial class AbsorbanceConverter : Form
    {
        private string selectedFormat = "txt"; // default format
        private DataTable mainTable = new DataTable();
        public AbsorbanceConverter()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                // WE first clean the table
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                // Do something when radioButton1 is checked
                selectedFormat = "csv";
            }
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                // Do something when radioButton2 is checked
                selectedFormat = "txt";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            // get it work
            var folderPath = folderBrowserDialog1.SelectedPath;
            var header = new string[] { "waveLength", "absorbance", "file" };

            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }
            else
            {
                // we assume that all absorbance data with ".txt" or other format are in the selected folder
                var files = Directory.GetFiles(folderPath, "*.txt"); // A txt file list
                // first insert the header
                foreach (var item in header)
                {
                    mainTable.Columns.Add(item);
                }
                foreach (var file in files)
                {
                    //var lines  = File.ReadAllLines(file); // we read all line one time, consider that the file is short
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            int lineNumber = 0;

                            while (!sr.EndOfStream)
                            {
                                string? line = sr.ReadLine(); // Use nullable string
                                if (line != null) // Check for null before using the line
                                {
                                    lineNumber++;
                                    // we want to skip the first 2 lines
                                    if (lineNumber > 2)
                                    {
                                        // get a array with fixed length
                                        var values = line.Split("\t");
                                        Console.WriteLine($"{values}\n");
                                        // we need to transform the array to a list to add the file name
                                        // now we can sure the list has 3 elements
                                        // first is wavelength, second is absorbance, third is file name
                                        var valueList = values.ToList();
                                        valueList.Add(Path.GetFileName(file));
                                        var arr = valueList.ToArray();
                                        //MessageBox.Show($"{Path.GetFileName(file)} is added");
                                        // now we can add the values to the table
                                        arr = arr.Select(element => element.Replace(",", ".")).ToArray();
                                        mainTable.Rows.Add(arr);
                                        //MessageBox.Show($"{Path.GetFileName(file)} is added");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // now we can show the table in the datagridview
            // attach the table to the datagridview
            dataGridView1.DataSource = mainTable;
            // we set the settings of the datagridview that user can not edit the data
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Get the file name from the text box
            string fileName = textBox2.Text;
            // Check if the file name is empty
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter a file name.");
                return;
            }
            string filePath = Path.Combine(folderBrowserDialog1.SelectedPath, $"{fileName}.{selectedFormat}");
            // Check if the file already exists
            if (File.Exists(filePath)) 
            {
                MessageBox.Show("File already exists. Please choose a different name.");
            }
            // check the selected format
            if (selectedFormat == "txt")
            {
                SaveAsTxt(filePath);
                MessageBox.Show("File saved 😊😊😊");
            }
            else if (selectedFormat == "csv")
            {
                SaveAsCsv(filePath);
                MessageBox.Show("File saved 😊😊😊");
            }
            else
            {
                MessageBox.Show("Unsupported file format.");
                return;
            }

        }
        private void SaveAsTxt(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = string.Join("\t", mainTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName));
                writer.WriteLine(header);

                foreach (DataRow row in mainTable.Rows)
                {
                    var line = string.Join("\t", row.ItemArray);
                    writer.WriteLine(line);
                }
            }
        }

        private void SaveAsCsv(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = string.Join(",", mainTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName));
                writer.WriteLine(header);

                foreach (DataRow row in mainTable.Rows)
                {
                    var line = string.Join(",", row.ItemArray);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
