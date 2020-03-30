# Azure.Generated.Template

Run `dotnet msbuild /t:GenerateCode` to generate code.

### AutoRest Configuration
> see https://aka.ms/autorest

``` yaml
title: Azure.Management.Compute
# require: $(this-folder)/../../readme.md
input-file:
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.Compute/stable/2019-12-01/compute.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.Compute/stable/2019-12-01/runCommands.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.Compute/stable/2019-04-01/skus.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.Compute/stable/2019-11-01/disk.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.Compute/stable/2019-12-01/gallery.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/compute/resource-manager/Microsoft.ContainerService/stable/2017-01-31/containerService.json
namespace: Azure.Management.Compute
payload-flattening-threshold: 2
```
# input-file:
#    -  $(this-folder)/swagger/TestSwagger.json
