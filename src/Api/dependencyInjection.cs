using Application.Service;
using Application.Services;
using Infra.Repository;
using AutoMapper;
using System.Reflection;


namespace Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencys(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CustomerService).Assembly);



            //Services
            services.AddScoped<ICustomerService, CustomerService>();


            //Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }

    
    }
}
