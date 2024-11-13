namespace SmartPulseTask.WebUi.Dtos
{
    public class TransactionHistoryGipDataDto
    {
        public long id { get; set; }
        public DateTime date { get; set; }
        public string contractName { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
    }
}
