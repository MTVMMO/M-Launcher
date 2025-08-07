using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows.Forms;

namespace Launcher
{
    class Config
    {
        #region Properties

        private static Config _Instance;

        private string _StartFile;

        public string StartFile
        {
            get { return _StartFile; }
            set { _StartFile = value; }
        }

        private string _RSSLink;

        public string RSSLink
        {
            get { return _RSSLink; }
            set { _RSSLink = value; }
        }

        private string _UpLink;
        //eu passei por aqui ass di macedo
        public string UpLink
        {
            get { return _UpLink; }
            set { _UpLink = value; }
        }
        //fim
        private string _ServerIP;

        public string ServerIP
        {
            get { return _ServerIP; }
            set { _ServerIP = value; }
        }

        private string _GSPort;
        public string GSPort
        {
            get { return _GSPort; }
            set { _GSPort = value; }
        }

        private string _CSPort;
        public string CSPort
        {
            get { return _CSPort; }
            set { _CSPort = value; }
        }

        private int _TimeZone;

        public int TimeZone
        {
            get { return _TimeZone; }
            set { _TimeZone = value; }
        }


        #endregion

        public static Config GetConfigs()
        {
            if (_Instance == null)
            {
                _Instance = new Config();
            }
            return _Instance;
        }

        protected Config()
        {

        }

        public void LoadLocalConfig(string FileName, string EncryptKey)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    MessageBox.Show("Arquivo Config não encontrado! \nPor favor, reinstale o cliente!", "Error");
                    Environment.Exit(0);
                }
                string[] ConfigsList_ = Regex.Split(SecureStringManager.Decrypt(File.ReadAllText(FileName), EncryptKey), "\r\n");
                _Instance.RSSLink = ConfigsList_[0];
                _Instance.ServerIP = ConfigsList_[1];
                _Instance.GSPort = ConfigsList_[2];
                _Instance.CSPort = ConfigsList_[3];
                _Instance.StartFile = ConfigsList_[4];
                //eu passei por aqui ass di macedo
                _Instance.UpLink = ConfigsList_[5];
                //fim
                _Instance.TimeZone = Convert.ToInt32(ConfigsList_[6]);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
