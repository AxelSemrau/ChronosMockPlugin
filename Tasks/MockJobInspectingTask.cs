using System.Linq;
using System.Text;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    public class JobInspectionDemo : ITask, INeedToInspectOtherTasks, INeedToCheckForJITLinks
    {
        #region Normal task methods

        void ITask.PreValidate()
        {
            if (mJITChecker != null)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("My enabled property {0}", mJITChecker(this, "Enabled") ? "will be set by a JIT expression" : "has an ordinary value"), "JobInspectionDemo");
            }
        }

        void ITask.PostValidate()
        {

        }

        private string GetJITInfo(ITaskInfo someTaskInfo, IAccessProperty somePropInfo)
        {
            if(mJITChecker == null)
            {
                return "N/A";
            }
            return mJITChecker(someTaskInfo.Task,somePropInfo.FullPath) ? "yes" : "no";
        }
        void ITask.Execute()
        {
            var sb = new StringBuilder();
            var i = 1;
            foreach (var someTaskInfo in mJobInspector.JobsTasks)
            {
                sb.AppendLine(string.Format("Task {0}", i++));
                sb.AppendFormat("Class {0}, {1} properties\r\n",
                    someTaskInfo.Task.GetType().Name,
                    someTaskInfo.PropertyAccessInfos.Count());
                foreach (var somePropInfo in someTaskInfo.PropertyAccessInfos)
                {
                    sb.AppendFormat("Property {0}: {1}, jit? {2}\r\n",
                        somePropInfo.PropInfo.Name,
                        somePropInfo.PropInfo.GetValue(somePropInfo.BaseObject, null),
                        GetJITInfo(someTaskInfo,somePropInfo));
                }
                sb.AppendLine();
            }
            System.Windows.Forms.MessageBox.Show(sb.ToString(), "Checked other tasks");
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
            set { mJobInspector = value; }
        }

        #endregion Job inspection
        #region JIT Check

        private JITCheckerDelegate mJITChecker; 
        public JITCheckerDelegate JITChecker { set { mJITChecker = value; } }
        #endregion


    }
}