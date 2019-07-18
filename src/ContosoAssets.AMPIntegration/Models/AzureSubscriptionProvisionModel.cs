using System;
using System.ComponentModel.DataAnnotations;
using ContosoAssets.SolutionManagement.Data;

namespace ContosoAssets.AMPIntegration.Models
{
    public class AzureSubscriptionProvisionModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name =
            "Preferred region - note this is an example for any other provisioning property that may be asked during provisioning.")]
        public CustomerRegionEnum? CustomerRegion { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Full name")] public string FullName { get; set; }

        [Display(Name = "Offer name")] public string OfferId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // This corresponds to the ID of the SKU on the native SKUs
        public Guid? PlanId { get; set; }

        [Display(Name = "Plan name")] public string PlanName { get; set; }

        // Subscription for the offer
        public Guid SubscriptionId { get; set; }

        [Display(Name = "Offer subscription name")]
        public string SubscriptionName { get; set; }
    }
}
