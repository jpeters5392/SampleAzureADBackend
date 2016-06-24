# SampleAzureADBackend

This application is intended to be a simple backend server for presentations involving Azure Active Directory

## Role Based Claims ##
Application roles are configured in the Azure AD application's manifest JSON file.  If there are more than 2 app roles defined then when you assign a user to the application you are prompted to select which role they receive.  App roles are passed along in the "roles" claim.

## Group Based Claims ##
Group claims are not sent by default.  However, once you have authenticated the user you may query the Graph API for the current user to retrieve their group memberships.  If you do this as part of the sign on process then you can add those to the initial claims if you want.  Otherwise you can just retrieve them as needed.  

## Client Secret ##
The client secret is a key that is generated in the Azure portal and is only available when you save it initially.
