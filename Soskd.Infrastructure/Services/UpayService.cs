// Soskd.Infrastructure/Services/UpayService.cs - Corrected Implementation
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Soskd.Infrastructure.Services
{
    public class UpayService : IUpayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UpayService> _logger;
        private readonly HttpClient _httpClient;

        public UpayService(IConfiguration configuration, ILogger<UpayService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<UpayPrepaymentResponse> PrepaymentAsync(UpayPrepaymentRequest request)
        {
            try
            {
                var serviceId = _configuration["UPay:ServiceId"];
                var accessToken = _configuration["UPay:AccessToken"];
                var apiUrl = _configuration["UPay:ApiUrl"] ?? "https://api.upay.uz/api";

                var soapRequest = CreatePrepaymentSoapRequest(request, serviceId!, accessToken!);

                _logger.LogInformation("Sending prepayment request to UPay for OrderId: {OrderId}", request.OrderId);

                var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "prepayment");

                var response = await _httpClient.PostAsync($"{apiUrl}/prepayment", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("UPay prepayment response received, Status: {StatusCode}", response.StatusCode);

                return ParsePrepaymentResponse(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during UPay prepayment for OrderId: {OrderId}", request.OrderId);
                return new UpayPrepaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Connection error occurred during prepayment"
                };
            }
        }

        public async Task<UpayConfirmPaymentResponse> ConfirmPaymentAsync(UpayConfirmPaymentRequest request)
        {
            try
            {
                var serviceId = _configuration["UPay:ServiceId"];
                var accessToken = _configuration["UPay:AccessToken"];
                var apiUrl = _configuration["UPay:ApiUrl"] ?? "https://api.upay.uz/api";

                var soapRequest = CreateConfirmPaymentSoapRequest(request, serviceId!, accessToken!);

                _logger.LogInformation("Sending payment confirmation to UPay for TransactionId: {TransactionId}", request.TransactionId);

                var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "confirmPayment");

                var response = await _httpClient.PostAsync($"{apiUrl}/confirmPayment", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return ParseConfirmPaymentResponse(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during UPay payment confirmation for TransactionId: {TransactionId}", request.TransactionId);
                return new UpayConfirmPaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Connection error occurred during payment confirmation"
                };
            }
        }

        public async Task<UpayCheckPaymentResponse> CheckPaymentAsync(string orderId)
        {
            try
            {
                var serviceId = _configuration["UPay:ServiceId"];
                var accessToken = _configuration["UPay:AccessToken"];
                var apiUrl = _configuration["UPay:ApiUrl"] ?? "https://api.upay.uz/api";

                var soapRequest = CreateCheckPaymentSoapRequest(orderId, serviceId!, accessToken!);

                var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "checkPayment");

                var response = await _httpClient.PostAsync($"{apiUrl}/checkPayment", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return ParseCheckPaymentResponse(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status for OrderId: {OrderId}", orderId);
                return new UpayCheckPaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Error checking payment status"
                };
            }
        }

        public bool VerifyCallback(UpayCallbackRequest callback)
        {
            try
            {
                var expectedAccessToken = _configuration["UPay:AccessToken"];
                var expectedServiceId = _configuration["UPay:ServiceId"];

                // Verify access token and service ID
                return callback.AccessToken == expectedAccessToken &&
                       callback.ServiceId == expectedServiceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying callback for OrderId: {OrderId}", callback.OrderId);
                return false;
            }
        }

        private string CreatePrepaymentSoapRequest(UpayPrepaymentRequest request, string serviceId, string accessToken)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>
                        <prepayment>
                            <serviceId>{serviceId}</serviceId>
                            <accessToken>{accessToken}</accessToken>
                            <orderId>{request.OrderId}</orderId>
                            <amount>{request.Amount:0}</amount>
                            <personalAccount>{request.PersonalAccount}</personalAccount>
                            <phone>{request.Phone}</phone>
                            <cardNumber>{request.CardNumber}</cardNumber>
                            <expireDate>{request.ExpireDate}</expireDate>
                            <returnUrl>{request.ReturnUrl}</returnUrl>
                        </prepayment>
                    </soap:Body>
                </soap:Envelope>";
        }

        private string CreateConfirmPaymentSoapRequest(UpayConfirmPaymentRequest request, string serviceId, string accessToken)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>
                        <confirmPayment>
                            <serviceId>{serviceId}</serviceId>
                            <accessToken>{accessToken}</accessToken>
                            <transactionId>{request.TransactionId}</transactionId>
                            <smsCode>{request.SmsCode}</smsCode>
                        </confirmPayment>
                    </soap:Body>
                </soap:Envelope>";
        }

        private string CreateCheckPaymentSoapRequest(string orderId, string serviceId, string accessToken)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>
                        <checkPayment>
                            <serviceId>{serviceId}</serviceId>
                            <accessToken>{accessToken}</accessToken>
                            <orderId>{orderId}</orderId>
                        </checkPayment>
                    </soap:Body>
                </soap:Envelope>";
        }

        private UpayPrepaymentResponse ParsePrepaymentResponse(string xmlResponse)
        {
            try
            {
                var doc = XDocument.Parse(xmlResponse);
                var response = new UpayPrepaymentResponse();

                var resultElement = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "prepaymentResponse");
                if (resultElement != null)
                {
                    var status = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "status")?.Value ?? "FAILED";
                    var transactionId = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "transactionId")?.Value;
                    var errorCode = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "errorCode")?.Value;
                    var errorMessage = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "errorMessage")?.Value;

                    response.Success = status == "SUCCESS" || status == "PENDING";
                    response.Status = status;
                    response.TransactionId = transactionId ?? string.Empty;
                    response.ErrorCode = errorCode ?? string.Empty;
                    response.ErrorMessage = errorMessage ?? string.Empty;
                    response.RequiresConfirmation = status == "PENDING";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing UPay prepayment response");
                return new UpayPrepaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Response parsing error"
                };
            }
        }

        private UpayConfirmPaymentResponse ParseConfirmPaymentResponse(string xmlResponse)
        {
            try
            {
                var doc = XDocument.Parse(xmlResponse);
                var response = new UpayConfirmPaymentResponse();

                var resultElement = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "confirmPaymentResponse");
                if (resultElement != null)
                {
                    var status = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "status")?.Value ?? "FAILED";
                    var transactionId = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "transactionId")?.Value;
                    var amount = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "amount")?.Value;
                    var errorCode = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "errorCode")?.Value;
                    var errorMessage = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "errorMessage")?.Value;

                    response.Success = status == "SUCCESS";
                    response.Status = status;
                    response.TransactionId = transactionId ?? string.Empty;
                    response.ErrorCode = errorCode ?? string.Empty;
                    response.ErrorMessage = errorMessage ?? string.Empty;

                    if (decimal.TryParse(amount, out var parsedAmount))
                        response.ProcessedAmount = parsedAmount;

                    if (response.Success)
                        response.ProcessedAt = DateTime.UtcNow;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing UPay confirm payment response");
                return new UpayConfirmPaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Response parsing error"
                };
            }
        }

        private UpayCheckPaymentResponse ParseCheckPaymentResponse(string xmlResponse)
        {
            try
            {
                var doc = XDocument.Parse(xmlResponse);
                var response = new UpayCheckPaymentResponse();

                var resultElement = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "checkPaymentResponse");
                if (resultElement != null)
                {
                    var status = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "status")?.Value ?? "UNKNOWN";
                    var transactionId = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "transactionId")?.Value;
                    var amount = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "amount")?.Value;

                    response.Success = status == "SUCCESS";
                    response.Status = status;
                    response.TransactionId = transactionId ?? string.Empty;

                    if (decimal.TryParse(amount, out var parsedAmount))
                        response.Amount = parsedAmount;

                    if (response.Success)
                        response.ProcessedAt = DateTime.UtcNow;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing UPay check payment response");
                return new UpayCheckPaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Response parsing error"
                };
            }
        }
    }
}