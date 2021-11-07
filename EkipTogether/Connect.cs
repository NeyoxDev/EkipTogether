using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EkipTogether
{
    public partial class ConnectForm : Form
    {
        public ConnectForm()
        {
            InitializeComponent();
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Saver.saveOptions();
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            var form = new CreateRoom();
            form.Show();
        }
        private void circularPicturBox1_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/ffTcfvdHGd");

        }
    }
}