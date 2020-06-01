﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Management.Compute.Models;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using Azure.Management.Storage.Models;
using NUnit.Framework;

namespace Azure.Management.Compute.Tests
{
    public class GalleryTests : VMTestBase
    {
        public GalleryTests(bool isAsync)
           : base(isAsync)
        {
            computeManagementTestUtilities = new ComputeManagementTestUtilities(isAsync);
        }
        [SetUp]
        public void ClearChallengeCacheforRecord()
        {
            if (Mode == RecordedTestMode.Record || Mode == RecordedTestMode.Playback)
            {
                InitializeBase();
            }
            //ComputeManagementClient computeClient;
            //ResourceManagementClient resourcesClient;
        }
        protected const string ResourceGroupPrefix = "galleryPsTestRg";
        protected const string GalleryNamePrefix = "galleryPsTestGallery";
        protected const string GalleryImageNamePrefix = "galleryPsTestGalleryImage";
        protected const string GalleryApplicationNamePrefix = "galleryPsTestGalleryApplication";
        private string galleryHomeLocation = "eastus2";
        private string sourceImageId = "";
        [Test]
        public async Task Gallery_CRUD_Tests()
        {
            EnsureClientsInitialized();
            string rgName = Recording.GenerateAssetName(ResourceGroupPrefix);
            string rgName2 = rgName + "New";

            await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(galleryHomeLocation));
            Trace.TraceInformation("Created the resource group: " + rgName);

