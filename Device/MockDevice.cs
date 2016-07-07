using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using AxelSemrau.Chronos.Plugin;
using MockPlugin.Tasks;

namespace MockPlugin.Device
{
    /// <summary>
    /// A chronos plugin implementation for a fake device.
    /// We pretend we are controlling a mixture of coffee machine and waiter robot.
    /// </summary>
    /// <remarks>
    /// Just an example implementation that doesn't do any real work.</remarks>
    public class MockDevice :
        IDevice,
        INeedAConnection,
        IProvideStatusMessages,
        IHaveDebugOutput,
        IAbortSchedules,
        INotifyPropertyChanged,
        IDisposable
    {
        #region private variables

        private bool mIsConnected;

        /// <summary>
        /// Default value for the connection string when a new device of this kind is added.
        /// </summary>
        private string mConnection = "COM17";

        /// <summary>
        /// For safe access to the thread controlling the GUI.
        /// We rely on our device instance being created from within this thread.
        /// </summary>
        private readonly Dispatcher mGUIdispatcher;

        #endregion private variables

        #region IDevice

        public MockDevice()
        {
            mGUIdispatcher = Dispatcher.CurrentDispatcher;
            Instances.Add(this);
        }

        internal static List<MockDevice> Instances = new List<MockDevice>();

        public void Dispose()
        {
            Instances.Remove(this);
            DebugOutput?.Invoke($"MockDevice {Name} disposed");
        }

        /// <summary>
        /// Visible to the user on the instruments page of the settings editor.
        /// </summary>
        public string DisplayedTypeName
        {
            get { return "ACME Coffee Maker"; }
        }

        /// <summary>
        /// Device class specification referred to by some messages.
        /// </summary>
        public string DeviceTypeDescription
        {
            get
            {
                return "coffee machine";
            }
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Pretend we are doing some operation on a complex parameter set.
        /// </summary>
        /// <param name="composition"></param>
        public void BrewFrappuccino(BrewFrappuccino.CompositionData composition)
        {
            ShowTheMessage(string.Format("Making a {0} mL frappuccino with cream type \"{1}\"{2}.",
                               composition.Volume,
                               composition.Cream,
                               composition.DeCaffeinated ? ", decaffeinated." : ""));
        }

        /// <summary>
        /// Inform the user of our connect attempt / success. Instead of establishing a real connection, show some message box.
        /// </summary>
        public void Connect()
        {
            ConnectionStateChanged(ConnectionState.Connecting);
            MessageBox.Show(String.Format("Device {0} was connected to {1}.",
                                Name,
                                Connection));
            ConnectionStateChanged(ConnectionState.Connected);
            mIsConnected = true;
            OnIsConnectedChanged();
        }

        /// <summary>
        /// Pretend we are closing the connection. Actual operation substituted by a message box.
        /// </summary>
        public void Disconnect()
        {
            ConnectionStateChanged(ConnectionState.Disconnecting);
            MessageBox.Show(String.Format("Device {0} was disconnected from {1}.",
                                Name,
                                Connection));
            ConnectionStateChanged(ConnectionState.Disconnected);
            mIsConnected = false;
            OnIsConnectedChanged();
        }

        public event Action<ConnectionState> ConnectionStateChanged;

        #endregion IDevice

        #region INeedAConnection

        public string Connection
        {
            get
            {
                return mConnection;
            }
            set
            {
                mConnection = value;
            }
        }

        #endregion INeedAConnection

        #region Custom methods and properties

        /// <summary>
        /// Let our device set a status message and display some message box instead of doing real work.
        /// </summary>
        /// <param name="messageText"></param>
        public void ShowTheMessage(string messageText)
        {
            SetStatusMessage("The device just did something wonderful.");
            MessageBox.Show(
                String.Format("The following message was shown addressing the mock device {0}:\r\n{1}",
                                Name,
                                messageText),
                "Mock Device",
                MessageBoxButtons.OK);
            DebugOutput(String.Format("Finished showing the message {0}", messageText));
        }

        /// <summary>
        /// Helper for our toolbox.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return mIsConnected;
            }
            set
            {
                if (!mIsConnected && value)
                {
                    Connect();
                }
                else if (mIsConnected && !value)
                {
                    Disconnect();
                }
            }
        }

        #endregion Custom methods and properties

        #region IProvideStatusMessages

        public event Action<string> SetStatusMessage;

        #endregion IProvideStatusMessages

        #region IHaveDebugOutput

        public event Action<string> DebugOutput;

        #endregion IHaveDebugOutput

        #region INotifyPropertyChanged

        /// <summary>
        /// For thread-safe update of the toolbox's GUI elements representing the connection state.
        /// </summary>
        private void OnIsConnectedChanged()
        {
            var myHandler = PropertyChanged;
            if (myHandler != null)
            {
                mGUIdispatcher.BeginInvoke(new Action(() =>
                {
                    myHandler(this, new PropertyChangedEventArgs("IsConnected"));
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #region IAbortSchedules

        /// <summary>
        /// This will trigger the AbortSchedule-Event 5 seconds after it was called from a task.
        /// </summary>
        /// <param name="reason"></param>
        /// <remarks>
        /// For a real device, this would be something like a leak, a failed pump or heater or anything else
        /// that makes it impossible to operate the device properly.
        /// </remarks>
        public void TriggerAbort(string reason)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
                                                         {
                                                             Thread.Sleep(5000);
                                                             AbortSchedule(reason);
                                                         });
        }

        public event Action<string> AbortSchedule;

        #endregion IAbortSchedules
    }
}