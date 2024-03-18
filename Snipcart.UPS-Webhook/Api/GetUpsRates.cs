using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Snipcart.UPS_Webhook.Dtos;
using Snipcart.UPS_Webhook.Models.Enums;
using Snipcart.UPS_Webhook.Services;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Snipcart.UPS_Webhook.Api
{
    public class GetUpsRates
    {
        private readonly IUpsService _upsService;
        private readonly ILogger<GetUpsRates> _logger;

        public GetUpsRates(IUpsService upsService, ILogger<GetUpsRates> logger)
        {
            _upsService = upsService;
            _logger = logger;
        }
        
        // Uses AuthorizationLevel.Function, will need to send code param
        // https://AZURE_FUNCTION_NAME.azurewebsites.net/api/UpsRates/1?code=FUNCTION_KEY
        [Function(nameof(GetUpsRates))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "UpsRates/{mode:int}")] HttpRequest req,
            Mode mode,
            [FromBody] OrderRequest orderRequest)
        {
            // May want to validate the request with Snipcart here
            // https://docs.snipcart.com/v3/webhooks/introduction#securing-your-webhook-endpoint

            try
            {
                _logger.LogInformation("Called GetUpsRates with mode {mode}", mode);
                return new OkObjectResult(await _upsService.WithMode(mode).GetRates(orderRequest.Content));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting UPS rates");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}