using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DefaultContext _Context;

        public ProductRepository(DefaultContext context)
        {
            _Context = context;
        }

        public async Task<Product> AddProduct(Product product)
        {
            await _Context.Products.AddAsync(product);
            await _Context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _Context.Products.Include(p => p.Rating).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllProductsCategories()
        {
            return await _Context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _Context.Products
                .Include(p => p.Rating)
                .FirstOrDefaultAsync(p => p.Id == id);

            //return await _Context.Products.FindAsync(id);
        }


        public async Task<Product> DeleteProduct(int id)
        {
            var product = await GetProductById(id);
            if (product != null)
            {
                _Context.Products.Remove(product);
                await _Context.SaveChangesAsync();
            }

            return product;
        }

        public async Task<Rating> UpdateRating(int productId, double newRate)
        {
            // Encontra o produto com a Rating associada
            var product = await _Context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                product.Rating = new Rating
                {
                    Rate = newRate,
                    Count = 1 // Inicializa o Count com 1
                };

                await _Context.SaveChangesAsync();
                return product.Rating;
            }

            // Tenta encontrar uma Rating associada ao novo valor
            var existingRating = product.Rating;

            if (existingRating != null && existingRating.Rate == newRate)
            {
                // Se a Rating já existir, incrementa a contagem
                existingRating.IncrementCount();
            }
            else
            {
                // Cria uma nova Rating associada ao produto
                existingRating = new Rating
                {
                    Rate = newRate,
                    Count = 1 // Inicializa o Count com 1
                };

                // Atualiza a Rating do produto
                product.Rating = existingRating;
            }

            // Salva as alterações no banco de dados
            await _Context.SaveChangesAsync();

            // Retorna a Rating atualizada ou recém-criada
            return existingRating;
        }
        public async Task<Product> UpdateProduct(Product product)
        {
            if (product != null)
            {
                _Context.Products.Update(product);
                await _Context.SaveChangesAsync();
            }


            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _Context.Products
                .Include(p => p.Rating)
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsAsync(int page, int size, string order)
        {
            var query = _Context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(order))
            {
                foreach (var orderClause in order.Split(','))
                {
                    var parts = orderClause.Trim().Split(' ');
                    var property = parts[0];
                    var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";
                    query = query.OrderBy($"{property} {direction}");
                }
            }

            var totalItems = await query.CountAsync();
            var products = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (products, totalItems);
        }

        public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsByCategoryAsync(string category, int page, int size, string order)
        {
            var query = _Context.Products.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(order))
            {
                foreach (var orderClause in order.Split(','))
                {
                    var parts = orderClause.Trim().Split(' ');
                    var property = parts[0];
                    var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";
                    query = query.OrderBy($"{property} {direction}");
                }
            }

            var totalItems = await query.CountAsync();
            var products = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (products, totalItems);
        }


        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetFilteredAndOrderedProductsAsync(
        int page, int size, string order, Dictionary<string, string> filters)
        {
            var query = _Context.Products.AsQueryable(); // Assuming Products is your DbSet<Product>

            // Apply Filters
            foreach (var filter in filters)
            {
                var field = filter.Key;
                var value = filter.Value;

                if (value.Contains("*"))
                {
                    var pattern = value.Replace("*", ""); // Remove asterisk for partial matches
                    query = query.Where(p => EF.Functions.Like(EF.Property<string>(p, field), $"%{pattern}%"));
                }
                else if (field.StartsWith("_min") || field.StartsWith("_max"))
                {
                    var baseField = field.Replace("_min", "").Replace("_max", "");
                    if (field.StartsWith("_min"))
                        query = query.Where(p => EF.Property<decimal>(p, baseField) >= decimal.Parse(value));
                    else if (field.StartsWith("_max"))
                        query = query.Where(p => EF.Property<decimal>(p, baseField) <= decimal.Parse(value));
                }
                else
                {
                    query = query.Where(p => EF.Property<string>(p, field) == value);
                }
            }

            // Apply Ordering
            if (!string.IsNullOrEmpty(order))
            {
                foreach (var orderClause in order.Split(','))
                {
                    var parts = orderClause.Trim().Split(' ');
                    var property = parts[0];
                    var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";
                    query = query.OrderBy($"{property} {direction}");
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id); // Default order by ID
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalItems);
        }
    }
}