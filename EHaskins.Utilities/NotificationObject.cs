using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace EHaskins.Utilities
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            VerifyPropertyName(property);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private bool _ThrowOnInvalidPropertyName = true;
        public bool ThrowOnInvalidPropertyName
        {
            get { return _ThrowOnInvalidPropertyName; }
            set
            {
                if (_ThrowOnInvalidPropertyName == value)
                    return;
                _ThrowOnInvalidPropertyName = value;
                RaisePropertyChanged("ThrowOnInvalidPropertyName");
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (properties == null)
                properties = new Dictionary<string, bool>();

            bool propExists;
            if (!properties.ContainsKey(propertyName))
            {
                if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                {
                    properties.Add(propertyName, false);
                }
                else
                {
                    properties.Add(propertyName, true);
                }
            }
            propExists = properties[propertyName];

            if (!propExists)
            {
                string msg = "Invalid property name: " + propertyName;
                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }
        Dictionary<string, bool> properties;
    }
}
