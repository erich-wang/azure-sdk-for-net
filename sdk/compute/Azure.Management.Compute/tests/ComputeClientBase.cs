// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
//using Azure.Core.Testing;
using Azure.Identity;
using Azure.Management.Network;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Storage;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    [ClientTestFixture]
    [NonParallelizable]
    public abstract class ComputeClientBase : RecordedTestBase<ComputeManagementTestEnvironment>
    {
        private const string ObjectIdKey = "ObjectId";
        private const string ApplicationIdKey = "ApplicationId";
        public string tenantId { get; set; }
        public string objectId { get; set; }
        public string applicationId { get; set; }
        public string location { get; set; }
        public string SubscriptionId { get; set; }
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
            if (Mode == RecordedTestMode.Playback && Recording.IsTrack1SessionRecord())
            {
                //this.tenantId = TestEnvironment.TenantId.TenantIdTrack1;
                //this.subscriptionId = TestEnvironment.SubscriptionId.SubscriptionIdTrack1;
                this.tenantId = TestEnvironment.TenantId;
                this.SubscriptionId = TestEnvironment.SubscriptionIdTrack1;
                //this.tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
                ////this.tenantId = TestEnvironment.TenantId;
                //this.SubscriptionId = TestEnvironment.SubscriptionIdTrack1;
            }
            else
            {
                this.tenantId = TestEnvironment.TenantId;
                this.SubscriptionId = TestEnvironment.SubscriptionId;
                //this.tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
                //this.SubscriptionId = "c9cbd920-c00c-427c-852b-8aaf38badaeb";
                //this.SubscriptionId = TestEnvironment.SubscriptionId;
            }
            this.applicationId = Recording.GetVariable(ApplicationIdKey, Guid.NewGuid().ToString());
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
        internal ResourcesManagementClient GetResourceManagementClient()
        {
            return InstrumentClient(new ResourcesManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ResourcesManagementClientOptions())));
        }
        internal ComputeManagementClient GetComputeManagementClient()
        {
            return InstrumentClient(new ComputeManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new ComputeManagementClientOptions())));
        }
        internal NetworkManagementClient GetNetworkManagementClient()
        {
            return InstrumentClient(new NetworkManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new NetworkManagementClientOptions())));
        }
        internal StorageManagementClient GetStorageManagementClient()
        {
            return InstrumentClient(new StorageManagementClient(this.SubscriptionId,
                TestEnvironment.Credential,
                Recording.InstrumentClientOptions(new StorageManagementClientOptions())));
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
