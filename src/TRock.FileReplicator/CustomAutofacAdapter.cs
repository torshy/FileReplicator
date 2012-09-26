using System;
using System.Collections.Generic;

using Autofac;

using Microsoft.Practices.ServiceLocation;

namespace TRock.FileReplicator
{
    public class CustomAutofacAdapter : ServiceLocatorImplBase
    {
        #region Fields

        private readonly IComponentContext _container;

        #endregion Fields

        #region Constructors

        public CustomAutofacAdapter(IComponentContext container)
        {
            _container = container;
        }

        #endregion Constructors

        #region Methods

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return new[]
            {
                _container.Resolve(serviceType)
            };
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            object instance;

            if (string.IsNullOrEmpty(key))
            {
                if (!_container.TryResolve(serviceType, out instance))
                {
                    throw new Exception(string.Format("cannot resolve type {0}", serviceType));
                }
            }
            else
            {
                if (!_container.TryResolveNamed(key, serviceType, out instance))
                {
                    throw new Exception(string.Format("cannot resolve type {0}", serviceType));
                }
            }

            return instance;
        }

        #endregion Methods
    }
}