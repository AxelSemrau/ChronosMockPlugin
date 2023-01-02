using System;
using System.ComponentModel;
using System.Text;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This task demonstrates how to make the property list depend on the picked autosampler.
    /// </summary>
    public class MockDynamicPropsDependingOnDevice : CustomTypeDescriptor, ITaskForDevice, IWantEditorUpdates
    {
        private const string NoDevice = "No device";
        private string mDevName;
        private string mDynPropValue;
        private IDevice mDevice;

        public void PreValidate()
        {
        }

        public void PostValidate()
        {
        }

        public void Execute()
        {
        }

        public string GetTaskAction() => "Do nothing";

        [DynamicPropertyMaster]
        public void SetDevice(IDevice yourDevice)
        {
            if (mDevice != yourDevice)
            {
                mDevice = yourDevice;
                // The PropertyEdited handler is called by Chronos for texts that can not be converted to a device instance.
                // Otherwise, we get the device set here. If so, we must react in the same way.
                PropertyEdited("Autosampler",mDevice);
            }
        }

        public void PropertyEdited(string propName, object propValue)
        {
            if (propName == "Autosampler")
            {
                var newDevName = ((propValue as IDevice)?.Name) ?? NoDevice;
                if (mDevName != newDevName)
                {
                    mDevName = newDevName;
                    TypeDescriptor.Refresh(this);
                }
            }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            var dynProp = new MyPropertyDescriptor(this);
            return new PropertyDescriptorCollection(new PropertyDescriptor[]{dynProp});
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return pd is MyPropertyDescriptor ? this : null;
        }

        public class MyPropertyDescriptor : PropertyDescriptor
        {
            private readonly MockDynamicPropsDependingOnDevice mParent;

            public MyPropertyDescriptor(MockDynamicPropsDependingOnDevice parent) : base(BuildPropName(parent.mDevName),null)
            {
                mParent = parent;
            }

            private static string BuildPropName(string parentDevName)
            {
                var sb = new StringBuilder("DynPropFor");
                foreach (var someChar in parentDevName?? "NoDevice")
                {
                    if (char.IsLetter(someChar))
                    {
                        sb.Append(someChar);
                    }
                }
                return sb.ToString();
            }

            public override bool CanResetValue(object component) => false;

            public override object GetValue(object component) => mParent.mDynPropValue;

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value) => mParent.mDynPropValue = value?.ToString();

            public override bool ShouldSerializeValue(object component) => true;

            public override Type ComponentType => typeof(MockDynamicPropsDependingOnDevice);
            public override bool IsReadOnly => false;
            public override Type PropertyType => typeof(string);
        }
    }
}
