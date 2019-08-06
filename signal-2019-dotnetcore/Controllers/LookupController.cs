using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using signal_2019_dotnetcore.Extensions;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace signal_2019_dotnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public LookupController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<object> Post([FromBody] LookupRequest request)
        {
            try
            {
                var client = _clientFactory.CreateClient("lookup");
                var uri = $"/v1/PhoneNumbers/{request.CountryCode}{request.PhoneNumber}/?Type=carrier";
                var response = await client.GetAsJObjectAsync(uri);

                return Ok(new { info = response });
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        public class LookupRequest
        {
            [Required, JsonProperty(PropertyName = "phone_number")]
            public string PhoneNumber { get; set; }

            [Required, JsonProperty(PropertyName = "country_code")]
            public string CountryCode { get; set; }
        }
    }
}
