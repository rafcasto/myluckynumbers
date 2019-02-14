using RestSharp;
using Newtonsoft;
using Newtonsoft.Json;
using DotNetCoreSelenium.Models;

namespace DotNetCoreSelenium.Repositories
{
    public class LottoRepository
    {
        public  Lotto GetLottoBy(string lottoNumber)
        {
            var client = new RestClient($"https://apigw.mylotto.co.nz/api/results/v1/results/lotto/{lottoNumber}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "a5fc1e30-15be-435a-8f4a-f715ba5a4fb0");
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);

            var lotto = JsonConvert.DeserializeObject<LottoWrapper>(response.Content);
            return lotto.Lotto;
        }
    }
}