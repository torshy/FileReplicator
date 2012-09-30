using System;
using System.ComponentModel;

namespace TRock.FileReplicator.Core
{
    public class ActivityLogEvent : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public DateTime DateTime
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}