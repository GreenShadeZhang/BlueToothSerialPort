using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace TestDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
           // Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    // Set the input focus to ensure that keyboard events are raised.
        //    this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        //}
        //private static bool IsCtrlKeyPressed()
        //{
        //    var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
        //    return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        //}

        //private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (IsCtrlKeyPressed())
        //    {
        //        switch (e.Key)
        //        {
        //            case VirtualKey.O: Gaibian(); break;
        //            case VirtualKey.A: ; break;
        //            case VirtualKey.S: ; break;
        //        }
        //    }
        //    //else
        //    //{
        //    //    TextContent.Text = "别松手啊";
        //    //}
        //}
        //private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        //{
        //    if (args.EventType.ToString().Contains("Down"))
        //    {
        //      var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
        //        if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
        //        {
        //            switch (args.VirtualKey)
        //            {
        //                case VirtualKey.V:
        //                    Dianwo.Holding+= Dianwo_;
        //                    break;
        //                //case VirtualKey.N:
        //                //    NewOrder_Tapped(this, null);
        //                //    break;
        //            }
        //        }
        //    }
        //}


            public void Gaibian()
        {
            TextContent.Text = "你按我了";
        }
        public void Huifu()
        {
            TextContent.Text = "别松手啊";
        }
        private void Dianwo_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                Gaibian();
            }
            else
            {
                TextContent.Text = "别松手啊";
            }
        }

     
    }
}



   

