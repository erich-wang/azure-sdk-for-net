// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Azure.Identity;
using Azure.Management.Compute.Models;
using Azure.Management.Network;
using Azure.Management.Network.Models;

namespace Azure.Management.Compute.Tests
{
    /// <summary>
    /// create vm
    /// </summary>
    public class CreateVMSample
    {
        public static async Task CreateVmAsync(
            string subscriptionId,
            string resourceGroup,
            string location,
            string vmName)
        {
            var computeClient = new ComputeManagementClient(subscriptionId, new DefaultAzureCredential());
            var networkClient = new NetworkManagementClient(subscriptionId, new DefaultAzureCredential());

            var availabilitySetsClient = computeClient.GetAvailabilitySetsClient();
            var virtualNetworksClient = networkClient.GetVirtualNetworksClient();
            var networkInterfaceClient = networkClient.GetNetworkInterfacesClient();
            var virtualMachinesClient = computeClient.GetVirtualMachinesClient();
            // Create AvailabilitySet
            var availabilitySet = new AvailabilitySet(location)
            {
                PlatformUpdateDomainCount = 5,
                PlatformFaultDomainCount = 2,
                Sku = new Sku() { Name = "Aligned" }  // TODO. Verify new codegen on AvailabilitySetSkuTypes.Aligned
            };

            availabilitySet = await availabilitySetsClient.CreateOrUpdateAsync(resourceGroup, vmName + "_aSet", availabilitySet);

            // Create IP Address
            // TODO verify why lack of (location) ctor.

            // Create VNet
            var vnet = new VirtualNetwork()
            {
                Location = location,
                AddressSpace = new AddressSpace() { AddressPrefixes = new List<string>() { "10.0.0.0/16" } },
                Subnets = new List<Subnet>()
                {
                    new Subnet()
                    {
                        Name = "mySubnet",
                        AddressPrefix = "10.0.0.0/24",
                    }
                },
            };

            vnet = await virtualNetworksClient
                .StartCreateOrUpdate(resourceGroup, vmName + "_vent", vnet)
                .WaitForCompletionAsync();

            // Create Network interface
            var nic = new NetworkInterface()
            {
                Location = location,
                IpConfigurations = new List<NetworkInterfaceIPConfiguration>()
                {
                    new NetworkInterfaceIPConfiguration()
                    {
                        Name = "Primary",
                        Primary = true,
                        Subnet = new Subnet() { Id = vnet.Subnets.First().Id },
                        PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                    }
                }
            };

            nic = await networkInterfaceClient
                .StartCreateOrUpdate(resourceGroup, vmName + "_nic", nic)
                .WaitForCompletionAsync();

            var vm = new VirtualMachine(location)
            {
                NetworkProfile = new Compute.Models.NetworkProfile { NetworkInterfaces = new[] { new NetworkInterfaceReference() { Id = nic.Id } } },
                OsProfile = new OSProfile
                {
                    ComputerName = "testVM",
                    AdminUsername = "username",
                    AdminPassword = "(YourPassword)",
                    LinuxConfiguration = new LinuxConfiguration { DisablePasswordAuthentication = false, ProvisionVMAgent = true }
                },
                StorageProfile = new StorageProfile()
                {
                    ImageReference = new ImageReference()
                    {
                        Offer = "UbuntuServer",
                        Publisher = "Canonical",
                        Sku = "18.04-LTS",
                        Version = "latest"
                    },
                    DataDisks = new List<DataDisk>()
                },
                HardwareProfile = new HardwareProfile() { VmSize = VirtualMachineSizeTypes.StandardB1Ms },
            };
            vm.AvailabilitySet.Id = availabilitySet.Id;

            var operaiontion = await virtualMachinesClient.StartCreateOrUpdateAsync(resourceGroup, vmName, vm);
            var vm = (await operaiontion.WaitForCompletionAsync()).Value;
        }
    }
}
