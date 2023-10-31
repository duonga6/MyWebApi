namespace MyWebApi.Data
{
    public enum OrderStatus
    {
        New = 0,
        Payment = 1,
        Complete = 2,
        Cancel = -1,
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelivery { get; set; }
        public OrderStatus Status { get; set; }
        public required string ReceiverName { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
