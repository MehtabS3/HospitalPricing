using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Systme.Threading.Tasks;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;

namespace HospitalPriceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class PricingController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public PricingController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet("get-prices")]
        public async Task<IActionResults> GetPrices(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL is required.");   //basic validation

            }

            try{
                var response = await _httpClient.GetAsync(url);
                if(!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Failed to get data : {response.StatusCode}");

                }
                vas csvContent = await response.Content.ReadAsStringAsync();
                return Ok("CSV data recieved. parsing will be implemented next.");

            }
            catch (Exception ex){
                return StatusCode(500,$"Internal Server Error: {ex.Message}")
                
            }

        }
        private List<TreatmentData> ParseCsv(string csvContent)
        {
            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(reader, new CsvConfiguration(CulturInfo.InvariantCulture){Delimiter = ","});

            var treatmentDataList = new List<TreatmentData>();
            bool headerFound = false;

            while (csv.Read())
            {
                var currentRow = csv.Context.Parser.Record;
                if(!headerFound && currentRow.Contains("description"))
                {
                    csv.ReadHeader();
                    headerFound = true;
                    break;
                }

            }
            if (!headerFound){
                Console.WriteLine("Header not found!");
                return treatmentDataList;
            }

            while (csv.Read())
            {
                try{
                    var treatment = csv.GetField<string>("description");
                    var minPrice = csv.GetField<decimal?>("standard_charge|min") ?? 0;
                    var maxPrice = csv.GetField<decimal?>("standard_charge|max") ?? 0;

                    treatmentDataList.Add( new TreatmentData{
                        Treatment = treatment;
                        MinPrice = minPrice;
                        MaxPrice = maxPrice;

                    });

                }
                catch(Exception ex) {
                    console.WriteLine($"Parsing Error: {ex.Message}");

                }
            }
        }
    }
}
