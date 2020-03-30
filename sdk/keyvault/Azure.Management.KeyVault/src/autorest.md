# Azure.Generated.Template

Run `dotnet msbuild /t:GenerateCode` to generate code.

### AutoRest Configuration
> see https://aka.ms/autorest

```yaml
title: Azure.Management.KeyVault
# require: $(this-folder)/../../readme.md
input-file:
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/keyvault/resource-manager/Microsoft.KeyVault/stable/2019-09-01/keyvault.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/keyvault/resource-manager/Microsoft.KeyVault/stable/2019-09-01/providers.json
namespace: Azure.Management.KeyVault
payload-flattening-threshold: 2
```
