using System;
using Microsoft.Practices.Prism.Events;
using TRock.FileReplicator.Core;

namespace TRock.FileReplicator.Events
{
    public class CopyErrorEvent : CompositePresentationEvent<Tuple<ReplicationItem, Exception>>
    {
        
    }
}