using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public class Form1 : Form
	{
		private static uint key = 99u;

		private IContainer components;

		private Button button1;

		private GroupBox groupBox1;

		private Button button2;

		private BackgroundWorker backgroundWorker1;

		private Label label1;

		private Label label2;

		private Button button3;

		private BackgroundWorker backgroundWorker2;

		private ListBox listBox1;

		private GroupBox groupBox2;

		private Button button4;

		private TextBox textBox1;

		public Form1()
		{
			InitializeComponent();
		}

		public void setTextLabel1(string s)
		{
			if (label1 != null)
			{
				label1.Text = s;
			}
		}

		public void setTextLabel2(string s)
		{
			if (label2 != null)
			{
				label2.Text = s;
			}
		}

		private void setTextLabelInvoke(string s, int type)
        {
            if (InvokeRequired)
			{
				switch (type)
				{
				case 1:
					Invoke(new Action<string>(setTextLabel1), s);
					return;
				case 2:
					Invoke(new Action<string>(setTextLabel2), s);
					return;
				default:
					return;
				}
			}

            switch (type)
            {
                case 1:
                    setTextLabel1(s);
                    return;
                case 2:
                    setTextLabel2(s);
                    return;
                default:
                    return;
            }
        }

		private bool My_CheckFileName(string fName)
		{
			for (int i = 0; i < listBox1.Items.Count; i++)
			{
				if (listBox1.Items[i].ToString() == fName)
				{
					return false;
				}
			}
			return true;
		}

		public static string XOR_EncryptDecrypt(string str)
		{
			char[] array = str.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)(array[i] ^ key);
			}
			return new string(array);
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				string currentDirectory = Directory.GetCurrentDirectory();
				string text = "";
				int num = 0;
				int num2 = 0;
                int num3 = 0;
                FileInfo file_0;
                int filessize = 0;
                
                for (int i = 0; i < currentDirectory.Length; i++)
				{
					if (currentDirectory[i] == '\\')
					{
						num = i;
					}
				}
				if (num != 0)
				{
					text = currentDirectory.Substring(num + 1);
				}
				FileStream output = new FileStream("update.info", FileMode.Create);
				BinaryWriter binaryWriter = new BinaryWriter(output);
				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
				string[] array = files;
                
				for (int j = 0; j < array.Length; j++)
				{
					string text2 = array[j];
					if (num != 0)
					{
						text = text2.Substring(currentDirectory.Length + 1);
					}
					if (My_CheckFileName(text))
					{
                        file_0 = new FileInfo(text);
                        filessize += Convert.ToInt32(file_0.Length);
                        num2++;
					}
					setTextLabelInvoke(string.Concat("Files : ", num2, "  Folders : ", num3), 1);
				}
				string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories);
				string[] array2 = directories;
				for (int k = 0; k < array2.Length; k++)
				{
					string path = array2[k];
					num3++;
					setTextLabelInvoke(string.Concat("Files : ", num2, "  Folders : ", num3), 1);
					string[] files2 = Directory.GetFiles(path);
					string[] array3 = files2;
					for (int l = 0; l < array3.Length; l++)
					{
						string arg_160_0 = array3[l];
						num2++;
                        file_0 = new FileInfo(text);
                        filessize += Convert.ToInt32(file_0.Length);
                        setTextLabelInvoke(string.Concat("Files : ", num2, "  Folders : ", num3), 1);
					}
				}
                binaryWriter.Write(XOR_EncryptDecrypt(Convert.ToString(num2)));
			    //MessageBox.Show(String.Format("{0}", filessize));
                binaryWriter.Write(XOR_EncryptDecrypt(Convert.ToString(filessize)));
				string[] files3 = Directory.GetFiles(Directory.GetCurrentDirectory());
				string[] array4 = files3;
				for (int m = 0; m < array4.Length; m++)
				{
					string text3 = array4[m];
					if (num != 0)
					{
						text = text3.Substring(currentDirectory.Length + 1);
					}
					if (My_CheckFileName(text))
					{
						Crc32 crc = new Crc32();
						string text4 = string.Empty;
						byte[] buffer = File.ReadAllBytes(text3);
						byte[] array5 = crc.ComputeHash(buffer);
						for (int n = 0; n < array5.Length; n++)
						{
							byte b = array5[n];
							text4 += b.ToString("x2").ToUpper();
						}
						setTextLabelInvoke(string.Concat("Files : ", num2, "  Folders: ", num3), 1);

                        FileInfo file;
                        int filesize = 0;
                        file = new FileInfo(text);
                        filesize += Convert.ToInt32(file.Length);

                        setTextLabelInvoke(text + " " + text4, 2);
						binaryWriter.Write(XOR_EncryptDecrypt(text));
						binaryWriter.Write(XOR_EncryptDecrypt(text4));
                        binaryWriter.Write(XOR_EncryptDecrypt(filesize.ToString()));
                        //MessageBox.Show(String.Format("{0}-{1} ({2})", text, text4, filesize));
					}
				}
				string[] directories2 = Directory.GetDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories);
				string[] array6 = directories2;
				for (int num4 = 0; num4 < array6.Length; num4++)
				{
					string text5 = array6[num4];
					if (num != 0)
					{
						text = text5.Substring(currentDirectory.Length + 1);
					}
					setTextLabelInvoke(string.Concat("11 : ", num2, "  22 : ", num3), 1);
					string[] files4 = Directory.GetFiles(text5);
					string[] array7 = files4;
					for (int num5 = 0; num5 < array7.Length; num5++)
					{
						string text6 = array7[num5];
						if (num != 0)
						{
							text = text6.Substring(currentDirectory.Length + 1);
						}
						if (My_CheckFileName(text))
						{
							Crc32 crc2 = new Crc32();
							string text7 = string.Empty;
							byte[] buffer2 = File.ReadAllBytes(text6);
							byte[] array8 = crc2.ComputeHash(buffer2);
							for (int num6 = 0; num6 < array8.Length; num6++)
							{
								byte b2 = array8[num6];
								text7 += b2.ToString("x2").ToUpper();
							}
							setTextLabelInvoke(string.Concat("Files : ", num2, "  Folders : ", num3), 1);

                            FileInfo file;
                            int filesize = 0;
                            file = new FileInfo(text);
                            filesize += Convert.ToInt32(file.Length);

                            setTextLabelInvoke(text + " " + text7, 2);
							binaryWriter.Write(XOR_EncryptDecrypt(text));
							binaryWriter.Write(XOR_EncryptDecrypt(text7));
                            binaryWriter.Write(XOR_EncryptDecrypt(filesize.ToString()));
                        }
					}
				}
				binaryWriter.Close();
				button1.Enabled = true;
				button2.Enabled = false;
			}
			catch (Exception ex)
			{
				button1.Enabled = true;
				button2.Enabled = false;
				MessageBox.Show(ex.Message);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!backgroundWorker1.IsBusy)
			{
				backgroundWorker1.RunWorkerAsync();
				button1.Enabled = false;
				button2.Enabled = true;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			button1.Enabled = true;
			button2.Enabled = false;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (!backgroundWorker2.IsBusy)
			{
				backgroundWorker2.RunWorkerAsync();
				button3.Enabled = false;
			}
		}

		private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
		{
			FileStream input = new FileStream("update.info", FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(input);
			Convert.ToInt32(XOR_EncryptDecrypt(binaryReader.ReadString()));
			while (binaryReader.PeekChar() > 0)
			{
				setTextLabelInvoke(XOR_EncryptDecrypt(binaryReader.ReadString()), 1);
				setTextLabelInvoke(XOR_EncryptDecrypt(binaryReader.ReadString()), 2);
			}
			binaryReader.Close();
			button3.Enabled = true;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (textBox1.Text != "ASDDD")
			{
				listBox1.Items.Add(textBox1.Text);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            groupBox1 = new GroupBox();
            button3 = new Button();
            button2 = new Button();
            label2 = new Label();
            label1 = new Label();
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker2 = new BackgroundWorker();
            listBox1 = new ListBox();
            groupBox2 = new GroupBox();
            button4 = new Button();
            textBox1 = new TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(140, 12);
            button1.Name = "button1";
            button1.Size = new Size(133, 51);
            button1.TabIndex = 0;
            button1.Text = "START";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(button2);
            groupBox1.Location = new Point(65, 63);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(333, 94);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Enter += groupBox1_Enter;
            // 
            // button3
            // 
            button3.Location = new Point(193, 56);
            button3.Name = "button3";
            button3.Size = new Size(126, 32);
            button3.TabIndex = 3;
            button3.Text = "CHECK";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(98, 56);
            button2.Name = "button2";
            button2.Size = new Size(101, 32);
            button2.TabIndex = 2;
            button2.Text = "Stop";
            button2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(-1, 0);
            label2.Name = "label2";
            label2.Size = new Size(325, 25);
            label2.TabIndex = 1;
            label2.Text = "Click START...";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(222, 160);
            label1.Name = "label1";
            label1.Size = new Size(167, 25);
            label1.TabIndex = 0;
            label1.Text = "Files: 0 Folders: 0";
            label1.Click += label1_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 24;
            listBox1.Items.AddRange(new object[] {
            "LauncherUpdate.exe",
            "update.info"});
            listBox1.Location = new Point(18, 191);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(208, 196);
            listBox1.TabIndex = 4;
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(23, 163);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(122, 22);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Exceptions";
            groupBox2.Enter += groupBox2_Enter;
            // 
            // button4
            // 
            button4.Location = new Point(285, 351);
            button4.Name = "button4";
            button4.Size = new Size(95, 36);
            button4.TabIndex = 6;
            button4.Text = "ADD";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(232, 316);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(148, 29);
            textBox1.TabIndex = 5;
            // 
            // Form1
            // 
            ClientSize = new Size(401, 399);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button4);
            Controls.Add(groupBox2);
            Controls.Add(listBox1);
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "Update Generator";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

		}

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
	}
}
