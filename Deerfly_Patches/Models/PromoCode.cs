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
        public string Code { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [ForeignKey("PromotionalItem")]
        public int PromotionalItemId { get; set; }
        public virtual Product PromotionalItem { get; set; }

        public decimal PromotionalItemPrice { get; set; }

        [ForeignKey("WithPurchaseOf")]
        public int WithPurchaseOfId { get; set; }
        public virtual Product WithPurchaseOf { get; set; }

        public decimal MinimumQualifyingPurchase { get; set; }

        public decimal PercentOffItem { get; set; }

        public decimal PercentOffOrder { get; set; }

        public decimal SpecialPrice { get; set; }

        public Product SpecialPriceItem { get; set; }

        public DateTime CodeStart { get; set; }

        public DateTime CodeEnd { get; set; }
    }
}
