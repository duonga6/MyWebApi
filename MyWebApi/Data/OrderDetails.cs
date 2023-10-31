namespace MyWebApi.Data
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public byte Discount { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
