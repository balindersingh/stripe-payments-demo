# Stripe 3D Secure 2 demo with .net core
## Pre-requisite
* Stripe test account
* ngrok account (optional if you want to deploy this to some public hosting). I am using it just to make my locally hosted application public since Stripe needs a callback endpoint to send some info.
## Config variables
* SecretKey : secret key of your stripe account 
* RedirectUrl : the url where this application will be hosted publicly so that stripe can use it as callback url
* PublishableKey: publishable key of your stripe account
* WebhookSecret: to listen to webhook you setup on your Stripe account to capture event specifically for ach debit payments
NOTE: Idealy, at a time only one account will be used but to test ACH debit payment for US, 2 additional keys are added as the non-US keys are set from Canadian account
## Development notes
* Let's start listening on 5000 port to ngrok endpoint (you can change it in settings files to listen on diffrent port)
* ```./ngrok http 5002```
* Make sure you update Stripe settings appropriately in appsettings.json
## Build and run
* Now you just need to run following command to see the application in action
* ```dotnet build && dotnet run```


## Stripe implementation notes
* For webhook (/api/stripe/webhook), create an endpoint in your Stripe dashboard with following settings:
    * Endpoint url : https://<yourwebsite>/api/stripe/webhook
    * Listen to : default (Events on your account)
    * Select events to listen to: 
        * payment_intent.payment_failed
        * payment_intent.succeeded
    * see screenshot ![Webhook Endpoint](/webhook-screenshot.png)
* And then add it in your appsettings -> StripeSettings -> WebhookSecret


## To publish to somewhere on the cloud i.e. Azure app service 
* ```dotnet publish -c Release -o ./publish```
* Then just right click on *publish* folder and click Deploy To Web App. Checkout [How to deploy .net core on Azure using VSCode](https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vscode?view=aspnetcore-2.2)