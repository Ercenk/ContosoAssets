using System.Security.Cryptography.X509Certificates;

namespace ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment
{
    public class CertificateCredentialProvider : ICredentialProvider
    {
        public string CertificateThumprint { get; set; }
        public StoreName CertificateStore { get; set; }
        public StoreLocation StoreLocation { get; set; }
    }
}
