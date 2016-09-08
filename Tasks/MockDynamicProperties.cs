using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Tasks
{
    /// <summary>
    /// This task demonstrates how to use a custom type descriptor to implement a dynamic list of properties.
    /// The number of properties can be changed by setting the PropCount property.
    /// </summary>
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
        private Dictionary<string, object> mPropsValues = new Dictionary<string, object>();

        public FlexibleArguments()
        {
            // Default descriptor for the only real property
            var reflPropDesc = TypeDescriptor.GetProperties(this, noCustomTypeDesc: true).Find("PropCount", false);
            myProps.Add(reflPropDesc);
            // Add some fake properties to start with
            // This one has a unit:
            myProps.Add(new MyPropertyDescriptor("SomeIntParam", typeof(int), "s"));
            var resProp = new MyPropertyDescriptor("SomeCalculationResult", typeof(int));
            resProp.AddAttribute(new ReadOnlyAttribute(true));
            myProps.Add(resProp);
            myProps.Add(new MyPropertyDescriptor("SomeStringParam", typeof(string)));
            myProps.Add(new MyPropertyDescriptor("SomeBoolParam", typeof(bool)));
            PropCount = (uint)myProps.Count;
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
            private readonly Type mType;

            public MyPropertyDescriptor(string name, Type proptype, string unit = null)
                : base(new MyMemberDescriptor(name))
            {
                mName = name;
                mType = proptype;
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

            public override Type PropertyType
            {
                get { return mType; }
            }

            public override Type ComponentType
            {
                get { return typeof(FlexibleArguments); }
            }

            public override void SetValue(object component, object value)
            {
                var myObj = component as FlexibleArguments;
                myObj.mPropsValues[mName] = value;
            }

            public override object GetValue(object component)
            {
                object retval = null;
                (component as FlexibleArguments).mPropsValues.TryGetValue(mName, out retval);
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

            public override bool IsReadOnly
            {
                get { return mExtraAttributes.OfType<ReadOnlyAttribute>().Any(); }
            }

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
            return myProps.Contains(pd) ? this : null;
        }

        /// <summary>
        /// Caches the list resulting from set_PropCount
        /// </summary>
        private PropertyDescriptorCollection mPropDescColl;

        public override PropertyDescriptorCollection GetProperties()
        {
            return mPropDescColl;
        }

        #endregion Type descriptor implementation

        private List<PropertyDescriptor> myProps = new List<PropertyDescriptor>();
        private uint mPropCount;

        [DynamicPropertyMaster]
        public uint PropCount
        {
            get
            {
                return mPropCount;
            }
            set
            {
                var oldCount = mPropCount;
                // fill up to whatever the user wants, but do not let him hide PropCount
                mPropCount = Math.Max(1, value);
                if (oldCount != mPropCount)
                {
                    for (int i = myProps.Count; i < mPropCount; ++i)
                    {
                        myProps.Add(new MyPropertyDescriptor(string.Format("FakeProp{0}", i), typeof(string)));
                    }
                    // when the number is decreased, we just present a part of our internal list
                    mPropDescColl = new PropertyDescriptorCollection(myProps.Take((int)mPropCount).ToArray());
                    TypeDescriptor.Refresh(this);
                }
            }
        }
    }
}