using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
using CustomCsvImporter;
using CustomCsvImporter.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CustomCsvImporterTests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    [DeploymentItem(@"TestFiles\ImportSettings.xml","CustomCsvImporter")]
    public class BasicTests
    {
        [TestMethod]
        public void CsvReaderWorks()
        {
            var testee = new ImportWorker();
            var data = testee.RawImport("TestData1.csv");
            Assert.AreEqual(3,data.Count,"Should have found three columns");
            Assert.IsTrue(data.Keys.SequenceEqual(new []{"SampleID","Comment","Concentration"}),"Unexpected columns");
            Assert.IsTrue(data["SampleID"].SequenceEqual(new []{"abc123","xyz456"}));
            Assert.IsTrue(data["Comment"].SequenceEqual(new[] { "The first sample", "The last sample" }));
            Assert.IsTrue(data["Concentration"].SequenceEqual(new[] { "1.28", "0.45" }));
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(@"TestFiles\CustomImportTest.cam",".")]
        public void ConfFileIsRead()
        {
            var testee = SetupTestee();
            Assert.IsFalse(string.IsNullOrEmpty(testee.GetChronosMethodFromConfig()), "Should have read some method name from the config file.");
        }

        [TestMethod]
        public void ColumnComparisionWorks()
        {
            var testee = SetupTestee();
            var colNames = new List<string> { "SampleID", "Comment", "Concentration" };
            testee.CheckMethodCompatibility(colNames, "fake");
            colNames.Add("OneTooMany");
            Assert.ThrowsException<ColumnMismatchException>(() => testee.CheckMethodCompatibility(colNames,"fake"),
                "Should have complained about an extra column");
            colNames.RemoveAt(3);
            colNames.RemoveAt(2);
            Assert.ThrowsException<ColumnMismatchException>(() =>
                testee.CheckMethodCompatibility(colNames, "fake"));
        }

        /// <summary>
        /// This demonstrates how you can write unit tests for your plugin, even if it depends on functionality provided by Chronos which
        /// is normally not available at test run time. Just Mock everything necessary.
        /// </summary>
        /// <returns></returns>
        private ImportWorker SetupTestee()
        {
            var mockConfig = new Mock<IConfigInfo>();
            mockConfig.SetupAllProperties();
            mockConfig.SetupGet(o => o.PathToMethods).Returns(TestContext.DeploymentDirectory);
            mockConfig.SetupGet(o => o.PathToInstrumentConfig).Returns(TestContext.DeploymentDirectory);
            var mockMethInfo = new Mock<IMethodInfo>();
            var colInfoGetter = mockMethInfo.SetupGet(o => o.Columns);

            // We only need the column names for our plugin, ignore the rest
            IReadOnlyList<IColumnInfo> ColInfosFrom(params string[] colNames)
            {
                var retval = new List<IColumnInfo>();
                foreach (var colName in colNames)
                {
                    var mock = new Mock<IColumnInfo>();
                    mock.SetupGet(o => o.Name).Returns(colName);
                    retval.Add(mock.Object);
                }

                return retval;
            }

            colInfoGetter.Returns(ColInfosFrom("SampleID", "Comment", "Concentration"));
            var testee = new ImportWorker(mockConfig.Object, filename => mockMethInfo.Object);
            return testee;
        }
    }
}
