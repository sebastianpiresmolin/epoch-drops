using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EpochDropsUploader
{
    public partial class Form1 : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private List<FileSystemWatcher> watchers = new();
        private TextBox logBox;

        public Form1()
        {
            InitializeComponent();

            string exePath = Application.ExecutablePath;
            StartupHelper.AddToStartup("EpochDropsUploader", exePath);

            logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10)
            };
            Controls.Add(logBox);

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Change installation path", null, OnChangePathClicked);
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            trayIcon = new NotifyIcon
            {
                Text = "Epoch Drops Uploader",
                Icon = new Icon("favicon.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            // Optional: hide window and only use tray
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();

            StartWatching();
        }

        public static bool IsValidRealm(string addonFilePath, string allowedRealm)
        {
            try
            {
                if (!File.Exists(addonFilePath))
                    return false;

                var firstLine = File.ReadLines(addonFilePath).FirstOrDefault();
                return firstLine?.Trim() == $"local allowedRealm = \"{allowedRealm}\"";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking realm: {ex.Message}");
                return false;
            }
        }

        private void StartWatching()
        {
            foreach (var w in watchers)
                w.Dispose();
            watchers.Clear();

            var paths = GetAllSavedVariablesPaths();

            foreach (var path in paths)
            {
                try
                {
                    var watcher = new FileSystemWatcher(path, "Epoch_Drops.lua")
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                        IncludeSubdirectories = false,
                        EnableRaisingEvents = true
                    };

                    watcher.Changed += OnEpochDropsChanged;
                    watcher.Created += OnEpochDropsChanged;

                    watchers.Add(watcher);
                    Log($"🔍 Watching: {Path.Combine(path, "Epoch_Drops.lua")}");
                }
                catch (Exception ex)
                {
                    Log($"⚠️ Failed to watch {path}: {ex.Message}");
                }
            }
        }

        private string ExtractJsonFromLua(string luaContent)
        {
            // Match: Epoch_DropsJSON = "...."
            var match = Regex.Match(luaContent, @"Epoch_DropsJSON\s*=\s*""(.*)""", RegexOptions.Singleline);

            if (!match.Success)
                throw new Exception("Epoch_DropsJSON not found in Lua file");

            var encodedJson = match.Groups[1].Value;

            // Unescape Lua string (e.g. turn `\\n` into `\n`)
            var json = Regex.Unescape(encodedJson);

            return json;
        }




        private async void OnEpochDropsChanged(object sender, FileSystemEventArgs e)
        {
            var config = Config.Load();
            var addonLuaPath = Path.Combine(config.WowRootPath, "Interface", "AddOns", "Epoch_Drops", "epoch_drops.lua");

            if (!IsValidRealm(addonLuaPath, Secrets.AllowedRealm))
            {
                Log("🚫 Upload aborted. Realm check failed.");
                return;
            }

            if (!File.Exists(e.FullPath))
            {
                Log("⚠️ File does not exist: " + e.FullPath);
                return;
            }

            Log($"📂 Detected change in: {e.FullPath}");
            string json;

            try
            {
                await Task.Delay(1000); // let the file settle

                try
                {
                    var luaRaw = File.ReadAllText(e.FullPath);
                    json = ExtractJsonFromLua(luaRaw);
                }
                catch (Exception ex)
                {
                    Log("⚠️ Error parsing Lua or converting to JSON: " + ex.Message);
                    return;
                }

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5223/upload");
                request.Headers.Add("X-Upload-Key", Secrets.UploadKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var newName = e.FullPath + ".uploaded_" + DateTime.Now.Ticks;
                    File.Move(e.FullPath, newName);
                    Log("✅ Upload succeeded. File renamed to: " + newName);
                }
                else
                {
                    Log($"❌ Upload failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log("⚠️ Error during upload: " + ex.Message);
            }
        }


        private void OnChangePathClicked(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select your World of Warcraft installation folder"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var newPath = dialog.SelectedPath;
                Config.Save(new Config { WowRootPath = newPath });
                MessageBox.Show("Installation path updated.", "Epoch Drops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StartWatching();
            }
        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            trayIcon.Visible = false;
            base.OnFormClosing(e);
        }

        private List<string> GetAllSavedVariablesPaths()
        {
            var config = Config.Load();
            var wowRoot = config.WowRootPath;
            var result = new List<string>();
            var accountRoot = Path.Combine(wowRoot, "WTF", "Account");

            if (!Directory.Exists(accountRoot))
            {
                Log("⚠️ Account folder not found.");
                return result;
            }

            foreach (var accountDir in Directory.GetDirectories(accountRoot))
            {
                var savedVars = Path.Combine(accountDir, "SavedVariables");
                if (Directory.Exists(savedVars))
                    result.Add(savedVars);
            }

            return result;
        }

        private void Log(string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(() => logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n"));
            }
            else
            {
                logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            }

            Console.WriteLine(message);
        }
    }
}
