# est-mit-web

This repository hosts the frontend for the manual invoice service, designed as a web application. It includes various interactive web pages, API integrations, and services necessary for the invoicing process. Key functionalities involve creating and managing invoices, uploading bulk data, and handling payment requests.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=rpa-mit-web) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-web&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=rpa-mit-web) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-web&metric=coverage)](https://sonarcloud.io/summary/new_code?id=rpa-mit-web) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-web&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=rpa-mit-web)
## Requirements

Amend as needed for your distribution, this assumes you are using windows with WSL.

- <details>
    <summary> .NET 6 SDK </summary>
    
    #### Basic instructions for installing the .NET 6 SDK on a debian based system.
  
    Amend as needed for your distribution.

    ```bash
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-6.0
    ```
</details>

- [Docker](https://docs.docker.com/desktop/install/linux-install/)
- Azure AD Credentials
- Azure Service Bus
---
## Local Setup

To run this service locally complete the following steps.

### Set up user secrets

Use the secrets-template to create a secrets.json in the same location.

Setting this key allows you to use local emulation for functions such as blob storage.

```json 
{
	"PrimaryConnection": "UseDevelopmentStorage=true"
}
```

Once this is done run the following command to add the projects user secrets

```bash
cat secrets.json | dotnet user-secrets set
```

These values can also be created as environment variables or as a development app settings file, but the preferred method is via user secrets.

## Create emulated storage

You need to create a local emulation of azure blob storage, this can be done using [azurite](https://github.com/Azure/Azurite).

In your console run the following commands.

```bash
docker pull mcr.microsoft.com/azure-storage/azurite
```

```bash
docker run --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

You can view the emulated storage using a tool such as [Azure Storage Explorer](https://github.com/microsoft/AzureStorageExplorer).

## Start the application

```bash
cd EST.MIT.Web
```

```bash
dotnet run
```

---
## Running in Docker

To create the application as a docker container run the following command in the parent directory.

```bash
docker compose up
```

It is also possible to standup the whole MIT service in docker by running

```
A Command
```

