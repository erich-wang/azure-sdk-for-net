// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.ProviderHub.Models
{
    using System.Linq;

    public partial class ResourceTypeRegistrationPropertiesIdentityManagement : IdentityManagementProperties
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ResourceTypeRegistrationPropertiesIdentityManagement class.
        /// </summary>
        public ResourceTypeRegistrationPropertiesIdentityManagement()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ResourceTypeRegistrationPropertiesIdentityManagement class.
        /// </summary>
        /// <param name="type">Possible values include: 'NotSpecified',
        /// 'SystemAssigned', 'UserAssigned', 'Actor',
        /// 'DelegatedResourceIdentity'</param>
        public ResourceTypeRegistrationPropertiesIdentityManagement(string type = default(string), string applicationId = default(string))
            : base(type, applicationId)
        {
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

    }
}
