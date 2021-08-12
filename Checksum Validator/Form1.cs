using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Checksum_Validator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens a new FileDialog
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Contains event data</param>
        private void btn_selectFile_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            tb_filePath.Text = filePath;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            var filePath = tb_filePath.Text;
            var originalChecksum = tb_checksum.Text;
            var fileHash = "";

            // Generate file hash
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    rtb_output.Text = fileHash;
                }
            }
        }
    }
}
