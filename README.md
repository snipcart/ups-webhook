# ups-webhook

This project is a simple webhook to use with our shipping webhook system. https://docs.snipcart.com/v3/webhooks/shipping
It connects to UPS API and returns the shipping rates for a given address and package.
It uses Azure functions to run the webhook and Azure Key Vault to store the UPS API credentials. You may rewrite to to use elsewhere, integrate in another backend or cloud service.
https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide

## Configuration

To run locally, create a local.settings.json file based on the example.
These same settings will be used in the Azure Function App Environment Variables.

To get your UPS Client ID and CLient Secret, you will need to create an Application on UPS. See details here : https://developer.ups.com/get-started

There are plenty more options to send to UPS. You may read about their Rate API here : https://developer.ups.com/api/reference?loc=en_US#operation/Rate
And modify the request object in the `UpsService` class.