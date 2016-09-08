using System;
using System.Threading.Tasks;

namespace MockPlugin.SampleList
{
    /// <summary>
    /// Provides an endless supply of nonsense sample lists.
    /// </summary>
    /// <remarks>
    /// A possible serious use would be to generate sample lists from LIMS data and to feed them
    /// to Chronos.
    /// </remarks>
    public class MockSampleListWorker :
        AxelSemrau.Chronos.Plugin.INeedToRunSampleLists,
        AxelSemrau.Chronos.Plugin.INeedCellAccess
    {
        private readonly TaskScheduler mScheduler;

        public MockSampleListWorker()
        {
            // for GUI synchronized operations
            mScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            // for the remote access example
            RemotePluginService.StartService();
        }

        public string ButtonCaption
        {
            get { return "Run plugin provided\r\nsample lists"; }
        }

        public System.Drawing.Icon ButtonIcon
        {
            get { return Properties.Resources.Mock; }
        }

        /// <summary>
        /// Ask the user if he wants to start more schedules.
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <returns></returns>
        /// <remarks>
        /// For a real plugin, this could be some check whether the analytical results of the last sample require intervention or not, before injecting the next sample.
        /// </remarks>
        private static bool OneMoreScheduleWanted(IntPtr mainHandle)
        {
            return System.Windows.Forms.MessageBox.Show("Start one more?",
                       "Restart loop",
                       System.Windows.Forms.MessageBoxButtons.OKCancel,
                       System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK;
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
            System.Windows.Window win = null;
            var mainHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            DoOnGUIThread(new Action(() =>
            {
                win = new ShowPluginIsInCharge();
                var wih = new System.Windows.Interop.WindowInteropHelper(win);
                var myHandle = wih.EnsureHandle();
                wih.Owner = mainHandle;
                win.Show();
            }));
            try
            {
                do
                {
                    System.Threading.Thread.Sleep(5000);
                    var ex = RunSampleList(this,
                        new AxelSemrau.Chronos.Plugin.RunSampleListEventArgs()
                        {
                            //SampleListFile = @"C:\Users\Patrick\Documents\Chronos\MoveTest.csl"
                            ExtendLastPlanner = true,
                            StartAndWaitForEnd = false
                        }
                    );
                    if (ex != null)
                    {
                        System.Windows.Forms.MessageBox.Show("Error: " + ex.Message, "Plugin Provided Schedule",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
                    }
                } while (OneMoreScheduleWanted(mainHandle));
            }
            finally
            {
                DoOnGUIThread(new Action(() => win.Close()));
            }
        }

        /// <summary>
        /// Execute the specified action in the GUI thread's context.
        /// </summary>
        /// <param name="theAction"></param>
        private void DoOnGUIThread(Action theAction)
        {
            var myTask = Task.Factory.StartNew(theAction,
                             System.Threading.CancellationToken.None,
                             TaskCreationOptions.None,
                             mScheduler);
            try
            {
                myTask.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions[0];
            }
        }

        public event AxelSemrau.Chronos.Plugin.RunSampleListHandler RunSampleList;

        #region Sample List Access

        private AxelSemrau.Chronos.Plugin.ISampleListAccessor mSampleList;

        /// <summary>
        /// Here we get an helper that allows us to manipulate the current sample list.
        /// </summary>
        public AxelSemrau.Chronos.Plugin.ISampleListAccessor SampleList
        {
            set { mSampleList = value; }
        }

        #endregion Sample List Access
    }
}