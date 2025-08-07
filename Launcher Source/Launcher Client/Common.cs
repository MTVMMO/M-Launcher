// Decompiled with JetBrains decompiler
// Type: Launcher.Exile.Common
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D4AA755B-F045-4A12-838C-6A7934E8D46E
// Assembly location: D:\NDev\NNTeam\nDev\launcher\Net 4.0\Launcher.exe

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Launcher.Exile
{
    internal class Common
    {
        public static Image ResizeImage(Image source, int Width, int Height, int dx, int dy)
        {
            RectangleF rectangleF = new RectangleF(0.0f, 0.0f, (float)source.Width, (float)source.Height);
            RectangleF rect = new RectangleF(0.0f, 0.0f, (float)Width, (float)Height);
            Image image = (Image)new Bitmap(Width, Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.FillRectangle((Brush)new SolidBrush(Color.White), rect);
            if ((double)((float)source.Width / (float)source.Height) >= 1.0)
            {
                float num1 = (float)Width / rectangleF.Width;
                float height = rectangleF.Height * num1;
                float num2 = (float)Height - height;
                graphics.DrawImage(source, 0.0f, num2 / 2f, (float)Width, height);
            }
            else
            {
                float num1 = (float)Height / rectangleF.Height;
                float width = rectangleF.Width * num1;
                float num2 = (float)Width - width;
                graphics.DrawImage(source, num2 / 2f, 0.0f, width, (float)Height);
            }
            return image;
        }

        public static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] contents = System.IO.File.ReadAllLines(fileName);
            contents[line_to_edit - 1] = newText;
            System.IO.File.WriteAllLines(fileName, contents);
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}
