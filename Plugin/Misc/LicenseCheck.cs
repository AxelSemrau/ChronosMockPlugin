using System;
using System.IO;
using System.Reflection;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.WPF.ViewModels;
using AxelSemrau.Chronos.Plugin.WPF.Views;
using MockPlugin.Misc;

[assembly: MockLicenseCheck]

namespace MockPlugin.Misc
{

    public class MockLicenseCheckAttribute : LicenseCheckerAttribute
    {
        public override string CheckLicense()
        {
            if (!LicenseAlreadyOk())
            {
                var vm = Helpers.UtilityFactories.CreateViewModel<LicenseCheckViewModel>();
                vm.AcceptedProductIDs = new []{4711};
                // Our extremely secure reference date for the license check.
                vm.ProductReferenceDate = new FileInfo(Assembly.GetExecutingAssembly().Location).CreationTime;
                vm.ValidityAfterReferenceDate = TimeSpan.FromDays(123);

                vm.Explanation = new NoLicenseNeededExplanation();
                vm.ActivationUriBuilder = () => new Uri($"https://github.com/AxelSemrau/ChronosMockPlugin?ID={vm.ComputerId}&Serial={vm.SerialNumber}");
                var view = Helpers.UtilityFactories.CreateView<LicenseCheckDialog>();
                view.DataContext = vm;
                view.ShowDialog();
                if (!vm.IsActivationKeyOk)
                {
                    Helpers.Debug.TraceWrite("No valid license");
                }
                // ReSharper disable once RedundantIfElseBlock
                else
                {
                    // save the license information
                }
            }
            // For our demo plugin, the license is always ok. Otherwise we would tell the reason here and get a log entry stating why the plugin can't be loaded.
            return null;
        }

        /// <summary>
        /// Check if we have some persistantly saved license state, and this state is ok.
        /// </summary>
        /// <returns>For our mock plugin: Always not ok.</returns>
        private bool LicenseAlreadyOk() => false;
    }
}
