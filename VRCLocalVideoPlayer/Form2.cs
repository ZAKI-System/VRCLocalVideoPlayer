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
    public partial class Form2 : Form
    {
        private readonly List<ExtensionModel> extensions;

        public event Action<string> UrlSent;

        public Form2(List<ExtensionModel> extensions)
        {
            InitializeComponent();
            this.extensions = extensions;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            listBox1.Items.AddRange(this.extensions.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.UrlSent?.Invoke("edge://history");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("選択してください", "Menu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.UrlSent?.Invoke(((ExtensionModel)listBox1.SelectedItem).OptionsUri);
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("選択してください", "Menu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.UrlSent?.Invoke(((ExtensionModel)listBox1.SelectedItem).PopupUri);
            this.Close();
        }

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            if (form1.LogWindow != null)
            {
                form1.LogWindow.Activate();
                this.Close();
                return;
            }
            form1.LogWindow = new Form3();
            form1.LogWindow.Show(form1);
            void LogWindowClosed(object sender2, EventArgs e2)
            {
                form1.LogWindow.FormClosed -= LogWindowClosed;
                form1.LogWindow = null;
            }
            form1.LogWindow.FormClosed += LogWindowClosed;
            this.Close();
            form1.LogWindow.Activate();
        }
    }
}
