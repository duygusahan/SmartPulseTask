namespace SmartPulseTask.WebUi.Models
{
    public class ContractSummary
    {
       
            public string ContractName { get; set; }
            public DateTime DateTime { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal TotalQuantity { get; set; }
            public decimal WeightedAveragePrice { get; set; }
        }
    }

