using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCsvImporter.Exceptions
{
    /// <summary>
    /// Special exception class to make sure we check for the right thing in our unit tests.
    /// </summary>
    public class ColumnMismatchException : InvalidOperationException
    {
        public ColumnMismatchException(string[] extraInFile, string[] missingInFile, string filename) : base(BuildMessage(extraInFile,
            missingInFile, filename))
        {

        }

        private static string BuildMessage(string[] extraInFile, string[] missingInFile, string filename)
        {
            var errmsg =
                $"There is a mismatch between CSV file and the column definitions of method {filename}:\n";
            if (extraInFile.Any())
            {
                errmsg += "The following columns in the file are not present in the method: " +
                          string.Join(", ", extraInFile) + "\n";
            }

            if (missingInFile.Any())
            {
                errmsg += "The following columns from the method are missing in the file: " +
                          string.Join(", ", missingInFile);
            }

            return errmsg;
        }
    }

}
