// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace Azure.Management.Resource.Models
{
    /// <summary> Plan for the resource. </summary>
    public partial class Plan
    {
        /// <summary> Initializes a new instance of Plan. </summary>
        public Plan()
        {
        }

        /// <summary> Initializes a new instance of Plan. </summary>
        /// <param name="name"> Gets or sets the plan ID. </param>
        /// <param name="publisher"> Gets or sets the publisher ID. </param>
        /// <param name="product"> Gets or sets the offer ID. </param>
        /// <param name="promotionCode"> Gets or sets the promotion code. </param>
        internal Plan(string name, string publisher, string product, string promotionCode)
        {
            Name = name;
            Publisher = publisher;
            Product = product;
            PromotionCode = promotionCode;
        }

        /// <summary> Gets or sets the plan ID. </summary>
        public string Name { get; set; }
        /// <summary> Gets or sets the publisher ID. </summary>
        public string Publisher { get; set; }
        /// <summary> Gets or sets the offer ID. </summary>
        public string Product { get; set; }
        /// <summary> Gets or sets the promotion code. </summary>
        public string PromotionCode { get; set; }
    }
}
