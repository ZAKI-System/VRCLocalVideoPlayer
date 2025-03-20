using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCLocalVideoPlayer
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            foreach (var log in Logger.GetAllLogs())
            {
                string logLevel = Enum.GetName(typeof(Logger.LogLevel), log.Value).ToUpper();
                textBox1.Text += $"[{logLevel}]: {log.Key}" + Environment.NewLine;
            }
            Logger.LogAdded += Logger_LogAdded;
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        /// <summary>
        /// ログ追記
        /// </summary>
        /// <param name="obj"></param>
        private void Logger_LogAdded(KeyValuePair<string, Logger.LogLevel> obj)
        {
            string logLevel = Enum.GetName(typeof(Logger.LogLevel), obj.Value).ToUpper();
            textBox1.Text += $"[{logLevel}]: {obj.Key}" + Environment.NewLine;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.LogAdded -= Logger_LogAdded;
        }
    }
}
