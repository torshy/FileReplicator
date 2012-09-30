using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;

using TRock.Extensions;
using TRock.FileReplicator.Commands;
using TRock.FileReplicator.Core;
using TRock.FileReplicator.Views.Fileset.Dialogs;
using TRock.FileReplicator.Views.Fileset.Tabs;

namespace TRock.FileReplicator.ViewModels
{
    public class ScriptViewModel : NotificationObject
    {
        #region Constructors

        public ScriptViewModel(ObservableCollection<Script> scripts)
        {
            Scripts = scripts;
            AddExpressionCommand = new DelegateCommand(ExecuteAddExpression);
            AddFileCommand = new DelegateCommand(ExecuteAddFile);
            RemoveScriptCommand = new AutomaticCommand<Script>(ExecuteRemoveScript, CanExecuteRemoveScript);
            EditScriptCommand = new AutomaticCommand<Script>(ExecuteEditScript, CanExecuteEditScript);
        }

        #endregion Constructors

        #region Properties

        public string Title
        {
            get;
            set;
        }

        public ObservableCollection<Script> Scripts
        {
            get;
            private set;
        }

        public ICommand AddExpressionCommand
        {
            get;
            private set;
        }

        public ICommand AddFileCommand
        {
            get;
            private set;
        }

        public ICommand RemoveScriptCommand
        {
            get;
            private set;
        }

        public ICommand EditScriptCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        private bool CanExecuteEditScript(Script script)
        {
            return script != null;
        }

        private void ExecuteEditScript(Script script)
        {
            if (script is ScriptExpression)
            {
                ScriptExpression expression = (ScriptExpression)script;
                ScriptEditorWindow editor = new ScriptEditorWindow();
                editor.Owner = App.Current.MainWindow;
                editor.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                editor.Expression = expression.Expression;

                if (editor.ShowDialog() == true)
                {
                    expression.Expression = editor.Expression;
                }
            }
            else if (script is ScriptFile)
            {
                ScriptFile scriptFile = (ScriptFile)script;
                Process.Start("notepad.exe", scriptFile.Path);
            }
        }

        private bool CanExecuteRemoveScript(Script script)
        {
            return script != null && script.IsRemovable;
        }

        private void ExecuteRemoveScript(Script script)
        {
            Scripts.Remove(script);
        }

        private void ExecuteAddFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".rb";
            dialog.Filter = "Ruby (.rb)|*.rb";

            if (dialog.ShowDialog() == true)
            {
                dialog.FileNames.ForEach(file => Scripts.Add(new ScriptFile
                {
                    Path = file,
                    IsRemovable = true
                }));
            }
        }

        private void ExecuteAddExpression()
        {
            Scripts.Add(new ScriptExpression
            {
                Expression = "puts 40+2",
                IsRemovable = true
            });
        }

        #endregion Methods
    }
}