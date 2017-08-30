using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Deerfly_Patches.Models
{
    public class Testimonial
    {
        [Key]
        public int TestimonialId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Label to display to viewers")]
        public string Label { get; set; }

        [DisplayName("Date of testimonial (optional)")]
        public DateTime ?Date { get; set; }

        [Required]
        [Url]
        [DisplayName("Upload image file")]
        public string ImageUrl { get; set; }
    }
}