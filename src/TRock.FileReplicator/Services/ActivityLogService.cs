using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TRock.FileReplicator.Core;

namespace TRock.FileReplicator.Services
{
    public class ActivityLogService : IActivityLogService
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ConcurrentDictionary<Guid, ObservableCollection<ActivityLogEvent>> _logCache;

        #endregion Fields

        #region Constructors

        public ActivityLogService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _logCache = new ConcurrentDictionary<Guid, ObservableCollection<ActivityLogEvent>>();
            MaxNumberOfActivityEventsPerLog = 250;
        }

        #endregion Constructors

        #region Properties

        public int MaxNumberOfActivityEventsPerLog
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void Log(Guid id, string activity)
        {
            if (_dispatcher.CheckAccess())
            {
                var log = GetActivityLog(id);
                log.Insert(0, new ActivityLogEvent {DateTime = DateTime.Now, Message = activity});

                if (log.Count > MaxNumberOfActivityEventsPerLog)
                {
                    log.RemoveAt(log.Count - 1);
                }
            }
            else
            {
                _dispatcher.BeginInvoke(new Action<Guid, string>(Log), DispatcherPriority.Background, id, activity);
            }
        }

        public void ClearLog(Guid id)
        {
            if (_dispatcher.CheckAccess())
            {
                GetActivityLog(id).Clear();
            }
            else
            {
                _dispatcher.BeginInvoke(new Action<Guid>(ClearLog), DispatcherPriority.Background, id);
            }
        }

        public IEnumerable<ActivityLogEvent> GetEvents(Guid id)
        {
            return GetActivityLog(id);
        }

        private ObservableCollection<ActivityLogEvent> GetActivityLog(Guid id)
        {
            ObservableCollection<ActivityLogEvent> log;
            if (!_logCache.TryGetValue(id, out log))
            {
                log = new ObservableCollection<ActivityLogEvent>();
                _logCache.TryAdd(id, log);
            }

            return log;
        }

        #endregion Methods
    }
}