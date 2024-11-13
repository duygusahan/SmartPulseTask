using SmartPulseTask.WebUi.Dtos;
using SmartPulseTask.WebUi.Models;
using static SmartPulseTask.WebUi.Models.ApiModel;

namespace SmartPulseTask.WebUi.Services
{
    public class TransactionService
    {
        
        public List<ContractSummary> CalculateContractStatistics(List<TransactionHistoryGipDataDto> data)
        {
            var groupedData = data.GroupBy(x => x.contractName)
                                  .Select(g => new ContractSummary
                                  {
                                      ContractName = g.Key,
                                      TotalAmount = g.Sum(x => (x.price * x.quantity) / 10),
                                      TotalQuantity = g.Sum(x => x.quantity / 10)
                                  }).ToList();

            foreach (var contract in groupedData)
            {
                if (contract.TotalQuantity != 0)
                {
                    contract.WeightedAveragePrice = contract.TotalAmount / contract.TotalQuantity;
                }
                else
                {
                    contract.WeightedAveragePrice = 0;
                }
            }

            return groupedData;
        }
    }
}
