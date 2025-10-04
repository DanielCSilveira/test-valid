using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IRabbitService
    {
        Task PublishAsync(string queue, object data);

    }
}
