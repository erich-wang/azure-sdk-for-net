// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace Azure.Management.Resource.Models
{
    /// <summary> The policy definition reference. </summary>
    public partial class PolicyDefinitionReference
    {
        /// <summary> Initializes a new instance of PolicyDefinitionReference. </summary>
        /// <param name="policyDefinitionId"> The ID of the policy definition or policy set definition. </param>
        public PolicyDefinitionReference(string policyDefinitionId)
        {
            if (policyDefinitionId == null)
            {
                throw new ArgumentNullException(nameof(policyDefinitionId));
            }

            PolicyDefinitionId = policyDefinitionId;
        }

        /// <summary> Initializes a new instance of PolicyDefinitionReference. </summary>
        /// <param name="policyDefinitionId"> The ID of the policy definition or policy set definition. </param>
        /// <param name="parameters"> The parameter values for the referenced policy rule. The keys are the parameter names. </param>
        /// <param name="policyDefinitionReferenceId"> A unique id (within the policy set definition) for this policy definition reference. </param>
        /// <param name="groupNames"> The name of the groups that this policy definition reference belongs to. </param>
        internal PolicyDefinitionReference(string policyDefinitionId, IDictionary<string, ParameterValuesValue> parameters, string policyDefinitionReferenceId, IList<string> groupNames)
        {
            PolicyDefinitionId = policyDefinitionId;
            Parameters = parameters;
            PolicyDefinitionReferenceId = policyDefinitionReferenceId;
            GroupNames = groupNames;
        }

        /// <summary> The ID of the policy definition or policy set definition. </summary>
        public string PolicyDefinitionId { get; set; }
        /// <summary> The parameter values for the referenced policy rule. The keys are the parameter names. </summary>
        public IDictionary<string, ParameterValuesValue> Parameters { get; set; }
        /// <summary> A unique id (within the policy set definition) for this policy definition reference. </summary>
        public string PolicyDefinitionReferenceId { get; set; }
        /// <summary> The name of the groups that this policy definition reference belongs to. </summary>
        public IList<string> GroupNames { get; set; }
    }
}
