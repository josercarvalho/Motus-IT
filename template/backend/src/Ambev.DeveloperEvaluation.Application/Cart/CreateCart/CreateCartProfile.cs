using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartProfile : Profile
{
    public CreateCartProfile()
    {
        CreateMap<CreateCartCommand, Cart>().ReverseMap();
        CreateMap<Cart, CreateCartResult>().ReverseMap();
        CreateMap<CreateCartProductResult, CartProduct>().ReverseMap();
    }
}
