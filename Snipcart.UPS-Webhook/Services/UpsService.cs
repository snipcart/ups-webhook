using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snipcart.UPS_Webhook.Configurations;
using Snipcart.UPS_Webhook.Dtos;
using Snipcart.UPS_Webhook.Dtos.Ups;
using Snipcart.UPS_Webhook.Dtos.Ups.Error;
using Snipcart.UPS_Webhook.Dtos.Ups.Rating;
using Snipcart.UPS_Webhook.Exceptions;
using Snipcart.UPS_Webhook.Extensions;
using Snipcart.UPS_Webhook.Helpers;
using Snipcart.UPS_Webhook.Models;
using Snipcart.UPS_Webhook.Models.Enums;

namespace Snipcart.UPS_Webhook.Services
{
    public interface IUpsService
    {
        IUpsService WithMode(Mode mode);
        Task<List<ShippingRate>> GetRates(RequestContent order);
    }

    public class UpsService(
        HttpClient httpClient,
        ILogger<UpsService> logger,
        IOptions<UpsConfiguration> upsConfiguration,
        IOptions<BusinessAddress> businessAddress)
        : IUpsService
    {
        private string _path = GetUpsUrl(Mode.Live);
        private readonly UpsConfiguration _upsConfiguration = upsConfiguration.Value;
        private readonly BusinessAddress _businessAddress = businessAddress.Value;
    
        private const string UpsTestBaseUrl = "https://wwwcie.ups.com";
        private const string UpsLiveBaseUrl = "https://onlinetools.ups.com";
        private const string UpsGetRatesUrl = "/api/rating/v2205/shop";
        private const string UpsGetTokenUrl = "/security/v1/oauth/token";

        public IUpsService WithMode(Mode mode)
        {
            _path = GetUpsUrl(mode);
            return this;
        }

        public async Task<List<ShippingRate>> GetRates(RequestContent order)
        {
            logger.LogInformation("Getting rates from UPS");
            var path = _path + UpsGetRatesUrl;
            var oAuthToken = await GetToken();
            var shopRateResponse = await SendAsync<ShopRateResponse, Dictionary<string, object>>(HttpMethod.Post, path, oAuthToken.AccessToken, OrderRequestToUpsRates(order));

            logger.LogInformation("Received rates from UPS: {rates}", JsonSerializer.Serialize(shopRateResponse));
            return ShippingRatesFromUpsRates(shopRateResponse);
        }
    
        private static string GetUpsUrl(Mode mode)
        {
            return mode switch
            {
                Mode.Live => UpsLiveBaseUrl,
                Mode.Test => UpsTestBaseUrl,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
        
        private async Task<TokenResponse> GetToken()
        {
            // We should store the token in a cache and check if it's expired before requesting a new one
            // But for this example we'll just request a new token every time
            
            var path = _path + UpsGetTokenUrl;
            return await SendFormAsync<TokenResponse>(HttpMethod.Post, path, new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            });
        }
    
        private static Dictionary<string, object> GetAddressDictionary(Address address)
        {
            var addressDictionary = new Dictionary<string, object>
            {
                ["Address"] = new Dictionary<string, object>
                {
                    ["AddressLine"] = new List<string>
                    {
                        address.Address1,
                        address.Address2
                    },
                    ["City"] = address.City,
                    ["StateProvinceCode"] = address.Province,
                    ["PostalCode"] = address.PostalCode,
                    ["CountryCode"] = address.Country
                }
            };
            return addressDictionary;
        }
    
        private static List<Dictionary<string, object>> GetPackages(IEnumerable<Dimension> dimensions, bool useImperial)
        {
            return dimensions.Select(dimension => 
                new Dictionary<string, object>
                {
                    ["PackagingType"] = new Dictionary<string, object>
                    {
                        ["Code"] = "00",
                        ["Description"] = "Unknown"
                    },
                    ["PackageWeight"] = GetWeight(dimension.Weight, useImperial),
                    ["Dimensions"] = new Dictionary<string, object>
                    {
                        ["UnitOfMeasurement"] = new Dictionary<string, object>
                        {
                            ["Code"] = useImperial ? "IN" : "CM",
                            ["Description"] = useImperial ? "Inches" : "Centimeters"
                        },
                        ["Length"] = ConvertDimension(dimension.Length).ToString("F2", CultureInfo.InvariantCulture),
                        ["Width"] = ConvertDimension(dimension.Width).ToString("F2",CultureInfo.InvariantCulture),
                        ["Height"] = ConvertDimension(dimension.Height).ToString("F2",CultureInfo.InvariantCulture)
                    }
                }).ToList();
        
            decimal ConvertDimension(decimal dimensionValue)
            {
                return useImperial ? UnitConverter.CmsToInches(dimensionValue) : dimensionValue;
            }
        }

        private Dictionary<string, object> OrderRequestToUpsRates(RequestContent order)
        {
            #region Example UPS Request Body
            /*
            {
                "RateRequest": {
                    "Shipment": {
                        "Shipper": {
                            "Name": "Shipper Name",
                            "ShipperNumber": "Your Shipper Number",
                            "Address": {
                                "AddressLine": [
                                    "Address Line 1"
                                ],
                                "City": "City",
                                "StateProvinceCode": "State Province Code",
                                "PostalCode": "Postal Code",
                                "CountryCode": "Country Code"
                            }
                        },
                        "ShipTo": {
                            "Name": "Ship To Name",
                            "Address": {
                                "AddressLine": [
                                    "Address Line 1"
                                ],
                                "City": "City",
                                "StateProvinceCode": "State Province Code",
                                "PostalCode": "Postal Code",
                                "CountryCode": "Country Code"
                            }
                        },
                        "ShipFrom": {
                            "Name": "Ship From Name",
                            "Address": {
                                "AddressLine": [
                                    "Address Line 1"
                                ],
                                "City": "City",
                                "StateProvinceCode": "State Province Code",
                                "PostalCode": "Postal Code",
                                "CountryCode": "Country Code"
                            }
                        },
                        "Service": {
                            "Code": "03",
                            "Description": "Service Code Description"
                        },
                        "Package": {
                            "PackagingType": {
                                "Code": "02",
                                "Description": "Rate"
                            },
                            "Dimensions": {
                                "UnitOfMeasurement": {
                                    "Code": "IN",
                                    "Description": "inches"
                                },
                                "Length": "5",
                                "Width": "4",
                                "Height": "3"
                            },
                            "PackageWeight": {
                                "UnitOfMeasurement": {
                                    "Code": "Lbs",
                                    "Description": "pounds"
                                },
                                "Weight": "1"
                            }
                        },
                        "ShipmentRatingOptions": {
                            "NegotiatedRatesIndicator": ""
                        }
                    }
                }
             */
            #endregion

            var useImperial = _businessAddress.Country.Equals("US", StringComparison.InvariantCultureIgnoreCase);
            var shipperDictionary = GetAddressDictionary(_businessAddress);

            if (_upsConfiguration.AccountNumber != null)
                shipperDictionary.Add("ShipperNumber", _upsConfiguration.AccountNumber);
        
            var packages = ShipmentHelper.RequestsSplit(order.Items);

            var returnStringObject = new Dictionary<string, object>
            {
                ["RateRequest"] = new Dictionary<string, object>
                {
                    ["Request"] = new Dictionary<string, object>
                    {
                        ["RequestOption"] = new List<string>
                        {
                            "timeintransit"
                        },
                        ["TransactionReference"] = new Dictionary<string, object>
                        {
                            ["CustomerContext"] = order.Token
                        }
                    },
                    ["Shipment"] = new Dictionary<string, object>
                    {
                        ["Shipper"] = shipperDictionary,
                        ["ShipTo"] = GetAddressDictionary(order.ShippingAddress),
                        ["Package"] = GetPackages(packages, useImperial),
                        ["ShipmentTotalWeight"] = GetWeight(order.GetTotalWeight(), useImperial)
                    }
                }
            };
            return returnStringObject;
        }
    
        // When the country is USA, UPS only accepts pounds, the rest of the world accepts kilograms
        private static Dictionary<string, object> GetWeight(decimal weightInGrams, bool useImperial)
        {
            var weight = useImperial ? UnitConverter.GramsToLbs(weightInGrams) : UnitConverter.GramsToKgs(weightInGrams);
            return new Dictionary<string, object>
            {
                ["UnitOfMeasurement"] = new Dictionary<string, object>
                {
                    ["Code"] = useImperial ? "LBS" : "KGS",
                    ["Description"] = useImperial ? "Pounds" : "Kilograms"
                },
                ["Weight"] = weight.ToString("F2", CultureInfo.InvariantCulture)
            };
        }
    
        private async Task<TResult> SendAsync<TResult, TInput>(HttpMethod method, string path, string accessToken = null, TInput payload = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            logger.LogInformation($"Sending request to {path} with method {method} and content {JsonSerializer.Serialize(payload)}");
            var requestMessage = CreateHttpRequestMessage(method, path, content);
        
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (accessToken is not null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
            return await GetResponseContent<TResult>(await httpClient.SendAsync(requestMessage));
        }
    
        private async Task<TResult> SendFormAsync<TResult>(HttpMethod method, string path, Dictionary<string, string> form = null)
        {
            var content = form is not null ? new FormUrlEncodedContent(form) : null;
            logger.LogInformation("Sending request to {path} with method {method} and content {form}", path, method, form);
            
            var requestMessage = CreateHttpRequestMessage(method, path, content);
            requestMessage.Headers.Add("x-merchant-id", _upsConfiguration.AccountNumber);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            requestMessage.Headers.Authorization =  new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_upsConfiguration.ClientId}:{_upsConfiguration.ClientSecret}")));

            return await GetResponseContent<TResult>(await httpClient.SendAsync(requestMessage));
        }

        private async Task<TResult> GetResponseContent<TResult>(HttpResponseMessage response)
        {
            var responseContent = response.Content;
            if (responseContent is null)
                throw new NullReferenceException("Response content is null");
        
            if (response.IsSuccessStatusCode)
                return await responseContent.ReadFromJsonAsync<TResult>();
        
            var errorResponse = await responseContent.ReadFromJsonAsync<ErrorResponse>();
            logger.LogError("UPS error response {statusCode}: {errorResponse}", response.StatusCode, JsonSerializer.Serialize(errorResponse));
            var error = errorResponse?.Response?.Errors?.FirstOrDefault();
            if (error is null)
                throw new Exception($"UPS error with status code {response.StatusCode}");
            throw new UpsException(errorResponse);
        }
    
        private static HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, HttpContent payload)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url, UriKind.Absolute),
                Content = payload
            };

            return requestMessage;
        }
    
        private List<ShippingRate> ShippingRatesFromUpsRates(ShopRateResponse response)
        {
            var shippingMethods = new UpsShippingMethods();
            // You might want to check the currency and potentially convert the Cost
            // ratedShipment.TotalCharges.CurrencyCode
            return response.RateResponse.RatedShipment.Select(ratedShipment => new ShippingRate
            {
                Cost = decimal.Parse(ratedShipment.TotalCharges.MonetaryValue, CultureInfo.InvariantCulture),
                Description = shippingMethods.GetMethodName(ratedShipment.Service.Code, logger),
                GuaranteedDeliveryDays = ratedShipment.GuaranteedDelivery?.BusinessDaysInTransit.ToNullableInt(),
                UserDefinedId = ratedShipment.Service?.Code
            }).ToList();
        }
    }
}