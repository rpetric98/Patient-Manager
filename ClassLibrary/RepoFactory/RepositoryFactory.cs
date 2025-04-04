using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.RepoFactory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IRepository<T> CreateRepository<T>() where T : class
        { 
            return _serviceProvider.GetRequiredService<IRepository<T>>();
        }
    }
}
