using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommand : IRequest<UpdateCartResult>
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartProductResult> CartProductsList { get; set; } = new List<UpdateCartProductResult>();

    public ValidationResultDetail Validate()
    {
        var validator = new UpdateCartValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    public UpdateCartCommand(int cartId, int userId, DateTime date, List<UpdateCartProductResult> products) // 
    {
        CartId = cartId;
        UserId = userId;
        Date = date;
        CartProductsList = products;
    }
}
