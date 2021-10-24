using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Checksum_Validator
{
    public partial class ChecksumValidatorMain : Form
    {
        public ChecksumValidatorMain()
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
                openFileDialog.Filter = "All files (*.*)|*.*";
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
            // Check if all forms are filled
            if (!CheckForms()) return;

            var filePath = tb_filePath.Text;
            var originalChecksum = tb_checksum.Text;
            var algorithmTypes = new List<RadioButton>() { rb_md5, rb_sha1, rb_sha256, rb_sha512 };

            var type = "";
            foreach (var algorithmType in algorithmTypes)
            {
                if (algorithmType.Checked) type = algorithmType.Text.Replace(" ", "").ToLower();
            }

            rtb_output.Text = CompareHashes(GenerateChecksum(filePath, type), originalChecksum) == true ? "Checksum's are equal!" :
            "Generated Checksum is not equal to the provided Checksum!\nCheck if your selected algorithm is the same as the provided. If it's still not equal, re-download the file and try again.";
        }

        private bool CheckForms()
        {
            var valid = true;
            rtb_output.Text = "";

            if (string.IsNullOrEmpty(tb_filePath.Text))
            {
                valid = false;
                rtb_output.Text += "Please select a file!\n";
                tb_filePath.BackColor = Color.Red;
                timer1.Interval = 500;
                timer1.Start();
            }
            if (string.IsNullOrEmpty(tb_checksum.Text))
            {
                valid = false;
                rtb_output.Text += "Please enter the provided checksum!";
                tb_checksum.BackColor = Color.Red;
                timer1.Interval = 500;
                timer1.Start();
            }

            return valid;
        }

        private bool CompareHashes(string generatedHash, string providedHash)
        {
            return providedHash.ToLower().Equals(generatedHash);
        }

        private string GenerateChecksum(string filePath, string type)
        {
            var fileHash = "";

            try
            {
                switch (type)
                {
                    case "sha256":
                        using (var sha256 = SHA256.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = sha256.ComputeHash(stream);
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                            }
                        }
                        break;

                    case "sha1":
                        using (var sha1 = SHA1.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = sha1.ComputeHash(stream);
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                            }
                        }
                        break;

                    case "sha512":
                        using (var sha512 = SHA512.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = sha512.ComputeHash(stream);
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                            }
                        }
                        break;

                    case "md5":
                        using (var md5 = MD5.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = md5.ComputeHash(stream);
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Could not generate the hash for {filePath}: {ex}");
            }
            return fileHash;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            tb_filePath.BackColor = DefaultBackColor;
            tb_checksum.BackColor = DefaultBackColor;
        }
    }
}
