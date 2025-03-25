using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartProfile : Profile
{
    public UpdateCartProfile()
    {
        CreateMap<UpdateCartCommand, Cart>().ReverseMap();
        CreateMap<Cart, UpdateCartResult>().ReverseMap();
        CreateMap<UpdateCartProductResult, CartProduct>().ReverseMap();
    }
}
