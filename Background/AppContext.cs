using System;
using System.Threading;
using System.Drawing;

using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace Background
{
    public static class Fixes
    {
        public static void SetNotifyIconText(this NotifyIcon ni, string text)
        {
            if (text.Length >= 128) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
            Type t = typeof(NotifyIcon);
            System.Reflection.BindingFlags hidden = 
                System.Reflection.BindingFlags.NonPublic |System.Reflection.BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }
    }
    class AppContext : System.Windows.Forms.ApplicationContext
    {
        private static readonly int UPDATE_PERIOD = 600000;
        private static readonly int IMAGE_LEVEL = 4;
        private static readonly string BASE_URL = @"http://himawari8-dl.nict.go.jp/himawari8/img/D531106/";
        private static readonly int BLOCK_WIDTH = 550;

        private static string RUN_LOCATION = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static string VALUE_NAME = "HimawariBackground";

        System.Threading.Timer timer;
        NotifyIcon notify;
        bool AutostartEnable = false;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        public AppContext()
        {
            notify = new NotifyIcon();
            notify.Icon = Properties.Resources.image;
            notify.Text = "Himawari";
            notify.Visible = true;
            notify.ContextMenu = CreateContextMenu();

            timer = new System.Threading.Timer(Update, null, 0, UPDATE_PERIOD);
        }
        public ContextMenu CreateContextMenu()
        {
            var contextMenu1 = new ContextMenu();
            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            var menuItem3 = new MenuItem();
            
            menuItem1.Index = 0;
            menuItem1.Text = "R&estart";
            menuItem1.Click += (o,e) => Application.Restart();

            menuItem2.Index = 2;
            menuItem2.Text = AutostartIsEnable() ? "Disable autostart":"Enable autostart";
            menuItem2.Click += (o, e) =>
            {
                if (AutostartEnable)
                {
                    DisableAutoStart();
                    AutostartEnable = false;
                    menuItem2.Text = "Enable autostart";
                }
                else
                {
                    EnableAutoStart();
                    AutostartEnable = true;
                    menuItem2.Text = "Disable autostart";
                }
            };
            
            menuItem2.Index = 2;
            menuItem2.Text = "E&xit";
            menuItem2.Click += (o, e) => Environment.Exit(0);

            contextMenu1.MenuItems.AddRange(
                        new MenuItem[] { menuItem1, menuItem2, menuItem3 });

            return contextMenu1;
        }

        private void Update(object arg)
        {
            bool success = true;
            Status.TotalCount++;
            Status.LastTry = DateTime.Now;

            string url = getUrl();
            Image img = null;
            try {
                img = DownloadImg(url);
            }
            catch (TimeoutException te) {
                Status.Message = "Connection problem\n";
                Status.Message += te.Message;
                success = false;
            }
            catch (Exception e)
            {
                Status.Message = e.GetType().ToString() + '\n';
                Status.Message += e.Message;
                success = false;
            }
            try
            {
                img.Save("im.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception e)
            {
                Status.Message = e.GetType().ToString();
                Status.Message += e.Message;
                success = false;
            }
            setWallpaper(img);
            img.Dispose();

            if (success)
            {
                Status.LastSucces = DateTime.Now;
                Status.SuccesCount++;
                notify.SetNotifyIconText(Status.GetReport());
            }
            else
                notify.SetNotifyIconText(Status.Message);
        }

       
        string getUrl()
        {
            var dtNow = DateTime.Now.ToUniversalTime();
            dtNow = dtNow.AddMinutes(-30 - dtNow.Minute % 10).AddSeconds(-dtNow.Second);

            return BASE_URL + IMAGE_LEVEL + "d/" + BLOCK_WIDTH + '/' +
                dtNow.ToString(@"yyyy\/MM\/dd\/HHmmss");
        }
        private Image DownloadImg(string url)
        {
            Image result = new Bitmap(BLOCK_WIDTH * IMAGE_LEVEL, BLOCK_WIDTH * IMAGE_LEVEL);
            Graphics graph = Graphics.FromImage(result);

                for (int y = 0; y < IMAGE_LEVEL; y++)
                    for (int x = 0; x < IMAGE_LEVEL; x++)
                    {
                        string fullUrl = url + '_' + x + '_' + y + ".png";
                        var wr = System.Net.WebRequest.CreateHttp(fullUrl);
                        wr.Timeout = 30000;

                        var resp = wr.GetResponse();
                        Image block = Image.FromStream(resp.GetResponseStream());
                        graph.DrawImage(block, x * BLOCK_WIDTH, y * BLOCK_WIDTH, BLOCK_WIDTH, BLOCK_WIDTH);
                        block.Dispose();
                        resp.Dispose();
                    }
            
         
            return result;
        }

        private void setWallpaper(Image img)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;


            string myPicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path = Path.Combine(myPicPath, @"Wallpapers\");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fullImName = path + @"himawari.jpg";
            img.Save(fullImName,
                System.Drawing.Imaging.ImageFormat.Jpeg);

            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 6.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                fullImName,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

        }


        void EnableAutoStart()
        {
            string pathToExeFile = Application.ExecutablePath;
            Registry.CurrentUser.CreateSubKey(RUN_LOCATION).SetValue(VALUE_NAME, (object)(pathToExeFile));
        }
        void DisableAutoStart()
        {
            Registry.CurrentUser.CreateSubKey(RUN_LOCATION).DeleteValue(VALUE_NAME);
        }
        bool AutostartIsEnable()
        {
            return Registry.CurrentUser.OpenSubKey(RUN_LOCATION, false) == null;
        }

    }
}
