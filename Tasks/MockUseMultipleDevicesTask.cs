using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
using MockPlugin.Device;

// ReSharper disable UnusedMember.Global

namespace MockPlugin.Tasks
{
    /// <summary>
    /// For some special cases it can be necessary to have a single task communicate with two different devices.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class MockUseMultipleDevicesTask : ITaskForDevice, IHaveRunlogOutput
    {
        private IDevice mDevice1;

        #region Implementation of ITask

        public void PreValidate()
        {
            
        }

        public void PostValidate()
        {
            
        }

        /// <summary>
        /// Showing how to use our device(s) from the task.
        /// </summary>
        public void Execute()
        {
            if (mDevice1 is IPAL3Services pal && pal.Simulating)
            {
                WriteToRunlog?.Invoke("Running simulation for validation");
            }
            else
            {
                WriteToRunlog?.Invoke($"Running {GetTaskAction()}");
                foreach (var dev in new[] {SecondCoffeeMakerOrPal3, ThirdAnyDevice}.OfType<MockDevice>())
                {
                        dev.SomeDummyMethod();
                }
            }
        }

        public string GetTaskAction()
        {
            return $"Task for devices {NameOrNotSet(mDevice1)}, {NameOrNotSet(SecondCoffeeMakerOrPal3)}, {NameOrNotSet(ThirdAnyDevice)}";
        }

        private string NameOrNotSet(IDevice dev)
        {
            return dev?.Name ?? "(not set)";
        }

        [DevicesLimited(typeof(IPAL3Services)) /* Accept only PAL3 as "Autosampler" */]
        public void SetDevice(IDevice yourDevice)
        {
            mDevice1 = yourDevice;
        }

        #endregion
        // Accept PAL3 or CoffeeMaker
        [DevicesLimited(typeof(MockDevice), typeof(IPAL3Services))]
        public IDevice SecondCoffeeMakerOrPal3 { get; set; }

        public IDevice ThirdAnyDevice { get; set; }

        #region Implementation of IHaveRunlogOutput

        public event Action<string> WriteToRunlog;

        #endregion
    }
}
