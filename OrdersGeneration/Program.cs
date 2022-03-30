using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace OrdersGeneration
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class OrderProduct : Product
    {
        public int order_id { get; set; }

    }

    public class Product
    {
        public string storage { get; set; }
        public string storage_id { get; set; }
        public string order_product_id { get; set; }
        public string product_id { get; set; }
        public string variant_id { get; set; }
        public string name { get; set; }
        public string attributes { get; set; }
        public string sku { get; set; }
        public string ean { get; set; }
        public string location { get; set; }
        public int warehouse_id { get; set; }
        public string auction_id { get; set; }
        public int price_brutto { get; set; }
        public int tax_rate { get; set; }
        public int quantity { get; set; }
        public int weight { get; set; }
    }

    public class BaseOrder
    {
        public string order_status_id { get; set; }
        public string date_add { get; set; }
        public string user_comments { get; set; }
        public string admin_comments { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string user_login { get; set; }
        public string currency { get; set; }
        public string payment_method { get; set; }
        public string payment_method_cod { get; set; }
        public string paid { get; set; }
        public string delivery_method { get; set; }
        public string delivery_price { get; set; }
        public string delivery_fullname { get; set; }
        public string delivery_company { get; set; }
        public string delivery_address { get; set; }
        public string delivery_city { get; set; }
        public string delivery_postcode { get; set; }
        public string delivery_country_code { get; set; }
        public string delivery_point_id { get; set; }
        public string delivery_point_name { get; set; }
        public string delivery_point_address { get; set; }
        public string delivery_point_postcode { get; set; }
        public string delivery_point_city { get; set; }
        public string invoice_fullname { get; set; }
        public string invoice_company { get; set; }
        public string invoice_nip { get; set; }
        public string invoice_address { get; set; }
        public string invoice_city { get; set; }
        public string invoice_postcode { get; set; }
        public string invoice_country_code { get; set; }
        public string want_invoice { get; set; }
        public string extra_field_1 { get; set; }
        public string extra_field_2 { get; set; }
        public List<Product> products { get; set; }
    }

    public class Order : BaseOrder
    {
        public int order_id { get; set; }
        public string shop_order_id { get; set; }
        public string external_order_id { get; set; }
        public string order_source { get; set; }
        public string order_source_id { get; set; }
        public string order_source_info { get; set; }
        public bool confirmed { get; set; }
        public int date_confirmed { get; set; }
        public int date_in_status { get; set; }
        public int payment_done { get; set; }
        public string delivery_package_module { get; set; }
        public string delivery_package_nr { get; set; }
        public string order_page { get; set; }
        public int pick_state { get; set; }
        public int pack_state { get; set; }
        public string delivery_country { get; set; }
        public string invoice_country { get; set; }
    }

    public class Root
    {
        public string status { get; set; }
        public List<Order> orders { get; set; }
    }


    internal class Program
    {
        static HttpWebRequest CreateAuthorization()
        {
            var url = "https://api.baselinker.com/connector.php";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Headers["X-BLToken"] = "3005808-3025752-RYXG71QNXRS42JN9Q80ZTLSVA07X3ACKFFINPHFMZHH63A2H20CFFQC5E2R1QEZ5";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            return httpRequest;
        }

        static HttpWebResponse GetResponse(string method)
        {
            var httpRequest = CreateAuthorization();

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(method);
            }
            return (HttpWebResponse)httpRequest.GetResponse();
        }

        static List<Order> GetOrders()
        {
            var httpResponse = GetResponse("method=getOrders");
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var root = JsonConvert.DeserializeObject<Root>(result);
                return root.orders;
            }
        }

        //There is no api method for getting only one order by ID
        static Order GetOrderByID(int orderID)
        {
            var orders = GetOrders();
            var httpResponse = GetResponse("method=addOrders");
            Order order = orders.Where(x => x.order_id == orderID).FirstOrDefault();
            if (order != null)
                return order;

            return null;
        }

        static string CopyOrder(Order order)
        {
            var orderJson = JsonConvert.SerializeObject(order);
            BaseOrder newOrder = JsonConvert.DeserializeObject<BaseOrder>(orderJson);
            var newOrderJson = JsonConvert.SerializeObject(newOrder);

            var parameters = newOrderJson;
            var encoded = WebUtility.UrlEncode(parameters);
            var httpResponse = GetResponse("method=addOrder" + "&parameters=" + encoded);

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

            return null;
        }

        static string AddOrderProduct(Order order)
        {
            OrderProduct orderProduct = new OrderProduct
            {
                order_id = order.order_id,
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
                quantity = 2,
                weight = 0
            };

            var newOrderJson = JsonConvert.SerializeObject(orderProduct);

            var parameters = newOrderJson;
            var encoded = WebUtility.UrlEncode(parameters);
            var httpResponse = GetResponse("method=addOrderProduct" + "&parameters=" + encoded);

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

            return null;
        }

        static void Main(string[] args)
        {
            var orders = GetOrders();
            var order = GetOrderByID(30179559);
            var test = CopyOrder(order);
            var product = AddOrderProduct(order);
            Console.WriteLine("MSG {0}", test);
            Console.WriteLine("Order ID is {0}", order.order_id);
            Console.WriteLine("Product ID is {0}", product);
            Console.ReadKey();

        }
    }
}
