using DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support.Interface;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Support
{
    public class CommonAction : IGeneralSupport
    {
        private static readonly Random Random = new Random();

        public string RandomString(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public byte[] ConvertObjectToByteArray(object obj)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public T GetResource<T>(string resourceName)
        {
            var resourcesDirectory = Directory.CreateDirectory(System.Environment.CurrentDirectory).GetDirectories("Resource")[0];
            var files = resourcesDirectory.GetFiles();
            FileInfo selectedResource = null;

            for (var fileIndex = 0; fileIndex < files.Length; fileIndex++)
            {
                if (files[fileIndex].Name.ToUpperInvariant().StartsWith(resourceName.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase))
                {
                    selectedResource = files[fileIndex];
                    break;
                }
            }

            if (selectedResource == null)
            {
                throw new Exception($"No resource with the name {resourceName} was found");
            }

            using (var streamReader = new StreamReader(selectedResource.FullName))
            {
                var content = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
