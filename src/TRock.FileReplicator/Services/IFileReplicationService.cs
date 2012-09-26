using TRock.FileReplicator.Models;

namespace TRock.FileReplicator.Services
{
    public interface IFileReplicationService
    {
        void Execute(Fileset fileset);
    }
}