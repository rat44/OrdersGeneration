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
    public class OrderController
    {
        public static List<Order> GetOrders(bool consoleLog = true, string logPath = "")
        {
            var httpResponse = Authentication.GetResponse("method=getOrders");
            if (httpResponse != null)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<Root>(result);
                    if (response.status != "SUCCESS")
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ErrorMEssage>(result);
                        Logger.GenLog("method=getOrders: " + errorMessage.error_code + ": " + errorMessage.error_message, consoleLog, logPath);
                    }
                    else
                        return response.orders;
                }
            }
            return null;
        }

        //There is no api method for getting only one order by ID
        public static Order GetOrderByID(int orderID, bool consoleLog = true, string logPath = "")
        {
            var orders = GetOrders(consoleLog, logPath);
            if (orders != null)
            {
                var httpResponse = Authentication.GetResponse("method=addOrders");
                if (httpResponse != null)
                {
                    Order order = orders.Where(x => x.order_id == orderID).FirstOrDefault();
                    if (order != null)
                        return order;
                    else
                        Logger.GenLog("method=addOrders: OrderID: " + orderID + "not found.", consoleLog, logPath);
                }
            }
            return null;
        }

        public static int CopyOrder(Order order, bool consoleLog = true, string logPath = "")
        {
            var orderJson = JsonConvert.SerializeObject(order);
            BaseOrder newOrder = JsonConvert.DeserializeObject<BaseOrder>(orderJson);
            newOrder.extra_field_1 = "Zamówienie utworzone na podstawie <numer oryginalnego zamówienia " + order.order_id;
            var newOrderJson = JsonConvert.SerializeObject(newOrder);

            var parameters = newOrderJson;
            var encoded = WebUtility.UrlEncode(parameters);
            var httpResponse = Authentication.GetResponse("method=addOrder" + "&parameters=" + encoded);
            if (httpResponse != null)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<CopyOrder>(result);
                    if (response.status != "SUCCESS")
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ErrorMEssage>(result);
                        Logger.GenLog("method=addOrder: " + errorMessage.error_code + ": " + errorMessage.error_message, consoleLog, logPath);
                    }
                    else
                    {
                        Logger.GenLog("method=addOrder: " + "Order: " + response.order_id + " created, based on: " + order.order_id, consoleLog, logPath);
                        return response.order_id;
                    }
                }
            }
            return 0;
        }
    }
}
