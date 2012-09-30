using Microsoft.Scripting.Hosting;

namespace TRock.FileReplicator.Core
{
    public class ScriptFile : Script
    {
        #region Properties

        public string Path
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Execute(ScriptEngine engine, ScriptScope scope)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                engine.ExecuteFile(Path, scope);
            }
        }

        #endregion Methods
    }
}