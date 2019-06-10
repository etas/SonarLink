// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SonarLink.TE.MVVM
{
    /// <summary>
    /// Source which can trigger PropertyChangedEvents
    /// </summary>
    public abstract class NotifyPropertyChangeSource : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    
        /// <summary>
        /// Raises the PropertyChange event for the property specified
        /// </summary>
        /// <param name="propertyName">Property name to update. Is case-sensitive.</param>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
