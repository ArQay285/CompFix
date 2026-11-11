using CompFix.Models;
using Microsoft.EntityFrameworkCore;
namespace CompFix.Services
{
    public static class ProductFilterExtensions
    {
        public static IQueryable<Product> ApplyProductFilter(this IQueryable<Product> query, ProductFilterParams filter)
        {
            
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.Name.Contains(filter.Search));
            }

           
            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            
            if (filter.HasDiscount == true)
            {
                query = query.Where(p => p.Discount > 0);
            }

            
            if (filter.StockStatus == "low")
            {
                query = query.Where(p => p.Quantity > 0 && p.Quantity <= 3);
            }
            else if (filter.StockStatus == "out")
            {
                query = query.Where(p => p.Quantity == 0);
            }

            
            if (filter.IsAvailable == true)
            {
                query = query.Where(p => p.IsAvailable && p.Quantity > 0);
            }

            
            if (filter.Condition.HasValue && filter.Condition.Value > 0)
            {
                query = query.Where(p => p.Condition == (ItemCondition)filter.Condition.Value);
            }

            
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

           
            query = filter.SortBy switch
            {
                "price_increase" => query.OrderBy(p => p.Price),
                "price_reduc" => query.OrderByDescending(p => p.Price),
                "date_old" => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return query;
        }
    }
}
