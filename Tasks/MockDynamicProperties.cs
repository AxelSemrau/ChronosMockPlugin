using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This task demonstrates how to use a custom type descriptor to implement a dynamic list of properties.
    /// The number of properties can be changed by setting the PropCount property.
    /// </summary>
    [ScheduleDiagramColor(nameof(Colors.RoyalBlue))]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FlexibleArguments : CustomTypeDescriptor, ITask
    {
        #region ITask implementation, does nothing but show the parameter list in the time table

        public void PreValidate()
        {
            // nothing
        }

        public void PostValidate()
        {
            // nothing
        }

        public void Execute()
        {
            // nothing
        }

        private string GetArgList()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var key in mPropsValues.Keys)
            {
                sb.AppendFormat("{0}={1};", key, mPropsValues[key]);
            }
            if (sb.Length > 0)
            {
                sb.Length -= 1;
            }
            else
            {
                return "No properties";
            }
            return sb.ToString();
        }

        public string GetTaskAction()
        {
            return GetArgList();
        }

        #endregion ITask implementation, does nothing but show the parameter list in the time table

        /// <summary>
        /// Storage for fake property names and values
        /// </summary>
        private readonly Dictionary<string, object> mPropsValues = new Dictionary<string, object>();

        public FlexibleArguments()
        {
            // if we don't return this when asked, the ScheduleDiagramColor declaration above will not be seen by Chronos.
            mAttrs = TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
            // Default descriptor for the only real property
            var reflProp = TypeDescriptor.GetProperties(this, noCustomTypeDesc: true).Find(nameof(PropCount), false);
            mProps.Add(reflProp);
            // Add some fake properties to start with
            // This one has a unit:
            mProps.Add(new MyPropertyDescriptor("SomeIntParam", typeof(int), "s"));
            var resProp = new MyPropertyDescriptor("SomeCalculationResult", typeof(int));
            resProp.AddAttribute(new ReadOnlyAttribute(true));
            mProps.Add(resProp);
            mProps.Add(new MyPropertyDescriptor("SomeStringParam", typeof(string)));
            mProps.Add(new MyPropertyDescriptor("SomeBoolParam", typeof(bool)));
            PropCount = (uint)mProps.Count;
        }

        #region Type descriptor implementation

        /// <summary>
        /// Member descriptor "implementation" - the defaults are ok for us
        /// </summary>
        /// <remarks>Required for the property descriptor</remarks>
        private class MyMemberDescriptor : MemberDescriptor
        {
            public MyMemberDescriptor(string name)
                : base(name)
            { }
        }

        /// <summary>
        /// Custom property descriptor, redirecting get/set into our mPropsValues dictionary.
        /// </summary>
        private class MyPropertyDescriptor : PropertyDescriptor
        {
            private readonly List<Attribute> mExtraAttributes = new List<Attribute>();
            private readonly string mName;

            public MyPropertyDescriptor(string name, Type proptype, string unit = null)
                : base(new MyMemberDescriptor(name))
            {
                mName = name;
                PropertyType = proptype;
                AddAttribute(new DefaultUnitAttribute(unit));
            }

            public void AddAttribute(Attribute someAttr)
            {
                mExtraAttributes.Add(someAttr);
            }

            protected override void FillAttributes(System.Collections.IList attributeList)
            {
                base.FillAttributes(attributeList);
                foreach (var someAttr in mExtraAttributes)
                {
                    attributeList.Add(someAttr);
                }
            }

            public override Type PropertyType { get; }

            public override Type ComponentType => typeof(FlexibleArguments);

            public override void SetValue(object component, object value)
            {
                ((FlexibleArguments) component).mPropsValues[mName] = value;
            }

            public override object GetValue(object component)
            {
                ((FlexibleArguments) component).mPropsValues.TryGetValue(mName, out var retval);
                return retval;
            }

            // dont't care about the rest
            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override void ResetValue(object component)
            {
                // nothing
            }

            public override bool IsReadOnly => mExtraAttributes.OfType<ReadOnlyAttribute>().Any();

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }

        /// <summary>
        /// Must be overridden, else you'll get NullRefrences when trying to work with the descriptor.
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return mProps.Contains(pd) ? this : null;
        }

        /// <summary>
        /// Caches the list resulting from set_PropCount
        /// </summary>
        private PropertyDescriptorCollection mPropDescColl;

        public override PropertyDescriptorCollection GetProperties()
        {
            return mPropDescColl;
        }

        public override AttributeCollection GetAttributes() => mAttrs;

        #endregion Type descriptor implementation

        private readonly List<PropertyDescriptor> mProps = new List<PropertyDescriptor>();
        private readonly AttributeCollection mAttrs;
        private uint mPropCount;

        [DynamicPropertyMaster]
        public uint PropCount
        {
            get => mPropCount;
            set
            {
                var oldCount = mPropCount;
                // fill up to whatever the user wants, but do not let him hide PropCount
                mPropCount = Math.Max(1, value);
                if (oldCount != mPropCount)
                {
                    for (int i = mProps.Count; i < mPropCount; ++i)
                    {
                        mProps.Add(new MyPropertyDescriptor($"FakeProp{i}", typeof(string)));
                    }
                    // when the number is decreased, we just present a part of our internal list
                    mPropDescColl = new PropertyDescriptorCollection(mProps.Take((int)mPropCount).ToArray());

                    // the list of dynamic properties has just changed, 
                    // alert the reflection manager so the time table can access the new properties
                    TypeDescriptor.Refresh(this);
                }
            }
        }
    }
}