using System.ComponentModel.DataAnnotations;

namespace ContosoAssets.WebApp.Areas.SubscriptionManagement.Models
{
    public class CustomerUserModel
    {
        public string CustomerName { get; set; }

        [Display(Name = "User name")] public string CustomerUserName { get; set; }
    }
}
