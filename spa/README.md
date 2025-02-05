# Info


If you would like to run this, you have to get the **ClientId** and **TenantId** from the Entra ID's App Registration page and update this file: 
> /src/environments/environment.dev.ts

```json
 msalConfig: {
        auth: {
            clientId: <client-id>,
            authority: 'https://login.microsoftonline.com/<tenant-id>'
        }
    }
```
If you're not familiar with how to register your SPA in Entra ID, you can follow [this](https://www.youtube.com/watch?v=QZnX_KXTpfI&t=60s "this").

Install Angular dependencies and run: 
```json
npm install or npm i
npm start or ng serve
```
 
## Quick Test
In this demonstration, I show how my account is validated by the Microsoft Identity platform, prompting me for two-factor authentication as part of the security measures. 

https://github.com/user-attachments/assets/6a000475-8b4d-44a0-9d21-a79858e89994

## Quick Reference
- This Angular project was created following the official tutorial of [Angualr.dev](https://angular.dev/tutorials "Angualr.dev") 
- For MSAL implementation, see official details [here](https://learn.microsoft.com/en-us/entra/identity-platform/msal-overview "here") 

