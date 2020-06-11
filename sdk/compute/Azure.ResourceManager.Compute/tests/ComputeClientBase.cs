// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Network;
using Azure.Management.Resources;
using Azure.Management.Storage;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Tests;
using Azure.ResourceManager.TestFramework;
using NUnit.Framework;
using OperationsClient = Azure.ResourceManager.Compute.OperationsClient;

namespace Azure.ResourceManager
{
    [RunFrequency(RunTestFrequency.Manually)]
    [ClientTestFixture]
    public abstract class ComputeClientBase : ManagementRecordedTestBase<ComputeManagementTestEnvironment>
    {
        public ResourceGroupsClient ResourceGroupsClient { get; set; }
        public ProvidersClient ProvidersClient { get; set; }
        public DeploymentsClient DeploymentsClient { get; set; }
        public TagsClient TagsClient { get; set; }
        public ResourcesClient ResourcesClient { get; set; }
        public VirtualMachineImagesClient VirtualMachineImagesClient { get; set; }
        public AvailabilitySetsClient AvailabilitySetsClient { get; set; }
        public ContainerServicesClient ContainerServicesClient { get; set; }
        public DedicatedHostGroupsClient DedicatedHostGroupsClient { get; set; }
        public DedicatedHostsClient DedicatedHostsClient { get; set; }
        public VirtualMachineExtensionImagesClient VirtualMachineExtensionImagesClient { get; set; }
        public ResourceSkusClient ResourceSkusClient { get; set; }
        public LogAnalyticsClient LogAnalyticsClient { get; set; }
        public OperationsClient OperationsClient { get; set; }
        public ProximityPlacementGroupsClient ProximityPlacementGroupsClient { get; set; }
        public VirtualMachinesClient VirtualMachinesClient { get; set; }
        public VirtualMachineRunCommandsClient VirtualMachineRunCommandsClient { get; set; }
        public VirtualMachineScaleSetExtensionsClient VirtualMachineScaleSetExtensionsClient { get; set; }
        public VirtualMachineScaleSetsClient VirtualMachineScaleSetsClient { get; set; }
        public VirtualMachineScaleSetVMsClient VirtualMachineScaleSetVMsClient { get; set; }
        public VirtualMachineScaleSetRollingUpgradesClient VirtualMachineScaleSetRollingUpgradesClient { get; set; }
        public DisksClient DisksClient { get; set; }
        public VirtualMachineSizesClient VirtualMachineSizesClient { get; set; }
        public SnapshotsClient SnapshotsClient { get; set; }
        public DiskEncryptionSetsClient DiskEncryptionSetsClient { get; set; }
        public VirtualNetworksClient VirtualNetworksClient { get; set; }
        public PublicIPAddressesClient PublicIPAddressesClient { get; set; }
        public StorageAccountsClient StorageAccountsClient { get; set; }
        public SubnetsClient SubnetsClient { get; set; }
        public NetworkInterfacesClient NetworkInterfacesClient { get; set; }
        public VirtualMachineExtensionsClient VirtualMachineExtensionsClient { get; set; }
        public GalleriesClient GalleriesClient { get; set; }
        public GalleryImagesClient GalleryImagesClient { get; set; }
        public GalleryImageVersionsClient GalleryImageVersionsClient { get; set; }
        public ImagesClient ImagesClient { get; set; }
        public GalleryApplicationsClient GalleryApplicationsClient { get; set; }
        public GalleryApplicationVersionsClient GalleryApplicationVersionsClient { get; set; }
        public BlobContainersClient BlobContainersClient { get; set; }
        public UsageClient UsageClient { get; set; }
        public ApplicationGatewaysClient ApplicationGatewaysClient { get; set; }
        public LoadBalancersClient LoadBalancersClient { get; set; }
        public NetworkSecurityGroupsClient NetworkSecurityGroupsClient { get; set; }
        public PublicIPPrefixesClient PublicIPPrefixesClient { get; set; }
        public string DefaultLocation { get; set; }
        protected ComputeClientBase(bool isAsync)
            : base(isAsync)
        {
        }

