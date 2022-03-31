using Newtonsoft.Json;
using OrdersGeneration.Resources;
using OrdersGeneration.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGeneration.Controllers
{
    public class ProductController
    {
        public static string AddOrderProduct(int orderID, bool consoleLog = true, string logPath = "")
        {
            if (orderID != 0)
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    order_id = orderID,
                    storage = "db",
                    storage_id = "0",
                    product_id = "777",
                    variant_id = "0",
                    name = "Gratis",
                    sku = "",
                    ean = "",
                    location = "",
                    attributes = "",
                    price_brutto = 1,
                    tax_rate = 0,
                    quantity = 1,
                    weight = 0
                };

                var newOrderJson = JsonConvert.SerializeObject(orderProduct);

                var parameters = newOrderJson;
                var encoded = WebUtility.UrlEncode(parameters);
                var httpResponse = Authentication.GetResponse("method=addOrderProduct" + "&parameters=" + encoded);
                if (httpResponse != null)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<ProductCreated>(result);
                        if (response.status != "SUCCESS")
                        {
                            var errorMessage = JsonConvert.DeserializeObject<ErrorMEssage>(result);
                            Logger.GenLog("method=addOrderProduct: " + errorMessage.error_code + ": " + errorMessage.error_message, consoleLog, logPath);
                        }
                        else
                        {
                            Logger.GenLog("method=addOrderProduct: " + "Product " + response.order_product_id + " created for order: " + orderID, consoleLog, logPath);
                            return result;
                        }
                    }
                }
            }
            else
            {
                Logger.GenLog("method=addOrderProduct: " + "OrderID not found", consoleLog, logPath);
            }
            return null;
        }
    }
}
