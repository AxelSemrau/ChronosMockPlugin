using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Device
{
    /// <summary>
    /// Shows how you can directly interact with all configured devices.
    /// </summary>
    /// <remarks>
    /// This way of interacting is only for very rare scenarios - you should use
    /// IToolbox or ITaskForDevice for device access whenever possible.
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    public class DeviceSurveillance : IWorkWithSampleLists, IDirectDeviceAccess, ITraceLogger
    {
        #region Implementation of IWorkWithSampleLists

        public string ButtonCaption => null;
        public Icon ButtonIcon => null;
        public void DoYourJob()
        {
            // nothing to see here
        }

        #endregion

        #region Implementation of IDirectDeviceAccess

        public IEnumerable<IDevice> ConfiguredDevices
        {
            set
            {
                // You could save the list for later use,
                // you could filter the list for specific interfaces,
                // you could call methods of the devices at any time -
                // just make sure you know what you are doing and don't mess with
                // toolboxes and sequence execution.
                Trace("** Start device list dump");
                foreach (var someDev in value)
                {
                    Trace($"{someDev.Name} of type {someDev.GetType().FullName}, connection state {Helpers.Devices.Single(info => info.Device == someDev).ConnectionState}");
                }
                Trace("** End device list dump");
            }
        }

        #endregion

        private void Trace(string txt)
        {
            TraceWrite?.Invoke(this,new TraceWriteEventArgs(txt));
        }
        #region Implementation of ITraceLogger

        public event EventHandler<TraceWriteEventArgs> TraceWrite;

        #endregion
    }
}
