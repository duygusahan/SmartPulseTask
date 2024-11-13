using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartPulseTask.WebUi.Services;
using static SmartPulseTask.WebUi.Models.ApiModel;
using System.Net.Http.Headers;
using System.Text;
using SmartPulseTask.WebUi.Dtos;
using static SmartPulseTask.WebUi.Services.TransactionService;
using SmartPulseTask.WebUi.Models;
using System.Diagnostics;

namespace SmartPulseTask.WebUi.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ApiService _apiService;
        private readonly TransactionService _transactionService;

        public DefaultController(ApiService apiService, TransactionService transactionService)
        {
            _apiService = apiService;
            _transactionService = transactionService;
        }


        public async Task<IActionResult> Index(string startDate, string endDate)
        {

            var transactionData = await _apiService.GetDataAsync(startDate, endDate);




            if (transactionData != null)
            {
                var contractStatistics = CalculateContractStatistics(transactionData);


                return View(contractStatistics);
            }


            return View(transactionData);
        }

        private List<ContractSummary> CalculateContractStatistics(List<TransactionHistoryGipDataDto> data)
        {
            var groupedData = data.GroupBy(x => x.contractName)
                                  .Select(g => new ContractSummary
                                  {
                                      ContractName = g.Key,
                                      TotalAmount = g.Sum(x => (x.price * x.quantity) / 10),
                                      TotalQuantity = g.Sum(x => x.quantity / 10),
                                      DateTime = ParseContractDate(g.Key)
                                  }).ToList();

            foreach (var contract in groupedData)
            {
                contract.WeightedAveragePrice = contract.TotalQuantity != 0
                                                ? contract.TotalAmount / contract.TotalQuantity
                                                : 0;
            }

            return groupedData;
        }
        private DateTime ParseContractDate(string contractName)
        {
            try
            {
                // contractName beklenen formata uygun mu kontrol et
                if (contractName.Length != 10 || !contractName.StartsWith("PH"))
                    throw new FormatException("Geçersiz kontrat formatı");

                // Yıl, ay, gün ve saat parçalarını al ve parse et
                int year = int.Parse("20" + contractName.Substring(2, 2)); 
                int month = int.Parse(contractName.Substring(4, 2));      
                int day = int.Parse(contractName.Substring(6, 2));         
                int hour = int.Parse(contractName.Substring(8, 2));        

                // DateTime nesnesini oluştur ve geri döndür
                return new DateTime(year, month, day, hour, 0, 0);
            }
            catch (Exception ex)
            {
                // Hata oluşursa loglama yapılabilir veya uygun bir şekilde işlenebilir
                Console.WriteLine($"Kontrat adından tarih parse edilirken hata oluştu '{contractName}': {ex.Message}");
                return DateTime.MinValue; // Hata durumunda varsayılan veya geçersiz bir değer döndür
            }
        }

        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "Bir hata oluştu"
            };
            return View(model);
        }

    }
}
