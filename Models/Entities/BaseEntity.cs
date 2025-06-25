using System.ComponentModel.DataAnnotations;

namespace ClientPortalBifurkacioni.Models.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
