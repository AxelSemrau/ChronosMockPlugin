using AxelSemrau.Chronos.Plugin;
using MockPlugin.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MockPlugin.SampleList;

// ReSharper disable LocalizableElement
#pragma warning disable 169

/*!
 * \brief A fake device.
 * This namespace contains the fake device driver and auxiliary classes for settings, toolbox and other infrastructure.
 */
namespace MockPlugin.Device
{


    /*! \namespace MockPlugin
     * \brief An example Chronos plugin.
     * This plugin demonstrates how to write plugins for Chronos. It should give you a rough idea how different things can be done, and which interfaces are needed to provide funtionality.
     */

    /// <summary>
    /// A chronos plugin implementation for a fake device.
    /// We pretend we are controlling a mixture of coffee machine and waiter robot.
    /// </summary>
    /// <remarks>
    /// Just an example implementation that doesn't do any real work.</remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MockDevice :
        // ReSharper disable once RedundantExtendsListEntry
        IDevice,
        INeedAConnection,
        IProvideStatusMessages,
        IHaveDebugOutput,
        IAbortSchedules,
        ICanInterrupt,
        INotifyPropertyChanged,
        IDisposable,
        IHaveMachineParameters<CoffeMakerParams>,
        IProvideDiagnosticLogs,
        IStopRuns,
        IScheduleStateAware
    {
        public const string DeviceTypeName = "ACME Coffee Maker";
        #region private variables

        private bool mIsConnected;

        #endregion private variables

        #region IDevice

        public MockDevice()
        {
            Instances.Add(this);
        }

        internal static readonly List<MockDevice> Instances = new List<MockDevice>();

        public void Dispose()
        {
            Instances.Remove(this);
            DebugOutput?.Invoke($"MockDevice {Name} disposed");
        }

        /// <summary>
        /// Visible to the user on the instruments page of the settings editor.
        /// </summary>
        public string DisplayedTypeName => DeviceTypeName;

        /// <summary>
        /// Device class specification referred to by some messages.
        /// </summary>
        public string DeviceTypeDescription => "coffee machine";

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
            WaitIfPaused();
            ShowTheMessage(
                $"Making a {composition.Volume} mL frappuccino with cream type \"{composition.Cream}\"{(composition.DeCaffeinated ? ", decaffeinated." : "")}.");
        }

        /// <summary>
        /// Just for testing if methods of this device can be called from some other point in our code.
        /// </summary>
        public void SomeDummyMethod()
        {
            DebugOutput?.Invoke($"Dummy method of {Name} was called.");
        }

        /// <summary>
        /// Inform the user of our connect attempt / success. Instead of establishing a real connection, show some message box.
        /// </summary>
        public void Connect()
        {
            ConnectionStateChanged(ConnectionState.Connecting);
            MessageBox.Show(Helpers.Gui.MainWindow, $"Device {Name} was connected to {Connection}.");
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
            MessageBox.Show(Helpers.Gui.MainWindow, $"Device {Name} was disconnected from {Connection}.");
            ConnectionStateChanged(ConnectionState.Disconnected);
            mIsConnected = false;
            OnIsConnectedChanged();
        }

        public event Action<ConnectionState> ConnectionStateChanged;

        #endregion IDevice

        #region INeedAConnection

        /// <summary>
        /// Connection as set in the Chronos instrument settings.
        /// </summary>
        /// <remarks>
        /// If this gets a bit more complicated, you can also use a UITypeEditor here.
        /// </remarks>
        [Editor(typeof(ConnectionEditor), typeof(UITypeEditor))]
        public string Connection { get; set; } = "COM17";

        #endregion INeedAConnection

        #region Custom methods and properties

        /// <summary>
        /// Let our device set a status message and display some message box instead of doing real work.
        /// </summary>
        /// <param name="messageText"></param>
        public void ShowTheMessage(string messageText)
        {
            WaitIfPaused();
            SetStatusMessage?.Invoke("The device just did something wonderful.");
            MessageBox.Show(Helpers.Gui.MainWindow,
                $"The following message was shown addressing the mock device {Name}:\r\n{messageText}",
                "Mock Device",
                MessageBoxButtons.OK);
            DebugOutput?.Invoke($"Finished showing the message {messageText}");
        }

