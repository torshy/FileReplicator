using System;

namespace TRock.FileReplicator.Serialization
{
    public class NamespaceMigration
    {
        #region Properties

        public string FromAssembly
        {
            get;
            set;
        }

        public string FromType
        {
            get;
            set;
        }

        public Type ToType
        {
            get;
            set;
        }

        #endregion Properties
    }
}