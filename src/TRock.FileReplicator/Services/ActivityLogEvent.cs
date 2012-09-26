using System;
using Microsoft.Practices.Prism.ViewModel;

namespace TRock.FileReplicator.Services
{
    public class ActivityLogEvent : NotificationObject
    {
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
    }
}