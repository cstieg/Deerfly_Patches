using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using Deerfly_Patches.Modules.PayPal;
using Deerfly_Patches.Models;
using Deerfly_Patches.ViewModels;
using Deerfly_Patches.Modules;

namespace DeerflyPatches.Controllers
{
    /// <summary>
    /// Controller to handle PayPal interface
    /// </summary>
    public class PayPalController : Controller
    {
        private PayPalApiClient _paypalClient;

        public PayPalController() : base()
        {
            _paypalClient = new PayPalApiClient();
        }

        public async Task GetAccessToken()
        {
            AccessToken accessToken = await _paypalClient.GetAccessToken();
        }

        [HttpPost]
        public async Task<string> GetUserInfo()
        {
            // Post order to PayPal API and return order ID to front end
            return await _paypalClient.GetUserInfo();
        }

        [HttpPost]
        public async Task<string> CreateOrder()
        {
            // Get access token
            AccessToken accessToken = await _paypalClient.GetAccessToken();

            // Get shopping cart from session
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            if (shoppingCart.GetOrder().ShipToAddress == null)
            {
                shoppingCart.GetOrder().ShipToAddress = new Address();
            }
            shoppingCart.GetOrder().ShipToAddress.Country = "US";

            // Create JSON string with order information
            shoppingCart.PayeeEmail = _paypalClient.ClientInfo.ClientAccount;
            string orderData = _paypalClient.CreateOrder(shoppingCart);

            // Post order to PayPal API and return order ID to front end
            return await _paypalClient.PostOrder(orderData, accessToken.AccessTokenString);
        }


        [HttpPost]
        public async Task<ActionResult> ExecutePayment(string paymentId, FormCollection data)
        {
            // Get access token
            AccessToken accessToken = await _paypalClient.GetAccessToken();

            string payerId = Request.Form["PayerID"];

            string uri = "https://api.sandbox.paypal.com/v1/payments/payment/" + paymentId + "/execute";
            string postData = JsonConvert.SerializeObject(new
            {
                payer_id = payerId
            });

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessTokenString);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(postData, Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);

                var result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
                var orderId = new JsonDeserializer(result).GetString("id");
            }
            return this.JOk();
        }
    }

}