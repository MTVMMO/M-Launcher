﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Launcher.Exile;
using Launcher.Properties;
using Microsoft.Win32;

namespace Launcher
{
	public partial class Options : Form
	{
        public Options()
        {
            this.InitializeComponent();
            this.comboBox1.Visible = false;
            this.comboBox3.Visible = true;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Webzen\\Mu\\Config");
            if (registryKey != null)
            {
                if (registryKey.GetValue("MusicOnOff") != null && registryKey.GetValue("SoundOnOff") != null && (registryKey.GetValue("Resolution") != null && registryKey.GetValue("ColorDepth") != null) && registryKey.GetValue("ID") != null && registryKey.GetValue("LangSelection") != null)
                {
                    int num1 = (int)registryKey.GetValue("MusicOnOff");
                    int num2 = (int)registryKey.GetValue("SoundOnOff");
                    int num3 = (int)registryKey.GetValue("Resolution");
                    int num4 = (int)registryKey.GetValue("ColorDepth");
                    string str1 = (string)registryKey.GetValue("ID");
                    string str2 = (string)registryKey.GetValue("LangSelection");
                    this.textBox1.Text = str1;
                        switch (num3)
                        {
                            case 0:
                                this.comboBox1.SelectedIndex = 0;
                                break;
                            case 1:
                                this.comboBox1.SelectedIndex = 0;
                                break;
                            case 2:
                                this.comboBox1.SelectedIndex = 1;
                                break;
                            case 3:
                                this.comboBox1.SelectedIndex = 2;
                                break;
                            case 4:
                                this.comboBox1.SelectedIndex = 3;
                                break;
                            case 5:
                                this.comboBox1.SelectedIndex = 4;
                                break;
                            case 6:
                                this.comboBox1.SelectedIndex = 5;
                                break;
                            case 7:
                                this.comboBox1.SelectedIndex = 6;
                                break;
                        }
                    
                    
                    {
                        try
                        {
                            string[] strArray = System.IO.File.ReadAllLines("LauncherOption.if");
                            switch (strArray[0])
                            {
                                case "DevModeIndex:1":
                                    this.comboBox3.SelectedIndex = 0;
                                    break;
                                case "DevModeIndex:10":
                                    this.comboBox3.SelectedIndex = 9;
                                    break;
                                case "DevModeIndex:2":
                                    this.comboBox3.SelectedIndex = 1;
                                    break;
                                case "DevModeIndex:3":
                                    this.comboBox3.SelectedIndex = 2;
                                    break;
                                case "DevModeIndex:4":
                                    this.comboBox3.SelectedIndex = 3;
                                    break;
                                case "DevModeIndex:5":
                                    this.comboBox3.SelectedIndex = 4;
                                    break;
                                case "DevModeIndex:6":
                                    this.comboBox3.SelectedIndex = 5;
                                    break;
                                case "DevModeIndex:7":
                                    this.comboBox3.SelectedIndex = 6;
                                    break;
                                case "DevModeIndex:8":
                                    this.comboBox3.SelectedIndex = 7;
                                    break;
                                case "DevModeIndex:9":
                                    this.comboBox3.SelectedIndex = 8;
                                    break;
                            }
                            this.textBox1.Text = strArray[2].Remove(0, 3);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    registryKey = Registry.CurrentUser.CreateSubKey("Software\\Webzen\\Mu\\Config");
                    registryKey.CreateSubKey("MusicOnOFF");
                    registryKey.CreateSubKey("SoundOnOFF");
                    registryKey.CreateSubKey("ID");
                    registryKey.CreateSubKey("Resolution");
                    registryKey.CreateSubKey("LangSelection");
                    registryKey.CreateSubKey("ColorDepth");
                    registryKey.SetValue("MusicOnOFF", (object)0, RegistryValueKind.DWord);
                    registryKey.SetValue("SoundOnOFF", (object)0, RegistryValueKind.DWord);
                    registryKey.SetValue("ID", (object)"", RegistryValueKind.String);
                    registryKey.SetValue("Resolution", (object)0, RegistryValueKind.DWord);
                    registryKey.SetValue("LangSelection", (object)"Eng", RegistryValueKind.String);
                    registryKey.SetValue("ColorDepth", (object)0, RegistryValueKind.DWord);
                }
            }
            else
                registryKey = Registry.CurrentUser.CreateSubKey("Software\\Webzen\\Mu\\Config");
            registryKey.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.pictureBox2.Cursor = Cursors.Hand;
            this.DialogResult = DialogResult.OK;
            RegistryKey subKey = Registry.CurrentUser.CreateSubKey("Software\\Webzen\\Mu\\Config");
            if (subKey != null)
            

                if (this.textBox1.Text != null)
                        subKey.SetValue("ID", (object)this.textBox1.Text, RegistryValueKind.String);
                    else
                        subKey.SetValue("ID", (object)"", RegistryValueKind.String);
                    switch (this.comboBox1.SelectedIndex)
                    {
                        case 0:
                            subKey.SetValue("Resolution", (object)1, RegistryValueKind.DWord);
                            break;
                        case 1:
                            subKey.SetValue("Resolution", (object)2, RegistryValueKind.DWord);
                            break;
                        case 2:
                            subKey.SetValue("Resolution", (object)3, RegistryValueKind.DWord);
                            break;
                        case 3:
                            subKey.SetValue("Resolution", (object)4, RegistryValueKind.DWord);
                            break;
                        case 4:
                            subKey.SetValue("Resolution", (object)5, RegistryValueKind.DWord);
                            break;
                        case 5:
                            subKey.SetValue("Resolution", (object)6, RegistryValueKind.DWord);
                            break;
                        case 6:
                            subKey.SetValue("Resolution", (object)7, RegistryValueKind.DWord);
                            break;
                        default:
                            subKey.SetValue("Resolution", (object)1, RegistryValueKind.DWord);
                            break;
                    }
                
               
               
                    string newText = "DevModeIndex:1";
                    if (this.comboBox3.SelectedIndex != -1)
                        newText = "DevModeIndex:" + (object)(this.comboBox3.SelectedIndex + 1);
                    string fileName = ".\\\\LauncherOption.if";
                    Common.lineChanger(newText, fileName, 1);
                    Common.lineChanger("ID:" + this.textBox1.Text, fileName, 3);
                }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Cursor = Cursors.Hand;
            this.DialogResult = DialogResult.Cancel;
        }

        private void Options_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Common.ReleaseCapture();
            Common.SendMessage(this.Handle, 161, 2, 0);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
