# Info
In a real project, I propose using **.NET Aspire** to simplify the process of microservices' service discovery and orchestration. For more details, please see [here](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview "here").

This simple .NET Minimal WebAPI retrieves data from the output of an Azure Data Factory Pipeline and can be deployed using `az` or `azd` commands, or by utilizing your own CI/CD pipeline. For example:
```bash
# Make sure to login to your Azure Subscription account by executing this azd command 
azd auth login
# You can still execute this to make sure everything is ok.
azd init
# Provision and deploy to Azure
azd up
# Delete any created servcices/resouces when you don't need it anymore
azd down
```
> *azd commands will do all the magic for you*, read more details [here](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/)。
