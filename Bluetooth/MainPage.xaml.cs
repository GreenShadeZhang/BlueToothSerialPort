using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
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

namespace Bluetooth
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
   
        private StreamSocket _socket;
        private RfcommDeviceService _service;
        private DeviceWatcher deviceWatcher = null;
        private StreamSocket chatSocket = null;
        private DataWriter chatWriter = null;
        private RfcommDeviceService chatService = null;
        private BluetoothDevice bluetoothDevice;
        public ObservableCollection<RfcommChatDeviceDisplay> ResultCollection
        {
            get;
            private set;
        }
        public MainPage()
        {
            this.InitializeComponent();
           // rootPage = MainPage.Current;
            ResultCollection = new ObservableCollection<RfcommChatDeviceDisplay>();
            DataContext = this;
        }
      
        private void Bluetooth_Click(object sender, RoutedEventArgs e)
        {

            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);
          
            deviceWatcher.Added += DeviceWatcher_Added;
           deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Start();
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    foreach (RfcommChatDeviceDisplay rfcommInfoDisp in ResultCollection)
                    {
                        if (rfcommInfoDisp.Id == args.Id)
                        {
                            rfcommInfoDisp.Update(args);
                            break;
                        }
                    }
                });
          
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
        await       this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            // Make sure device name isn't blank
            if (args.Name != "")
            {
                ResultCollection.Add(new RfcommChatDeviceDisplay(args));
              
            }

        });




        }

    }
  
    public class RfcommChatDeviceDisplay : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public RfcommChatDeviceDisplay(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;
            //UpdateGlyphBitmapImage();
        }

        public DeviceInformation DeviceInformation
        {
            get
            {
                return deviceInfo;
            }

            private set
            {
                deviceInfo = value;
            }
        }

        public string Id
        {
            get
            {
                return deviceInfo.Id;
            }
        }

        public string Name
        {
            get
            {
                return deviceInfo.Name;
            }
        }

        //public BitmapImage GlyphBitmapImage
        //{
        //    get;
        //    private set;
        //}

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            deviceInfo.Update(deviceInfoUpdate);
            //  UpdateGlyphBitmapImage();
        }

        //private async void UpdateGlyphBitmapImage()
        //{
        //    DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
        //    BitmapImage glyphBitmapImage = new BitmapImage();
        //    await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
        //    GlyphBitmapImage = glyphBitmapImage;
        //    OnPropertyChanged("GlyphBitmapImage");
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
