// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;

namespace Azure.Management.Storage.Models
{
    /// <summary> Sets the CORS rules. You can include up to five CorsRule elements in the request. </summary>
    public partial class CorsRules
    {
        /// <summary> Initializes a new instance of CorsRules. </summary>
        public CorsRules()
        {
        }

        /// <summary> Initializes a new instance of CorsRules. </summary>
        /// <param name="rules"> The List of CORS rules. You can include up to five CorsRule elements in the request. </param>
        internal CorsRules(IList<CorsRule> rules)
        {
            Rules = rules;
        }
    }
}
