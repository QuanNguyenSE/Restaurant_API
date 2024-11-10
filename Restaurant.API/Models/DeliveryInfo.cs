using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Owned]
    public class DeliveryInfo
    {
        [Column("Name")]
        public string? Name { get; set; }
        [Column("PhoneNumber")]
        public string? PhoneNumber { get; set; }
        [Column("StreetAddress")]
        public string? StreetAddress { get; set; }
        [Column("City")]
        public string? City { get; set; }
        [Column("State")]
        public string? State { get; set; }
    }
}
