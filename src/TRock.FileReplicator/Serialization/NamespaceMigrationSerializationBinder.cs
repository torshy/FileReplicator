using System;
using System.Linq;

using Newtonsoft.Json.Serialization;

namespace TRock.FileReplicator.Serialization
{
    public class NamespaceMigrationSerializationBinder : DefaultSerializationBinder
    {
        #region Fields

        private readonly NamespaceMigration[] _migrations;

        #endregion Fields

        #region Constructors

        public NamespaceMigrationSerializationBinder(params NamespaceMigration[] migrations)
        {
            _migrations = migrations;
        }

        #endregion Constructors

        #region Methods

        public override Type BindToType(string assemblyName, string typeName)
        {
            var migration = _migrations.SingleOrDefault(p => p.FromAssembly == assemblyName && p.FromType == typeName);
            if (migration != null)
            {
                return migration.ToType;
            }
            return base.BindToType(assemblyName, typeName);
        }

        #endregion Methods
    }
}