﻿// <copyright file="INotifyPropertyChangedExtensions.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace TRock.FileReplicator.Commands.INotifyPropertyChanged
{
    /// <summary>
    /// Provides extension methods for objects implementing <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Subscribes a handler to the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for a specific property.
        /// </summary>
        /// <typeparam name="TObject">The type implementing <see cref="INotifyPropertyChanged"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The object implementing <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="expression">The lambda expression selecting the property.</param>
        /// <param name="handler">The handler that is invoked when the property changes.</param>
        /// <returns>The actual delegate subscribed to <see cref="INotifyPropertyChanged.PropertyChanged"/>.</returns>
        public static PropertyChangedEventHandler SubscribeToPropertyChanged<TObject, TProperty>(
            this TObject source,
            Expression<Func<TObject, TProperty>> expression,
            Action<TObject> handler)
            where TObject : System.ComponentModel.INotifyPropertyChanged
        {
            // This is similar but not identical to:
            //   http://www.ingebrigtsen.info/post/2008/12/11/INotifyPropertyChanged-revisited.aspx
            string propertyName = source.GetPropertyName(expression);
            PropertyChangedEventHandler ret = (s, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        handler(source);
                    }
                };
            source.PropertyChanged += ret;
            return ret;
        }
    }
}
