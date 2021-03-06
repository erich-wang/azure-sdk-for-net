// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.KubernetesConfiguration.Models
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Extension Instance object.
    /// </summary>
    [Rest.Serialization.JsonTransformation]
    public partial class ExtensionInstance : ProxyResource
    {
        /// <summary>
        /// Initializes a new instance of the ExtensionInstance class.
        /// </summary>
        public ExtensionInstance()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ExtensionInstance class.
        /// </summary>
        /// <param name="id">Resource Id</param>
        /// <param name="name">Resource name</param>
        /// <param name="type">Resource type</param>
        /// <param name="systemData">Top level metadata
        /// https://github.com/Azure/azure-resource-manager-rpc/blob/master/v1.0/common-api-contracts.md#system-metadata-for-all-azure-resources</param>
        /// <param name="extensionType">Type of the Extension, of which this
        /// resource is an instance of.  It must be one of the Extension Types
        /// registered with Microsoft.KubernetesConfiguration by the Extension
        /// publisher.</param>
        /// <param name="autoUpgradeMinorVersion">Flag to note if this instance
        /// participates in auto upgrade of minor version, or not.</param>
        /// <param name="releaseTrain">ReleaseTrain this extension instance
        /// participates in for auto-upgrade (e.g. Stable, Preview, etc.) -
        /// only if autoUpgradeMinorVersion is 'true'.</param>
        /// <param name="version">Version of the extension for this extension
        /// instance, if it is 'pinned' to a specific version.
        /// autoUpgradeMinorVersion must be 'false'.</param>
        /// <param name="scope">Scope at which the extension instance is
        /// installed.</param>
        /// <param name="configurationSettings">Configuration settings, as
        /// name-value pairs for configuring this instance of the
        /// extension.</param>
        /// <param name="configurationProtectedSettings">Configuration settings
        /// that are sensitive, as name-value pairs for configuring this
        /// instance of the extension.</param>
        /// <param name="installState">Status of installation of this instance
        /// of the extension. Possible values include: 'Pending', 'Installed',
        /// 'Failed'</param>
        /// <param name="statuses">Status from this instance of the
        /// extension.</param>
        /// <param name="creationTime">DateLiteral (per ISO8601) noting the
        /// time the resource was created by the client (user).</param>
        /// <param name="lastModifiedTime">DateLiteral (per ISO8601) noting the
        /// time the resource was modified by the client (user).</param>
        /// <param name="lastStatusTime">DateLiteral (per ISO8601) noting the
        /// time of last status from the agent.</param>
        /// <param name="errorInfo">Error information from the Agent - e.g.
        /// errors during installation.</param>
        /// <param name="identity">The identity of the configuration.</param>
        public ExtensionInstance(string id = default(string), string name = default(string), string type = default(string), SystemData systemData = default(SystemData), string location = default(string), string extensionType = default(string), bool? autoUpgradeMinorVersion = default(bool?), string releaseTrain = default(string), string version = default(string), Scope scope = default(Scope), IDictionary<string, string> configurationSettings = default(IDictionary<string, string>), IDictionary<string, string> configurationProtectedSettings = default(IDictionary<string, string>), string installState = default(string), IList<ExtensionStatus> statuses = default(IList<ExtensionStatus>), string creationTime = default(string), string lastModifiedTime = default(string), string lastStatusTime = default(string), ErrorDefinition errorInfo = default(ErrorDefinition), ConfigurationIdentity identity = default(ConfigurationIdentity))
            : base(id, name, type, systemData)
        {
            Location = location;
            ExtensionType = extensionType;
            AutoUpgradeMinorVersion = autoUpgradeMinorVersion;
            ReleaseTrain = releaseTrain;
            Version = version;
            Scope = scope;
            ConfigurationSettings = configurationSettings;
            ConfigurationProtectedSettings = configurationProtectedSettings;
            InstallState = installState;
            Statuses = statuses;
            CreationTime = creationTime;
            LastModifiedTime = lastModifiedTime;
            LastStatusTime = lastStatusTime;
            ErrorInfo = errorInfo;
            Identity = identity;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the location of the configuration.
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets type of the Extension, of which this resource is an
        /// instance of.  It must be one of the Extension Types registered with
        /// Microsoft.KubernetesConfiguration by the Extension publisher.
        /// </summary>
        [JsonProperty(PropertyName = "properties.extensionType")]
        public string ExtensionType { get; set; }

        /// <summary>
        /// Gets or sets flag to note if this instance participates in auto
        /// upgrade of minor version, or not.
        /// </summary>
        [JsonProperty(PropertyName = "properties.autoUpgradeMinorVersion")]
        public bool? AutoUpgradeMinorVersion { get; set; }

        /// <summary>
        /// Gets or sets releaseTrain this extension instance participates in
        /// for auto-upgrade (e.g. Stable, Preview, etc.) - only if
        /// autoUpgradeMinorVersion is 'true'.
        /// </summary>
        [JsonProperty(PropertyName = "properties.releaseTrain")]
        public string ReleaseTrain { get; set; }

        /// <summary>
        /// Gets or sets version of the extension for this extension instance,
        /// if it is 'pinned' to a specific version. autoUpgradeMinorVersion
        /// must be 'false'.
        /// </summary>
        [JsonProperty(PropertyName = "properties.version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets scope at which the extension instance is installed.
        /// </summary>
        [JsonProperty(PropertyName = "properties.scope")]
        public Scope Scope { get; set; }

        /// <summary>
        /// Gets or sets configuration settings, as name-value pairs for
        /// configuring this instance of the extension.
        /// </summary>
        [JsonProperty(PropertyName = "properties.configurationSettings")]
        public IDictionary<string, string> ConfigurationSettings { get; set; }

        /// <summary>
        /// Gets or sets configuration settings that are sensitive, as
        /// name-value pairs for configuring this instance of the extension.
        /// </summary>
        [JsonProperty(PropertyName = "properties.configurationProtectedSettings")]
        public IDictionary<string, string> ConfigurationProtectedSettings { get; set; }

        /// <summary>
        /// Gets or sets status of installation of this instance of the
        /// extension. Possible values include: 'Pending', 'Installed',
        /// 'Failed'
        /// </summary>
        [JsonProperty(PropertyName = "properties.installState")]
        public string InstallState { get; set; }

        /// <summary>
        /// Gets or sets status from this instance of the extension.
        /// </summary>
        [JsonProperty(PropertyName = "properties.statuses")]
        public IList<ExtensionStatus> Statuses { get; set; }

        /// <summary>
        /// Gets dateLiteral (per ISO8601) noting the time the resource was
        /// created by the client (user).
        /// </summary>
        [JsonProperty(PropertyName = "properties.creationTime")]
        public string CreationTime { get; private set; }

        /// <summary>
        /// Gets dateLiteral (per ISO8601) noting the time the resource was
        /// modified by the client (user).
        /// </summary>
        [JsonProperty(PropertyName = "properties.lastModifiedTime")]
        public string LastModifiedTime { get; private set; }

        /// <summary>
        /// Gets dateLiteral (per ISO8601) noting the time of last status from
        /// the agent.
        /// </summary>
        [JsonProperty(PropertyName = "properties.lastStatusTime")]
        public string LastStatusTime { get; private set; }

        /// <summary>
        /// Gets error information from the Agent - e.g. errors during
        /// installation.
        /// </summary>
        [JsonProperty(PropertyName = "properties.errorInfo")]
        public ErrorDefinition ErrorInfo { get; private set; }

        /// <summary>
        /// Gets or sets the identity of the configuration.
        /// </summary>
        [JsonProperty(PropertyName = "identity")]
        public ConfigurationIdentity Identity { get; set; }

    }
}
