using System.Linq;
using System.Text;
using AxelSemrau.Chronos.Plugin;
// ReSharper disable LocalizableElement

namespace MockPlugin.Tasks
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// Shows how to get information about other tasks in the schedule.
    /// </summary>
    public class JobInspectionDemo : ITask, INeedToInspectOtherTasks, INeedToCheckForJITLinks
    {
        #region Normal task methods

        void ITask.PreValidate()
        {
            if (mJitChecker != null)
            {
                System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow,
                    $"My enabled property {(mJitChecker(this, "Enabled") ? "will be set by a JIT expression" : "has an ordinary value")}", "JobInspectionDemo");
            }
        }

        /// <summary>
        /// Shows how you can disable other tasks in the schedule creation phase.
        /// If there's a previous JobInspectionDemo task, let's turn it off.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DisablePreviousTwin()
        {
            var prevTask = mJobInspector.CompleteSchedule
                .TakeWhile(someTaskInfo => someTaskInfo.PluginTask != mJobInspector.CurrentTask.PluginTask).LastOrDefault();
            if (prevTask?.PluginTask is JobInspectionDemo)
            {
                prevTask.Enabled = false;
            }
        }

        void ITask.PostValidate()
        {
            DisablePreviousTwin();
        }

        private string GetJitInfo(ITaskInfo someTaskInfo, IAccessProperty somePropInfo)
        {
            if(mJitChecker == null)
            {
                return "N/A";
            }
            return mJitChecker(someTaskInfo.Task,somePropInfo.FullPath) ? "yes" : "no";
        }
        void ITask.Execute()
        {
            var sb = new StringBuilder();
            var i = 1;
            foreach (var someTaskInfo in mJobInspector.JobsTasks)
            {
                sb.AppendLine($"Task {i++}");
                sb.AppendFormat("Class {0}, {1} properties\r\n",
                    someTaskInfo.Task.GetType().Name,
                    someTaskInfo.PropertyAccessInfos.Count());
                foreach (var somePropInfo in someTaskInfo.PropertyAccessInfos)
                {
                    sb.AppendFormat("Property {0}: {1}, jit? {2}\r\n",
                        somePropInfo.PropInfo.Name,
                        somePropInfo.PropInfo.GetValue(somePropInfo.BaseObject, null),
                        GetJitInfo(someTaskInfo,somePropInfo));
                }
                sb.AppendLine();
            }
            System.Windows.Forms.MessageBox.Show(Helpers.Gui.MainWindow, sb.ToString(), "Checked other tasks");
        }

        string ITask.GetTaskAction()
        {
            return "Snooping around";
        }

        #endregion Normal task methods

        #region Job inspection

        private IInspectJob mJobInspector;

        IInspectJob INeedToInspectOtherTasks.JobInspector
        {
            set => mJobInspector = value;
        }

        #endregion Job inspection
        #region JIT Check

        private JITCheckerDelegate mJitChecker; 
        public JITCheckerDelegate JITChecker { set => mJitChecker = value; }
        #endregion


    }
}