using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.RepoFactory
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>() where T : class;
    }
}
