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
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Setting up  the "openFileDialog"
                openFileDialog.InitialDirectory = $"c:\\{Environment.UserName}\\Downloads";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }

            tb_filePath.Text = filePath;
        }

        /// <summary>
        /// An event that gets triggered if the button gets clicked
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event args </param>
        private void btn_confirm_Click(object sender, EventArgs e)
        {
            // Check if all forms are filled
            if (!CheckForms()) return;

            var filePath = tb_filePath.Text;
            var originalChecksum = tb_checksum.Text;
            var algorithmTypes = new List<RadioButton>() { rb_md5, rb_sha1, rb_sha256, rb_sha512 };

            // Check which radio button is checked to determine the algorithm
            var type = "";
            foreach (var algorithmType in algorithmTypes)
            {
                if (algorithmType.Checked) type = algorithmType.Text.Replace(" ", "").ToLower();
            }
            Log.Information($"The selected algorithm for {filePath} is {type}.");

            // Set the result text for the output box
            rtb_output.Text = CompareHashes(GenerateChecksum(filePath, type), originalChecksum) == true ? "Checksum's are equal!" :
            "Generated Checksum is not equal to the provided Checksum!\nCheck if your selected algorithm is the same as the provided. If it's still not equal, re-download the file and try again.";
        }

        /// <summary>
        /// Checks if all mandatory forms are filled
        /// </summary>
        /// <returns> true, if all forms are filled and false if not. </returns>
        private bool CheckForms()
        {
            var valid = true;
            rtb_output.Text = "";

            try
            {
                if (string.IsNullOrEmpty(tb_filePath.Text))
                {
                    valid = false;
                    rtb_output.Text += "Please select a file!\n";

                    // Highlight the form to be filled and start the timer to reset the effect after 500 milliseconds
                    tb_filePath.BackColor = Color.Red;
                    timer1.Interval = 500;
                    timer1.Start();
                }
                if (string.IsNullOrEmpty(tb_checksum.Text))
                {
                    valid = false;
                    rtb_output.Text += "Please enter the provided checksum!";

                    // Highlight the form to be filled and start the timer to reset the effect after 500 milliseconds
                    tb_checksum.BackColor = Color.Red;
                    timer1.Interval = 500;
                    timer1.Start();
                }
            }
            catch (Exception ex)
            {
                rtb_output.Text = "Couldn't check if all forms are filled. Please check the logs!";
                Log.Error($"Couldn't check if all forms are filled: {ex}.");
                return false;
            }

            return valid;
        }

        /// <summary>
        /// Compares both, the generated hash and the provided one to check if they're equal or not
        /// </summary>
        /// <param name="generatedHash"> The generated hash </param>
        /// <param name="providedHash"> The provided hash </param>
        /// <returns> true, if both hashes are equal and true if not. </returns>
        private bool CompareHashes(string generatedHash, string providedHash)
        {
            return providedHash.ToLower().Equals(generatedHash);
        }

        /// <summary>
        /// Generates the checksum of the selected algorithm
        /// </summary>
        /// <param name="filePath"> The filepath of the selected file </param>
        /// <param name="type"></param>
        /// <returns></returns>
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

                                // "hash" is an array of bytes. When combining, a "-" gets inserted after every byte. Thats why we have to remove all "-" in order to compare
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                                Log.Information($"Successfully generated the SHA256 hash of {filePath}.");
                            }
                        }
                        break;

                    case "sha1":
                        using (var sha1 = SHA1.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = sha1.ComputeHash(stream);

                                // "hash" is an array of bytes. When combining, a "-" gets inserted after every byte. Thats why we have to remove all "-" in order to compare
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                                Log.Information($"Successfully generated the SHA1 hash of {filePath}.");
                            }
                        }
                        break;

                    case "sha512":
                        using (var sha512 = SHA512.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = sha512.ComputeHash(stream);

                                // "hash" is an array of bytes. When combining, a "-" gets inserted after every byte. Thats why we have to remove all "-" in order to compare
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                                Log.Information($"Successfully generated the SHA512 hash of {filePath}.");
                            }
                        }
                        break;

                    case "md5":
                        using (var md5 = MD5.Create())
                        {
                            using (var stream = File.OpenRead(filePath))
                            {
                                var hash = md5.ComputeHash(stream);

                                // "hash" is an array of bytes. When combining, a "-" gets inserted after every byte. Thats why we have to remove all "-" in order to compare
                                fileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                                Log.Information($"Successfully generated the MD5 hash of {filePath}.");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                rtb_output.Text = $"Could not generate the hash for {filePath}. Please check the logs!";
                Log.Error($"Could not generate the hash for {filePath}: {ex}");
            }
            return fileHash;
        }

        /// <summary>
        /// An event that gets triggered after an tick of the timer
        /// </summary>
        /// <param name="sender"> The sender </param>
        /// <param name="e"> Event args </param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();

                // After the timer stops, both forms' background are resetted to their default background color
                tb_filePath.BackColor = DefaultBackColor;
                tb_checksum.BackColor = DefaultBackColor;
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't handle the timer: {ex}.");
            }
        }
    }
}
