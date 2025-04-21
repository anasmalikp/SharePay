using AutoMapper;
using SharePay.Models.ViewModels;

namespace SharePay.Models.Mapper
{
    public class MapperClass : Profile
    {
        public MapperClass()
        {
            CreateMap<Users, UsersVM>().ReverseMap();
            CreateMap<Expenses, ExpenseVM>().ReverseMap();
        }
    }
}
