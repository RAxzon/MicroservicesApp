using System;
using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;

        public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            try
            {
                var basket = await _basketRepository.GetBasket(userName);

                return Ok(basket ?? new ShoppingCart(userName));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting shopping cart with username: {userName}", ex);
                throw new Exception($"Error getting basket with username: {userName}", ex);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody]ShoppingCart basket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return Ok(await _basketRepository.UpdateBasket(basket));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating the basket with username: {basket.UserName}", ex);
                    throw new Exception($"Error updating the basket with username: {basket.UserName}", ex);
                }
            }

            return BadRequest(basket);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            try
            {
                await _basketRepository.DeleteBasket(userName);
                return Ok(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting the basket with username: {userName}", ex);
                throw new Exception($"Error deleting the basket with username: {userName}", ex);
            }
        }
    }
}
