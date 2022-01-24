using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.IO.Compression;

namespace NorthstarModBrowserUpdater
{
    public partial class Form1 : Form
    {
        long gitId= 445768377;
        string ProgramLocation = "";
        GitHubClient git;
        public Form1()
        {

            InitializeComponent();
            git = new GitHubClient(new ProductHeaderValue("a"));
            findTitanfallLocation();
            downloadUpdate(getReleases().Result.Assets[0].BrowserDownloadUrl);
        }
        public void findTitanfallLocation()
        {

            if (File.Exists("NorthStarModBrowser.config"))
            {
                string TitanfallDir = File.ReadAllText("NorthStarModBrowser.config");
                if (File.Exists(Path.Combine(TitanfallDir, "Titanfall2.exe")))
                {
                    ProgramLocation = Path.Combine(TitanfallDir, ProgramLocation, "Norhtstarmodbrowser");
                    return;
                }

            }
            start:
            openFileDialog1.ShowDialog();
            if (Path.GetFileName(openFileDialog1.FileName) == "Titanfall2.exe")
            {
                ProgramLocation = Path.Combine(Path.GetDirectoryName(openFileDialog1.FileName), ProgramLocation);
            }
            else
            {
                MessageBox.Show("Selected File wasnt Titanfall2.exe");
                goto start;
            }
            File.WriteAllText("NorthStarModBrowser.config", Path.GetDirectoryName(openFileDialog1.FileName));

        }
        string installLocation;
        public async void downloadUpdate(string url)
        {
            installLocation = Path.Combine(ProgramLocation, "nsmbUpdate.zip");
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadProgressChanged += progressChanged;
            webClient.DownloadFileCompleted += fileDownloaded;
            webClient.Headers.Add("user-agent", "Anything");
            if (System.IO.Directory.Exists(installLocation)) System.IO.Directory.Delete(installLocation, true);
            await webClient.DownloadFileTaskAsync(new Uri(url), installLocation);
        }
        public void fileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            
            if (System.IO.Directory.Exists(Path.Combine(ProgramLocation, "Norhtstarmodbrowser"))) System.IO.Directory.Delete(Path.Combine(ProgramLocation, "Norhtstarmodbrowser"),true);
            ZipFile.ExtractToDirectory(installLocation, ProgramLocation);
            if (File.Exists("NorthStarModBrowser.exe")) File.Delete("NorthStarModBrowser.exe");
            File.Copy(Path.Combine(ProgramLocation, "Norhtstarmodbrowser", "NorthStarModBrowser.exe"), "NorthStarModBrowser.exe");
            label1.Text = "Update Complete";

        }
        public void progressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        public async Task<Release> getReleases()
        {
            // Retrieve a List of Releases in the Repository, and get latest using [0]-subscript
            var latest = git.Repository.Release.GetAll(gitId).Result[0];
            return latest;
        }

    }
}
