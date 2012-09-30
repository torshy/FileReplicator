using System;
using System.Collections.Generic;

namespace TRock.FileReplicator.Core
{
    public interface IActivityLogService
    {
        void Log(Guid id, string activity);

        void ClearLog(Guid id);

        IEnumerable<ActivityLogEvent> GetEvents(Guid id);
    }
}