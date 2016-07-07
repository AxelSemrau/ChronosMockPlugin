using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Acquisition_Service
{
    public class SimpleParameters
    {
        public string InstrumentMethod { get; set; }
        public int InstrumentNumber { get; set; }
        public bool WaitForEmptyQueue { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
            return $"Method = {InstrumentMethod}, Instrument = {InstrumentNumber}, WaitForEmptyQueue = {WaitForEmptyQueue}";
        }

        #endregion
    }
    public class MockSimpleAcquisitionService : AxelSemrau.Chronos.Plugin.IAcquisitionService<SimpleParameters>, 
        AxelSemrau.Chronos.Plugin.ITraceLogger,
        IHaveConfigurator,
        IConfigurableAcquisitionService, ISequenceAwareAcquisitionService
    {
        #region Implementation of IAcquisitionServiceBase

        public string Name => "MockSimpleAcquisition";
        public bool IsAvailable => true;
        public bool Abort { set { TraceLog($"Abort flag {(value ? "set" : "reset")}");} }
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

        private const string rootEl = "MockServiceConfigRootElement";

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
            new System.Windows.Interop.WindowInteropHelper(dlg) { Owner = owner };
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
            get { return ConfigToXml(mConfigParam); }
            set { mConfigParam = ConfigFromXml(value); }
        }

        private string ConfigFromXml(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var el = XElement.Parse(value);
                if (el?.Name == rootEl)
                {
                    return el.Value;
                }
            }
            return mConfigParam;
        }


        private string ConfigToXml(string configParam)
        {
            var el = new XElement(rootEl) {Value = configParam};
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
    }
}
