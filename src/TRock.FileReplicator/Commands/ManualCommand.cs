﻿#region Header

// <copyright file="ManualCommand.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

#endregion Header

using System;
using System.Windows.Input;

namespace TRock.FileReplicator.Commands
{
    /// <summary>
    /// Delegate-based parameterless command, implementing <see cref="CanExecuteChanged"/> as a weak event.
    /// </summary>
    public sealed class ManualCommand : ICommand, IDisposable
    {
        #region Fields

        /// <summary>
        /// Implementation of <see cref="CanExecuteChanged"/>.
        /// </summary>
        private CanExecuteChangedCore canExecuteChanged = new CanExecuteChangedCore();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualCommand"/> class with null delegates.
        /// </summary>
        public ManualCommand()
        {
            this.CanExecuteChangedSubscription = (sender, e) => this.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualCommand"/> class with the specified delegates.
        /// </summary>
        /// <param name="execute">The delegate invoked to execute this command.</param>
        /// <param name="canExecute">The delegate invoked to determine if this command may execute. This may be invoked when <see cref="RaiseCanExecuteChanged"/> is invoked.</param>
        public ManualCommand(Action execute, Func<bool> canExecute)
            : this()
        {
            this.Execute = execute;
            this.CanExecute = canExecute;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// This is a weak event. Provides notification that the result of <see cref="ICommand.CanExecute"/> may be different.
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged.CanExecuteChanged += value; }
            remove { this.canExecuteChanged.CanExecuteChanged -= value; }
        }

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the delegate invoked to determine if this command may execute. Setting this does not raise <see cref="CanExecuteChanged"/>.
        /// </summary>
        public Func<bool> CanExecute
        {
            get; set;
        }

        /// <summary>
        /// Gets a delegate that invokes <see cref="NotifyCanExecuteChanged"/>. This delegate exists for the lifetime of this command.
        /// </summary>
        public EventHandler CanExecuteChangedSubscription
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the delegate invoked to execute this command. Setting this does not raise <see cref="CanExecuteChanged"/>.
        /// </summary>
        public Action Execute
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Frees all weak references to delegates held by <see cref="CanExecuteChanged"/>.
        /// </summary>
        public void Dispose()
        {
            this.canExecuteChanged.Dispose();
        }

        /// <summary>
        /// Determines if this command can execute.
        /// </summary>
        /// <param name="parameter">This parameter is ignored.</param>
        /// <returns>Whether this command can execute.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="parameter">This parameter is ignored.</param>
        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event for any listeners still alive, and removes any references to garbage collected listeners.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            this.canExecuteChanged.OnCanExecuteChanged();
        }

        #endregion Methods
    }
}