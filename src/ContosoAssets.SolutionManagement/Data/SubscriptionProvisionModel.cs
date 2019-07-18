using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.SolutionManagement.Data
{
    public class SubscriptionProvisionModel
    {
        [Display(Name = "Activation code (default: activate)")]
        public string ActivationCode { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name =
            "Preferred region - note this is an example for any other provisioning property that may be asked during provisioning.")]
        public CustomerRegionEnum? CustomerRegion { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public Guid? PlanId { get; set; }
    }
}
