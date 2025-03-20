using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCLocalVideoPlayer
{
    internal class VRCVideoLogReader
    {
        private string lastVideoUrl;
        private Task updateTask;
        private bool state;

        /// <summary>
        /// VideoURLが変更された時のイベント
        /// </summary>
        public event Action<string> VideoUrlChanged;

        public VRCVideoLogReader()
        {
            Logger.LogInfo("VRCVideoLogReader起動");
        }

        public void ChangeState(bool state)
        {
            this.state = state;
            if (state)
            {
                this.updateTask = Task.Run(async () =>
                {
                    while (this.state)
                    {
                        this.UpdateVideoUrl();
                        await Task.Delay(500);
                    }

                });
            }
        }

        /// <summary>
        /// VideoURLを更新
        /// </summary>
        private void UpdateVideoUrl()
        {
            // ログからURLを取る
            var videoUrl = this.GetVideoUrl();

            // 最後に取ったURLと違ったら
            if (this.lastVideoUrl != videoUrl)
            {
                this.lastVideoUrl = videoUrl;
                // URLがあればイベント起動
                if (videoUrl != "") this.VideoUrlChanged?.Invoke(videoUrl);
            }
        }

        /// <summary>
        /// VRChat最新のログからVideoURLを検索
        /// </summary>
        /// <returns>VideoURL</returns>
        public string GetVideoUrl()
        {
            string localLowPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow"));
            string directoryPath = Path.Combine(localLowPath, "VRChat", "VRChat");
            string searchPattern = "output_log_*.txt";

            // すべてのファイルを取得
            string[] files = Directory.GetFiles(directoryPath, searchPattern);
            // ファイル名から日時を抽出し、最新のファイルを選択
            var latestFile = files
                .Select(file => new
                {
                    FileName = file,
                    DateTime = ParseDateTimeFromFileName(Path.GetFileName(file))
                })
                .OrderByDescending(f => f.DateTime)
                .FirstOrDefault();
            if (latestFile == null)
            {
                return "";
            }

            // ファイルを逆から読む
            var reader = new ReverseLineReader(latestFile.FileName);
            foreach (string line in reader)
            {
                // ワールド入場まで到達したら終了
                if (line.Contains("[Behaviour] Entering world"))
                {
                    return "";
                }
                // 動画URLを検索
                if (line.Contains("[Video Playback]"))
                {
                    string pattern = @"'([^']*)'";
                    Match match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            // すり抜けても終了
            return "";

        }

        /// <summary>
        /// ファイル名から日時を解析するメソッド
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static DateTime ParseDateTimeFromFileName(string fileName)
        {
            string dateTimePart = fileName.Replace("output_log_", "").Replace(".txt", "");
            return DateTime.ParseExact(dateTimePart, "yyyy-MM-dd_HH-mm-ss", null);
        }
    }
}
