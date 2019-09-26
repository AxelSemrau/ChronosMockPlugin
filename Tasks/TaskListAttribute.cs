using System.Collections;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.MethodEditor;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This attribute uses the task to which it is applied to query for other tasks in the method editor.
    /// The list of returned values is just a list of the tasks currently in the method editor.
    /// </summary>
    public class TaskListAttribute : StandardValueProviderAttribute
    {
        public override IEnumerable GetStandardValues(ITask theTask)
        {
            if (theTask is IPluginTaskAdapter adapter && adapter.Plugin is MockMethodEditorSnoopingTask snooper)
            {
                var theSnooper = snooper.MethodEditorSnooper;
                if (theSnooper != null)
                {
                    foreach (var editedTask in snooper.MethodEditorSnooper.EditedTasks)
                    {
                        if (editedTask.Task == theTask)
                        {
                            yield return $"This task: {editedTask}";
                        }
                        else
                        {
                            yield return $"{editedTask}{GetCaffeineInfo(editedTask)}";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If this is a BrewFrappuccino task, we'll find out if it uses decaf coffee.
        /// </summary>
        /// <param name="editedTask"></param>
        /// <returns>
        /// Just a demonstration how you can inspect properties of other tasks that are not just plain text.
        /// </returns>
        private string GetCaffeineInfo(ITaskInEditorInfo editedTask)
        {
            if (editedTask.Task is IPluginTaskAdapter adapter && adapter.Plugin is BrewFrappuccino brewer)
            {
                return $", {(brewer.Composition.DeCaffeinated ? "" : "not ")} decaffeinated";
            }
            return "";
        }
    }
}