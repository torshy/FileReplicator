using Microsoft.Scripting.Hosting;

namespace TRock.FileReplicator.Models
{
    public class ScriptExpression : Script
    {
        #region Properties

        public string Expression
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Execute(ScriptEngine engine, ScriptScope scope)
        {
            if (!string.IsNullOrEmpty(Expression))
            {
                engine.Execute(Expression, scope);
            }
        }

        #endregion Methods
    }
}