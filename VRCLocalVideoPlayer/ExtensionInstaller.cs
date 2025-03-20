using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VRCLocalVideoPlayer
{
    internal class ExtensionInstaller
    {
        /// <summary>
        /// extensionをインストール
        /// </summary>
        /// <param name="extensionIds">インストールするIDの一覧</param>
        /// <returns></returns>
        public static async Task Install(string[] extensionIds)
        {
            Logger.LogInfo("Extension Install開始");
            string extDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");
            if (!Directory.Exists(extDir))
            {
                Directory.CreateDirectory(extDir);
            }

            // extensionごと
            foreach (var extId in extensionIds)
            {
                string extDirIn = Path.Combine(extDir, extId);
                if (!Directory.Exists(extDirIn))
                {
                    Logger.LogInfo(extId + " ダウンロード");
                    Directory.CreateDirectory(extDirIn);
                    using (MemoryStream extBin = await ExtensionInstaller.DownloadExtension(extId))
                    {
                        ExtensionInstaller.UnpackCrxFile(extBin, extDirIn);
                    }
                }
            }
        }

        /// <summary>
        /// extensionをダウンロード
        /// </summary>
        /// <param name="extId">extensionのID</param>
        /// <returns>ダウンロードしたextensionのstream</returns>
        private static async Task<MemoryStream> DownloadExtension(string extId)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://edge.microsoft.com/extensionwebstorebase/v1/crx?response=redirect&x=id%3D{extId}%26installsource%3Dondemand%26uc";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                return new MemoryStream(fileBytes);
            }
        }

        /// <summary>
        /// crxを展開
        /// </summary>
        /// <param name="crxStream">crxのstream</param>
        /// <param name="destinationPath">展開先</param>
        /// <exception cref="Exception">展開失敗時</exception>
        private static void UnpackCrxFile(Stream crxStream, string destinationPath)
        {
            const int MinimumCrxHeaderLength = 12;

            string tempFilename;

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            if (crxStream.Length < MinimumCrxHeaderLength)
                throw new Exception("Could not find Crx file header at the begin of the file.");

            var buffer = new byte[4];
            crxStream.Read(buffer, 0, buffer.Length);
            if (Encoding.ASCII.GetString(buffer) != "Cr24")
                throw new Exception("Invalid Crx file header");

            crxStream.Read(buffer, 0, buffer.Length);
            var version = BitConverter.ToUInt32(buffer, 0);
            if (version != 3)
                throw new Exception(string.Format("Invalid Crx version ({0}). Only Crx version 3 is supported.", version));

            crxStream.Read(buffer, 0, buffer.Length);
            var headerLength = BitConverter.ToUInt32(buffer, 0);
            if (crxStream.Length < crxStream.Position + headerLength)
                throw new Exception(string.Format("Invalid Crx header length ({0}).", headerLength));

            crxStream.Seek(headerLength, SeekOrigin.Current);


            tempFilename = Path.GetTempFileName();
            File.Delete(tempFilename);
            using (var fileStream = File.Create(tempFilename))
            {
                crxStream.CopyTo(fileStream);
            }

            using (var zipArchive = ZipFile.OpenRead(tempFilename))
            {
                zipArchive.ExtractToDirectory(destinationPath);
            }

            File.Delete(tempFilename);
        }
    }
}
