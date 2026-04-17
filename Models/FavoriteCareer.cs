using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerCompass.API.Models
{
    public class FavoriteCareer
    {
        public int Id { get; set; }

        // 🔹 Foreign key to User
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        // 🔹 Foreign key to Career
        public int CareerId { get; set; }

        [ForeignKey("CareerId")]
        public Career? Career { get; set; }
    }
}