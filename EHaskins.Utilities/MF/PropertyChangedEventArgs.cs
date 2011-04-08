#if NETMF
using System;
using Microsoft.SPOT;

namespace System.ComponentModel
{
    public class PropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the PropertyChangedEventArgs class.
        /// </summary>
        /// <param name="property"></param>
        public PropertyChangedEventArgs(string property)
        {
            _Property = property;
        }

        private string _Property;
        public string PropertyName
        {
            get { return _Property; }
            set
            {
                _Property = value;
            }
        }
        
    }
}
#endif