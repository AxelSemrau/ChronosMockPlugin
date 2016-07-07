using System;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// A task which does not need access to the custom device.
    /// </summary>
    public class ShowSomeGreeting :  ITask, IDemoAwareTask, IHaveRunlogOutput, ITraceLogger, 
        // ambigous name otherwise: For historical reasons, Chronos has a type of the same name in the global namespace, sorry!
        AxelSemrau.Chronos.Plugin.IReactOnCultureChanges
    {

        private string mGreetingsText = DefaultGreetingsAttribute.defGreeting;
        /// <summary>
        /// Parameter which can be edited in the method editor, with some predefined values.
        /// </summary>

        [DefaultGreetings]
        public string GreetingsText
        {
            get
            {
                return mGreetingsText;
            }
            set
            {
                mGreetingsText = value;
            }
        }

        /// <summary>
        /// Simple example how to provide a combobox with standard values for a property
        /// </summary>
        private class DefaultGreetingsAttribute : Attribute, System.Collections.IEnumerable
        {
            internal const string defGreeting = "Some default greeting";
            private string[] mDefGreetings = { defGreeting, "Howdy!", "Good day!" };

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return mDefGreetings.GetEnumerator();
            }
        }

        public void PreValidate()
        {
            System.Windows.Forms.MessageBox.Show("PreValidate");
        }

        public void PostValidate()
        {
            System.Windows.Forms.MessageBox.Show("PostValidate");    
        }

        /// <summary>
        /// Pretend we are doing some work by showing a message box.
        /// </summary>
        /// <remarks>Also documents this in the runlog.</remarks>
        public void Execute()
        {
            if (TraceWrite != null)
            {
                TraceWrite(this, new TraceWriteEventArgs(String.Format("Executing with greeting {0}", GreetingsText)));
            }
            if (WriteToRunlog != null)
            {
                WriteToRunlog(string.Format(Properties.LocalizeMockPlugin.ShowingAMessageboxWithTheTextX0, GreetingsText));
            }
            System.Windows.Forms.MessageBox.Show(
                "Simple task shows a message:\r\n" + GreetingsText, 
                "Message shown by the mock task",
                System.Windows.Forms.MessageBoxButtons.OK);
        }

        public void DemoExecute()
        {
            System.Windows.Forms.MessageBox.Show("If we were really  executing, we would show something else.", "Demo Execution of Plugin Task");
        }

        /// <summary>
        /// The result of this function will be shown in the timetable.
        /// </summary>
        /// <returns></returns>
        public string GetTaskAction()
        {
            return string.Format("Just shows a message box with {0}",GreetingsText);
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
    }
}
