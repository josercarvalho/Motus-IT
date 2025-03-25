using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ICartRepository
    {

        Task<(IEnumerable<Cart> Items, int TotalCount)> GetCartsAsync(int page, int size, string orderBy);
        Task<IEnumerable<Cart>> GetCartsAsync();
        Task<Cart> GetCartByIdAsync(int id);
        Task<Cart> AddCartAsync(Cart cart);
        Task<Cart?> UpdateCartAsync(Cart cart);
        Task<Cart> DeleteCartAsync(int id);
        Task<(IEnumerable<Cart> Carts, int TotalItems)> GetPagedCartsAsync(int page, int size, string order);
        Task<(IEnumerable<Cart> Items, int TotalItems)> GetFilteredAndOrderedCartsAsync(
        int page, int size, string order, Dictionary<string, string> filters);
    }
}
