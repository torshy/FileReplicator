namespace TRock.FileReplicator.Core
{
    public interface IFileReplicationService
    {
        void Execute(Fileset fileset);
    }
}