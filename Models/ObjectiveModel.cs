using AutoMapper;
using System.Globalization;
using TravelsBE.Entity;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;

namespace TravelSBE.Models
{
    public class ObjectiveModel : BaseAuditEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? City { get; set; }
        public double? MedieReview { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public double Distance { get; set; } = 0;
        public string? Website { get; set; }
        public string? Interval { get; set; }
        public string? Pret { get; set; }
        public int? Type { get; set; }
        public int? Duration { get; set; }
        public ObjectiveType? ObjectiveType { get; set; }
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
}
