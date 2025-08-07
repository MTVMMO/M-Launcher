using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ConfigCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string content_ = 
                txtRSS.Text + "\r\n" +
                txtServerIP.Text + "\r\n" +
                txtGSPort.Text + "\r\n" +
                txtCSPort.Text + "\r\n" +
                txtStartFile.Text + "\r\n" +
                //eu passei por aqui ass di macedo
                textUp.Text + "\r\n" +
                //fim
                TimeZone.Value.ToString();
                
            content_ = SecureStringManager.Encrypt(content_, "28755");
            SaveFile("Launcher.bmd", content_);
        }

        private void SaveFile(string FileName_, string Content_)
        {
            File.WriteAllText(FileName_, Content_);
            MessageBox.Show("File Saved.");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog() { Filter = "LauncherConfig(Launcher.bmd)|Launcher.bmd" };
            string ExecutablePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            openFileDialog1.InitialDirectory = ExecutablePath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
            {
                MessageBox.Show("Can't load config file!", "Error!");
            }
            string[] ConfigsList_ = Regex.Split(SecureStringManager.Decrypt(File.ReadAllText(filename), "28755"), "\r\n");
            txtRSS.Text = ConfigsList_[0];
            txtServerIP.Text = ConfigsList_[1];
            txtGSPort.Text = ConfigsList_[2];
            txtCSPort.Text = ConfigsList_[3];
            txtStartFile.Text = ConfigsList_[4];
            //eu passei por aqui ass di macedo
            textUp.Text = ConfigsList_[5];
            //fim
            TimeZone.Value            = Convert.ToInt32( ConfigsList_[6]);
            
        }

        private void txtRSS_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
