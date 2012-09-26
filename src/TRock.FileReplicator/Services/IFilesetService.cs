using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TRock.FileReplicator.Models;

namespace TRock.FileReplicator.Services
{
    public interface IFilesetService
    {
        #region Properties

        IObservable<Fileset> FilesetAdded
        {
            get;
        }

        IObservable<Fileset> FilesetRemoved
        {
            get;
        }

        IEnumerable<Fileset> Filesets
        {
            get;
        }

        #endregion Properties

        #region Methods

        Task<IEnumerable<Fileset>> Save();

        Task<Fileset> Save(Fileset fileset);

        Task<Fileset> Add();

        Task Remove(Fileset fileset);

        #endregion Methods
    }
}