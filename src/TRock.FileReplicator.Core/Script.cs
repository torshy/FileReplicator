using Microsoft.Scripting.Hosting;

namespace TRock.FileReplicator.Core
{
    public abstract class Script
    {
        #region Properties

        public bool IsRemovable
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public abstract void Execute(ScriptEngine engine, ScriptScope scope);

        #endregion Methods
    }
}