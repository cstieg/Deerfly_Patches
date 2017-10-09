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

        public async Task<ActionResult> GetUserInfo()
        {
            string authorizationCode = Request.Params.Get("code");
            Session.Add("PayPalAuthorizationCode", authorizationCode);
            UserInfo userInfo = await _paypalClient.GetUserInfo(authorizationCode);
            ShoppingCart shoppingCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("_shopping_cart");
            if (shoppingCart.GetOrder().ShipToAddress == null)
            {
                shoppingCart.GetOrder().ShipToAddress = new Address();
            }
            Address shippingAddress = shoppingCart.GetOrder().ShipToAddress;
            shippingAddress.Recipient = userInfo.Name;
            shippingAddress.Address1 = userInfo.Address.StreetAddress;
            shippingAddress.City = userInfo.Address.City;
            shippingAddress.State = userInfo.Address.State;
            shippingAddress.Zip = userInfo.Address.PostalCode;
            shippingAddress.Country = userInfo.Address.Country;
            shippingAddress.Type = AddressType.Shipping;
            HttpContext.Session.SetObjectAsJson("_shopping_cart", shoppingCart);

            return View("CloseLoginWindow");
        }

        [HttpPost]
        public async Task<string> CreateOrder()
        {
            // Get access token
            AccessToken accessToken = (AccessToken) await _paypalClient.GetAccessToken<AccessToken>();

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
            AccessToken accessToken = (AccessToken) await _paypalClient.GetAccessToken<AccessToken>();

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