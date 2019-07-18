namespace ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment
{
    internal class ClientSercretCredentialProvider : ICredentialProvider
    {
        public ClientSercretCredentialProvider(string clientSecret)
        {
            this.ClientSecret = clientSecret;
        }

        public string ClientSecret { get; }
    }
}
