using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerCompass.API.Models
{
    public class SavedCareer
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CareerId { get; set; }

        [ForeignKey("CareerId")]
        public Career? Career { get; set; }
    }
}