        protected void InitializeBase()
        {
            var resourceManagementClient = GetResourceManagementClient();
            ResourceGroupsClient = resourceManagementClient.GetResourceGroupsClient();
            ProvidersClient = resourceManagementClient.GetProvidersClient();
            DeploymentsClient = resourceManagementClient.GetDeploymentsClient();
            TagsClient = resourceManagementClient.GetTagsClient();
            ResourcesClient = resourceManagementClient.GetResourcesClient();
            var ComputeManagementClient = GetComputeManagementClient();
            VirtualMachineImagesClient = ComputeManagementClient.GetVirtualMachineImagesClient();
            AvailabilitySetsClient = ComputeManagementClient.GetAvailabilitySetsClient();
            ContainerServicesClient = ComputeManagementClient.GetContainerServicesClient();
            DedicatedHostGroupsClient = ComputeManagementClient.GetDedicatedHostGroupsClient();
            DedicatedHostsClient = ComputeManagementClient.GetDedicatedHostsClient();
            VirtualMachineExtensionImagesClient = ComputeManagementClient.GetVirtualMachineExtensionImagesClient();
            ResourceSkusClient = ComputeManagementClient.GetResourceSkusClient();
            LogAnalyticsClient = ComputeManagementClient.GetLogAnalyticsClient();
            OperationsClient = ComputeManagementClient.GetOperationsClient();
            ProximityPlacementGroupsClient = ComputeManagementClient.GetProximityPlacementGroupsClient();
            VirtualMachinesClient = ComputeManagementClient.GetVirtualMachinesClient();
            VirtualMachineRunCommandsClient = ComputeManagementClient.GetVirtualMachineRunCommandsClient();
            VirtualMachineScaleSetExtensionsClient = ComputeManagementClient.GetVirtualMachineScaleSetExtensionsClient();
            VirtualMachineScaleSetsClient = ComputeManagementClient.GetVirtualMachineScaleSetsClient();
            VirtualMachineScaleSetVMsClient = ComputeManagementClient.GetVirtualMachineScaleSetVMsClient();
            VirtualMachineScaleSetRollingUpgradesClient = ComputeManagementClient.GetVirtualMachineScaleSetRollingUpgradesClient();
            DisksClient = ComputeManagementClient.GetDisksClient();
            VirtualMachineSizesClient = ComputeManagementClient.GetVirtualMachineSizesClient();
            SnapshotsClient = ComputeManagementClient.GetSnapshotsClient();
            DiskEncryptionSetsClient = ComputeManagementClient.GetDiskEncryptionSetsClient();
            VirtualMachineExtensionsClient = ComputeManagementClient.GetVirtualMachineExtensionsClient();
            GalleriesClient = ComputeManagementClient.GetGalleriesClient();
            GalleryImagesClient = ComputeManagementClient.GetGalleryImagesClient();
            GalleryImageVersionsClient = ComputeManagementClient.GetGalleryImageVersionsClient();
            ImagesClient = ComputeManagementClient.GetImagesClient();
            GalleryApplicationsClient = ComputeManagementClient.GetGalleryApplicationsClient();
            GalleryApplicationVersionsClient = ComputeManagementClient.GetGalleryApplicationVersionsClient();
            UsageClient = ComputeManagementClient.GetUsageClient();
            var NetworkManagementClient = GetNetworkManagementClient();
            PublicIPAddressesClient = NetworkManagementClient.GetPublicIPAddressesClient();
            SubnetsClient = NetworkManagementClient.GetSubnetsClient();
            NetworkInterfacesClient = NetworkManagementClient.GetNetworkInterfacesClient();
            ApplicationGatewaysClient = NetworkManagementClient.GetApplicationGatewaysClient();
            LoadBalancersClient = NetworkManagementClient.GetLoadBalancersClient();
            NetworkSecurityGroupsClient = NetworkManagementClient.GetNetworkSecurityGroupsClient();
            PublicIPPrefixesClient = NetworkManagementClient.GetPublicIPPrefixesClient();
            VirtualNetworksClient = NetworkManagementClient.GetVirtualNetworksClient();
            var StorageManagementClient = GetStorageManagementClient();
            StorageAccountsClient = StorageManagementClient.GetStorageAccountsClient();
            BlobContainersClient = StorageManagementClient.GetBlobContainersClient();
            DefaultLocation = "southeastasia";
        }
        internal ComputeManagementClient GetComputeManagementClient()
        {
            return CreateClient<ComputeManagementClient>(this.TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ComputeManagementClientOptions()));
        }
        internal NetworkManagementClient GetNetworkManagementClient()
        {
            return CreateClient<NetworkManagementClient>(this.TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new NetworkManagementClientOptions()));
        }
        internal StorageManagementClient GetStorageManagementClient()
        {
            return CreateClient<StorageManagementClient>(this.TestEnvironment.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new StorageManagementClientOptions()));
        }
        public void WaitSeconds(double seconds)
        {
            if (Mode != RecordedTestMode.Playback)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
            }
        }

        public void WaitMinutes(double minutes)
        {
            WaitSeconds(minutes * 60);
        }
    }
}
