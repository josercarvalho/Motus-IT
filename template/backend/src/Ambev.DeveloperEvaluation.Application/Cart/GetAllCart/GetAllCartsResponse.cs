namespace Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;

public class GetAllCartsResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<GetAllCartsProductResponse> CartProductsList { get; set; } = new List<GetAllCartsProductResponse>();

}