        /// <summary>
        /// Helper for our toolbox.
        /// </summary>
        public bool IsConnected
        {
            get => mIsConnected;
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
                Helpers.Gui.GuiTaskFactory.StartNew(() => myHandler(this, new PropertyChangedEventArgs(nameof(IsConnected))));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #region IAbortSchedules

        /// <summary>
        /// This will trigger the AbortSchedule-Event 5 seconds after it was called from a task.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="softStop"></param>
        /// <remarks>
        /// For a real device, this would be something like a leak, a failed pump or heater or anything else
        /// that makes it impossible to operate the device properly.
        /// </remarks>
        public void TriggerAbort(string reason, bool softStop)
        {
            Task.Run(() =>
                     {
                         Thread.Sleep(5000);
                         if (softStop)
                         {
                             StopRun?.Invoke(new StopRunArgs()
                             {
                                 How = StopRunArgs.StopMode.NoNewJobs, StopQueue = true, Reason = reason,
                                 RestartRemainingJobs = false
                             });
                         }
                         else
                         {
                             AbortSchedule?.Invoke(reason);
                         }
                     });
}

        public event Action<string> AbortSchedule;

        public void CheckForAbort()
        {
            if (mAborted.IsSet)
            {
                throw new OperationCanceledException("Aborted");
            }
        }

        #endregion IAbortSchedules

        #region Implementation of IHaveMachineParameters<CoffeMakerParams>

        public CoffeMakerParams Parameters { get; set; } = new CoffeMakerParams();

        public Task ApplyParametersAsync(bool waitUntilSetpointIsReached, CancellationToken canToken)
        {
            
            SetStatusMessage?.Invoke($"Applying parameters: '{Parameters}' and {(!waitUntilSetpointIsReached ? "not " : "")} waiting for setpoint.");
            if (waitUntilSetpointIsReached)
            {
                return Task.Delay(TimeSpan.FromSeconds(10),canToken);
            }
            return Task.CompletedTask;
        }

        #endregion Implementation of IHaveMachineParameters<CoffeMakerParams>

        public void WaitIfPaused()
        {
            WaitHandle.WaitAny(new[] { mPauseEnded.WaitHandle, mAborted.WaitHandle });
            CheckForAbort();
        }

        #region Implementation of ICanInterrupt

        /// <summary>
        /// Using an event here instead of a simple bool helps us avoid polling while checking for the events.
        /// </summary>
        private readonly ManualResetEventSlim mPauseEnded = new ManualResetEventSlim(true);

        private readonly ManualResetEventSlim mAborted = new ManualResetEventSlim(false);

        public bool Aborted
        {
            get => mAborted.IsSet;
            set
            {
                if (value) { mAborted.Set(); } else { mAborted.Reset(); }
            }
        }

        public bool Paused
        {
            get => !mPauseEnded.IsSet;
            set
            {
                if (value)
                {
                    mPauseEnded.Reset();
                }
                else
                {
                    mPauseEnded.Set();
                }
            }
        }

        #endregion Implementation of ICanInterrupt

        public IEnumerable<string> LogPaths =>
            MockSampleListWorker.GetFakeLogs($"{GetType().FullName} instance {Name}");

        public Func<StopRunArgs, Task> StopRun { get; set; }

        #region Schedule state events

        private IScheduleEvents mScheduleEvents;

        public IScheduleEvents ScheduleEvents
        {
            set
            {
                if (mScheduleEvents != null)
                {
                    mScheduleEvents.ScheduleStateChanged -= ScheduleStateChangedHandler;
                }

                mScheduleEvents = value;
                if (value != null)
                {
                    mScheduleEvents.ScheduleStateChanged += ScheduleStateChangedHandler;
                }
            }
        }

        private void ScheduleStateChangedHandler(object sender, ScheduleStateEventArgs e)
        {
            DebugOutput?.Invoke($"Schedule {e.PlanerName} ({e.PlanerID}) state {e.State}, abort reason {(string.IsNullOrEmpty(e.AbortReason) ? "N/A" : e.AbortReason)}");   
        }
        #endregion
    }
}