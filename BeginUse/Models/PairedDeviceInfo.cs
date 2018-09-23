using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace BeginUse.Models
{
    public class PairedDeviceInfo
    {
        internal PairedDeviceInfo(RfcommDeviceService deviceInfo)
        {
            this.DeviceInfo = deviceInfo;
            this.ID = this.DeviceInfo.ConnectionServiceName;
            this.Name = this.DeviceInfo.Device.Name;
        }

        public string Name { get; private set; }
        public string ID { get; private set; }
        public RfcommDeviceService DeviceInfo { get; private set; }
    }
}
