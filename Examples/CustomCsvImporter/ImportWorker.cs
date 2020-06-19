using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using AxelSemrau.Chronos.Plugin;
using CustomCsvImporter.Exceptions;
using DataAccess;
using Microsoft.Win32;

namespace CustomCsvImporter
{
    /// <summary>
    /// Demonstration of a very basic and relatively restrictive CSV importer. No fancy column mappings - it just uses one fixed (config file) Chronos method, and requires an 1:1 column match
    /// with the CSV file (standard Excel export options applied).
    /// </summary>
    /// <remarks>
    /// The idea is to have smaller example projects, focusing on specific problems as they come up, as the mock plugin trying to show it all can perhaps get a bit confusing.
    /// </remarks>
    public class ImportWorker : IWorkWithSampleLists, INeedCellAccess
    {
        private readonly IConfigInfo mConfInfo;
        private readonly Func<string, IMethodInfo> mMethInfoGetter;

        /// <summary>
        /// Called by Chronos when the button is pressed. Runs on a thread pool thread, all GUI interactions have to be done on the GUI thread.
        /// </summary>
        public void DoYourJob()
        {
            try
            {
                var chronosMethod = GetChronosMethodFromConfig();

                var sampleListFilename = Helpers.Gui.GuiTaskFactory.StartNew(PickCsvFile).Result;
                if (!string.IsNullOrEmpty(sampleListFilename))
                {

                    var data = RawImport(sampleListFilename);
                    CheckMethodCompatibility(data.Keys.ToArray(), chronosMethod);
                    // Everything does not look too bad, let's try to fill the sample list.
                    Helpers.Gui.GuiTaskFactory.StartNew(() => FillSampleList(chronosMethod, data)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Helpers.Gui.GuiTaskFactory.StartNew(() => MessageBox.Show("Error during import:\n" + ex.Message,
                    "CSV Import failed", MessageBoxButton.OK, MessageBoxImage.Error));
                // re-throw for logging the stack trace.
                throw;
            }

        }

        /// <summary>
        /// This is run on the GUI thread and should transfer the data from CSV to the sample list.
        /// </summary>
        /// <param name="methodFilename"></param>
        /// <param name="data"></param>
        private void FillSampleList(string methodFilename, Dictionary<string, List<string>> data)
        {
            var sl = SampleList;
            sl.Clear();
            var lines = data.First().Value.Count;
            for(var lineNo = 0; lineNo < lines; ++lineNo)
            {
                // backwards compatibility quirk: After Clear(), we already have one line present.
                var line = lineNo > 0 ? sl.Insert() : sl.Lines.Single();
                // creates the columns for the method
                line.Method = methodFilename;
                foreach (var colName in data.Keys)
                {
                    line.Cells[colName].Value = data[colName][lineNo];
                }
            }
        }


        public void CheckMethodCompatibility(IList<string> colNames, string chronosMethod)
        {
            var methInfo = mMethInfoGetter(chronosMethod);
            var methodsColNames = methInfo.Columns.Select(someMethInfo => someMethInfo.Name).ToArray();
            var extraColsInData = colNames.Except(methodsColNames).ToArray();
            var missingColsInData = methodsColNames.Except(colNames).ToArray();
            if (extraColsInData.Length + missingColsInData.Length > 0)
            {

                throw new ColumnMismatchException(extraColsInData, missingColsInData, chronosMethod);
            }
        }

        public string GetChronosMethodFromConfig()
        {
            var methPath = mConfInfo.PathToMethods;
            var confFilePath = Path.Combine(mConfInfo.PathToInstrumentConfig, Assembly.GetExecutingAssembly().GetName().Name, "ImportSettings.xml");
            var confDoc = XDocument.Load(confFilePath);
            var methName = confDoc.Root?.Element("ChronosMethod")?.Value;
            if (string.IsNullOrEmpty(methName))
            {
                throw new FileNotFoundException("Could not get Chronos method from configuration file.");
            }

            var fullPath = Path.IsPathRooted(methName) ? methName : Path.Combine(methPath, methName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Could not find method file {fullPath}, specified in config file {confFilePath}");
            }
            return fullPath;
        }

        public Dictionary<string, List<string>> RawImport(string filename)
        {
            var retval = new Dictionary<string,List<string>>();
            var data = DataTable.New.ReadCsv(filename);
            foreach (var someColName in data.ColumnNames)
            {
                var colValues = new List<string>(data.NumRows);
                retval[someColName] = colValues;
                foreach (var someRow in data.Rows)
                {
                    colValues.Add(someRow[someColName]);
                }
            }
            return retval;
        }

        private string PickCsvFile()
        { 
            var ofd = new OpenFileDialog()
            {
                CheckFileExists = true, CheckPathExists = true, DefaultExt = "CSV", AddExtension = true,
                Filter = "CSV File|*.CSV",
                Title = "Demo Custom CSV Import"
            };
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                return ofd.FileName;
            }
            return null;
        }

        /// <summary>
        /// Used when created by Chronos
        /// </summary>
        public ImportWorker() : this(Helpers.Config, GetDefaultMethodInfoGetter())
        {
        }

        private static Func<string, IMethodInfo> GetDefaultMethodInfoGetter()
        {
            if (Helpers.Storage != null)
            {
                return Helpers.Storage.Method;
            }
            return null;
        }

        /// <summary>
        /// Used by unit tests to provide information that normally Chronos provides.
        /// </summary>
        /// <param name="confInfo"></param>
        /// <param name="methInfoGetter"></param>
        public ImportWorker(IConfigInfo confInfo, Func<string,IMethodInfo> methInfoGetter)
        {
            mConfInfo = confInfo;
            mMethInfoGetter = methInfoGetter;
        }

        public string ButtonCaption => "Import...";
        public Icon ButtonIcon => Resources.CsvButton;
        public ISampleListAccessor SampleList { private get; set; }
    }
}
