using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using AxelSemrau.Chronos;
using AxelSemrau.Chronos.Plugin;
// ReSharper disable LocalizableElement

/*!
 * \brief The classes in this namespace demonstrate how to interact with the Chronos sample list.
 */
namespace MockPlugin.SampleList
{
    /// <summary>
    /// Provides an endless supply of nonsense sample lists.
    /// </summary>
    /// <remarks>
    /// A possible serious use would be to generate sample lists from LIMS data and to feed them
    /// to Chronos.
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MockSampleListWorker :
        INeedToRunSampleLists,
        INeedCellAccess, IDirectDeviceAccess, IDisposable,
        INotifyPropertyChanged,
        IProvideDiagnosticLogs,
        IStopRuns
    {
        private const string NormalButtonLabel = "Run plugin provided\r\nsample lists";
        private readonly TaskFactory mGuiFactory;

        public MockSampleListWorker()
        {
            // for GUI synchronized operations
            mGuiFactory = new TaskFactory(Helpers.Gui.GuiThreadScheduler);
            // for the remote access example
            RemotePluginService.StartService();
        }

        public string ButtonCaption { get; private set; } = NormalButtonLabel;

        public System.Drawing.Icon ButtonIcon { get; private set; } = Properties.Resources.MockNormal;

        /// <summary>
        /// Ask the user if he wants to start more schedules.
        /// </summary>
        /// <param name="ownerWin"></param>
        /// <returns></returns>
        /// <remarks>
        /// For a real plugin, this could be some check whether the analytical results of the last sample require intervention or not, before injecting the next sample.
        /// </remarks>
        private bool OneMoreScheduleWanted(Window ownerWin)
        {
            bool oneMore = false;
            DoOnGUIThread(() => oneMore = MessageBox.Show(ownerWin, "Start one more?",
                "Restart loop",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes);
            return oneMore;
        }

        /// <summary>
        /// Take over control. When this function exits, Chronos is in charge again.
        /// </summary>
        /// <remarks>
        /// We show some dialog window to make clear what we are doing, and that we are in charge.
        /// </remarks>
        public void DoYourJob()
        {
            // Use the process' main window as owner, so that our blocking
            // window can not be hidden behind the main window.
            ShowPluginIsInCharge win = null;
            ButtonIcon = Properties.Resources.MockBusy;
            ButtonCaption = "Plugin busy";
            OnPropertyChanged(nameof(ButtonIcon));
            OnPropertyChanged(nameof(ButtonCaption));

            DoOnGUIThread(() =>
            {
                // checking if it also works when called from the GUI thread
                Core.ExecutionQueue.RemoveFailedPlanners();
                win = new ShowPluginIsInCharge();
                var wih = new WindowInteropHelper(win);
                var winHandle = wih.EnsureHandle();
                HandleAbortButton(win);
                HandleStopButton(win);
                Helpers.Gui.OwnMyWindow(winHandle);
                win.Show();
            });
            try
            {
                do
                {
                    // prevent previously failed planners from stopping us
                    Core.ExecutionQueue.RemoveFailedPlanners();
                    Helpers.Debug.TraceWrite($"Just FYI: Standard sample lists are at {Helpers.Config.PathToSampleLists ?? "(N/A)"} and methods are at {Helpers.Config.PathToMethods ?? "(N/A)"}, the instrument config is at {Helpers.Config.PathToInstrumentConfig}");
                    TurnOffResets();
                    System.Threading.Thread.Sleep(5000);
                    var ex = RunSampleList?.Invoke(this,
                        new RunSampleListEventArgs()
                        {
                            //SampleListFile = @"C:\Users\Patrick\Documents\Chronos\MoveTest.csl"
                            ExtendLastPlanner = false,
                            StartAndWaitForEnd = false,
                            SwitchToSchedulesView = false,
                            RespectSelection = false
                        }
                    );
                    if (ex != null)
                    {
                        System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,$"Error: {ex.Message}", "Plugin Provided Schedule",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
                    }
                } while (OneMoreScheduleWanted(win));
            }
            finally
            {
                DoOnGUIThread(() => win.Close());
                ButtonIcon = Properties.Resources.MockNormal;
                ButtonCaption = NormalButtonLabel;
                OnPropertyChanged(nameof(ButtonIcon));
                OnPropertyChanged(nameof(ButtonCaption));
            }
        }

        private void HandleAbortButton(ShowPluginIsInCharge win)
        {
            win.AbortButton.Click += (s, e) =>
            {
                win.AbortButton.IsEnabled = false;
                var abortWaiter = mStopRun.Invoke(new StopRunArgs() {How = StopRunArgs.StopMode.Immediately});
                abortWaiter.ContinueWith((t) =>
                {
                    try
                    {
                        win.AbortButton.IsEnabled = true;
                    }
                    catch
                    {
                        // suppress exceptions in case the button does not exist any more
                    }
                }, Helpers.Gui.GuiThreadScheduler);
            };
        }

        private void HandleStopButton(ShowPluginIsInCharge win)
        {
            // This is just a quick and dirty piece of code for showing the IStopRun interface usage.
            // 
            win.StopButton.Click += (s, e) =>
            {
                win.StopButton.IsEnabled = false;
                var stopWaiter = mStopRun.Invoke(new StopRunArgs() { How = StopRunArgs.StopMode.NoNewJobs, RestartRemainingJobs = false,StopQueue = true});
                stopWaiter.ContinueWith((t) =>
                {
                    try
                    {
                        win.StopButton.IsEnabled = true;
                    }
                    catch
                    {
                        // suppress exceptions in case the button does not exist any more
                    }
                }, Helpers.Gui.GuiThreadScheduler);
            };
        }

        private void TurnOffResets()
        {
            foreach (var somePAL in ConfiguredDevices.OfType<IPal3Access>())
            {
                somePAL.Options.AlwaysResetAfterSequence = false;
                somePAL.Options.ResetBeforeSequence = false;
            }
        }

        /// <summary>
        /// Execute the specified action in the GUI thread's context.
        /// </summary>
        /// <param name="theAction"></param>
        private void DoOnGUIThread(Action theAction)
        {
            mGuiFactory.StartNew(theAction).GetAwaiter().GetResult();
        }

        public event RunSampleListHandler RunSampleList;

        #region Sample List Access

        /// <summary>
        /// Here we get an helper that allows us to manipulate the current sample list.
        /// </summary>
        public ISampleListAccessor SampleList
        {
            set
            {
                // not using it in this demo plugin yet
            }
        }

        #endregion Sample List Access

        #region Implementation of IDirectDeviceAccess

        public IEnumerable<IDevice> ConfiguredDevices { get; set; }

        #endregion

        public void Dispose()
        {
            RemotePluginService.StopService();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<string> LogPaths => GetFakeLogs(GetType().FullName);

        /// <summary>
        /// Creates a few fake log files.
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFakeLogs(string creator)
        {
            for (var i = 1; i <= 5; ++i)
            {
                var fakePath = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllText(fakePath, $"Fake log entry in file {i} created at {DateTime.Now:hh:mm:ss} by {creator}");
                yield return fakePath;
            }
        }

        private Func<StopRunArgs, Task> mStopRun;
        public Func<StopRunArgs, Task> StopRun
        {
            set => mStopRun = value;
        }
    }
}