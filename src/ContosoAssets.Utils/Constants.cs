namespace ContosoAssets.Utils
{
    public static class Constants
    {
        public const string CustomerAdminRoleName = "CustomerAdmin";

        public const string CustomerUserRoleName = "User";

        // See https://github.com/aspnet/AspNetCore/blob/master/src/Azure/AzureAD/Authentication.AzureAD.UI/src/AzureADDefaults.cs#L38
        public const string AzureADSchemeName = "AzureAD";

        // See https://github.com/aspnet/AspNetCore/blob/master/src/Identity/Core/src/IdentityConstants.cs#L15
        public const string IdentityApplicationSchemeName = "Identity.Application";
    }
}
