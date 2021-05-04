using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discount.API.Entities;
using Discount.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Discount.API.Controllers
{ 
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(IDiscountRepository discountRepository, ILogger<DiscountController> logger)
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {
            try
            {
                var coupons = await _discountRepository.GetDiscounts();

                if (!coupons.Any())
                {
                    _logger.LogInformation("No coupons could be found.");
                    return NotFound("No coupons could be found");
                }

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get coupons", ex);
            }
        }

        [Route("[Action]/{ProductName}", Name = "GetDiscount")]
        [HttpGet]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Coupon>> GetCoupon(string productName)
        {
            try
            {
                var coupon = await _discountRepository.GetDiscount(productName);
                if (coupon == null)
                {
                    return NotFound($"Couldn't find any discount for: {productName}");
                }

                return Ok(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get coupon for: {productName}", ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _discountRepository.CreateDiscount(coupon);
                    return CreatedAtRoute("GetDiscount", new {coupon.ProductName, coupon.Amount}, coupon);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create discount for {coupon.ProductName}", ex);
                }
            }

            return BadRequest($"Invalid format, try again");
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return Ok(await _discountRepository.UpdateDiscount(coupon));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upgrade discount for {coupon.ProductName}", ex);
                }
            }

            return BadRequest($"Invalid format, try again");
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
            try
            {
                return Ok(await _discountRepository.DeleteDiscount(productName));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upgrade discount for: {productName}", ex);
            }
        }
    }
}
