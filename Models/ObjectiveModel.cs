using AutoMapper;
using System.Globalization;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ObjectiveModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string> Images { get; set; }
    }
}
