using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ContosoAssets.SolutionManagement.AzureMarketplaceFulfillment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using SaaSFulfillmentClient;
using SaaSFulfillmentClient.AzureAD;
using SaaSFulfillmentClient.Models;
using Xunit;

namespace FulfillmentSdkTests
{
    public class FulfillmentManagerTests
    {
        private const string MockApiVersion = "2018-09-15";

        private const string MockUri = "https://marketplaceapi.microsoft.com/api/saas";

        private readonly IConfigurationRoot configuration;

        private readonly Mock<ICredentialProvider> credentialProviderMock;

        private readonly Mock<IFulfillmentClient> fulfillmentClientMock;

        private readonly Mock<ILogger> loggerMock;

        private readonly Mock<IOptionsMonitor<SecuredFulfillmentClientConfiguration>> optionsMonitorMock;

        private Mock<IConfidentialClientApplication> adApplicationMock;

        private Mock<IFulfillmentClient> client;

        public FulfillmentManagerTests()
        {
            this.loggerMock = new Mock<ILogger>();
            this.client = new Mock<IFulfillmentClient>();

            this.optionsMonitorMock = new Mock<IOptionsMonitor<SecuredFulfillmentClientConfiguration>>();
            this.optionsMonitorMock
                .Setup(om => om.CurrentValue)
                .Returns(new SecuredFulfillmentClientConfiguration
                {
                    FulfillmentService = new FulfillmentClientConfiguration
                    {
                        BaseUri = MockUri,
                        ApiVersion = MockApiVersion
                    }
                });

            this.credentialProviderMock = new Mock<ICredentialProvider>();

            this.fulfillmentClientMock = new Mock<IFulfillmentClient>();

            this.adApplicationMock = new Mock<IConfidentialClientApplication>();

            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<FulfillmentManagerTests>();
            this.configuration = builder.Build();
        }

        [Fact]
        public async Task CanActivateSubscription()
        {
            var loggerMock = new Mock<ILogger<FulfillmentManager>>();
            var fulfillmentManager = new FulfillmentManager(
                this.fulfillmentClientMock.Object,
                loggerMock.Object);

            var subscriptionId = Guid.NewGuid();

            var subscriptionDetails = new MarketplaceSubscription { PlanId = "id", SubscriptionId = subscriptionId };

            this.fulfillmentClientMock.Setup(c => c.ActivateSubscriptionAsync(It.IsAny<Guid>(),
                    It.IsAny<ActivatedSubscription>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new FulfillmentRequestResult { Success = true }));

            var result =
                await fulfillmentManager.ActivateSubscriptionAsync(subscriptionId, subscriptionDetails.PlanId, null);

            this.fulfillmentClientMock
                .Verify(c =>
                        c.ActivateSubscriptionAsync(
                            It.Is<Guid>(s => s == subscriptionId),
                            It.Is<ActivatedSubscription>(a => a.PlanId == "id"),
                            It.IsAny<Guid>(),
                            It.IsAny<Guid>(),
                            It.IsAny<CancellationToken>()),
                    Times.Once());
            Assert.True(result.State == SubscriptionState.Complete);
        }

        [Fact]
        public void CanBuildWithOptionsAndCertificate()
        {
            var configurationDictionary = new Dictionary<string, string>
            {
                {"FulfillmentClient:AzureActiveDirectory:ClientId", "client"},
                {"FulfillmentClient:FulfillmentService:ApiVersion", "api"},
                {"FulfillmentClient:FulfillmentService:BaseUri", "uri"}
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationDictionary).Build();

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

            //Assert.Throws<NotImplementedException >(() =>
            //services.AddFulfillmentManager(options => configuration.Bind("FulfillmentClient", options),
            //    credentialBuilder => credentialBuilder.WithCertificateAuthentication(
            //        System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser,
            //        System.Security.Cryptography.X509Certificates.StoreName.My,
            //        "thumbprint")));

            services.AddFulfillmentClient(options => configuration.Bind("FulfillmentClient", options),
                credentialBuilder => credentialBuilder.WithCertificateAuthentication(
                    StoreLocation.CurrentUser,
                    StoreName.My,
                    "thumbprint"));

            var serviceProvider = services.BuildServiceProvider();

            Assert.Throws<NotImplementedException>(() =>
            {
                var fulfillmentManager = serviceProvider.GetRequiredService<IFulfillmentManager>();
            });
        }

        [Fact]
        public void CanBuildWithOptionsAndSecret()
        {
            var configurationDictionary = new Dictionary<string, string>
            {
                {"FulfillmentClient:AzureActiveDirectory:ClientId", Guid.NewGuid().ToString()},
                {"FulfillmentClient:FulfillmentService:ApiVersion", "api"},
                {"FulfillmentClient:FulfillmentService:BaseUri", "uri"}
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationDictionary).Build();

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

            services.AddFulfillmentClient(options => configuration.Bind("FulfillmentClient", options),
                credentialBuilder => credentialBuilder.WithClientSecretAuthentication("secret"));

            var serviceProvider = services.BuildServiceProvider();

            var fulfillmentManager = serviceProvider.GetRequiredService<IFulfillmentManager>();
        }

        [Fact]
        public async Task CanResolveSubscription()
        {
            var loggerMock = new Mock<ILogger<FulfillmentManager>>();

            var fulfillmentManager = new FulfillmentManager(
                this.fulfillmentClientMock.Object,
                loggerMock.Object);

            this.fulfillmentClientMock.Setup(f => f.ResolveSubscriptionAsync(It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ResolvedSubscription()));

            var subscriptionId = await fulfillmentManager.ResolveSubscriptionAsync("authCode");

            this.fulfillmentClientMock
                .Verify(c =>
                        c.ResolveSubscriptionAsync(
                            It.Is<string>(a => a == "authCode"),
                            It.IsAny<Guid>(),
                            It.IsAny<Guid>(),
                            It.IsAny<CancellationToken>()),
                    Times.Once());
        }

        private IConfidentialClientApplication AdApplicationFactory()
        {
            var appKey = this.configuration["AppKey"];

            return ConfidentialClientApplicationBuilder
                .Create("84aca647-1340-454b-923c-a21a9003b28e")
                .WithClientSecret(appKey)
                .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .Build();
        }
    }
}
