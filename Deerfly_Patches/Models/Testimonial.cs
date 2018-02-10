using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeerflyPatches.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Label to display to viewers")]
        public string Label { get; set; }

        [DisplayName("Date of testimonial (optional)")]
        public DateTime ?Date { get; set; }

        [DisplayName("Upload image file")]
        public string ImageUrl { get; set; }
        public string ImageSrcSet { get; set; }
    }
}