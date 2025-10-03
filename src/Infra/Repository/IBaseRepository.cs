using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public interface IBaseRepository<T,PK> where T : class 
    {

        Task<T?> Get(PK id);

        Task<IEnumerable<T>> GetAll();

        Task<PK> Insert(T entity);

    }
}
