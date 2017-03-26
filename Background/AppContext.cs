using System;
using System.Threading;
using System.Drawing;

using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace Background
{
    class AppContext : System.Windows.Forms.ApplicationContext
    {
        private static readonly int UPDATE_PERIOD = 600000;
        private static readonly int IMAGE_LEVEL = 4;
        private static readonly string BASE_URL = @"http://himawari8-dl.nict.go.jp/himawari8/img/D531106/";
        private static readonly int BLOCK_WIDTH = 550;

        NotifyIcon notify;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        private System.Threading.Timer timer;
        public AppContext()
        {
            notify = new System.Windows.Forms.NotifyIcon();

            notify.Icon = (System.Drawing.Icon)Background.Properties.Resources.image;
            notify.Text = "Himawari";
            notify.Visible = true;
            notify.ContextMenu = CreateContextMenu();
            //notify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);



            timer = new System.Threading.Timer(Update, null, 0, UPDATE_PERIOD);
        }

        public ContextMenu CreateContextMenu()
        {

            var components = new System.ComponentModel.Container();
            var contextMenu1 = new System.Windows.Forms.ContextMenu();
            var menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(
                        new MenuItem[] {   menuItem1 });
            menuItem1.Index = 0;
            menuItem1.Text = "E&xit";


            return (ContextMenu)contextMenu1;
        }

        private void Update(object arg)
        {
            string url = getUrl();
            var img = DownloadImg(url);
            img.Save("im.png", System.Drawing.Imaging.ImageFormat.Png);
            setWallpaper(img);

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

        private Image DownloadImg(string url)
        {
            Image result = new Bitmap(BLOCK_WIDTH * IMAGE_LEVEL, BLOCK_WIDTH * IMAGE_LEVEL);
            Graphics graph = Graphics.FromImage(result);

            try
            {
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
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return result;
        }
        private string getUrl()
        {
            var dtNow = DateTime.Now.ToUniversalTime();
            dtNow = dtNow.AddMinutes(-30 - dtNow.Minute % 10).AddSeconds(-dtNow.Second);

            return BASE_URL + IMAGE_LEVEL + "d/" + BLOCK_WIDTH + '/' + 
                dtNow.ToString(@"yyyy\/MM\/dd\/HHmmss");
        }
    }
}
