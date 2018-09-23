using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace TextBox
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        BitmapImage img = new BitmapImage();
        private async void MyButton_Click(object sender, RoutedEventArgs e)
        {
           // List<Byte> allBytes = new List<byte>();
            string sourceURL = "http://192.168.43.210:8080/?action=stream";
            //Uri uri = new Uri(sourceURL);
            byte[] buffer = new byte[1000000];
            int read, total = 0;
            int c = 0;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
            //req.Credentials = new NetworkCredential("root", "admin");
            WebResponse resp = await req.GetResponseAsync();
            Stream stream = resp.GetResponseStream();
            while ((read = stream.Read(buffer, total, 100)) != 0)
            {
                c += 1;
                total += read;
                if(c>100)
                {
                    break;
                }
              //  allBytes.AddRange(buffer.Take(read));
            }
            var file = await KnownFolders.PicturesLibrary.CreateFileAsync(
                       "树莓派截图" + DateTime.Now.Ticks + ".mp4", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file,buffer);
            // allBytes.AddRange();



            //    BitmapImage img = new BitmapImage();

            //    //  img.SetSourceAsync();
            //    MemoryStream stream1 = new MemoryStream(buffer);
            //    IRandomAccessStream stream2 = new InMemoryRandomAccessStream();
            //    stream2 = stream1.AsRandomAccessStream();
            //    stream2.Seek(0);
            //    img.DecodePixelWidth = 160;

            //    img.DecodePixelHeight = 100;
            //await    img.SetSourceAsync(stream2);


            //    //  await img.SetSourceAsync(stream);

        }

        private async void Tupian_Click(object sender, RoutedEventArgs e)
        {
            List<Byte> allBytes = new List<byte>();
            // 把截取的图片文件保存到当前的应用文件里面
            using (var response = await HttpWebRequest.Create("http://192.168.43.210:8080/?action=stream").GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {

                    byte[] buffer = new byte[4000];
                    int bytesRead = 0;
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, 4000)) > 0)
                    {
                       // allBytes.AddRange(buffer.Take(bytesRead));
                    }
                    var file = await KnownFolders.PicturesLibrary.CreateFileAsync(
                           "树莓派截图" + DateTime.Now.Ticks + ".mp4", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(file, buffer);
                }
               
            }
         
        }

        // BitmapImage bitmapImage = new BitmapImage();
        //await bitmapImage.SetSourceAsync(buffer);
        //Bitmap bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, total));
        //pictureBox1.Image = bmp;
    }

}
