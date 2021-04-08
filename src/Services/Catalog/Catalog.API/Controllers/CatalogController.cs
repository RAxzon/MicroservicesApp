using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class CatalogController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository prodProductRepository, ILogger<CatalogController> logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _productRepository = prodProductRepository ?? throw new ArgumentException(nameof(prodProductRepository));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProducts();

                if (!products.Any())
                {
                    _logger.LogInformation("No products were found");
                    return NotFound($"No products were found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                throw new Exception("Error getting products", ex);
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            try
            {
                var product = await _productRepository.GetProduct(id);

                if (product == null)
                {
                    _logger.LogInformation($"Product with {id} was not found");
                    return NotFound($"The product with id: {id}, was not found.");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product");
                throw new Exception("Error getting product", ex);
            }
        }

        [Route("[Action]/{category}", Name = "GetProductsByCategory")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            try
            {
                var products = await _productRepository.GetProductByCategory(category);

                if (!products.Any())
                {
                    _logger.LogInformation($"Products with {category} were not found");
                    return NotFound($"The category: {category}, was not found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                throw new Exception("Error getting products", ex);
            }
        }

        [Route("[Action]/{name}", Name = "GetProductsByName")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
        {
            try
            {
                var products = await _productRepository.GetProductByCategory(name);

                if (!products.Any())
                {
                    _logger.LogInformation($"Products with {name} were not found");
                    return NotFound($"The name: {name}, was not found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                throw new Exception("Error getting products", ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productRepository.CreateProduct(product);
                    return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,$"Error creating {product}");
                    throw new Exception($"Error adding {product}", ex);
                }
            }

            return BadRequest(product);

        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            try
            {
                await _productRepository.UpdateProduct(product);
                _logger.LogInformation($"Deleted product with id: {product.Id}");
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with id: {product.Id}");
                throw new Exception($"Error updating product with id: {product.Id}", ex);
            }
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                await _productRepository.DeleteProduct(id);
                _logger.LogInformation($"Deleted product with id: {id}");
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with id: {id}");
                throw new Exception($"Error updating product with id: {id}", ex);
            }
        }
    }
}
