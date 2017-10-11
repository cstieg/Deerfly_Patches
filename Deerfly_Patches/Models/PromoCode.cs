using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deerfly_Patches.Models
{
    public class PromoCode
    {
        [Key]
        public int PromoCodeId { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(20, MinimumLength = 2)]
        [Display(Description = "Enter the promotional code here")]
        public string Code { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [ForeignKey("PromotionalItem")]
        public Nullable<int> PromotionalItemId { get; set; }
        [Display(Name = "Promotional Item", Description = "A promotional item that is added to the cart")]
        public virtual Product PromotionalItem { get; set; }

        [Display(Name = "Promotional Item Price", Description = "The price of the promotional item; may be 0.00")]
        public decimal? PromotionalItemPrice { get; set; }

        [ForeignKey("WithPurchaseOf")]
        public int? WithPurchaseOfId { get; set; }
        [Display(Name = "With Purchase Of", Description = "An item that must be purchased to qualify for the promo code")]
        public virtual Product WithPurchaseOf { get; set; }

        [Display(Name = "Minimum Qualifying Purchase", Description = "The minimum purchase amount to qualify for the promo code")]
        public decimal? MinimumQualifyingPurchase { get; set; }

        [Display(Name = "Percent Off Order", Description = "A percentage by which the entire order is reduced")]
        public decimal? PercentOffOrder { get; set; }

        [Display(Name = "Percent Off Item", Description = "A percentage by which the Special Price Item is reduced")]
        public decimal? PercentOffItem { get; set; }

        [Display(Name = "Special Price", Description = "The amount at which the Special Price Item is sold")]
        public decimal? SpecialPrice { get; set; }

        [ForeignKey("SpecialPriceItem")]
        public int? SpecialPriceItemId { get; set; }
        [Display(Name = "Special Price Item", Description = "The item for Percent Off Item or Special Price discount")]
        public virtual Product SpecialPriceItem { get; set; }

        [Display(Name = "Code Start Date")]
        public DateTime? CodeStart { get; set; }

        [Display(Name = "Code End Date")]
        public DateTime? CodeEnd { get; set; }
    }
}
