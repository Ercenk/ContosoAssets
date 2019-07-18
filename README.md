# ContosoAssets
## A sample multi-tenant solution integrates with Azure Marketplace.

This sample intends to demonstrate how to integrate an existing solution to Azure Marketplace to publish it as a SaaS offer.

The multi-tenant solution uses its own identity management for managing the customer users. The users a can track their company's assets. The solution keeps track of the usage, and checks it against the plan the customer signed up for.

To run
- Register a multi-tenant application on Azure Active Directory
- Modify the appsettings.json files with the new application's App Id and App Key
- Run docker-compose up

A more detailed documentation is going to follow. Please stay tuned.
