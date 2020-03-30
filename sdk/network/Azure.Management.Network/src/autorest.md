# Azure.Generated.Template

Run `dotnet msbuild /t:GenerateCode` to generate code.

### AutoRest Configuration
> see https://aka.ms/autorest

```yaml
title: Azure.Management.Network
# require: $(this-folder)/../../readme.md
input-file:
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/applicationGateway.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/applicationSecurityGroup.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/availableDelegations.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/availableServiceAliases.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/azureFirewall.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/azureFirewallFqdnTag.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/bastionHost.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/checkDnsAvailability.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/ddosCustomPolicy.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/ddosProtectionPlan.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/endpointService.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/expressRouteCircuit.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/expressRouteCrossConnection.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/expressRouteGateway.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/expressRoutePort.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/firewallPolicy.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/ipGroups.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/loadBalancer.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/natGateway.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/network.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/networkInterface.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/networkProfile.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/networkSecurityGroup.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/networkVirtualAppliance.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/networkWatcher.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/operation.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/privateEndpoint.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/privateLinkService.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/publicIpAddress.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/publicIpPrefix.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/routeFilter.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/routeTable.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/serviceCommunity.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/serviceEndpointPolicy.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/serviceTags.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/usage.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/virtualNetwork.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/virtualNetworkGateway.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/virtualNetworkTap.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/virtualRouter.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/virtualWan.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/vmssNetworkInterface.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/vmssPublicIpAddress.json
    - https://raw.githubusercontent.com/Azure/azure-rest-api-specs/master/specification/network/resource-manager/Microsoft.Network/stable/2019-12-01/webapplicationfirewall.json
namespace: Azure.Management.Network
payload-flattening-threshold: 2
```
