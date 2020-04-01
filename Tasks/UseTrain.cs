using System;
using AxelSemrau.Chronos.Plugin;
using MockPlugin.Device;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace MockPlugin.Tasks
{

    /// <summary>
    /// Just show that some kind of operation can be done with our "Train" device parts.
    /// </summary>
    public class UseTrain : ITaskForDevice
    {
        private TrainPart mMyTrainPart;

        public void PreValidate()
        {
            
        }

        public void PostValidate()
        {
            if (mMyTrainPart == null)
            {
                throw new InvalidOperationException("Need a train part as \"Autosampler\"");
            }
        }

        public bool DoorsOpen { get; set; }

        public void Execute()
        {
            mMyTrainPart.DoorsOpen = DoorsOpen;
        }

        public string GetTaskAction()
        {
            return $"{(DoorsOpen ? "Opening" : "Closing")} the doors of {mMyTrainPart?.Name ?? "N/A"}";
        }

        [DevicesLimited(typeof(TrainPart))]
        public void SetDevice(IDevice yourDevice)
        {
            mMyTrainPart = yourDevice as TrainPart;
        }
    }
}
