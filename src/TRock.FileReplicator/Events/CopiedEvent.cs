using Microsoft.Practices.Prism.Events;

using TRock.FileReplicator.Models;

namespace TRock.FileReplicator.Events
{
    public class CopiedEvent : CompositePresentationEvent<ReplicationItem>
    {
         
    }
}