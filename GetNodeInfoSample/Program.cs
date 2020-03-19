using NodeAPIClient.Services;
using System;
using System.Text.Json;

namespace GetNodeInfoSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string ip = "165.22.220.8"; // do1
            ushort port = 9088;
            var service = new NodeInfoService();
            var response = service.GetNodeInfo(ip, port, 60000);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string text = JsonSerializer.Serialize(response, options);
            System.IO.File.WriteAllText(@"node_info.json", text);
        }
    }
}
