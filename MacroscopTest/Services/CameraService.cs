using MacroscopTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MacroscopTest.Services
{
    public class CameraService : ICameraService
    {
        private readonly HttpClient _httpClient;

        public CameraService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<CameraModel>> GetCamerasAsync()
        {
            List<CameraModel> cameras = new List<CameraModel>();

            try
            {
                string apiUrl = "http://demo.macroscop.com:8080/configex?login=root";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string xmlContent = await response.Content.ReadAsStringAsync();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContent);

                    XmlNodeList cameraNodes = xmlDoc.SelectNodes("""//ChannelInfo""");

                    foreach (XmlNode cameraNode in cameraNodes)
                    {
                        string id = cameraNode.Attributes["Id"]?.Value;
                        string name = cameraNode.Attributes["Name"]?.Value;

                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                        {
                            cameras.Add(new CameraModel { Id = id, Name = name });
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Responce has an error!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return cameras;
        }
    }
}
