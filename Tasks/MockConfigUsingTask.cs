using System;
using System.Text;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This tasks uses the config helper to access some information about the instrument configuration and just dumps it to the runlog.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class DumpConfigInfo : ITask, IHaveRunlogOutput
    {
        #region Implementation of ITask

        public void PreValidate()
        {
            // nothing
        }

        public void PostValidate()
        {
            // nothing
        }

        public void Execute()
        {
            var info = Helpers.Config;
            var sb = new StringBuilder();
            sb.AppendLine($"Path to methods: {info.PathToMethods}");
            sb.AppendLine($"Default sample lists location: {info.PathToSampleLists}");
            sb.AppendLine($"Location of instrument configuration: {info.PathToInstrumentConfig}");
            sb.AppendLine();
            sb.AppendLine("Trays:");
            foreach (var tray in info.Trays)
            {
                sb.AppendLine($"{tray.Name} on sampler {tray.Sampler?.ToString() ?? "N/A"}, valid indices {tray.FirstIndex} - {tray.LastIndex} ({tray.NoOfPositions} total)");
            }
            WriteToRunlog?.Invoke(sb.ToString());
        }

        public string GetTaskAction() => "Dump some configuration information to the runlog";

        #endregion

        #region Implementation of IHaveRunlogOutput

        public event Action<string> WriteToRunlog;

        #endregion
    }
}
