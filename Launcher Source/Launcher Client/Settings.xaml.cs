using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Animation;


namespace Launcher
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Options : Window
    {
        //private bool EffectsChecked = false;
        //private bool MusicChecked = false;
        private int Resolution_Value;
        //private int ColorDepth_Value;
        private readonly DoubleAnimation _oa;
        private bool m_IsPressed = false;

        public Options()
        {
            InitializeComponent();

            this.MouseDown += new MouseButtonEventHandler(MainBgr_MouseDown);
            this.PreviewMouseDown += new MouseButtonEventHandler(MainBgr_PreviewMouseDown);
            this.PreviewMouseUp += new MouseButtonEventHandler(MainBgr_PreviewMouseUp);

            _oa = new DoubleAnimation();
            _oa.From = 0;
            _oa.To = 1;
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);
        }

        private void MainBgr_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                m_IsPressed = true;
            }
            else
            {
                m_IsPressed = false;
            }
        }

        private void MainBgr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_IsPressed = false;
        }

        private void MainBgr_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_IsPressed)
            {
                this.DragMove();
            }
        }

        private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _oa.From = 1;
            _oa.To = 0;
            _oa.Completed += new EventHandler((send, ea) =>
            {
                this.Close();
            });
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);
        }

        private void Cansel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveRegistryConfigs()
        {
            try
            {
                for (int i = 0; i < Resolution.Items.Count; i++)
                {
                    if (Resolution.Text == Resolution.Items[i].ToString())
                    {
                        Resolution_Value = i;
                    }
                }

                var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

                key.SetValue("Resolution", Resolution_Value, RegistryValueKind.DWord);

                if (Language.SelectedIndex == 0)
                {
                    key.SetValue("LangSelection", "Eng", RegistryValueKind.ExpandString);
                }
                if (Language.SelectedIndex == 1)
                {
                    key.SetValue("LangSelection", "Por", RegistryValueKind.ExpandString);
                }
                if (Language.SelectedIndex == 2)
                {
                    key.SetValue("LangSelection", "Spn", RegistryValueKind.ExpandString);
                }

                if (Effects.IsChecked == true)
                {
                    key.SetValue("SoundOnOff", "1", RegistryValueKind.DWord);
                }
                else
                {
                    key.SetValue("SoundOnOff", "0", RegistryValueKind.DWord);
                }

                if (Music.IsChecked == true)
                {
                    key.SetValue("MusicOnOff", "1", RegistryValueKind.DWord);
                }
                else
                {
                    key.SetValue("MusicOnOff", "0", RegistryValueKind.DWord);
                }

                if (WinMode.IsChecked == true)
                {
                    key.SetValue("WindowMode", "1", RegistryValueKind.DWord);
                }
                else
                {
                    key.SetValue("WindowMode", "0", RegistryValueKind.DWord);
                }

                key.SetValue("ID", AccountBox.Text, RegistryValueKind.ExpandString);

                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadRegistryConfigs()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

                string login = "";
                login = key.GetValue("ID").ToString();
                AccountBox.Text = login;

                string lang = "";
                lang = key.GetValue("LangSelection").ToString();
                if (lang == "Eng")
                {
                    Language.SelectedIndex = 0;
                    key.SetValue("LangSelection", (object)"Eng", RegistryValueKind.String);
                }

                if (lang == "Por")
                {
                    Language.SelectedIndex = 1;
                    key.SetValue("LangSelection", (object)"Por", RegistryValueKind.String);
                }

                if (lang == "Spn")
                {
                    Language.SelectedIndex = 2;
                    key.SetValue("LangSelection", (object)"Spn", RegistryValueKind.String);
                }


                if (key.GetValue(@"SoundOnOff").ToString() == "1")
                {
                    Effects.IsChecked = true;
                    //EffectsChecked = true;
                }

                if (key.GetValue(@"MusicOnOff").ToString() == "1")
                {
                    Music.IsChecked = true;
                   // MusicChecked = true;
                }

                if (key.GetValue(@"WindowMode").ToString() == "1")
                {
                    WinMode.IsChecked = true;
                }

                if (key.GetValue(@"Resolution").ToString() == "0")
                {
                    Resolution.SelectedIndex = 0;
                }
                if (key.GetValue(@"Resolution").ToString() == "1")
                {
                    Resolution.SelectedIndex = 1;
                }
                if (key.GetValue(@"Resolution").ToString() == "2")
                {
                    Resolution.SelectedIndex = 2;
                }
                if (key.GetValue(@"Resolution").ToString() == "3")
                {
                    Resolution.SelectedIndex = 3;
                }
                if (key.GetValue(@"Resolution").ToString() == "4")
                {
                    Resolution.SelectedIndex = 4;
                }
                if (key.GetValue(@"Resolution").ToString() == "5")
                {
                    Resolution.SelectedIndex = 5;
                }
                if (key.GetValue(@"Resolution").ToString() == "6")
                {
                    Resolution.SelectedIndex = 6;
                }
                if (key.GetValue(@"Resolution").ToString() == "7")
                {
                    Resolution.SelectedIndex = 7;
                }
                if (key.GetValue(@"Resolution").ToString() == "8")
                {
                    Resolution.SelectedIndex = 8;
                }
                if (key.GetValue(@"Resolution").ToString() == "9")
                {
                    Resolution.SelectedIndex = 9;
                }
                if (key.GetValue(@"Resolution").ToString() == "10")
                {
                    Resolution.SelectedIndex = 10;
                }
                if (key.GetValue(@"Resolution").ToString() == "11")
                {
                    Resolution.SelectedIndex = 11;
                }
                if (key.GetValue(@"Resolution").ToString() == "12")
                {
                    Resolution.SelectedIndex = 12;
                }
                if (key.GetValue(@"Resolution").ToString() == "13")
                {
                    Resolution.SelectedIndex = 13;
                }
                if (key.GetValue(@"Resolution").ToString() == "14")
                {
                    Resolution.SelectedIndex = 14;
                }
                if (key.GetValue(@"Resolution").ToString() == "15")
                {
                    Resolution.SelectedIndex = 15;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRegistryConfigs();
                Resolution.Items.Add("640x480");
                Resolution.Items.Add("800x600");
                Resolution.Items.Add("1024x768");
                Resolution.Items.Add("1152x864");
                Resolution.Items.Add("1280x768");
                Resolution.Items.Add("1280x800");
                Resolution.Items.Add("1280x960");
                Resolution.Items.Add("1280x1024");
                Resolution.Items.Add("1366x768");
                Resolution.Items.Add("1440x900");
                Resolution.Items.Add("1600x1200");
                Resolution.Items.Add("1680x1050");
                Resolution.Items.Add("1920x1080");
                Resolution.Items.Add("1920x1200");
                Resolution.Items.Add("1910x970");
                Resolution.Items.Add("1350x650");
                Language.Items.Add("English");
                Language.Items.Add("Portuguese");
                Language.Items.Add("Spanish");
            }
            catch (System.Exception)
            {

            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveRegistryConfigs();
            this.Close();
        }
    }
}
