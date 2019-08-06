using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using signal_2019_dotnetcore.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace signal_2019_dotnetcore.Controllers
{
    [ApiController]
    [Route("api/verification")]
    public class VerificationController : Controller
    {
        private readonly TwilioOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        public VerificationController(IHttpClientFactory clientFactory, IOptions<TwilioOptions> twilioOptions)
        {
            var options = twilioOptions.Value;

            if (string.IsNullOrEmpty(options.VerifyServiceSid))
                throw new ArgumentException(nameof(options.VerifyServiceSid));

            _clientFactory = clientFactory;
            _options = options;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartVerification([FromBody] StartRequest request)
        {
            try
            {
                var client = _clientFactory.CreateClient("verify");
                var postData = new Dictionary<string, string>(){
                    { "To", $"+{request.CountryCode}{request.PhoneNumber}"},
                    { "Channel", request.Channel},
                    { "Locale", request.Locale},
                };

                var uri = $"/v2/Services/{_options.VerifyServiceSid}/Verifications";
                var response = await client.PostFormAsJObjectAsync(uri, postData);

                return Ok(response);
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> CheckVerification([FromBody] CheckRequest request)
        {
            try
            {
                var client = _clientFactory.CreateClient("verify");
                var postData = new Dictionary<string, string>(){
                    { "To", $"+{request.CountryCode}{request.PhoneNumber}"},
                    { "Code", request.Code}
                };

                var uri = $"/v2/Services/{_options.VerifyServiceSid}/VerificationsCheck";
                var response = await client.PostFormAsJObjectAsync(uri, postData);

                return Ok(response);
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        public class StartRequest
        {
            [Required, JsonProperty(PropertyName = "country_code")]
            public string CountryCode { get; set; }

            [Required, JsonProperty(PropertyName = "phone_number")]
            public string PhoneNumber { get; set; }

            [Required, JsonProperty(PropertyName = "via")]
            public string Channel { get; set; }

            [Required, JsonProperty(PropertyName = "locale")]
            public string Locale { get; set; }
        }

        public class CheckRequest
        {
            [Required, JsonProperty(PropertyName = "country_code")]
            public string CountryCode { get; set; }

            [Required, JsonProperty(PropertyName = "phone_number")]
            public string PhoneNumber { get; set; }

            [Required, JsonProperty(PropertyName = "token")]
            public string Code { get; set; }
        }
    }
}
