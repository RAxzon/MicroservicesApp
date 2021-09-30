using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Extensions;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
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
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DiscountGrpcService _discountRGrpcService;

        public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger, DiscountGrpcService discountRGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _basketRepository = basketRepository ?? throw new ArgumentException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _discountRGrpcService = discountRGrpcService ?? throw new ArgumentException(nameof(discountRGrpcService));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentException(nameof(publishEndpoint));
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
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody]ShoppingCart basket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach(var item in basket.Items)
                    {
                        try
                        {
                            var coupon = await _discountRGrpcService.GetDiscount(item.ProductName);
                            item.Price = BasketDiscountExtensions.CalculateDiscount(item.Price, coupon.Amount);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error updating discount for basket: {basket}", ex);
                        }
                    }

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

        [HttpPost("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var basket = await _basketRepository.GetBasket(basketCheckout.UserName);

                    if (basket == null)
                    {
                        _logger.LogInformation($"Basket with username: {basketCheckout.UserName}, not found");
                        return NotFound($"Basket with username: {basketCheckout.UserName}, not found");
                    }

                    var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
                    eventMessage.EmailAddress = "Richiehstad@gmail.com";
                    eventMessage.TotalPrice = basket.TotalPrice;

                    await _publishEndpoint.Publish(eventMessage);

                    await _basketRepository.DeleteBasket(basketCheckout.UserName);

                    return Accepted();

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error getting basket", ex);
                    throw new Exception($"Error getting basket", ex);
                }
            }

            return BadRequest();
        }
    }
}