            string galleryName = Recording.GenerateAssetName(GalleryNamePrefix);
            Gallery galleryIn = GetTestInputGallery();
            await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryIn)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName, rgName));

            Gallery galleryOut = await GalleriesClient.GetAsync(rgName, galleryName);
            Trace.TraceInformation("Got the gallery.");
            Assert.NotNull(galleryOut);
            ValidateGallery(galleryIn, galleryOut);

            galleryIn.Description = "This is an updated description";
            await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryIn)).WaitForCompletionAsync();
            Trace.TraceInformation("Updated the gallery.");
            galleryOut = await GalleriesClient.GetAsync(rgName, galleryName);
            ValidateGallery(galleryIn, galleryOut);

            Trace.TraceInformation("Listing galleries.");
            string galleryName2 = galleryName + "New";
            await ResourceGroupsClient.CreateOrUpdateAsync(rgName2, new ResourceGroup(galleryHomeLocation));
            Trace.TraceInformation("Created the resource group: " + rgName2);
            computeManagementTestUtilities.WaitSeconds(10);
            await (await GalleriesClient.StartCreateOrUpdateAsync(rgName2, galleryName2, galleryIn)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName2, rgName2));
            List<Gallery> listGalleriesInRgResult = await (GalleriesClient.ListByResourceGroupAsync(rgName)).ToEnumerableAsync();
            Assert.True(listGalleriesInRgResult.Count()==1);
            //Assert.Single(listGalleriesInRgResult);
            //Assert.Null(listGalleriesInRgResult.NextPageLink);
            List<Gallery> listGalleriesInSubIdResult = await (GalleriesClient.ListAsync()).ToEnumerableAsync();
            // Below, >= instead of == is used because this subscription is shared in the group so other developers
            // might have created galleries in this subscription.
            Assert.True(listGalleriesInSubIdResult.Count() >= 2);

            Trace.TraceInformation("Deleting 2 galleries.");
            await (await GalleriesClient.StartDeleteAsync(rgName, galleryName)).WaitForCompletionAsync();
            await (await GalleriesClient.StartDeleteAsync(rgName2, galleryName2)).WaitForCompletionAsync();
            listGalleriesInRgResult = await (GalleriesClient.ListByResourceGroupAsync(rgName)).ToEnumerableAsync();
            Assert.IsEmpty(listGalleriesInRgResult);
            // resource groups cleanup is taken cared by MockContext.Dispose() method.
        }

        [Test]
        public async Task GalleryImage_CRUD_Tests()
        {
            EnsureClientsInitialized();
            string rgName = Recording.GenerateAssetName(ResourceGroupPrefix);

            await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(galleryHomeLocation));
            Trace.TraceInformation("Created the resource group: " + rgName);
            string galleryName = Recording.GenerateAssetName(GalleryNamePrefix);
            Gallery gallery = GetTestInputGallery();
            await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, gallery)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName, rgName));

            string galleryImageName = Recording.GenerateAssetName(GalleryImageNamePrefix);
            GalleryImage inputGalleryImage = GetTestInputGalleryImage();
            await (await GalleryImagesClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryImageName, inputGalleryImage)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery image: {0} in gallery: {1}", galleryImageName,
                galleryName));

            GalleryImage galleryImageFromGet = await GalleryImagesClient.GetAsync(rgName, galleryName, galleryImageName);
            Assert.NotNull(galleryImageFromGet);
            ValidateGalleryImage(inputGalleryImage, galleryImageFromGet);

            inputGalleryImage.Description = "Updated description.";
            await (await GalleryImagesClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryImageName, inputGalleryImage)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Updated the gallery image: {0} in gallery: {1}", galleryImageName,
                galleryName));
            galleryImageFromGet = await GalleryImagesClient.GetAsync(rgName, galleryName, galleryImageName);
            Assert.NotNull(galleryImageFromGet);
            ValidateGalleryImage(inputGalleryImage, galleryImageFromGet);

            List<GalleryImage> listGalleryImagesResult = await (GalleryImagesClient.ListByGalleryAsync(rgName, galleryName)).ToEnumerableAsync();
            Assert.IsTrue(listGalleryImagesResult.Count()==1);
            //Assert.Single(listGalleryImagesResult);
            //Assert.Null(listGalleryImagesResult.NextPageLink);

            await (await GalleryImagesClient.StartDeleteAsync(rgName, galleryName, galleryImageName)).WaitForCompletionAsync();
            listGalleryImagesResult = await (GalleryImagesClient.ListByGalleryAsync(rgName, galleryName)).ToEnumerableAsync();
            Assert.IsEmpty(listGalleryImagesResult);
            Trace.TraceInformation(string.Format("Deleted the gallery image: {0} in gallery: {1}", galleryImageName,
                galleryName));

            await (await GalleriesClient.StartDeleteAsync(rgName, galleryName)).WaitForCompletionAsync();
        }

        [Test]
        public async Task GalleryImageVersion_CRUD_Tests()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", galleryHomeLocation);
            EnsureClientsInitialized();
            string rgName = Recording.GenerateAssetName(ResourceGroupPrefix);
            VirtualMachine vm = null;
            string imageName = Recording.GenerateAssetName("psTestSourceImage");

            try
            {

                vm = await CreateCRPImage(rgName, imageName);
                Assert.False(string.IsNullOrEmpty(sourceImageId));
                Trace.TraceInformation(string.Format("Created the source image id: {0}", sourceImageId));

                string galleryName = Recording.GenerateAssetName(GalleryNamePrefix);
                Gallery gallery = GetTestInputGallery();
                await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, gallery)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName,
                    rgName));
                string galleryImageName = Recording.GenerateAssetName(GalleryImageNamePrefix);
                GalleryImage inputGalleryImage = GetTestInputGalleryImage();
                await (await GalleryImagesClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryImageName, inputGalleryImage)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery image: {0} in gallery: {1}", galleryImageName,
                    galleryName));

                string galleryImageVersionName = "1.0.0";
                GalleryImageVersion inputImageVersion = GetTestInputGalleryImageVersion(sourceImageId);
                await (await GalleryImageVersionsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryImageName,
                    galleryImageVersionName, inputImageVersion)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery image version: {0} in gallery image: {1}",
                    galleryImageVersionName, galleryImageName));

                GalleryImageVersion imageVersionFromGet = await GalleryImageVersionsClient.GetAsync(rgName,
                    galleryName, galleryImageName, galleryImageVersionName);
                Assert.NotNull(imageVersionFromGet);
                ValidateGalleryImageVersion(inputImageVersion, imageVersionFromGet);
    //            imageVersionFromGet = await GalleryImageVersionsClient.GetAsync(rgName, galleryName, galleryImageName,
    //galleryImageVersionName, ReplicationStatusTypes.ReplicationStatus);
                imageVersionFromGet = await GalleryImageVersionsClient.GetAsync(rgName, galleryName, galleryImageName,
                    galleryImageVersionName);
                Assert.AreEqual(StorageAccountType.StandardLRS, imageVersionFromGet.PublishingProfile.StorageAccountType);
                Assert.AreEqual(StorageAccountType.StandardLRS,
                    imageVersionFromGet.PublishingProfile.TargetRegions.First().StorageAccountType);
                Assert.NotNull(imageVersionFromGet.ReplicationStatus);
                Assert.NotNull(imageVersionFromGet.ReplicationStatus.Summary);

                inputImageVersion.PublishingProfile.EndOfLifeDate = DateTime.Now.AddDays(100).Date;
                await (await GalleryImageVersionsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryImageName,
                    galleryImageVersionName, inputImageVersion)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Updated the gallery image version: {0} in gallery image: {1}",
                    galleryImageVersionName, galleryImageName));
                imageVersionFromGet = await GalleryImageVersionsClient.GetAsync(rgName, galleryName,
                    galleryImageName, galleryImageVersionName);
                Assert.NotNull(imageVersionFromGet);
                ValidateGalleryImageVersion(inputImageVersion, imageVersionFromGet);

                Trace.TraceInformation("Listing the gallery image versions");
                List<GalleryImageVersion> listGalleryImageVersionsResult = await (GalleryImageVersionsClient.
                    ListByGalleryImageAsync(rgName, galleryName, galleryImageName)).ToEnumerableAsync();
                Assert.IsTrue(listGalleryImageVersionsResult.Count()==1);
                //Assert.Single(listGalleryImageVersionsResult);
                //Assert.Null(listGalleryImageVersionsResult.NextPageLink);

                await (await GalleryImageVersionsClient.StartDeleteAsync(rgName, galleryName, galleryImageName, galleryImageVersionName)).WaitForCompletionAsync();
                listGalleryImageVersionsResult = await (GalleryImageVersionsClient.
                    ListByGalleryImageAsync(rgName, galleryName, galleryImageName)).ToEnumerableAsync();
                //Assert.Null(listGalleryImageVersionsResult.NextPageLink);
                Trace.TraceInformation(string.Format("Deleted the gallery image version: {0} in gallery image: {1}",
                    galleryImageVersionName, galleryImageName));

                computeManagementTestUtilities.WaitMinutes(5);
                await (await ImagesClient.StartDeleteAsync(rgName, imageName)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the CRP image.");
                await (await VirtualMachinesClient.StartDeleteAsync(rgName, vm.Name)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the virtual machine.");
                await (await GalleryImagesClient.StartDeleteAsync(rgName, galleryName, galleryImageName)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the gallery image.");
                await (await GalleriesClient.StartDeleteAsync(rgName, galleryName)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the gallery.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
                if (vm != null)
                {
                    await (await VirtualMachinesClient.StartDeleteAsync(rgName, vm.Name)).WaitForCompletionAsync();
                }
                await (await ImagesClient.StartDeleteAsync(rgName, imageName)).WaitForCompletionAsync();
            }
        }

        [Test]
        public async Task GalleryApplication_CRUD_Tests()
        {
            string location = ComputeManagementTestUtilities.DefaultLocations;
            EnsureClientsInitialized();
            string rgName = Recording.GenerateAssetName(ResourceGroupPrefix);

            await ResourceGroupsClient.CreateOrUpdateAsync(rgName, new ResourceGroup(location));
            Trace.TraceInformation("Created the resource group: " + rgName);
            string galleryName = Recording.GenerateAssetName(GalleryNamePrefix);
            Gallery gallery = GetTestInputGallery();
            gallery.Location = location;
            await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, gallery)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName, rgName));

            string galleryApplicationName = Recording.GenerateAssetName(GalleryApplicationNamePrefix);
            GalleryApplication inputGalleryApplication = GetTestInputGalleryApplication();
            await (await GalleryApplicationsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryApplicationName, inputGalleryApplication)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Created the gallery application: {0} in gallery: {1}", galleryApplicationName,
                galleryName));

            GalleryApplication galleryApplicationFromGet = await GalleryApplicationsClient.GetAsync(rgName, galleryName, galleryApplicationName);
            Assert.NotNull(galleryApplicationFromGet);
            ValidateGalleryApplication(inputGalleryApplication, galleryApplicationFromGet);

            inputGalleryApplication.Description = "Updated description.";
            await (await GalleryApplicationsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryApplicationName, inputGalleryApplication)).WaitForCompletionAsync();
            Trace.TraceInformation(string.Format("Updated the gallery application: {0} in gallery: {1}", galleryApplicationName,
                galleryName));
            galleryApplicationFromGet = await GalleryApplicationsClient.GetAsync(rgName, galleryName, galleryApplicationName);
            Assert.NotNull(galleryApplicationFromGet);
            ValidateGalleryApplication(inputGalleryApplication, galleryApplicationFromGet);

            await (await GalleryApplicationsClient.StartDeleteAsync(rgName, galleryName, galleryApplicationName)).WaitForCompletionAsync();

            Trace.TraceInformation(string.Format("Deleted the gallery application: {0} in gallery: {1}", galleryApplicationName,
                galleryName));

            await (await GalleriesClient.StartDeleteAsync(rgName, galleryName)).WaitForCompletionAsync();
        }

        [Test]
        [Ignore("need to be skipped")]
        public async Task GalleryApplicationVersion_CRUD_Tests()
        {
            string originalTestLocation = Environment.GetEnvironmentVariable("AZURE_VM_TEST_LOCATION");
            string location = computeManagementTestUtilities.DefaultLocation;
            Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", location);
            EnsureClientsInitialized();
            string rgName = Recording.GenerateAssetName(ResourceGroupPrefix);
            string applicationName = Recording.GenerateAssetName("psTestSourceApplication");
            string galleryName = Recording.GenerateAssetName(GalleryNamePrefix);
            string galleryApplicationName = Recording.GenerateAssetName(GalleryApplicationNamePrefix);

            try
            {
                string applicationMediaLink = await CreateApplicationMediaLink(rgName, "test.txt");

                Assert.False(string.IsNullOrEmpty(applicationMediaLink));
                Trace.TraceInformation(string.Format("Created the source application media link: {0}", applicationMediaLink));

                Gallery gallery = GetTestInputGallery();
                gallery.Location = location;
                await (await GalleriesClient.StartCreateOrUpdateAsync(rgName, galleryName, gallery)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery: {0} in resource group: {1}", galleryName,
                    rgName));
                GalleryApplication inputGalleryApplication = GetTestInputGalleryApplication();
                await (await GalleryApplicationsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryApplicationName, inputGalleryApplication)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery application: {0} in gallery: {1}", galleryApplicationName,
                    galleryName));

                string galleryApplicationVersionName = "1.0.0";
                GalleryApplicationVersion inputApplicationVersion = GetTestInputGalleryApplicationVersion(applicationMediaLink);
                await (await GalleryApplicationVersionsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryApplicationName,
                    galleryApplicationVersionName, inputApplicationVersion)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Created the gallery application version: {0} in gallery application: {1}",
                    galleryApplicationVersionName, galleryApplicationName));

                GalleryApplicationVersion applicationVersionFromGet = await GalleryApplicationVersionsClient.GetAsync(rgName,
                    galleryName, galleryApplicationName, galleryApplicationVersionName);
                Assert.NotNull(applicationVersionFromGet);
                ValidateGalleryApplicationVersion(inputApplicationVersion, applicationVersionFromGet);
                //applicationVersionFromGet = await GalleryApplicationVersionsClient.Get(rgName, galleryName, galleryApplicationName,
                //    galleryApplicationVersionName, ReplicationStatusTypes.ReplicationStatus);
                applicationVersionFromGet = await GalleryApplicationVersionsClient.GetAsync(rgName, galleryName, galleryApplicationName,
                        galleryApplicationVersionName);
                Assert.AreEqual(StorageAccountType.StandardLRS, applicationVersionFromGet.PublishingProfile.StorageAccountType);
                Assert.AreEqual(StorageAccountType.StandardLRS,
                    applicationVersionFromGet.PublishingProfile.TargetRegions.First().StorageAccountType);
                Assert.NotNull(applicationVersionFromGet.ReplicationStatus);
                Assert.NotNull(applicationVersionFromGet.ReplicationStatus.Summary);

                inputApplicationVersion.PublishingProfile.EndOfLifeDate = DateTime.Now.AddDays(100).Date;
                await (await GalleryApplicationVersionsClient.StartCreateOrUpdateAsync(rgName, galleryName, galleryApplicationName,
                    galleryApplicationVersionName, inputApplicationVersion)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Updated the gallery application version: {0} in gallery application: {1}",
                    galleryApplicationVersionName, galleryApplicationName));
                applicationVersionFromGet = await GalleryApplicationVersionsClient.GetAsync(rgName, galleryName,
                    galleryApplicationName, galleryApplicationVersionName);
                Assert.NotNull(applicationVersionFromGet);
                ValidateGalleryApplicationVersion(inputApplicationVersion, applicationVersionFromGet);

                await (await GalleryApplicationVersionsClient.StartDeleteAsync(rgName, galleryName, galleryApplicationName, galleryApplicationVersionName)).WaitForCompletionAsync();
                Trace.TraceInformation(string.Format("Deleted the gallery application version: {0} in gallery application: {1}",
                    galleryApplicationVersionName, galleryApplicationName));

                await (await GalleryApplicationsClient.StartDeleteAsync(rgName, galleryName, galleryApplicationName)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the gallery application.");
                await (await GalleriesClient.StartDeleteAsync(rgName, galleryName)).WaitForCompletionAsync();
                Trace.TraceInformation("Deleted the gallery.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_VM_TEST_LOCATION", originalTestLocation);
            }
        }

        private void ValidateGallery(Gallery galleryIn, Gallery galleryOut)
        {
            Assert.False(string.IsNullOrEmpty(galleryOut.ProvisioningState.ToString()));

            if (galleryIn.Tags != null)
            {
                foreach (KeyValuePair<string, string> kvp in galleryIn.Tags)
                {
                    Assert.AreEqual(kvp.Value, galleryOut.Tags[kvp.Key]);
                }
            }

            if (!string.IsNullOrEmpty(galleryIn.Description))
            {
                Assert.AreEqual(galleryIn.Description, galleryOut.Description);
            }

            Assert.False(string.IsNullOrEmpty(galleryOut?.Identifier?.UniqueName));
        }

        private void ValidateGalleryImage(GalleryImage imageIn, GalleryImage imageOut)
        {
            Assert.False(string.IsNullOrEmpty(imageOut.ProvisioningState.ToString()));

            if (imageIn.Tags != null)
            {
                foreach (KeyValuePair<string, string> kvp in imageIn.Tags)
                {
                    Assert.AreEqual(kvp.Value, imageOut.Tags[kvp.Key]);
                }
            }

            Assert.AreEqual(imageIn.Identifier.Publisher, imageOut.Identifier.Publisher);
            Assert.AreEqual(imageIn.Identifier.Offer, imageOut.Identifier.Offer);
            Assert.AreEqual(imageIn.Identifier.Sku, imageOut.Identifier.Sku);
            Assert.AreEqual(imageIn.Location, imageOut.Location);
            Assert.AreEqual(imageIn.OsState, imageOut.OsState);
            Assert.AreEqual(imageIn.OsType, imageOut.OsType);
            if (imageIn.HyperVGeneration == null)
            {
                Assert.AreEqual(HyperVGeneration.V1, imageOut.HyperVGeneration);
            }
            else
            {
                Assert.AreEqual(imageIn.HyperVGeneration, imageOut.HyperVGeneration);
            }

            if (!string.IsNullOrEmpty(imageIn.Description))
            {
                Assert.AreEqual(imageIn.Description, imageOut.Description);
            }
        }

        private void ValidateGalleryImageVersion(GalleryImageVersion imageVersionIn,
            GalleryImageVersion imageVersionOut)
        {
            Assert.False(string.IsNullOrEmpty(imageVersionOut.ProvisioningState.ToString()));

            if (imageVersionIn.Tags != null)
            {
                foreach (KeyValuePair<string, string> kvp in imageVersionIn.Tags)
                {
                    Assert.AreEqual(kvp.Value, imageVersionOut.Tags[kvp.Key]);
                }
            }

            Assert.AreEqual(imageVersionIn.Location, imageVersionOut.Location);
            Assert.False(string.IsNullOrEmpty(imageVersionOut.StorageProfile.Source.Id),
                "imageVersionOut.PublishingProfile.Source.ManagedImage.Id is null or empty.");
            Assert.NotNull(imageVersionOut.PublishingProfile.EndOfLifeDate);
            Assert.NotNull(imageVersionOut.PublishingProfile.PublishedDate);
            Assert.NotNull(imageVersionOut.StorageProfile);
        }

        private Gallery GetTestInputGallery()
        {
            return new Gallery(galleryHomeLocation)
            {
                Description = "This is a sample gallery description"
            };
        }

        private GalleryImage GetTestInputGalleryImage()
        {
            return new GalleryImage(galleryHomeLocation)
            {
                Identifier = new GalleryImageIdentifier("testPub", "testOffer", "testSku"),
                OsState = OperatingSystemStateTypes.Generalized,
                OsType = OperatingSystemTypes.Windows,
                Description = "This is the gallery image description.",
                HyperVGeneration = null
            };
        }

        private GalleryImageVersion GetTestInputGalleryImageVersion(string sourceImageId)
        {
            return new GalleryImageVersion(galleryHomeLocation)
            {
                PublishingProfile = new GalleryImageVersionPublishingProfile
                {
                    ReplicaCount = 1,
                    StorageAccountType = StorageAccountType.StandardLRS,
                    TargetRegions = new List<TargetRegion> {
                        new TargetRegion(galleryHomeLocation) {
                            RegionalReplicaCount = 1,
                            StorageAccountType = StorageAccountType.StandardLRS
                        }
                    },
                    EndOfLifeDate = DateTime.Today.AddDays(10).Date
                },
                StorageProfile = new GalleryImageVersionStorageProfile
                {
                    Source = new GalleryArtifactVersionSource
                    {
                        Id = sourceImageId
                    }
                }
            };
        }

        private async Task<VirtualMachine> CreateCRPImage(string rgName, string imageName)
        {
            string storageAccountName = Recording.GenerateAssetName("saforgallery");
            string asName = Recording.GenerateAssetName("asforgallery");
            ImageReference imageRef = await GetPlatformVMImage(useWindowsImage: true);
            StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName); // resource group is also created in this method.
            VirtualMachine inputVM = null;
            var returnTwoVM = await CreateVM(rgName, asName, storageAccountOutput, imageRef);
            VirtualMachine createdVM = returnTwoVM.Item1;
            inputVM = returnTwoVM.Item2;
            Image imageInput = new Image(m_location)
            {
                Tags = new Dictionary<string, string>()
                {
                    {"RG", "rg"},
                    {"testTag", "1"},
                },
                StorageProfile = new ImageStorageProfile()
                {
                    OsDisk = new ImageOSDisk(OperatingSystemTypes.Windows, OperatingSystemStateTypes.Generalized)
                    {
                        BlobUri = createdVM.StorageProfile.OsDisk.Vhd.Uri,
                    },
                    ZoneResilient = true
                },
                HyperVGeneration = HyperVGenerationTypes.V1
            };
            await (await ImagesClient.StartCreateOrUpdateAsync(rgName, imageName, imageInput)).WaitForCompletionAsync();
            Image getImage = await ImagesClient.GetAsync(rgName, imageName);
            sourceImageId = getImage.Id;
            return createdVM;
        }

        private async Task<string> CreateApplicationMediaLink(string rgName, string fileName)
        {
            string storageAccountName = Recording.GenerateAssetName("saforgallery");
            string asName = Recording.GenerateAssetName("asforgallery");
            StorageAccount storageAccountOutput = await CreateStorageAccount(rgName, storageAccountName); // resource group is also created in this method.
            string applicationMediaLink = @"https://saforgallery1969.blob.core.windows.net/sascontainer/test.txt\";
            if (Mode == RecordedTestMode.Record)
            {
                var accountKeyResult =  await (StorageAccountsClient.ListKeysAsync(rgName, storageAccountName));
                StorageAccount storageAccount = new StorageAccount(DefaultLocation);
                //StorageAccount storageAccount = new StorageAccount(new StorageCredentials(storageAccountName, accountKeyResult.Body.Key1), useHttps: true);

                //var blobClient = storageAccount.CreateCloudBlobClient();
                BlobContainer container = await BlobContainersClient.GetAsync(rgName, storageAccountName, "sascontainer");

                //byte[] blobContent = Encoding.UTF8.GetBytes("Application Package Test");
                //byte[] bytes = new byte[512]; // Page blob must be multiple of 512
                //System.Buffer.BlockCopy(blobContent, 0, bytes, 0, blobContent.Length);
                //var blobClient = storageAccount.CreateCloudBlobClient();
                //CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
                //bool created = container.CreateIfNotExistsAsync().Result;

                //CloudPageBlob pageBlob = container.GetPageBlobReference(fileName);
                //byte[] blobContent = Encoding.UTF8.GetBytes("Application Package Test");
                //byte[] bytes = new byte[512]; // Page blob must be multiple of 512
                //System.Buffer.BlockCopy(blobContent, 0, bytes, 0, blobContent.Length);
                //pageBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

                //SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
                //sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddDays(-1);
                //sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddDays(2);
                //sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

                ////Generate the shared access signature on the blob, setting the constraints directly on the signature.
                //string sasContainerToken = pageBlob.GetSharedAccessSignature(sasConstraints);

                ////Return the URI string for the container, including the SAS token.
                //applicationMediaLink = pageBlob.Uri + sasContainerToken;
            }
            return applicationMediaLink;
        }

        private GalleryApplication GetTestInputGalleryApplication()
        {
            return new GalleryApplication(ComputeManagementTestUtilities.DefaultLocations)
            {
                Eula = "This is the gallery application EULA.",
                SupportedOSType = OperatingSystemTypes.Windows,
                PrivacyStatementUri = "www.privacystatement.com",
                ReleaseNoteUri = "www.releasenote.com",
                Description = "This is the gallery application description.",
            };
        }

        private GalleryApplicationVersion GetTestInputGalleryApplicationVersion(string applicationMediaLink)
        {
            return new GalleryApplicationVersion(DefaultLocation)
            {
                PublishingProfile = new GalleryApplicationVersionPublishingProfile(
                    new UserArtifactSource("test.zip", applicationMediaLink))
                {
                    ReplicaCount = 1,
                    StorageAccountType = StorageAccountType.StandardLRS,
                    TargetRegions = new List<TargetRegion> {
                        new TargetRegion(DefaultLocation){ RegionalReplicaCount = 1, StorageAccountType = StorageAccountType.StandardLRS }
                    },
                    EndOfLifeDate = DateTime.Today.AddDays(10).Date
                }
            };
        }

        private void ValidateGalleryApplication(GalleryApplication applicationIn, GalleryApplication applicationOut)
        {
            if (applicationIn.Tags != null)
            {
                foreach (KeyValuePair<string, string> kvp in applicationIn.Tags)
                {
                    Assert.AreEqual(kvp.Value, applicationOut.Tags[kvp.Key]);
                }
            }
            Assert.AreEqual(applicationIn.Eula, applicationOut.Eula);
            Assert.AreEqual(applicationIn.PrivacyStatementUri, applicationOut.PrivacyStatementUri);
            Assert.AreEqual(applicationIn.ReleaseNoteUri, applicationOut.ReleaseNoteUri);
            Assert.AreEqual(applicationIn.Location.ToLower(), applicationOut.Location.ToLower());
            Assert.AreEqual(applicationIn.SupportedOSType, applicationOut.SupportedOSType);
            if (!string.IsNullOrEmpty(applicationIn.Description))
            {
                Assert.AreEqual(applicationIn.Description, applicationOut.Description);
            }
        }

        private void ValidateGalleryApplicationVersion(GalleryApplicationVersion applicationVersionIn, GalleryApplicationVersion applicationVersionOut)
        {
            Assert.False(string.IsNullOrEmpty("properties.provisioningState"));

            if (applicationVersionIn.Tags != null)
            {
                foreach (KeyValuePair<string, string> kvp in applicationVersionIn.Tags)
                {
                    Assert.AreEqual(kvp.Value, applicationVersionOut.Tags[kvp.Key]);
                }
            }

            Assert.AreEqual(applicationVersionIn.Location.ToLower(), applicationVersionOut.Location.ToLower());
            Assert.False(string.IsNullOrEmpty(applicationVersionOut.PublishingProfile.Source.MediaLink),
                "applicationVersionOut.PublishingProfile.Source.MediaLink is null or empty.");
            Assert.NotNull(applicationVersionOut.PublishingProfile.EndOfLifeDate);
            Assert.NotNull(applicationVersionOut.PublishingProfile.PublishedDate);
            Assert.NotNull(applicationVersionOut.Id);
        }
    }
}
