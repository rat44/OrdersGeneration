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
using OrdersGeneration.Resources;
using OrdersGeneration.Services;
using OrdersGeneration.Controllers;

namespace OrdersGeneration
{
    public class ErrorMEssage
    {
        public string status { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var consoleLog = true;
            var logPath = "";
            string exitLine;
            Console.WriteLine("Show logs in console? (no for save log in file) y/n");
            var inputLogAnswer = Console.ReadLine();
            if (inputLogAnswer == "n")
            {
                consoleLog = false;
                Console.WriteLine("Provide file directory:");
                logPath = Console.ReadLine();
            }

            var orders = OrderController.GetOrders(consoleLog, logPath);
            foreach (var ord in orders)
            {
                Console.WriteLine("Available orders: id: {0}", ord.order_id);
            }

            Console.WriteLine("Provide order id:");
            var inputOrderId = Console.ReadLine();

            if (int.TryParse(inputOrderId, out int orderId))
            {
                var order = OrderController.GetOrderByID(orderId, consoleLog, logPath);
                var copiedOrderID = OrderController.CopyOrder(order, consoleLog, logPath);
                var product = ProductController.AddOrderProduct(copiedOrderID, consoleLog, logPath);
            }
            else
            {
                Console.WriteLine($"{inputOrderId} is not a number");
            }
            
            Console.ReadKey();
        }
    }
}
