using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AxelSemrau.Chronos.Plugin;

namespace MockPlugin.Acquisition_Service
{
    /// <summary>
    /// Parameter class for an acquisition service that has a variable number of properties
    /// </summary>
    public class DynamicAcqPars : CustomTypeDescriptor
    {
        private int mNoOfFakePars = 3;
        private readonly List<PropertyDescriptor> mPropList = new List<PropertyDescriptor>();
        private readonly List<string> mFakePars = new List<string>();

        public DynamicAcqPars()
        {
            UpdateParlist();
        }
        [DynamicPropertyMaster]
        public uint NoOfFakePars
        {
            get { return (uint)mNoOfFakePars; }
            set
            {
                mNoOfFakePars = (int)value;
                UpdateParlist();
            }
        }

        private void UpdateParlist()
        {
            mPropList.Clear();
            mPropList.AddRange(TypeDescriptor.GetProperties(this,noCustomTypeDesc:true).OfType<PropertyDescriptor>());
            var oldCount = mFakePars.Count;
            if (oldCount > mNoOfFakePars)
            {
                mFakePars.RemoveRange(mNoOfFakePars,oldCount-mNoOfFakePars);
            }
            else if(oldCount < mNoOfFakePars)
            {
                for (var i = oldCount + 1; i <= mNoOfFakePars; ++i)
                {
                    mFakePars.Add($"Placeholder {i}");
                }
            }
            else
            {
                // wenn nichts geändert wurde, kein Refresh nötig
                return;
            }
            TypeDescriptor.Refresh(this);
        }

        #region Overrides of CustomTypeDescriptor

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(mPropList.Concat(GetFakePropDescriptors()).ToArray());
        }

        private IEnumerable<PropertyDescriptor> GetFakePropDescriptors()
        {
            for(var index = 0; index < mFakePars.Count; ++index)
            {
                yield return new StringListPropertyDescriptor(mFakePars, index);
            }
        }

        /// <summary>
        /// Maps a property to an entry in our string list.
        /// </summary>
        private class StringListPropertyDescriptor : PropertyDescriptor
        {
            private readonly int mIndex;
            private readonly List<string> mParList;

            public StringListPropertyDescriptor(List<string> fakePars, int index) : base(name:$"DynPar{index+1}",attrs: new Attribute[] { })
            {
                mParList = fakePars;
                mIndex = index;
            }

            #region Overrides of PropertyDescriptor

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return mParList[mIndex];
            }

            public override void ResetValue(object component)
            {
                
            }

            public override void SetValue(object component, object value)
            {
                mParList[mIndex] = value?.ToString();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override Type ComponentType => typeof(DynamicAcqPars);
            public override bool IsReadOnly => false;
            public override Type PropertyType => typeof(string);

            #endregion
        }

        #endregion

        public override string ToString()
        {
            return $"{mNoOfFakePars} Parameters, Values: {string.Join(", ", mFakePars)}";
        }
    }
    /// <summary>
    /// Acquisition service for a parameter class that has a variable number of properties.
    /// </summary>
    public class MockDynamicParAcquisitionService : IAcquisitionService<DynamicAcqPars>
    {
        #region Implementation of IAcquisitionServiceBase

        public string Name => "MockDynamicParAcquisition";
        public bool IsAvailable => true;
        public bool Abort { get; set; } = false;

        #endregion

        #region Implementation of IAcquisitionService<DynamicAcqPars>

        public void Validate(DynamicAcqPars parameters)
        {
            // nothing
        }

        public void RunAcquisition(DynamicAcqPars parameters)
        {
            // nothing
        }

        #endregion
    }
}
