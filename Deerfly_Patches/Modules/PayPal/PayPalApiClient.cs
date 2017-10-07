using Deerfly_Patches.Models;
using Deerfly_Patches.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Deerfly_Patches.Modules.PayPal
{
    /// <summary>
    /// Client for PayPal API
    /// </summary>
    public class PayPalApiClient
    {
        private static string payPalBaseURL = "https://api.sandbox.paypal.com/v1/";
        private HttpClient httpClient = new HttpClient();

        public ClientInfo ClientInfo { get; set; }
        public AccessToken AccessToken { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }

        public PayPalApiClient()
        {
            ClientInfo = GetClientSecrets();
            ReturnUrl = ClientInfo.ReturnUrl;
            CancelUrl = ClientInfo.ReturnUrl;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en_US"));
        }

        /// <summary>
        /// Gets client id and secret from PayPal.json in root directory
        /// </summary>
        /// <returns>ClientInfo object containing client id info</returns>
        public ClientInfo GetClientSecrets()
        {
            string file = HostingEnvironment.MapPath("/PayPal.json");
            string json = System.IO.File.ReadAllText(file);
            ClientInfo paypalSecrets = JsonConvert.DeserializeObject<ClientInfo>(json);
            return paypalSecrets;
        }

        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            var byteArray = Encoding.ASCII.GetBytes(ClientInfo.ClientId + ":" + ClientInfo.ClientSecret);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            return header;
        }

        /// <summary>
        /// Gets an access token to be able to access PayPal API service
        /// </summary>
        /// <param name="paypalSecrets">Object containing PayPal client id info</param>
        /// <returns>AccessToken to use in accessing PayPal API</returns>
        public async Task<AccessToken> GetAccessToken()
        {
            // Store access token to reuse until expires
            if (AccessToken != null && !AccessToken.IsExpired)
            {
                return AccessToken;
            }

            using (var client = httpClient)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Authorization = GetAuthenticationHeader();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, payPalBaseURL + "oauth2/token")
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    })
                };
                var response = await client.SendAsync(request);
                var result = response.Content.ReadAsStringAsync().Result;
                AccessToken = JsonConvert.DeserializeObject<AccessToken>(result);
                return AccessToken;
            }
        }

        public async Task<string> GetUserAccessToken(string userCode)
        {
            string url = payPalBaseURL + "identity/openidconnect/tokenservice";
            string result;
            using (var client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Authorization = GetAuthenticationHeader();
                AccessToken accessToken = await GetAccessToken();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", userCode)
                    })
                };
                var response = await client.SendAsync(request);
                result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
            }
            return result;
        }


        /// <summary>
        /// Make API call to PayPal with specified data.  
        /// Generic wrapper generalizing elements needed by all API calls, such as access token header.
        /// </summary>
        /// <param name="url">API endpoint URL being called</param>
        /// <param name="data">Data to pass to endpoint</param>
        /// <param name="accessToken">Access token authorizing access</param>
        /// <returns>String result of call</returns>
        public async Task<string> PayPalCall(string url, string data, string accessToken, string method = "POST")
        {
            if (!"POST|GET".Contains(method))
            {
                throw new ArgumentException("Argument 'method' must either be 'GET' or 'POST'");
            }
            if (method == "GET" && data != "")
            {
                throw new ArgumentException("A GET request cannot contain data!");
            }
            HttpMethod httpMethod = method == "GET" ? HttpMethod.Get : HttpMethod.Post;
            string result;
            using (var client = httpClient)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
                if (method == "POST")
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }
                request.Headers.Add("Accept", "application/json");
                var response = await client.SendAsync(request);
                result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
            }
            return result;
        }



        /// <summary>
        /// Gets user info from PayPal API
        /// </summary>
        /// <param name="accessToken">Access token authorizing API call</param>
        /// <returns>String result of API call</returns>
        public async Task<string> GetUserInfo()
        {
            AccessToken accessToken = await GetAccessToken();
            return await PayPalCall(payPalBaseURL + "identity/openidconnect/userinfo?schema=openid", "", accessToken.AccessTokenString, "GET");
        }

        /// <summary>
        /// Creates an order to pass to PayPal API
        /// </summary>
        /// <param name="shoppingCart">Shopping cart object containing items to put in order</param>
        /// <returns>JSON representation of the order in the format expected by PayPal</returns>
        public string CreateOrder(ShoppingCart shoppingCart)
        {
            object data = new
            {
                intent = "order",
                payer = new
                {
                    payment_method = "paypal"
                },
                transactions = new List<object>
                {
                    new
                    {
                        amount = new
                        {
                            currency = "USD",
                            total = shoppingCart.GrandTotal,
                            details = new
                            {
                                shipping = shoppingCart.TotalShipping,
                                subtotal = shoppingCart.TotalExtendedPrice,
                                tax = "0.00"
                            }
                        },
                        payee = new
                        {
                            email = shoppingCart.PayeeEmail
                        },
                        description = "Order from Detex, manufacturer of Deerfly Patches",
                        item_list = new
                        {
                            items = GetPayPalItems(shoppingCart),
                            shipping_address = GetPayPalAddress(shoppingCart.GetOrder().ShipToAddress)
                        }
                    }
                },
                redirect_urls = new
                {
                    return_url = ReturnUrl,
                    cancel_url = ReturnUrl
                }
            };
            string dataJSON = JsonConvert.SerializeObject(data);
            return dataJSON;
        }

        /// <summary>
        /// Posts an order to PayPal API
        /// </summary>
        /// <param name="data">JSON order data in PayPal format</param>
        /// <param name="accessToken">Access token authorizing PayPal call</param>
        /// <returns></returns>
        public async Task<string> PostOrder(string data, string accessToken)
        {
            string result;
            string orderId = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, payPalBaseURL + "payments/payment")
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(result.ToString());
                }
                orderId = new JsonDeserializer(result).GetString("id");
            }
            return result;
        }

        /// <summary>
        /// Converts address to PayPal object format
        /// </summary>
        /// <param name="address">Address to convert</param>
        /// <returns>Object representation of address in PayPal format</returns>
        private object GetPayPalAddress(Address address)
        {
            return new
            {
                recipient_name = address.Recipient,
                line1 = address.Address1,
                line2 = address.Address2,
                city = address.City,
                country_code = address.Country,
                postal_code = address.Zip,
                phone = address.Phone,
                state = address.State
            };
        }

        /// <summary>
        /// Converts OrderDetail model to PayPal item format
        /// </summary>
        /// <param name="orderDetail">OrderDetail object containing item being purchased</param>
        /// <returns>Purchase item in PayPal object format</returns>
        private object GetPayPalItem(OrderDetail orderDetail)
        {
            return new
            {
                name = orderDetail.Product.Name,
                quantity = orderDetail.Quantity,
                price = orderDetail.ExtendedPrice,
                sku = orderDetail.Product.ProductId,
                currency = "USD"
            };
        }

        /// <summary>
        /// Converts shopping cart info to PayPal object format
        /// </summary>
        /// <param name="shoppingCart">Shopping cart containing items to be purchased</param>
        /// <returns>Shopping cart items in PayPal object format</returns>
        private List<object> GetPayPalItems(ShoppingCart shoppingCart)
        {
            List<object> items = new List<object>();
            List<OrderDetail> shoppingCartItems = shoppingCart.GetItems();

            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                items.Add(GetPayPalItem(shoppingCartItems[i]));
            }
            return items;
        }
    }
}
