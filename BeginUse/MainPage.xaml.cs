using BeginUse.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace BeginUse
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    
        string Title = "Generic Bluetooth Serial Universal Windows App";
        private RfcommDeviceService _service;
        private StreamSocket _socket;
        private DataWriter dataWriterObject;
        private DataReader dataReaderObject;
        ObservableCollection<PairedDeviceInfo> _pairedDevices;
        DeviceInformationCollection DeviceInfoCollection = null;
       ObservableCollection<string> str = new ObservableCollection<string>();
       // string dataResult;
        //public string DataResult { get {
        //        return dataResult;
        //    }set {
        //        Set(ref dataResult, value); 
        //    } }
        ObservableCollection<PairedDeviceInfo> PairedDevices

        {
            get { return _pairedDevices; }
            set { value = _pairedDevices; }
        }
        private CancellationTokenSource ReadCancellationTokenSource;


        public MainPage()
        {
            this.InitializeComponent();
            InitializeRfcommDeviceService();
         
        }
        /// <summary>
        /// 初始化蓝牙串口设备
        /// </summary>
        async void InitializeRfcommDeviceService()
        {
            try
            {

            

                //获取可用的蓝牙串口设备信息
                DeviceInformationCollection DeviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
                var numDevices = DeviceInfoCollection.Count();

                // By clearing the backing data, we are effectively clearing the ListBox
                _pairedDevices = new ObservableCollection<PairedDeviceInfo>();
                _pairedDevices.Clear();

                if (numDevices == 0)
                {
                    //MessageDialog md = new MessageDialog("No paired devices found", "Title");
                    //await md.ShowAsync();
                    System.Diagnostics.Debug.WriteLine("InitializeRfcommDeviceService: No paired devices found.");
                }
                else
                {
                    // Found paired devices.
                    foreach (var deviceInfo in DeviceInfoCollection)
                    {
                        // DeviceInformation deviceInformation = await DeviceInformation.CreateFromIdAsync(deviceInfo.Id);
                        var service = await RfcommDeviceService.FromIdAsync(deviceInfo.Id);
                        //var de = service.Device.DeviceInformation;
                        _pairedDevices.Add(new PairedDeviceInfo(service));
                    }
                }
                //ConnectDevices.ItemsSource = PairedDevices;
                MyCombobox.ItemsSource = PairedDevices;
               // MyCombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("InitializeRfcommDeviceService: " + ex.Message);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //OutBuff = new Windows.Storage.Streams.Buffer(100);
            Button button = (Button)sender;
            if (button != null)
            {
                switch ((string)button.Content)
                {
                    case "Disconnect":
                        await this._socket.CancelIOAsync();
                        _socket.Dispose();
                        _socket = null;
                        //this.textBlockBTName.Text = "";
                        //this.TxtBlock_SelectedID.Text = "";
                        this.buttonDisconnect.IsEnabled = false;
                        this.buttonSend.IsEnabled = false;
                        this.buttonStartRecv.IsEnabled = false;
                        this.buttonStopRecv.IsEnabled = false;
                        break;
                    //case "Send":
                    //    //await _socket.OutputStream.WriteAsync(OutBuff);
                    //    Send(this.textBoxSendText.Text);
                    //    //  this.textBoxSendText.Text = "";
                    //    break;
                    //case "Clear Send":
                    //    // this.textBoxRecvdText.Text = "";
                    //    break;
                    case "Start Recv":
                        this.buttonStartRecv.IsEnabled = false;
                        this.buttonStopRecv.IsEnabled = true;
                        Listen();
                        break;
                    case "Stop Recv":
                        this.buttonStartRecv.IsEnabled = false;
                        this.buttonStopRecv.IsEnabled = false;
                        CancelReadTask();
                        break;
                    case "Refresh":
                        InitializeRfcommDeviceService();
                        break;
                }
            }
        }
      

     async   private void MyCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyRing.IsActive = true;
            MyText.Text = "正在连接，请稍后";
            var combo = (ComboBox)sender;
        var a=    combo.SelectedItem as PairedDeviceInfo;

            Parallel.Invoke();
            bool success = true;
            try
            {
                _service = await RfcommDeviceService.FromIdAsync(a.ID);

                if (_socket != null)
                {
                    // Disposing the socket with close it and release all resources associated with the socket
                    _socket.Dispose();
                }

                _socket = new StreamSocket();
                try
                {
                    // Note: If either parameter is null or empty, the call will throw an exception
                    await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);
                }
                catch (Exception ex)
                {
                    success = false;
                    System.Diagnostics.Debug.WriteLine("Connect:" + ex.Message);
                }
                // If the connection was successful, the RemoteAddress field will be populated
                if (success)
                {
                    this.buttonDisconnect.IsEnabled = true;
                    this.buttonSend.IsEnabled = true;
                    this.buttonStartRecv.IsEnabled = true;
                    this.buttonStopRecv.IsEnabled = false;
                    string msg = String.Format("Connected to {0}!", _socket.Information.RemoteAddress.DisplayName);
                    //MessageDialog md = new MessageDialog(msg, Title);
                    System.Diagnostics.Debug.WriteLine(msg);
                    //await md.ShowAsync();
                    MyRing.IsActive = false;
                    MyText.Text = "连接成功";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Overall Connect: " + ex.Message);
                _socket.Dispose();
                _socket = null;
            }


        }

        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Listen()
        {
            try
            {
                ReadCancellationTokenSource = new CancellationTokenSource();
                if (_socket.InputStream != null)
                {
                    dataReaderObject = new DataReader(_socket.InputStream);
                    this.buttonStopRecv.IsEnabled = true;
                    this.buttonDisconnect.IsEnabled = false;
                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                this.buttonStopRecv.IsEnabled = false;
                this.buttonStartRecv.IsEnabled = false;
                this.buttonSend.IsEnabled = false;
                this.buttonDisconnect.IsEnabled = false;
                //this.textBlockBTName.Text = "";
                //this.TxtBlock_SelectedID.Text = "";
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    System.Diagnostics.Debug.WriteLine("Listen: Reading task was cancelled, closing device and cleaning up");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Listen: " + ex.Message);
                }
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;
           // uint ReadBufferLength = 1024;
            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                try
                {
                    string recvdtxt = dataReaderObject.ReadString(bytesRead);
               str.Add(recvdtxt);
               
                    System.Diagnostics.Debug.WriteLine(recvdtxt);
                    // outputTextBox.Text += recvdtxt;
                    //  dataResult += recvdtxt;
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, new DispatchedHandler(() =>
                     {
                         MyBar.Value = str.Count;
                         MyValue.Text = str.Count.ToString();
                         MyTextBox.Text += recvdtxt;
                       //  MyRich.Document.SetText( Windows.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, recvdtxt+=recvdtxt);
                      //  MyListView.ItemsSource = str;
                         //MyText2.Text += recvdtxt;
                     }));
                    //outputTextBox.
                    //recvdtxt += recvdtxt;
                    // MyRich.Document.Selection.GetIndex(Windows.UI.Text.TextRangeUnit.Word);
                    // MyRich.Document.Selection.
                    //MyRich.Document.Selection.SetText(Windows.UI.Text.TextSetOptions.None, recvdtxt);
                    //     MyRich.Document.SetText(Windows.UI.Text.TextSetOptions.None, recvdtxt);


                    //UI更新测试


               





                    //UIElement uIElement=null;
                    //bool isVerticalScrolling = true;
                    //bool smoothScrolling = true;
                    //float? zoomFactor = null;
                    //var transform = uIElement.TransformToVisual((UIElement)MyScro.Content);
                    //var position = transform.TransformPoint(new Point(0, 0));
                    //if (isVerticalScrolling)
                    //{
                    //MyScro.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
                    //}
                    //else
                    //{MyScro.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
                    //}
                    //  MyScro.ChangeView();
                    /*if (_Mode == Mode.JustConnected)
                    {
                        if (recvdtxt[0] == ArduinoLCDDisplay.keypad.BUTTON_SELECT_CHAR)
                        {
                            _Mode = Mode.Connected;

                            //Reset back to Cmd = Read sensor and First Sensor
                            await Globals.MP.UpdateText("@");
                            //LCD Display: Fist sensor and first comamnd
                            string lcdMsg = "~C" + Commands.Sensors[0];
                            lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[1] + "           ";
                            Send(lcdMsg);

                            backButton_Click(null, null);
                        }
                    }
                    else if (_Mode == Mode.Connected)
                    {
                        await Globals.MP.UpdateText(recvdtxt);
                        recvdText.Text = "";
                        status.Text = "bytes read successfully!";
                    }*/
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ReadAsync: " + ex.Message);
                }

            }
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        private void MyScro_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        //private void MyText2_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //  MyText2.SelectionStart = MyText2.Text.Length;
        //    //  MyText2.Sc
        //    var grid = (Grid)VisualTreeHelper.GetChild(MyText2, 0);
        //    for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
        //    {
        //        object obj = VisualTreeHelper.GetChild(grid, i);
        //        if (!(obj is ScrollViewer)) continue;
        //        ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
        //        break;
        //    }
        //}
        //private void OutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    outputTextBox.SelectionStart = outputTextBox.Text.Length;
        //   // outputTextBox.Sc
        //    //var grid = (Grid)VisualTreeHelper.GetChild(outputTextBox, 0);
        //    //for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
        //    //{
        //    //    object obj = VisualTreeHelper.GetChild(grid, i);
        //    //    if (!(obj is ScrollViewer)) continue;
        //    //    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
        //    //    break;
        //    //}
        //}
    }
}
