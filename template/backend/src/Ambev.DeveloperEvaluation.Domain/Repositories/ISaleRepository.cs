using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllSales();
        Task<Sale> GetSaleById(int id);
        Task<Sale> CreateSale(Sale sale);
        Task<Sale> UpdateSale(Sale sale);
        Task<Sale> DeleteSale(int id);
    }
}
