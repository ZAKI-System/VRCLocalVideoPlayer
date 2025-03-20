using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCLocalVideoPlayer
{
    public partial class Form1 : Form
    {
        private VRCVideoLogReader logReader;
        private Process vlcProcess;
        private readonly string[] extensionIds = new string[0];
        private readonly List<ExtensionModel> extensions = new List<ExtensionModel>();

        public Form3 LogWindow { get; set; }

        public Form1()
        {
            InitializeComponent();
            try
            {
                JsonNode json = JsonNode.Parse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtensionList.json")));
                this.extensionIds = json.AsArray().Select((x) => x.ToString()).ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
            InitializeWebView2();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.logReader = new VRCVideoLogReader();
            this.logReader.VideoUrlChanged += LogReader_VideoUrlChanged;
        }

        private void LogReader_VideoUrlChanged(string obj)
        {
            this.Invoke(new Action(() =>
            {
                if (!(this.vlcProcess?.HasExited ?? true))
                {
                    this.vlcProcess.Kill();
                }
                if (obj.Contains("//nico.ms/") || obj.EndsWith(".m3u8"))
                {
                    webView21.CoreWebView2.Navigate("https://www.youtube.com");
                    this.vlcProcess = Process.Start(@"C:\Program Files\VideoLAN\VLC\vlc.exe", $"\"{obj}\"");
                }
                else
                {
                    webView21.CoreWebView2.Navigate(obj);
                }
            }));
        }

        /// <summary>
        /// WebView2の事前設定
        /// </summary>
        private async void InitializeWebView2()
        {
            Logger.LogInfo("InitializeWebView2開始");
            await ExtensionInstaller.Install(this.extensionIds);
            // 拡張機能を有効化
            var options = new CoreWebView2EnvironmentOptions
            {
                AreBrowserExtensionsEnabled = true
            };
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            // WebView2の初期化
            await webView21.EnsureCoreWebView2Async(environment);
            Logger.LogInfo("WebView2の初期化完了");
        }

        /// <summary>
        /// WebView2の初期化完了時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebView2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            Logger.LogInfo("WebView2の初期化完了時処理開始");
            // 拡張機能を追加
            try
            {
                string extDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");
                foreach (var extId in this.extensionIds)
                {
                    Logger.LogInfo(extId + " 追加");
                    string extDirIn = Path.Combine(extDir, extId);
                    var extension = await webView21.CoreWebView2.Profile.AddBrowserExtensionAsync(extDirIn);
                    JsonNode manifest = JsonNode.Parse(File.ReadAllText(Path.Combine(extDirIn, "manifest.json")));
                    string name = manifest["name"].ToString();
                    string optionsPage = manifest["options_ui"]?["page"]?.ToString();
                    string popup = (manifest["browser_action"] ?? manifest["action"])?["default_popup"]?.ToString();
                    this.extensions.Add(new ExtensionModel()
                    {
                        Id = extension.Id,
                        StoreId = extId,
                        Name = manifest["name"].ToString(),
                        OptionsUri = optionsPage != null ? $"chrome-extension://{extension.Id}/{optionsPage}" : null,
                        PopupUri = popup != null ? $"chrome-extension://{extension.Id}/{popup}" : null
                    });
                    Logger.LogInfo(manifest["name"].ToString() + " 追加完了");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("拡張機能の追加でエラーが発生しました\r\n" + ex.ToString());
                MessageBox.Show("拡張機能の追加でエラーが発生しました\r\n" + ex.ToString(), AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            //debug
            foreach (var item in this.extensions)
            {
                Debug.WriteLine(item.Id);
                Debug.WriteLine(item.StoreId);
                Debug.WriteLine(item.Name);
                Debug.WriteLine(item.OptionsUri);
                Debug.WriteLine(item.PopupUri);
            }
            // ページ読み込み
            webView21.CoreWebView2.Navigate(ConfigurationManager.AppSettings.GetValues("homeUrl")?.FirstOrDefault() ?? "about:blank");
            // ログ監視開始
            this.logReader.ChangeState(true);
        }

        /// <summary>
        /// WwbView2 URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Logger.LogInfo(e.Uri + " に移動");
            textBox1.Text = e.Uri;
            webView21.Focus();
        }

        /// <summary>
        /// 𝑱𝒂𝒗𝒂𝑺𝒄𝒓𝒊𝒑𝒕 𝒊𝒏𝒋𝒆𝒄𝒕𝒊𝒐𝒏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            webView21.ExecuteScriptAsync(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InjectScript.js")));
        }

        /// <summary>
        /// back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (webView21.CoreWebView2.CanGoBack)
            {
                webView21.CoreWebView2.GoBack();
            }
        }

        /// <summary>
        /// forward
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (webView21.CoreWebView2.CanGoForward)
            {
                webView21.CoreWebView2.GoForward();
            }
        }

        /// <summary>
        /// reload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadButton_Click(object sender, EventArgs e)
        {
            webView21.CoreWebView2.Reload();
        }

        /// <summary>
        /// URL key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string url = textBox1.Text;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    // URLが有効な形式かを確認
                    if (!url.StartsWith("http://") && !url.StartsWith("https://")
                        && !url.StartsWith("edge://")
                        && !url.StartsWith("extension://") && !url.StartsWith("chrome-extension://"))
                    {
                        url = "https://" + url;
                    }
                    webView21.CoreWebView2.Navigate(url);
                }
            }
        }

        /// <summary>
        /// menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this.extensions);
            void OnUrlSent(string url)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    webView21.CoreWebView2.Navigate(url);
                }
            }
            form2.UrlSent += OnUrlSent;
            form2.ShowDialog(this);
            form2.UrlSent -= OnUrlSent;
        }


    }
}
