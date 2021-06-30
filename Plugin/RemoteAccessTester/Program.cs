using System;
using System.Windows.Forms;

/*!
 * \brief A minimal example program that shows how you could remotely control Chronos.
 * The program does not interface with Chronos directly, but through a service provided by the MockPlugin.
     */
namespace RemoteAccessTester
{
    /// <summary>
    /// Class containing the entry point.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MockPlugin.RemoteAccessTester.MessageTesterForm());
        }
    }
}
