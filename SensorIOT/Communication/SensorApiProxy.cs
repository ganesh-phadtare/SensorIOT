using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SensorIOT.Communication
{
    public class SensorApiProxy
    {
        private static HttpClient __client = null;
        static SensorApiProxy()
        {
            if (__client == null)
            {
                __client = new HttpClient();
                __client.Timeout = TimeSpan.FromMinutes(30);
                __client.BaseAddress = new Uri("http://192.168.35.124/SensorWebAPI/");
                //__client.BaseAddress = new Uri("http://192.168.35.139/Select.DMS/");
                //__client.DefaultRequestHeaders.Accept.Clear();http://localhost:51072/
                //__client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            }
        }

        public void Fetch(int executionType, Dictionary<string, string> data)
        {
            var json = ConvertToJsonString(data);
            ViewFor(executionType, json);
        }

        private string ConvertToJsonString(object data)
        {
            string jsonString = string.Empty;
            try
            {

                jsonString = JsonConvert.SerializeObject(data);

            }
            catch (JsonSerializationException exception)
            {

                Console.Write(exception.Message);
                throw;
            }
            return jsonString;

        }

        private void ViewFor(int executionType, string data)
        {
            HttpContent contentPost = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                __client.PostAsync(string.Format("view/test/{0}", executionType), contentPost);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
