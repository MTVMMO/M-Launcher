using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Winforms = System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media.Animation;
using Launcher.Exile;


namespace Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String[,] rssData = null;
        bool IsMinimized;
        string SiteAdress;
        private int iUpdFileCnt;
        private double TotalFileSize = 0;
        //private double NewFileSize = 0;
        private static uint key = 99;
        public static MainWindow _FormInstance;
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
        System.ComponentModel.BackgroundWorker backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
        private System.Threading.ManualResetEvent _busy = new System.Threading.ManualResetEvent(false);
        System.Windows.Forms.Timer timerTime = new System.Windows.Forms.Timer();
        private readonly DoubleAnimation _oa;
        private Config Configs_ = null;
        private static Mutex mutex;

        //private static Mutex start_m;

        private bool IsLauncherStarted = false;

        private bool m_IsPressed = false;

        // Get a handle to an application window.
        //[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        //public static extern IntPtr FindWindow(string lpClassName,
        //    string lpWindowName);

        //// Activate an application window.
        //[DllImport("USER32.DLL")]
        //public static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern string SendMessage(IntPtr hWnd, int msg);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        private struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private void BringWindowToFront()
        {
            IntPtr wdwIntPtr = FindWindow(null, "Launcher");

            //get the hWnd of the process
            Windowplacement placement = new Windowplacement();
            GetWindowPlacement(wdwIntPtr, ref placement);

            // Check if window is minimized
            if (placement.showCmd == 2)
            {
                //the window is hidden so we restore it
                ShowWindow(wdwIntPtr, ShowWindowEnum.Restore);
            }

            //set user's focus to the window
            //SetForegroundWindow(wdwIntPtr);
            // Code to display a window regardless of its current state
            ShowWindow(wdwIntPtr, ShowWindowEnum.Show);  // Make the window visible if it was hidden
            ShowWindow(wdwIntPtr, ShowWindowEnum.Restore);  // Next, restore it if it was minimized
            SetForegroundWindow(wdwIntPtr);  // Finally, activate the window 
        }

        public MainWindow()
        {
            InitializeComponent();

            int Date = int.Parse(DateTime.Now.ToString("dd"));

            ni.Icon = Launcher.Properties.Resources.ico;//new System.Drawing.Icon(Application.GetResourceStream(new Uri("Resources/favicon.ico", UriKind.Relative)).Stream);
            ni.DoubleClick += delegate(object sender, EventArgs args) { Show(); WindowState = WindowState.Normal; ni.Visible = false; };
            this.MouseDown += new MouseButtonEventHandler(MainBgr_MouseDown);
            this.PreviewMouseDown += new MouseButtonEventHandler(MainBgr_PreviewMouseDown);
            this.PreviewMouseUp += new MouseButtonEventHandler(MainBgr_PreviewMouseUp);

            //bool IsOnline = testSite("http://madmu.ru/");
            //if (IsOnline == true)
            //{
            //    MessageBox.Show("Online");
            //}
            //else
            //{
            //    MessageBox.Show("Offline");
            //}

            CheckRegistryPath();

            bool firstInstance;
            mutex = new Mutex(false, "JTLAUNCHER1.2", out firstInstance);
            if (!firstInstance)
            {
                //Process[] aProc = Process.GetProcessesByName("Launcher");
                //IntPtr calculatorHandle = FindWindow("", "Launcher");
                //IntPtr[] handles = Process.GetProcessesByName("Launcher")
                //     .Where(process => process.MainWindowHandle != IntPtr.Zero)
                //     .Select(process => process.MainWindowHandle)
                //     .ToArray();
                //if (aProc.Length > 0)
                // {
                //BringWindowToFront();
                //  aProc[0].Kill();
                //SetForegroundWindow(aProc[0].MainWindowHandle);
                //ShowWindow(aProc[0].MainWindowHandle, 3);
                //SendMessage(aProc[0].MainWindowHandle, 3);
                // }
                //if (calculatorHandle == IntPtr.Zero)
                //{
                //    MessageBox.Show("Calculator is not running.");
                //    return;
                //}
                MessageBox.Show("Launcher Já Em Uso!", "Launcher");
                this.Close();
            }

            CheckMinimized();

            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker3.WorkerSupportsCancellation = true;

            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            //
            // backgroundworker3 
            //
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            //backgroundWorker3.RunWorkerAsync();
            // ----
            _FormInstance = this;
            progressbar1.Width = 548; //590
            progressbar2.Width = 548;
            label1.Content = "";
            try
            {
                //image.Source = GetImage("Data/Launcher/banner.png");//new BitmapImage(new Uri(@"\Data\Launcher\Image_1.png", UriKind.Relative));

                //FileStream fileStream =
                //new FileStream("Data/Launcher/banner.png", FileMode.Open, FileAccess.Read);

                //var img = new System.Windows.Media.Imaging.BitmapImage();
                //img.BeginInit();
                //img.StreamSource = fileStream;
                //img.EndInit();

                //image.Source = img;
                //fileStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arquivo de imagem não existe", "Error!");
            }

            Configs_ = Config.GetConfigs();
            Configs_.LoadLocalConfig("Data/Local/Launcher.bmd", "28755");

            //RSSDate1_LB.Content = DateTime.Now.ToString("F");


            if (Configs_.RSSLink.Length > 0)
            {
                SetRSS();
            }


            //eu passei por aqui ass di macedo
            SiteAdress = Configs_.UpLink;
            //fim


            //CheckPort(Configs_.ServerIP, int.Parse(Configs_.CSPort), labelCS_State);
            CheckPort(Configs_.ServerIP, int.Parse(Configs_.GSPort), labelGS_State);

            _oa = new DoubleAnimation();
            _oa.From = 0;
            _oa.To = 1;
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);

            timerTime.Tick += (timer1_Tick);
            timerTime.Interval = 100;
            timerTime.Start();
            labelTime_Value.Foreground = Brushes.ForestGreen;

            //try
            //{
            //    this.Icon = GetImage("Data/Launcher/icon.ico");//new BitmapImage(new Uri(@"\Data\Launcher\Image_1.png", UriKind.Relative));
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Image file dont exits1", "Error!");
            //}


            //MessageBox.Show(String.Format("{0} \n {1} {2} {3} {4}", Configs_.RSSLink, Configs_.ServerIP, Configs_.GSPort, Configs_.CSPort, Configs_.TimeZone));
        }

        private void CheckRegistryPath()
        {
            RegistryKey myKey;

            myKey = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);
            if (myKey == null)
            {
                myKey = Registry.CurrentUser.CreateSubKey(@"Software\Webzen\Mu\Config");
            }
            myKey.Close();
        }

        public bool testSite(string url)
        {

            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {

            }
            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.UtcNow;
            time = time.AddHours(Configs_.TimeZone);
            //Time.Text = "SERVER TIME: " + time.ToString("HH:mm:ss");
            labelTime_Value.Content = time.ToString("HH:mm:ss");
        }

        private static BitmapImage GetImage(string imageUri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("pack://siteoforigin:,,,/" + imageUri, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static string XOR_EncryptDecrypt(string str)
        {
            char[] f = str.ToCharArray();
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (char)((uint)f[i] ^ key);
            }
            return new string(f);
        }

        private void CheckMinimized()
        {
            string str = ".\\\\LauncherOption.if";
            try
            {
                if (System.IO.File.ReadAllLines(str)[1] == "WindowMode:1")
                {
                    Common.lineChanger("WindowMode:1", str, 2);
                    IsMinimized = true;
                    //WinMode.IsChecked = true;
                }
                else
                {
                    Common.lineChanger("WindowMode:0", str, 2);
                    IsMinimized = false;
                    //WinMode.IsChecked = false;
                }

            }
            catch
            {
                if (!System.IO.File.Exists(str))
                {
                    string[] contents = new string[3]
                    {
                        "DevModeIndex:1",
                        "WindowMode:1",
                        "ID:"
                    };
                    System.IO.File.WriteAllLines(str, contents);
                }
                else
                    Common.lineChanger("WindowMode:1", str, 2);
            }
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

        private void button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _oa.From = 1;
            _oa.To = 0;
            _oa.Completed += new EventHandler((send, ea) =>
            {
                Environment.Exit(0);
            });
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);
        }

        private void OnFadeComplete(object sender, EventArgs e)
        {
            Close();
        }

        private void SetWinMode()
        {
            string str = ".\\\\LauncherOption.if";
            {
                if (System.IO.File.ReadAllLines(str)[1] == "WindowMode:0")
                {
                    Common.lineChanger("WindowMode:1", str, 2);
                    IsMinimized = true;
                }
                else
                {
                    Common.lineChanger("WindowMode:0", str, 2);
                    IsMinimized = false;
                }

            }
        }

        private void WinMode_Click(object sender, RoutedEventArgs e)
        {
            SetWinMode();
        }

        private void OptionBtn_Click(object sender, RoutedEventArgs e)
        {
            Options OptionForm = new Options();
            OptionForm.Owner = this;
            OptionForm.ShowInTaskbar = false;
            OptionForm.Show();
        }

        private void minimaze_btn_Click(object sender, RoutedEventArgs e)
        {
            Minimaze();
        }

        private void Minimaze()
        {
            WindowState = WindowState.Minimized;
            if (WindowState.Minimized == this.WindowState)
            {
                ni.Visible = true;
                Hide();
                ni.ShowBalloonTip(1000, "Launcher", "Minimizado!", System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Net.WebClient _WebClient = new System.Net.WebClient();
                label1.Content = "Verificando atualizações...";

                try
                {
                    //MiniUpdate
                    _WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(MiniUpdDownloadFileCompleted);
                    _WebClient.DownloadFileAsync(new Uri(SiteAdress + "MiniUpdate/update.info"), "update.info", "update.info");
                    flag = 1;
                }
                catch (Exception webEx)
                {
                    MessageBox.Show(webEx.Message);
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void CheckUpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                //FullUpdate 
                System.Net.WebClient _WebClient = new System.Net.WebClient();
                _WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(FullUpdDownloadFileCompleted);
                _WebClient.DownloadFileAsync(new Uri(SiteAdress + "FullUpdate/client.info"), "client.info", "client.info");
                label1.Content = "Verificando atualizações...";
                SetStateLabel("Verifique se há atualizações.");
            }
            catch (Exception)
            {

            }
        }

        public void setTextLabel1(string s)
        {
            if (label1 != null)
            {
                label1.Content = s;
            }
        }
        public void SetProgBar(int value)
        {
            progressbar1.Height = 15;
            progressbar1.Width = (double)((int)((double)value * 5.48));
        }
        public void SetProgBar2(int value)
        {
            progressbar2.Height = 15;
            progressbar2.Width = (double)((int)((double)value * 5.48));
        }
        public void setPicturebox4(string s)
        {
            //if (CheckUpd55 != null)
            //{
            //    CheckUpd55.Enabled = Convert.ToBoolean(s);
            //}
        }
        public void setPicturebox7(string s)
        {
            //if (pictureBox7 != null)
            //{
            //    Graphics g = pictureBox7.CreateGraphics();
            //    Pen pen = new Pen(Color.FromArgb(89, 62, 55), 2);
            //    int width = 344 * Convert.ToInt32(s) / 100;
            //    g.DrawRectangle(pen, 2, 2, width, 1);
            //}
        }

        private void CheckPort(string Ip, int Port, Label LB)
        {
            TcpClient TcpScan = new TcpClient();
            var result = TcpScan.BeginConnect(Ip, Port, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            if (!success)
            {
                LB.Foreground = Brushes.Red;
                LB.Content = "SERVER OFFLINE!";
            }
            else
            {
                LB.Foreground = Brushes.LawnGreen;
                LB.Content = "SERVER ONLINE!";
            }
        }

        private void setTextLabelInvoke(string s, int type)
        {
            if (!Dispatcher.CheckAccess())
            {
                switch (type)
                {
                    case 1:
                        Dispatcher.Invoke(new Action<string>(this.setTextLabel1), s);
                        break;
                    case 2:
                        Dispatcher.Invoke(new Action<string>(this.setPicturebox4), s);
                        break;
                    case 3:
                        Dispatcher.Invoke(new Action<string>(this.setPicturebox7), s);
                        break;
                    case 4:
                        Dispatcher.Invoke(new Action<int>(this.SetProgBar), int.Parse(s));
                        break;
                    case 5:
                        Dispatcher.Invoke(new Action<int>(this.SetProgBar2), int.Parse(s));
                        break;
                    case 6:
                        Dispatcher.Invoke(new Action<int>(this.SetLabelUpdDW), int.Parse(s));
                        break;
                    case 7:
                        Dispatcher.Invoke(new Action<int>(this.SetLabelTotalDW), int.Parse(s));
                        break;
                }
            }
            else

                switch (type)
                {
                    case 1:
                        this.setTextLabel1(s);
                        break;
                    case 2:
                        this.setPicturebox4(s);
                        break;
                    case 3:
                        this.setPicturebox7(s);
                        break;
                    case 4:
                        this.SetProgBar(int.Parse(s));
                        break;
                    case 5:
                        this.SetProgBar2(int.Parse(s));
                        break;
                    case 6:
                        this.SetLabelUpdDW(int.Parse(s));
                        break;
                    case 7:
                        this.SetLabelTotalDW(int.Parse(s));
                        break;
                }
        }

        private void FullUpdDownloadFileCompleted(System.Object sender, AsyncCompletedEventArgs evA)
        {
            try
            {
                if (evA.Error != null)
                {
                    string filename = (string)evA.UserState;
                    label1.Content = "Ocorreu um erro durante a atualização...";
                    File.Delete(filename);
                }
                else
                {
                    if (backgroundWorker1.IsBusy != true)
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void MiniUpdDownloadFileCompleted(System.Object sender, AsyncCompletedEventArgs evA)
        {
            if (evA.Error != null)
            {
                string filename = (string)evA.UserState;
                label1.Content = "Ocorreu um erro durante a atualização...";
                File.Delete(filename);
            }
            else
            {
                if (backgroundWorker3.IsBusy != true)
                {
                    backgroundWorker3.RunWorkerAsync();

                }
            }
        }

        private void _DownloadCheckSumFileCompleted(System.Object sender, AsyncCompletedEventArgs evA)
        {
            try
            {
                progbytes += newbytes;
                newbytes = 0;
                bytesold = 0;
                if (evA.Error != null)
                {

                    string filename = (string)evA.UserState;
                    //MessageBox.Show(filename);
                    setTextLabelInvoke("Ocorreu um erro durante o carregamento...", 1);
                    File.Delete(filename);
                }

                _busy.Set();
            }
            catch (Exception)
            {
                MessageBox.Show("Falha ao baixar! [" + (string)evA.UserState + "]");
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (File.Exists("client.info"))
                {
                    FileStream clientInfo = new FileStream("client.info", FileMode.Open);
                    BinaryReader binRead = new BinaryReader(clientInfo);

                    iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    int iCurFile = 0;
                    int iPerc = 0;

                    SetStateLabel("Preparação Para Renovação Arquivos...");

                    while (binRead.PeekChar() > 0)
                    {
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (iCurFile <= iUpdFileCnt)
                        {
                            iPerc = (iCurFile * 100) / iUpdFileCnt;
                            setTextLabelInvoke("" + iPerc, 3);
                            iCurFile++;
                        }

                        String fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                        String fileHash = XOR_EncryptDecrypt(binRead.ReadString());

                        string[] pathInfo = fileDir.Split('\\');

                        //Create directory if doesn't exist
                        FileInfo file = new FileInfo(fileDir);
                        file.Directory.Create();

                        setTextLabelInvoke("Checking [" + fileDir + "]...", 1);

                        //Get CRC32
                        try
                        {
                            Crc32 crc32 = new Crc32();
                            String hash = String.Empty;

                            if (File.Exists(fileDir))
                            {
                                byte[] fs = File.ReadAllBytes(fileDir);
                                foreach (byte b in crc32.ComputeHash(fs))
                                {
                                    hash += b.ToString("x2").ToUpper();
                                }
                            }
                            else
                            {
                                hash = null;
                            }

                            //UpdDir(Full)
                            if (hash == null || hash != fileHash)
                            {
                                if (hash != fileHash)
                                {
                                    System.Net.WebClient _CheckSumDownload = new System.Net.WebClient();
                                    //_CheckSumDownload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                    _CheckSumDownload.DownloadFileAsync(new Uri(SiteAdress + "FullUpdate/" + fileDir), fileDir, fileDir);
                                    _CheckSumDownload.DownloadFileCompleted += new AsyncCompletedEventHandler(_DownloadCheckSumFileCompleted);
                                    setTextLabelInvoke("Loading: [" + fileDir + "]", 1);
                                    SetStateLabel("Atualização Do Cliente Em Andamento.");
                                    _busy.WaitOne();
                                    _busy.Reset();
                                }
                            }
                        }
                        catch (Exception myExc)
                        {
                            MessageBox.Show(myExc.Message);
                            Environment.Exit(0);
                        }
                    }

                    binRead.Close();

                    setTextLabelInvoke("true", 2);
                    //setTextLabelInvoke("true", 4);
                    setTextLabelInvoke("100", 3);
                    setTextLabelInvoke("O Update Acabou, Você Pode Começar O Jogo.!", 1);
                }
                else
                {
                    setTextLabelInvoke("true", 2);
                    setTextLabelInvoke("Ocorreu um erro durante a atualização...", 1);
                }
            }
            catch (Exception)
            {

            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (File.Exists("update.info"))
                {
                    FileStream clientInfo = new FileStream("update.info", FileMode.Open);
                    BinaryReader binRead = new BinaryReader(clientInfo);

                    iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    double TotalFileSize1 = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    //MessageBox.Show(TotalFileSize.ToString());
                    int iCurFile = 0;
                    int iPerc = 0;

                    SetStateLabel("Preparação Para Renovação De Arquivos.");

                    while (binRead.PeekChar() > 0)
                    {
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (iCurFile <= iUpdFileCnt)
                        {
                            iPerc = (iCurFile * 100) / iUpdFileCnt;
                            setTextLabelInvoke("" + iPerc, 3);
                            iCurFile++;
                        }

                        String fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                        String fileHash = XOR_EncryptDecrypt(binRead.ReadString());
                        double FileSize = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));

                        string[] pathInfo = fileDir.Split('\\');

                        //Create directory if doesn't exist
                        FileInfo file = new FileInfo(fileDir);
                        file.Directory.Create();

                        setTextLabelInvoke("Checking [" + fileDir + "]...", 1);

                        //Get CRC32
                        try
                        {
                            Crc32 crc32 = new Crc32();
                            String hash = String.Empty;

                            if (File.Exists(fileDir))
                            {
                                byte[] fs = File.ReadAllBytes(fileDir);
                                foreach (byte b in crc32.ComputeHash(fs))
                                {
                                    hash += b.ToString("x2").ToUpper();
                                }
                            }
                            else
                            {
                                hash = null;
                            }
                            //UpdDir(Mini)
                            if (hash == null || hash != fileHash)
                            {
                                if (hash != fileHash)
                                {
                                    System.Net.WebClient _CheckSumDownload = new System.Net.WebClient();
                                    string FileName = fileDir;
                                    if (fileDir == "Launcher.exe") { FileName = "NewLauncher.exe"; }
                                    _CheckSumDownload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                    _CheckSumDownload.DownloadFileAsync(new Uri(SiteAdress + "MiniUpdate/" + fileDir), FileName, FileName);
                                    _CheckSumDownload.DownloadFileCompleted += new AsyncCompletedEventHandler(_DownloadCheckSumFileCompleted);
                                    setTextLabelInvoke("Loading: [" + fileDir + "]", 1);
                                    SetStateLabel("Atualização do Cliente Em Progress.");
                                    _busy.WaitOne();
                                    _busy.Reset();
                                }
                            }
                            else
                            {

                            }
                        }
                        catch (Exception myExc)
                        {
                            MessageBox.Show(myExc.Message);
                            Environment.Exit(0);
                        }
                    }

                    binRead.Close();

                    setTextLabelInvoke("true", 2);
                    //setTextLabelInvoke("true", 4);
                    setTextLabelInvoke("100", 3);
                    setTextLabelInvoke("O Update Acabou, Você Pode Começar O Jogo.!", 1);
                }
                else
                {
                    setTextLabelInvoke("true", 2);
                    setTextLabelInvoke("Ocorreu Um Erro Durante A Atualização...", 1);
                }
            }
            catch (Exception)
            {


            }
        }

        void SetLabelTotalDW(int value)
        {
            labelTotalDW.Content = String.Format("{0}%", value);
        }

        void SetLabelUpdDW(int value)
        {
            labelUpdDW.Content = String.Format("{0}%", value);
        }

        double newbytes = 0;
        double bytesold = 0;
        double progbytes = 0;
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            setTextLabelInvoke(int.Parse(Math.Truncate(percentage).ToString()).ToString(), 5);
            setTextLabelInvoke(int.Parse(Math.Truncate(percentage).ToString()).ToString(), 6);

            double totalbytes = TotalFileSize;
            double newpercent = (bytesIn + progbytes) / totalbytes * 100;
            double bytes = bytesIn;
            bytesIn -= bytesold;
            bytesold = bytes;
            newbytes += bytesIn;

            // setTextLabelInvoke(String.Format("{0} {1} {2} [{3}]", Math.Truncate(newbytes), Math.Truncate(totalBytes), Math.Truncate(progbytes), newpercent), 1);
            setTextLabelInvoke(int.Parse(Math.Truncate(newpercent).ToString()).ToString(), 4);
            setTextLabelInvoke(int.Parse(Math.Truncate(newpercent).ToString()).ToString(), 7);

        }

        private void client_DownloadProgressChanged2(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            setTextLabelInvoke(int.Parse(Math.Truncate(percentage).ToString()).ToString(), 5);
        }

        int flag = 0;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SelfUpdate();

                var ClientInfoPath = ExecutablePath + @"\client.info";

                if (File.Exists(ClientInfoPath))
                {
                    File.Delete(ClientInfoPath);
                }

                var UpdateInfoPath = ExecutablePath + @"\update.info";

                if (File.Exists(UpdateInfoPath))
                {
                    File.Delete(UpdateInfoPath);
                }

                SetStateLabel("Cliente está totalmente atualizado.");

                if (flag == 1)
                {
                    try
                    {
                        //CreateConnectionKeys();
                        if (IsLauncherStarted == false)
                        {
                            mutex = new Mutex(false, "LTPLAUNCHERSTART", out IsLauncherStarted);
                        }

                        System.Diagnostics.Process.Start(Winforms.Application.StartupPath + "\\" + Configs_.StartFile, "Updater");
                        Environment.Exit(0);
                        //Minimaze();
                    }
                    catch (Exception ma)
                    {
                        MessageBox.Show(ma.Message);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SelfUpdate();

                var ClientInfoPath = ExecutablePath + @"\client.info";

                if (File.Exists(ClientInfoPath))
                {
                    File.Delete(ClientInfoPath);
                }

                var UpdateInfoPath = ExecutablePath + @"\update.info";

                if (File.Exists(UpdateInfoPath))
                {
                    File.Delete(UpdateInfoPath);
                }

                SetStateLabel("Atualização Completa.");

                if (flag == 1)
                {
                    try
                    {
                        //CreateConnectionKeys();
                        if (IsLauncherStarted == false)
                        {
                            mutex = new Mutex(false, "LTPLAUNCHERSTART", out IsLauncherStarted);
                        }

                        System.Diagnostics.Process.Start(Winforms.Application.StartupPath + "\\" + Configs_.StartFile, "Updater");
                        Environment.Exit(0);
                        //Minimaze();
                    }
                    catch (Exception ma)
                    {
                        MessageBox.Show(ma.Message);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (File.Exists("update.info"))
                {
                    FileStream clientInfo = new FileStream("update.info", FileMode.Open);
                    BinaryReader binRead = new BinaryReader(clientInfo);

                    iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    double TotalFileSize1 = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    //MessageBox.Show(TotalFileSize.ToString());
                    //int iCurFile = 0;
                    //int iPerc = 0;

                    //SetStateLabel("Подготовка к обновлению.");

                    while (binRead.PeekChar() > 0)
                    {
                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        //if (iCurFile <= iUpdFileCnt)
                        //{
                        //    iPerc = (iCurFile * 100) / iUpdFileCnt;
                        //    setTextLabelInvoke("" + iPerc, 3);
                        //    iCurFile++;
                        //}
                        // MessageBox.Show("fasdfdas");
                        String fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                        String fileHash = XOR_EncryptDecrypt(binRead.ReadString());
                        double FileSize = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));

                        string[] pathInfo = fileDir.Split('\\');

                        //Create directory if doesn't exist
                        FileInfo file = new FileInfo(fileDir);
                        file.Directory.Create();

                        // setTextLabelInvoke("Проверка [" + fileDir + "]...", 1);

                        //Get CRC32
                        try
                        {
                            Crc32 crc32 = new Crc32();
                            String hash = String.Empty;

                            if (File.Exists(fileDir))
                            {
                                byte[] fs = File.ReadAllBytes(fileDir);
                                foreach (byte b in crc32.ComputeHash(fs))
                                {
                                    hash += b.ToString("x2").ToUpper();
                                }
                            }
                            else
                            {
                                hash = null;
                            }
                            //UpdDir(Mini)
                            if (hash == null || hash != fileHash)
                            {
                                if (hash != fileHash)
                                {
                                    TotalFileSize += FileSize;
                                    //MessageBox.Show(String.Format("Total={0} Size={1}", TotalFileSize, FileSize));
                                }
                            }
                        }
                        catch (Exception myExc)
                        {
                            MessageBox.Show(myExc.Message);
                            Environment.Exit(0);
                        }
                    }

                    binRead.Close();

                    //setTextLabelInvoke("true", 2);
                    //setTextLabelInvoke("true", 4);
                    // setTextLabelInvoke("100", 3);
                    // setTextLabelInvoke("Проверка окончена, можете запустить игру теперь!", 1);
                }
                else
                {
                    //setTextLabelInvoke("true", 2);
                    //setTextLabelInvoke("Произошла ошибка при обновлении...", 1);
                }
            }
            catch (Exception)
            {


            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (backgroundWorker2.IsBusy != true)
                {
                    backgroundWorker2.RunWorkerAsync();

                }
            }
            catch (System.Exception)
            {

            }
        }

        //----

        private void SetStateLabel(string Text)
        {
            if (!Dispatcher.CheckAccess())
            {
                //this.BeginInvoke(new MethodInvoker(() =>
                //{
                //    StateLabel.Text = Text;
                //}));
                Dispatcher.BeginInvoke(new Winforms.MethodInvoker(() =>
                {
                    StateLabel.Content = Text;
                }));
            }
            else
            {
                StateLabel.Content = Text;
            }
        }

        private string ExecutablePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        // public string UpLink { get; }

        private void StartExec(string FileName)
        {
            try
            {
                SetStateLabel(Configs_.StartFile + " started.");
                System.Diagnostics.Process.Start(ExecutablePath + @"\" + FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("StartExec() {0}", ex.Message));
            }
        }

        private void RunUpdater()
        {
            var IsNewLauncherPresent = CheckIsNewLauncherPresent(ExecutablePath + @"\NewLauncher.exe");
            if (IsNewLauncherPresent)
            {
                //log: MessageBox.Show("NewLauncher is present.", "MuLauncher");
                var Process = new Process();
                Process.StartInfo.Verb = "runas";
                Process.StartInfo.FileName = ExecutablePath + @"\MuUpdater.exe";
                Process.Start();
                Environment.Exit(0);
            }
        }

        private void FacebookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.facebook.com/Mugrego/");
            }
            catch
            {
            }
        }

        private void ForumClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://mulepo.com.br");
            }
            catch
            {
            }
        }

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://mulepo.com.br");
            }
            catch
            {
            }
        }

        private void SelfUpdate()
        {
            try
            {
                var IsUpdated = GetSelfUpdateState();
                if (IsUpdated)
                {
                    RunUpdater();
                    //log: MessageBox.Show("NewLauncher is updated. Click to start", "MuLauncher");
                    // MessageBox.Show("Main start.");
                    //Environment.Exit(0);
                }
                else
                {
                    RunUpdater();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(string.Format("SelfUpdate() {0}", Ex.Message));
            }
        }

        private bool GetSelfUpdateState()
        {
            try
            {
                var IsSelfUpdatePresent = Properties.Settings.Default.IsSelfUpdatePresent;
                //log: MessageBox.Show(string.Format("IsSelfUpdatePresent: {0}", IsSelfUpdatePresent));
                return IsSelfUpdatePresent;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(string.Format("GetSelfUpdateState(). {0}", Ex.Message));
                return false;
            }
        }

        private void SetSelfUpdateState(bool State)
        {
            try
            {
                Properties.Settings.Default.IsSelfUpdatePresent = State;
                Properties.Settings.Default.Save();
                //log: MessageBox.Show(string.Format("IsSelfUpdatePresent: {0}", State));
            }
            catch (Exception Ex)
            {
                MessageBox.Show(string.Format("SetSelfUpdateState() {0}", Ex.Message));
            }
        }

        private bool CheckIsNewLauncherPresent(string FilePath)
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    SetSelfUpdateState(true);
                    return true;
                }

                SetSelfUpdateState(false);
                return false;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(string.Format("CheckIsNewLauncherPresent() {0}", Ex.Message));
                SetSelfUpdateState(false);
                return false;
            }
        }

        //----

        private String[,] getRssData(String channel)
        {
            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(channel);
                System.Net.WebResponse myResponse = myRequest.GetResponse();

                System.IO.Stream rssStream = myResponse.GetResponseStream();
                System.Xml.XmlDocument rssDoc = new System.Xml.XmlDocument();

                rssDoc.Load(rssStream);

                System.Xml.XmlNodeList rssItems = rssDoc.SelectNodes("rss/channel/item");

                String[,] tempRssData = new String[100, 4];

                for (int i = 0; i < rssItems.Count; i++)
                {
                    System.Xml.XmlNode rssNode;

                    rssNode = rssItems.Item(i).SelectSingleNode("title");
                    if (rssNode != null)
                    {
                        tempRssData[i, 0] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 0] = "";
                    }

                    rssNode = rssItems.Item(i).SelectSingleNode("pubDate");
                    if (rssNode != null)
                    {
                        tempRssData[i, 1] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 1] = "";
                    }

                    rssNode = rssItems.Item(i).SelectSingleNode("link");
                    if (rssNode != null)
                    {
                        tempRssData[i, 2] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 2] = "";
                    }

                }
                return tempRssData;
            }
            catch (Exception e)
            {
                String[,] tempRssData = new String[100, 4];
                return tempRssData;
            }
        }

        private void SetRSS()
        {
            // RSS1_LB.Content = "";
            //  RSS2_LB.Content = "";
            //  RSS3_LB.Content = "";
            // RSS4_LB.Content = "";

            //RSSDate1_LB.Content = "";
            // RSSDate2_LB.Content = "";
            // RSSDate3_LB.Content = "";
            // RSSDate4_LB.Content = "";

            // RSS1_LB.Visibility = Visibility.Hidden;
            // RSS2_LB.Visibility = Visibility.Hidden;
            // RSS3_LB.Visibility = Visibility.Hidden;
            // RSS4_LB.Visibility = Visibility.Hidden;

            // RSSDate1_LB.Visibility = Visibility.Hidden;
            // RSSDate2_LB.Visibility = Visibility.Hidden;
            // RSSDate3_LB.Visibility = Visibility.Hidden;
            //RSSDate4_LB.Visibility = Visibility.Hidden;
            // ----
            rssData = getRssData(Configs_.RSSLink);
            for (int i = 0; i < 4; i++)
            {
                if (rssData[i, 0] != null)
                {
                    if (i == 0)
                    {
                        // RSS1_LB.Visibility = Visibility.Visible;
                        //RSSDate1_LB.Visibility = Visibility.Visible;

                        //RSS1_LB.Content = rssData[i, 0];
                        // RSSDate1_LB.Content = rssData[i, 1];
                    }
                    else if (i == 1)
                    {
                        // RSS2_LB.Visibility = Visibility.Visible;
                        //  RSSDate2_LB.Visibility = Visibility.Visible;

                        // RSS2_LB.Content = rssData[i, 0];
                        //RSSDate2_LB.Content = rssData[i, 1];
                    }
                    else if (i == 2)
                    {
                        //RSS3_LB.Visibility = Visibility.Visible;
                        // RSSDate3_LB.Visibility = Visibility.Visible;

                        // RSS3_LB.Content = rssData[i, 0];
                        // RSSDate3_LB.Content = rssData[i, 1];
                    }
                    else if (i == 3)
                    {
                        //RSS4_LB.Visibility = Visibility.Visible;
                        // RSSDate4_LB.Visibility = Visibility.Visible;

                        // RSS4_LB.Content = rssData[i, 0];
                        //  RSSDate4_LB.Content = rssData[i, 1];
                    }
                }
            }
        }



        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetStateLabel("");
            }
            catch (System.Exception)
            {

            }
        }

        private void RSS1_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (rssData[0, 2] == null)
            {
                return;
            }
            Process.Start(rssData[0, 2]);
        }

        private void RSS2_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (rssData[1, 2] == null)
            {
                return;
            }
            Process.Start(rssData[1, 2]);
        }

        private void RSS3_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (rssData[2, 2] == null)
            {
                return;
            }
            Process.Start(rssData[2, 2]);
        }

        private void RSS4_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (rssData[3, 2] == null)
            {
                return;
            }
            Process.Start(rssData[3, 2]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginAnimation(OpacityProperty, _oa);
        }

        private void Image1_png_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

    }
}