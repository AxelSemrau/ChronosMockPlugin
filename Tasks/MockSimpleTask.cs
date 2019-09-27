using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using AxelSemrau.Chronos.Plugin;
// ReSharper disable LocalizableElement

/*!
 * \brief Example task implementations.
 * Since there are lots of things that can be done from a task, the demo was split into many different examples each showing only few facets of what's possible.
 * If you still think this is confusing, please let us know where we could simplify these examples.
 */
namespace MockPlugin.Tasks
{
    /// <summary>
    /// A task which does not need access to the custom device.
    /// </summary>
    [ScheduleDiagramColor(nameof(Colors.Coral))]
    public class ShowSomeGreeting :  ITask, IDemoAwareTask, IHaveRunlogOutput, ITraceLogger,
        // ambigous name otherwise when referring to Chronos.exe: For historical reasons, Chronos has a type of the same name in the global namespace, sorry!
        global::AxelSemrau.Chronos.Plugin.IReactOnCultureChanges,
        IStopRuns
    {
        /// <summary>
        /// Parameter which can be edited in the method editor, with some predefined values.
        /// </summary>

        [DefaultGreetings]
        public string GreetingsText { get; set; } = DefaultGreetingsAttribute.DefGreeting;

        /// <summary>
        /// See in .Execute - this is just a flag to tell us if we should stop the schedule.
        /// </summary>
        public bool OkThisIsGoodbye { get; set; }

        /// <summary>
        /// Simple example how to provide a combobox with standard values for a property
        /// </summary>
        private class DefaultGreetingsAttribute : Attribute, System.Collections.IEnumerable
        {
            internal const string DefGreeting = "Some default greeting";
            private readonly string[] mDefGreetings = { DefGreeting, "Howdy!", "Good day!" };

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                var greetingsList = mDefGreetings.ToList();
                greetingsList.Add($"Current greeting of {DateTime.Now:HH:mm:ss}");
                return greetingsList.GetEnumerator();
            }
        }

        public void PreValidate()
        {
            System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,"PreValidate");
        }

        public void PostValidate()
        {
            System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,"PostValidate");    
        }

        /// <summary>
        /// Pretend we are doing some work by showing a message box.
        /// </summary>
        /// <remarks>Also documents this in the runlog.</remarks>
        public void Execute()
        {
            TraceWrite?.Invoke(this, new TraceWriteEventArgs($"Executing with greeting {GreetingsText}"));
            WriteToRunlog?.Invoke(string.Format(Properties.LocalizeMockPlugin.ShowingAMessageboxWithTheTextX0, GreetingsText));
            System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,
                "Simple task shows a message:\r\n" + GreetingsText, 
                "Message shown by the mock task",
                System.Windows.Forms.MessageBoxButtons.OK);

            // Stop the schedule if this flag was set.
            if (OkThisIsGoodbye)
            {
                StopRun?.Invoke(new StopRunArgs(){How = StopRunArgs.StopMode.NoNewJobs,Reason = "It is time to say goodbye",RestartRemainingJobs = false, StopQueue = false});
            }
        }

        public void DemoExecute()
        {
            System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,"If we were really  executing, we would show something else.", "Demo Execution of Plugin Task");
        }

        /// <summary>
        /// The result of this function will be shown in the timetable.
        /// </summary>
        /// <returns></returns>
        public string GetTaskAction()
        {
            return $"Just shows a message box with {GreetingsText}{(OkThisIsGoodbye ? ", then makes the schedule stop.": "")}";
        }

        public event Action<string> WriteToRunlog;

        public event EventHandler<TraceWriteEventArgs> TraceWrite;

        /// <summary>
        /// Switch the resources with localized items to the newly selected culture.
        /// </summary>
        /// <param name="newCulture"></param>
        public void UICultureChanged(System.Globalization.CultureInfo newCulture)
        {
            Properties.LocalizeMockPlugin.Culture = newCulture;
        }

        public Func<StopRunArgs, Task> StopRun { get; set; }
    }
}
