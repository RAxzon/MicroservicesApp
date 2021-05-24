using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                _logger.LogError($"Discount with ProductName: {request.ProductName} was not found.");
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName: {request.ProductName} was not found."));
            }

            _logger.LogInformation($"Successfully received: {coupon}");

            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Model);

            try
            {
                await _repository.CreateDiscount(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create coupon", ex);
            }

            _logger.LogInformation($"Successfully created {request.Model}");
            
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Model);

            try
            {
                await _repository.UpdateDiscount(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update coupon", ex);
            }

            _logger.LogInformation($"Successfully updated {request.Model}");

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            try
            {
                var deleted = await _repository.DeleteDiscount(request.ProductName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete coupon", ex);
            }

            var result = new DeleteDiscountResponse
            {
                Success = true
            };

            return result;
        }
    }
}
