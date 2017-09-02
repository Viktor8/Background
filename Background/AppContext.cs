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
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }
    }
    class AppContext : ApplicationContext
    {
        private static readonly int UPDATE_PERIOD = 600000;
        private static readonly int IMAGE_LEVEL = 4;
        private static readonly string BASE_URL = @"http://himawari8-dl.nict.go.jp/himawari8/img/D531106/";
        private static readonly int BLOCK_WIDTH = 550;

        private static string RUN_LOCATION = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static string VALUE_NAME = "HimawariBackground";

        System.Threading.Timer timer;
        NotifyIcon notify;
        bool AutostartEnable;
        bool isEnabled;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        public AppContext()
        {
            AutostartEnable = AutostartIsEnable();
            isEnabled = true;

            notify = new NotifyIcon();
            notify.Icon = Properties.Resources.image;
            notify.Text = "Himawari";
            notify.Visible = true;
            notify.Click += (o, e) => notify.SetNotifyIconText(Status.GetReport());
            notify.ContextMenu = CreateContextMenu();

            
            timer = new System.Threading.Timer(Update, null, 0, UPDATE_PERIOD);
        }
        public ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();
            var menuItem0 = new MenuItem();
            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            var menuItem3 = new MenuItem();

            menuItem0.Index = 0;
            menuItem0.Text = isEnabled ? "Disable updating" : "Enable updating";
            menuItem0.Click += (o, e) =>
            {
                if (isEnabled)
                {
                    isEnabled = false;
                    menuItem0.Text = "Enable updating";
                }
                else
                {
                    isEnabled = true;
                    menuItem0.Text = "Disable updating";
                }

            };

            menuItem1.Index = 0;
            menuItem1.Text = "R&estart";
            menuItem1.Click += (o, e) => Application.Restart();

            menuItem2.Index = 1;
            menuItem2.Text = AutostartEnable ? "Disable autostart" : "Enable autostart";
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

            menuItem3.Index = 2;
            menuItem3.Text = "E&xit";
            menuItem3.Click += (o, e) => { Environment.Exit(0); timer.Dispose(); };

            menu.MenuItems.AddRange(
                        new MenuItem[] { menuItem0, menuItem1, menuItem2, menuItem3 });

            return menu;
        }

        private void Update(object arg)
        {
            if (!isEnabled)
            {
                Status.Message = "Himawari\nLast update: {0,HH:mm:ss}.\nUpdating disabled";
                notify.SetNotifyIconText(Status.Message);
                return;
            }
            bool success = true;
            Status.TotalCount++;
            Status.LastTry = DateTime.Now;
            Status.NowUpdating = true;
            notify.SetNotifyIconText(Status.GetReport());


            Image img = null;
            try
            {
                string url = getUrl();
                img = DownloadImg(url);
                setWallpaper(img);
            }
            catch (Exception e)
            {
                Status.Message = e.GetType().ToString() + '\n';
                Status.Message += e.Message;
                success = false;
            }
            Status.NowUpdating = false;


            if (success)
            {
                Status.LastSucces = DateTime.Now;
                Status.SuccesCount++;
                notify.SetNotifyIconText(Status.GetReport());
            }
            else
                notify.SetNotifyIconText(Status.Message
                    +"\n Next try: " +
                    Status.LastTry.AddMinutes(10).ToString("HH:mm::ss"));
            if (img != null) img.Dispose();
            GC.Collect();
        }


        string getUrl()
        {
            var dtNow = DateTime.Now.ToUniversalTime();
            dtNow = dtNow.AddMinutes(-30 - dtNow.Minute % 10).AddSeconds(-dtNow.Second);

            var JapanTime = dtNow.AddHours(9);
            if (JapanTime.AddMinutes(-20).Hour == 23)
                throw new Exception("Image not available via satellite position.");

            return BASE_URL + IMAGE_LEVEL + "d/" + BLOCK_WIDTH + '/' +
                dtNow.ToString(@"yyyy\/MM\/dd\/HHmmss");
        }
        private Image DownloadImg(string url)
        {
            Image result = new Bitmap(BLOCK_WIDTH * IMAGE_LEVEL, BLOCK_WIDTH * IMAGE_LEVEL);
            Image block = null;
            Graphics graph = Graphics.FromImage(result);

            for (int y = 0; y < IMAGE_LEVEL; y++)
                for (int x = 0; x < IMAGE_LEVEL; x++)
                {
                    string fullUrl = url + '_' + x + '_' + y + ".png";
                    var wr = System.Net.WebRequest.Create(fullUrl);
                    wr.Timeout = 30000;

                    var resp = (System.Net.HttpWebResponse)wr.GetResponse();
                    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var s = resp.GetResponseStream();
                        block = Image.FromStream(s);
                    }
                    else
                        throw new Exception("Could not load image");
                    graph.DrawImage(block, x * BLOCK_WIDTH, y * BLOCK_WIDTH, BLOCK_WIDTH, BLOCK_WIDTH);
                    if (block != null ) block.Dispose();
                    
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
            string ExecutablePath = Application.ExecutablePath;
            Registry.CurrentUser.CreateSubKey(RUN_LOCATION).SetValue(VALUE_NAME, ExecutablePath);
        }
        void DisableAutoStart()
        {
            Registry.CurrentUser.CreateSubKey(RUN_LOCATION).DeleteValue(VALUE_NAME);
        }
        bool AutostartIsEnable()
        {
            return Registry.CurrentUser.OpenSubKey(RUN_LOCATION, false) != null;    
        }

    }
}
