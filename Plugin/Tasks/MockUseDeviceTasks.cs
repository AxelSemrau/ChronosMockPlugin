﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using AxelSemrau.Chronos.Plugin;
using AxelSemrau.Chronos.Plugin.Consumables;
using Ctc.Palplus.Integration.Driver.Entities;
using MockPlugin.Consumables;
using MockPlugin.Device;
using MockPlugin.Properties;
using MockPlugin.SampleListColumns;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MockPlugin.Tasks
{
    /// <summary>
    /// Base class for our example, just contains some empty default implementations and a check for the right device type.
    /// </summary>
    public abstract class CoffeeMachineBaseTask : IConsumer
    {
        protected MockDevice mDevice;

        /// <summary>
        /// It makes no sense to use any other "Autosampler" than our fake coffee machine here.
        /// </summary>
        /// <param name="yourDevice"></param>
        protected void CheckForCoffeeMachine(IDevice yourDevice)
        {
            mDevice = yourDevice as MockDevice;
            if (mDevice == null && yourDevice != null)
            {
                throw new ArgumentException(
                    $"Device type {yourDevice.DisplayedTypeName} is not usable for a {GetType().Name}.");
            }
            if (yourDevice == null)
            {
                throw new ArgumentException("This kind of task needs an autosampler.");
            }
        }

        public void SetDevice(IDevice yourDevice)
        {
            CheckForCoffeeMachine(yourDevice);
        }

        public virtual void PreValidate()
        {
        }

        public virtual void PostValidate()
        {
        }

        public abstract void Execute();
        public abstract string GetTaskAction();

        public IConsumableManipulator Consumables { get; set; }
    }

    /// <summary>
    /// Custom category in the method editor for coffee making related tasks
    /// </summary>
    public class CoffeeCategory : CustomTaskCategoryAttribute
    {
        public CoffeeCategory(int ranking) : base(ranking)
        {
        }

        /// <summary>
        /// Localization aware category name
        /// </summary>
        public override string Name => LocalizeMockPlugin.CoffeeCategory_Name_Coffee;
    }

    /// <summary>
    /// Calls a method of the device class with one if its task parameters.
    /// </summary>
    /// <remarks>
    /// This is a simple example of a task that makes use of the device.
    /// It has some ordinary properties and feeds these values into some operation on the device.
    /// </remarks>
    [CoffeeCategory(1)]
    [ScheduleDiagramColor("Sienna")]
    public class BrewCoffee : CoffeeMachineBaseTask, ITaskForDevice
    {
        /// <summary>
        /// Do something with our device: The resulting message box is displayed by the device.
        /// </summary>
        public override void Execute()
        {
            mDevice.ShowTheMessage(Message);
            RegisterCoffeeConsumption();
        }

        public override void PostValidate()
        {
            RegisterCoffeeConsumption();
        }

        /// <summary>
        /// Inform the consumables tracker of the needed coffee amount.
        /// </summary>
        private void RegisterCoffeeConsumption()
        {
            Consumables.ModifyLevel(CoffeeConsumableManager.GetLocationIdentifier(mDevice, MockConsumablesForCoffeeMakerDevice.Coffee.Name), new Quantity(-7, Units.Gram));
        }

        /// <summary>
        /// Show this task's action in the timetable.
        /// </summary>
        /// <returns></returns>
        public override string GetTaskAction()
        {
            return LocalizeMockPlugin.BrewCoffee_GetTaskAction_Send_a_message_to_my_device;
        }

        /// <summary>
        /// Trivial property which can be changed in the method editor.
        /// </summary>
        public string Message { get; set; } = LocalizeMockPlugin.BrewCoffee_mMessage_Do_you_want_sugar_;
    }

    /// <summary>
    /// A task working on a complex parameter set.
    /// </summary>
    /// <remarks>
    /// This task has some more complex property which is assumed to be not suitable for simple text editing and token substitution or calculations.
    /// Therefore, we have to provide an editor of our own. The property data will be serialized and stored within the Chronos method.
    /// The Volume parameter can also be set directly from the method editor like a normal property.
    /// Additionally, the task has a rough idea of how long it can take.
    /// </remarks>
    [CoffeeCategory(2)]
    public class BrewFrappuccino : CoffeeMachineBaseTask, ITaskForDevice, IWantEditorUpdates, IGiveARuntimeHint , INotifyPropertyChanged
    {
        /// <summary>
        /// Enum properties result in nice drop-down lists.
        /// </summary>
        [TypeConverter(typeof(CreamTypeConverter))]
        public enum CreamType
        {
            Normal,
            LowFat,
            Vegan
        }

        /// <summary>
        /// Let's pretend the composition is really complex and better done with a custom editor.
        /// </summary>
        [Editor(typeof(CompositionEditor), typeof(UITypeEditor))]
        public class CompositionData
        {
            [XmlIgnore]
            public MockDevice DevInEditor { get; set; }

            public CreamType Cream
            {
                get;
                set;
            }

            public bool MuchIce { get; set; }

            public bool DeCaffeinated { get; set; }

            public uint Volume { get; set; }

            /// <summary>
            /// Default composition.
            /// </summary>
            public CompositionData()
            {
                Cream = CreamType.LowFat;
                MuchIce = false;
                DeCaffeinated = false;
                Volume = 250;
            }

            /// <summary>
            /// This value will be shown (read only) in the property field, with a button next to it that invokes our editor.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format(LocalizeMockPlugin.CompositionData_ToString_Cream___0___MuchIce___1___Decaffeinated___2___Size___3__mL,
                    Cream, MuchIce, DeCaffeinated, Volume);
            }
        }

        #region Component model UI type editor

        /// <summary>
        /// Provide an editor for our complex parameter set, the standard component model way.
        /// </summary>
        public class CompositionEditor : UITypeEditor
        {
            /// <summary>
            /// Which kind of editor should be shown?
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                // we want a real dialog, not just a drop down box
                return UITypeEditorEditStyle.Modal;
            }

            /// <summary>
            /// Pass the value to edit to our editor dialog and return the changed(?) value.
            /// </summary>
            /// <param name="context"></param>
            /// <param name="provider"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                var editProvider = (IWindowsFormsEditorService)provider?.GetService(typeof(IWindowsFormsEditorService));
                // create the editor form
                var compData = (CompositionData)value;
                // we could get a reference to the task here: var theTask = context.Instance as BrewFrappuccino;
                using (var myEditor = new FrappuccinoCompositionEditor(compData))
                {
                    // our editor will only modify the parameters if it has been closed by clicking OK
                    editProvider?.ShowDialog(myEditor);
                }
                return value;
            }
        }

        #endregion Component model UI type editor

        private CompositionData mComposition;

        /// <summary>
        /// Our extremely complex composition which could in no way be done with
        /// normal text-editable properties.
        /// </summary>
        public CompositionData Composition
        {
            get
            {
                if (mComposition != null && mComposition.DevInEditor == null)
                {
                    mComposition.DevInEditor = mDevInEditor;
                }
                return mComposition;
            }
            set 
            { 
                mComposition = value;
                if (mComposition != null)
                {
                    mComposition.DevInEditor = mDevInEditor;
                }
                RaiseVolumeChanged();
            }
        }

        public override string GetTaskAction()
        {
            return LocalizeMockPlugin.BrewFrappuccino_GetTaskAction_Brew_a_frappuccino__composition__ + Composition;
        }


        [DefaultUnit(Units.MilliLiter)]
        public uint Volume
        {
            get => mComposition.Volume;
            set
            {
                if(mComposition.Volume == value) return;
                mComposition.Volume = value;
                RaiseVolumeChanged();
            }
        }
        internal void RaiseVolumeChanged()
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(Volume)));
        }
        [DefaultUnit("mL")]
        public uint CupSize
        {
            get;
            set;
        }

        public BrewFrappuccino()
        {
            Composition = new CompositionData();
            CupSize = 250;
        }

        /// <summary>
        /// Send the recipe to our device.
        /// </summary>
        public override void Execute()
        {
            mDevice.BrewFrappuccino(Composition);
            RegisterConsumption(Composition);
        }

        public override void PostValidate()
        {
            RegisterConsumption(Composition);
        }

        /// <summary>
        /// Register consumption of coffee / cream with the consumables tracker.
        /// </summary>
        /// <param name="composition"></param>
        private void RegisterConsumption(CompositionData composition)
        {
            var sizeFactor = composition.Volume / 125.0;
            Consumables.ModifyLevel(CoffeeConsumableManager.GetLocationIdentifier(mDevice,MockConsumablesForCoffeeMakerDevice.Coffee.Name),new Quantity(-7*sizeFactor, Units.Gram));
            string creamName;
            switch (composition.Cream)
            {
                case CreamType.LowFat:
                    // intentionally picked name of a component that is not tracked. Results just in a log entry, not visible on tracker's page.
                    creamName = "Low Fat Milk";
                    break;
                case CreamType.Normal:
                    creamName = MockConsumablesForCoffeeMakerDevice.Milk.Name;
                    break;
                case CreamType.Vegan:
                    creamName = MockConsumablesForCoffeeMakerDevice.VeganCream.Name;
                    break;
                default:
                    creamName = "";
                    break;
            }
            Consumables.ModifyLevel(CoffeeConsumableManager.GetLocationIdentifier(mDevice, creamName),new Quantity(-10*sizeFactor,Units.MilliLiter));
        }

        /// <summary>
        /// The method editor informs us about changed values.
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <remarks>
        /// It is possible that the given propValue can not be converted to our 
        /// property type - for example, if there is a calculation or a reference
        /// to a different task's property in the method editor field.
        /// </remarks>
        public void PropertyEdited(string propName, object propValue)
        {
            if (propName == nameof(Volume))
            {
                // the uint32 converter can't convert from uint32 to uint32 but throws an exception.
                if (propValue is uint u)
                {
                    Volume = u;
                }
                else
                {
                    var conv = TypeDescriptor.GetConverter(Volume, true);
                    if (conv.IsValid(propValue))
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        Volume = (uint)conv.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, propValue);
                    }
                }
            }
            else if (propName == "Autosampler")
            {
                mDevInEditor = MockDevice.Instances.FirstOrDefault(someDev => someDev.Name == propValue?.ToString());
                if (mComposition != null)
                {
                    mComposition.DevInEditor = mDevInEditor;
                }
            }           
        }

        private MockDevice mDevInEditor;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Implementation of IGiveARuntimeHint

        /// <summary>
        /// Return some fake runtimes depending on the requested volume.
        /// </summary>
        public int? CalculatedRuntime
        {
            get
            {
                if (Volume > 250)
                {
                    return 30;
                }

                if (Volume > 100)
                {
                    return 15;
                }

                return 10;
            }
        }

        #endregion
    }

    /// <summary>
    /// This task will trigger a timer in our device which will make it complain about an error situation, even if at that time no task is trying to use it.
    /// </summary>
    [CoffeeCategory(3)]
    // ReSharper disable once UnusedType.Global
    public class PretendCoffeeMachineIsBroken : CoffeeMachineBaseTask, ITaskForDevice
    {
        public bool SoftStop { get; set; }
        public override void Execute()
        {
            mDevice.TriggerAbort(LocalizeMockPlugin
                    .PretendCoffeeMachineIsBroken_Execute_The_coffee_machine_s_heater_failed_,SoftStop);
        }

        public override string GetTaskAction()
        {
            return String.Format(LocalizeMockPlugin.PretendCoffeeMachineIsBroken_GetTaskAction_Will_make_the_device_abort_the_schedule_after_a_few_seconds_,SoftStop);
        }
    }
    /// <summary>
    /// In contrast to PretendCoffeeMachineIsBroken, this simulates an error that an autosampler could have while it is used by a task.
    /// </summary>
    [CoffeeCategory(4)]
    public class CoffeeMachineDoesNotWorkProperly : CoffeeMachineBaseTask, ITaskForDevice
    {
        public string ErrorDescription { get; set; } = "Some random error message that is shown to the user";
        public ErrorType ErrorType { get; set; } = ErrorType.MissingVial;
        /// <summary>
        /// Should we consider the error fixed after error handling has run?
        /// </summary>
        public bool ResolvedAfterHandling { get; set; } = false;
        public override void Execute()
        {
            mDevice.RaiseError(ErrorDescription,ErrorType, ResolvedAfterHandling);
        }

        public override string GetTaskAction()
        {
            return $"Raising error type {ErrorType} with description \"{ErrorDescription}\"";
        }
    }
}