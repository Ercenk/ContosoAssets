# ContosoAssets

## A sample multi-tenant solution integrates with Azure Marketplace.

### At a glance

This sample intends to demonstrate how to integrate an existing solution to Azure Marketplace to publish it as a SaaS offer.

The multi-tenant solution uses its own identity management for managing the customer users. The users a can track their company's assets. The solution keeps track of the usage, and checks it against the plan the customer signed up for.

To run

- Register a multi-tenant application for landing page and another application for API interaction on Azure Active Directory
- Modify the appsettings.json files with the new application's App Id and App Key
- Run docker-compose up

Please see the related [section](https://github.com/Ercenk/AzureMarketplaceSaaSApiClient#integrating-a-software-as-a-solution-with-azure-marketplace) on my marketplace REST API client implementation for an overview of the integration concepts.  

## Areas of interest for a SaaS Solution

Over the many years working with various teams and helping them plan, architect and develop multi-tenant SaaS solutions. There are of course many other pillars for building successful solutions, however, I'd like to focus on the multi-tenancy aspect. I would like to emphasize that my discussion points in this section are generic, and **not particular to Azure Marketplace integration**.

I can distill my learnings in the following perspectives.

- [Customer onboarding](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.SolutionManagement/CustomerManagement/ICustomerManager.cs#L10) and [provisioning](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.SolutionManagement/Provisioning/IProvisioningManager.cs)
- [User identities](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.WebApp/Startup.cs#L85), [user management](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.SolutionManagement/CustomerManagement/ICustomerManager.cs#L12) and [realm discovery](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.Utils/StringExtensions.cs#L7)
- [Usage metering](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.WebApp/Controllers/MeteredController.cs)
- [SKU management](https://github.com/Ercenk/ContosoAssets/blob/master/src/ContosoAssets.SolutionManagement/SubscriptionManagement/ISubscriptionManager.cs#L21)

### Customer onboarding and provisioning

 The natural implication of a multi-tenant solution is to have tenant boundaries. Boundaries imply groups of resources per customer. Customer onboarding and provisioning refers to the overall activities for drawing the boundaries and allocating resources as necessary for a new customer. Let's take the simplest example of a three tier solution. Let's assume no branding on the UI layer, and fully shared business layer. This brings us to the data store. We can talk about tenant boundaries in a number of ways:

- Share tables, where customer data is identified by values on a column
- Share database, a separate table for each customer
- Share server, a separate database for each customer
- Share platform, a separate server for each customer

As you can notice, creating the boundary is a major aspect, but provisioning (allocating) resources for customers depends on many other aspects of the solution, such as scalability, security concerns etc.

### User identities, user management and realm discovery

The solution should use an identity provider (IP) such as Azure Active Directory, or any other user management solution to identify a user, and map the user to a customer, as well as management of the users who can use the solution for that customer.

### Usage metering

The solution should track the usage of the solution features. I recommend metering anything you can meter, regardless of their relevance to offered SKUs. Such metering can give you valuable information how your solution is used, and monetize the components as necessary in the future. Metering and instrumenting the usage will give you opportunities to identify areas where you can attract more customers, and monetize your solution better.

### SKU Management

The solution should model the usage limits per resources and map them to SKUs. The metering data then can be used to check against the resource usage limits, and determine upsell opportunities, as well as keeping the customer within their subscribed SKU boundaries.

## Notes

## Secrets

Secrets such as API keys are managed through "dotnet user-secrets" command. For example, to set the value for "FulfillmentClient:AzureActiveDirectory:AppKey" use the following command:

``` sh
dotnet user-secrets set "FulfillmentClient:AzureActiveDirectory:AppKey" "secret here"
```

Please see the user secrets [documentation](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows) for more details.
