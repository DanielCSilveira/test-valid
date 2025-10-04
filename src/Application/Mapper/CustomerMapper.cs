using Application.DTO;
using AutoMapper;
using Infra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreatedAt)
                );
        }
    }
}
