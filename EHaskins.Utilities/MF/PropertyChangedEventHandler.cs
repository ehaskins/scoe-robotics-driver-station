#if NETMF
using System;

namespace System.ComponentModel
{
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
}
#endif