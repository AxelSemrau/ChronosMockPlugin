using AxelSemrau.Chronos.Plugin.MethodEditor;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This task just shows how you can react on the method editor contents and provide context sensitive property standard values.
    /// </summary>
    public class MockMethodEditorSnoopingTask : IMethodEditorSnoopingTask
    {

        [TaskList]
        public string SomeProperty { get; set; }

        public void PreValidate()
        {
            // nothing to do
        }

        public void PostValidate()
        {
            // nothing to do
        }

        public void Execute()
        {
            // nothing to do
        }

        public string GetTaskAction() =>
            "This task is just meant as a demonstration for method editor interaction and does not do anything.";

        /// <summary>
        /// Since the getter is internal, the property will not be visible in the method editor.
        /// </summary>
        public IMethodEditorSnooper MethodEditorSnooper { internal get; set; }
    }
}
