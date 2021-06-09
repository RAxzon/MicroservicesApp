namespace Basket.API.Extensions
{
    public static class BasketDiscountExtensions
    {
        public static int CalculateDiscount(decimal price, int discount)
        {
            var newPrice = (int)price - discount;

            var validatePrice = newPrice <= 0 ? 1 : newPrice;

            return validatePrice;
        }
    }
}
