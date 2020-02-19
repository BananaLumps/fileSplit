using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace fileSplit
{
    public partial class Form1 : Form
    {
        public string fileContent = string.Empty;
        public string filePath = string.Empty;
        public string fileExtension = string.Empty;
        public string fileName = string.Empty;
        public string outputDir = string.Empty;
        public OpenFileDialog openFileDialog = new OpenFileDialog();
        public int currentLine = 0;
        public int fileNum = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private static async void WriteCharacters(string input, string od, string fname, string fnum, string fext)
        {
            using (StreamWriter file =
                        new StreamWriter(od + @"\" + fname + fnum + fext, true))
            {
                await file.WriteAsync(input);
            }
        }

        [STAThread]
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                label1.Text = filePath;
                fileName = openFileDialog.SafeFileName.Split('.')[0];
                fileExtension = "." + openFileDialog.SafeFileName.Split('.')[1];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            Application.DoEvents();
            progressBar.Value = 0;
            //Read the contents of the file into a stream
            var fileStream = openFileDialog.OpenFile();
            StringBuilder buffer = new StringBuilder();

            using (StreamReader sr = new StreamReader(fileStream))
            {
                Stream baseStream = sr.BaseStream;
                long length = baseStream.Length;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (currentLine > Convert.ToInt32(textBox1.Text))
                    {
                        buffer.AppendLine(line);
                        currentLine = 0;
                        WriteCharacters(buffer.ToString(), outputDir, fileName, fileNum.ToString(), fileExtension);
                        fileNum++;
                        buffer = new StringBuilder();
                    }
                    else
                    {
                        buffer.AppendLine(line);
                        currentLine++;
                    }
                    progressBar.Value = Convert.ToInt32((double)baseStream.Position / length * 100);
                    Application.DoEvents();
                }
                fileStream.Dispose();
                label1.Text = "Done, Select another file";
                Thread.Sleep(200);
                button2.Enabled = true;
                progressBar.Value = 0;
                currentLine = 0;
                fileNum = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                outputDir = folderBrowserDialog1.SelectedPath;
                label3.Text = outputDir;
            }
        }
    }
}