using Serilog;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Checksum_Validator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var logPath = ConfigurationManager.AppSettings["LogPath"];
            Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File($"{logPath}\\ChecksumValidator.log", outputTemplate: "{Timestamp:dd-MM-yyyy} | {Timestamp:HH:mm:ss} | [{Level}] | {Message:lj}{NewLine}{Exception}").MinimumLevel.Debug().CreateLogger();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChecksumValidatorMain());
        }
    }
}
