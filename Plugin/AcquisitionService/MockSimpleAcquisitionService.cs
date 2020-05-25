using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using AxelSemrau.Chronos.Plugin;
// ReSharper disable UnusedAutoPropertyAccessor.Global

/*!
 * \brief Enables you to add support for a Chromatography Data System (or similar) to Chronos.
 * An acquisition service added this way will behave like the builtin services and can be used with the Acquisition task.
 */
namespace MockPlugin.AcquisitionService
{
    /// <summary>
    /// Example parameter set for an acquisition service.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class SimpleParameters
    {
        public string InstrumentMethod { get; set; }
        public int InstrumentNumber { get; set; } = 1;
        public bool WaitForEmptyQueue { get; set; }

        [DefaultUnit("min")]
        public double GetReadyTimeout { get; set; }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum SampleTypeValue
        {
            Calibration,
            Analysis,
            Special
        }

        public SampleTypeValue SampleType { get; set; } = SampleTypeValue.Analysis;

        #region Overrides of Object

        public override string ToString()
        {
            return $"Method = {InstrumentMethod}, Instrument = {InstrumentNumber}, WaitForEmptyQueue = {WaitForEmptyQueue}, GetReadyTimeout = {GetReadyTimeout}, SampleType = {SampleType}";
        }

        #endregion
    }
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// An example acquisition service using a fixed list of parameters.
    /// </summary>
    /// <remarks>
    /// For a more complex example, see MockDynamicParAcquisitionService.
    /// </remarks>
    public class MockSimpleAcquisitionService : IAcquisitionService<SimpleParameters>, 
        ITraceLogger,
        IHaveConfigurator,
        IConfigurableAcquisitionService,
        ISequenceAwareAcquisitionService,
        IHaveRunlogOutput,
        ICommandUsingAcquisitionService<MockCommandAndParameters>
    {
        #region Implementation of IAcquisitionServiceBase

        public string Name => "MockSimpleAcquisition";
        public bool IsAvailable => true;
        public bool Abort { set => TraceLog($"Abort flag {(value ? "set" : "reset")}"); }
        public void ValidateCommand(MockCommandAndParameters cmdAndPars)
        {
            WriteToRunlog?.Invoke($"Validating command '{cmdAndPars.SomeFakeCommand}' for instrument {cmdAndPars.InstrumentNumber}");
        }

        public void RunCommand(MockCommandAndParameters cmdAndPars)
        {
            WriteToRunlog?.Invoke($"Running command '{cmdAndPars.SomeFakeCommand}' for instrument {cmdAndPars.InstrumentNumber}");
        }

        #endregion

        #region Implementation of IAcquisitionService<SimpleParameters>

        public void Validate(SimpleParameters parameters)
        {
            TraceLog($"Validating simple parameters: {parameters}");
        }

        public void RunAcquisition(SimpleParameters parameters)
        {
            TraceLog($"Running acquisition with parameters: {parameters}");
        }

        #endregion

        private void TraceLog(string txt)
        {
            TraceWrite?.Invoke(this, new TraceWriteEventArgs(txt));
        }

        #region Implementation of ITraceLogger

        public event EventHandler<TraceWriteEventArgs> TraceWrite;

        private const string RootEl = "MockServiceConfigRootElement";

        #endregion

        /// <summary>
        /// Some fake configuration parameter
        /// </summary>
        private string mConfigParam = "Foobar";

        #region Implementation of IHaveConfigurator

        public string ShowConfigDialog(IntPtr owner, string oldConfig)
        {

            var dlg = new ConfigDialog()
            {
                ParamText = ConfigFromXml(oldConfig)
            };
            var dummy = new System.Windows.Interop.WindowInteropHelper(dlg) { Owner = owner };
            if (dlg.ShowDialog() ?? false)
            {
                return ConfigToXml(dlg.ParamText);
            }
            return oldConfig;
        }

        #endregion

        #region Implementation of IConfigurableAcquisitionService

        public string Configuration
        {
            get => ConfigToXml(mConfigParam);
            set => mConfigParam = ConfigFromXml(value);
        }

        private string ConfigFromXml(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var el = XElement.Parse(value);
                if (el.Name == RootEl)
                {
                    return el.Value;
                }
            }
            return mConfigParam;
        }


        private string ConfigToXml(string configParam)
        {
            var el = new XElement(RootEl) {Value = configParam};
            return el.ToString();
        }

        #endregion

        #region Implementation of ISequenceAwareAcquisitionService

        public void BeginSequence(string pathToChronosSampleList)
        {
            TraceLog($"Sequence ${pathToChronosSampleList} was started.");
        }

        public void EndSequence()
        {
            TraceLog("Sequence has ended.");
        }

        #endregion

        public event Action<string> WriteToRunlog;
    }

    /// <summary>
    /// Example definition of a command and its parameters that can be called for an AxelSemrau.Chronos.Plugin.ICommandUsingAcquisitionService.
    /// </summary>
    public class MockCommandAndParameters
    {
        public int InstrumentNumber { get; set; }
        public string SomeFakeCommand { get; set; } = "SayHello";
        public override string ToString() => $"'{SomeFakeCommand}' (instrument {InstrumentNumber})";
    }
}
