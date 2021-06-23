using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace MergePDF_s
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WriteLog(textBox3, " - Selecione a Pasta Com os PDF's.");
            WriteLog(textBox3, 
                " - Pdf's no formato ( Arquivo-1.pdf, Arquivo-2.pdf, Arquivo-3.pdf )." + 
                Environment.NewLine +
                " ---------------------------------------------------------------------------------------------------------" + 
                Environment.NewLine);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result.ToString() == "Ok")
            {
                
                textBox1.Text = dialog.FileName;
                WriteLog(textBox3, " - PDF's em : " + dialog.FileName);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            CommonFileDialogResult result = dialog.ShowDialog();
            if(result.ToString() == "Ok")
            {
                textBox2.Text = dialog.FileName;
                WriteLog(textBox3, " - Salvar em : " + dialog.FileName);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                button3.Enabled = false;
                WriteLog(textBox3,
                    " ---------------------------------------------------------------------------------------------------------" +
                    Environment.NewLine +
                    " - Gerando Arquivos :" + Environment.NewLine +
                    Environment.NewLine);
                try
                {


                    DirectoryInfo directoryInfo = new DirectoryInfo(textBox1.Text);

                    //Get the PDF files in folder path into FileInfo
                    FileInfo[] files = directoryInfo.GetFiles("*.pdf");

                    List<String> Files = new List<string>();
                    foreach (FileInfo file in files)
                    {
                        if (file.Name.EndsWith("-1.pdf")) { Files.Add(file.Name.Replace("-1.pdf", "")); }
                    }
                    progressBar1.Maximum = Files.Count;
                    progressBar1.Value = 0;
                    foreach (String FileName in Files)
                    {

                        MergeFileAsync(FileName, directoryInfo);
                        progressBar1.Value++;
                    }
                    button3.Enabled = true;
                    WriteLog(textBox3, Environment.NewLine + " --- Finalizado ---");
                }
                catch (Exception error)
                {
                    WriteLog(textBox3, Environment.NewLine + " ( " + error + " ) ");
                    button3.Enabled = true;
                }
            }
            else
            {
                if (textBox1.Text == "")
                {
                    WriteLog(textBox3, "  * Selecione a Local com os PDF's * ");
                }
                if (textBox2.Text == "")
                {
                    WriteLog(textBox3, "  * Selecione o Local Para Salvar * ");
                }
            }
        }
        async void MergeFileAsync(String FileName, DirectoryInfo directoryInfo)
        {
            await MergeFile(FileName, directoryInfo);
        }
        private Task MergeFile(String FileName, DirectoryInfo directoryInfo)
        {
            //Create a new PDF document 
            PdfDocument document = new PdfDocument();
            document.EnableMemoryOptimization = true;

            FileInfo[] files = directoryInfo.GetFiles(FileName + "*.pdf");
            foreach (FileInfo file in files)
            {
                //Load the PDF document 
                FileStream fileStream = new FileStream(file.FullName, FileMode.Open);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(fileStream);

                //Merge PDF file
                PdfDocumentBase.Merge(document, loadedDocument);

                //Close the existing PDF document 
                loadedDocument.Close(true);
            }
            //Save the PDF document
            string filename = textBox2.Text + "\\" + FileName + ".pdf";

            WriteLog(textBox3, "  § Created :" + filename);
            FileStream doc = new FileStream(filename, FileMode.Create);
            document.Save(doc);
            document.Close(true);
            Thread.Sleep(100);
            return Task.CompletedTask;
        }

        public void WriteLog(TextBox tb, string log)
        {
            tb.AppendText(log + Environment.NewLine);
        }
    }
}