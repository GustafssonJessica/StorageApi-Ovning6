using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.Data;
using StorageApi.DTOs;
using StorageApi.Models;

namespace StorageApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StorageApiContext _context;

        public ProductsController(StorageApiContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct(string? categoryName)
        {
            var query = _context.Product.AsQueryable();
            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category == categoryName);
            }

            var productsDtos = await query.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Count = p.Count
            }).ToListAsync();

            return Ok(productsDtos);

        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            var productDto = await _context.Product.
                Where(p => p.Id == id).
                Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Count = p.Count
                }).FirstOrDefaultAsync();


            if (product == null)
            {
                return NotFound();
            }

            return Ok(productDto);
        }

        // GET:api/products/stats
        [HttpGet("stats")]
        public async Task<ActionResult<ProductStatsDto>> GetStats()
        {
            var totalCount = await _context.Product.SumAsync(p => p.Count);
            var totalValue = await _context.Product.SumAsync(p => p.Count * p.Price);
            var averagePrice = await _context.Product.AverageAsync(p => p.Price);

            return new ProductStatsDto
            {
                TotalCount = totalCount,
                TotalValue = totalValue,
                AveragePrice = averagePrice
            };
        }





        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                Shelf = product.Shelf,
                Count = product.Count,
                Description = product.Description
            };

            _context.Product.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
