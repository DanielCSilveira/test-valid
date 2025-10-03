using Infra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public interface ICustomerRepository: IBaseRepository<Customer,Guid>
    {
    }
}
