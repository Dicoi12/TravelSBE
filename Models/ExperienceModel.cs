using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelSBE.Entity;
using TravelSBE.Entity.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace TravelsBE.Models;

public class ExperienceModel : BaseAuditEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public string? LocationName { get; set; }

    public int? Rating { get; set; }

    public List<ObjectiveImage> Images { get; set; }
    public bool IsPublic { get; set; } = true;

}
