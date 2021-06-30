using System;
using System.Collections.Generic;
using System.Linq;
using AxelSemrau.Chronos.Plugin;
// ReSharper disable UnusedMember.Global

namespace MockPlugin.Device
{
    /// <summary>
    /// For lack of a better idea, the demonstration for the multipart device is a train consisting of a locomotive and some different cars.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Train : IMultipartDevice, IHaveRunlogOutput
    {
        public string DisplayedTypeName => "Train";
        public string DeviceTypeDescription => "Public Transport Train";
        public string Name { get; set; }
        public void Connect()
        {
            // connection state is irrelevant for the scope of this demonstration.
        }

        public void Disconnect()
        {
        }

        private readonly List<TrainPart> mAllParts = new List<TrainPart>();

        /// <summary>
        /// Just adding all parts of the train. The parts keep references to the full train.
        /// </summary>
        public Train()
        {
            mAllParts.Add(new TrainPart(this,TrainPartType.Locomotive));
            mAllParts.Add(new TrainPart(this, TrainPartType.DiningCar));
            for (int i = 0; i < 7; ++i)
            {
                mAllParts.Add(new TrainPart(this, TrainPartType.PassengerCar) {Num = i+57});
            }
        }
        
        /// <summary>
        /// Not used.
        /// </summary>
        public event Action<ConnectionState> ConnectionStateChanged
        {
            add {  }
            remove {  }
        }

        public IReadOnlyCollection<IDevice> Parts => mAllParts;

        internal void ClosedSomeDoor()
        {
            if (mAllParts.All(somePart => !somePart.DoorsOpen))
            {
                // Very important information about some events can be written to the runlog.
                WriteToRunlog?.Invoke($"Important: Train {Name} has all doors closed.");
            }
        }

        public event Action<string> WriteToRunlog;
    }

    public enum TrainPartType
    {
        PassengerCar,
        Locomotive,
        DiningCar
    }

    /// <summary>
    /// The device part (well, train part) can't do much, just send status messages when the doors are opened/closed.
    /// </summary>
    public class TrainPart : IDevice, IProvideStatusMessages
    {
        private readonly Train mTrain;
        private readonly TrainPartType mMyType;
        public string DisplayedTypeName => "Part of a train";
        public string DeviceTypeDescription => "Locomotive or car";
        /// <summary>
        /// We have to return a descriptive name that also allows identification of the base device - just do it like the PAL3 and
        /// return BaseName + ":" + PartName
        /// </summary>
        public string Name
        {
            get => BuildName();
            set { var dummy = value; }
        }

        /// <summary>
        /// Important! Without overriding ToString here, you will not be able to pick the device from an autosampler column in the sample list.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        public int Num { get; set; }

        private string BuildName()
        {
            switch (mMyType)
            {
                case TrainPartType.DiningCar:
                    return $"{mTrain.Name}:Diner";
                case TrainPartType.Locomotive:
                    return $"{mTrain.Name}:Locomotive";
                default:
                    return $"{mTrain.Name}:Car{Num}";
            }
        }

        public void Connect()
        {
            
        }

        public void Disconnect()
        {
            
        }

        public TrainPart(Train parent, TrainPartType myType)
        {
            mTrain = parent;
            mMyType = myType;
        }

        private bool mDoorsAreOpen;
        public bool DoorsOpen
        {
            get => mDoorsAreOpen;
            set
            {
                if (value != mDoorsAreOpen)
                {
                    mDoorsAreOpen = value;
                    SetStatusMessage?.Invoke($"Doors are {(value ? "open" : "closed")}");
                    if (!value)
                    {
                        mTrain.ClosedSomeDoor();
                    }
                }
            }
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public event Action<ConnectionState> ConnectionStateChanged
        {
            add {  }
            remove {  }
        }

        public event Action<string> SetStatusMessage;
    }
}
