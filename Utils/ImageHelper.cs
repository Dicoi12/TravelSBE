using System.Collections.Generic;
using System.Linq;
using TravelSBE.Entity;
using Microsoft.Extensions.Configuration;

namespace TravelSBE.Utils
{
    public static class ImageHelper
    {
        private static readonly string _baseUrl;

        static ImageHelper()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _baseUrl = configuration["BaseUrl"];
        }

        public static List<string> ConvertToImageUrls(List<ObjectiveImage> images)
        {
            return images.Select(img => $"{_baseUrl}{img.FilePath}").ToList();
        }
        public static string GetFirstImageURL(List<ObjectiveImage> images)
        {
            return images.Select(img=> $"{_baseUrl}{img.FilePath}").First();
        }
    }
}

