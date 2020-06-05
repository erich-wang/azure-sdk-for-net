﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Core;

namespace Azure.Management.Resources201705.Models
{
    public partial class DeploymentProperties
    {
        [CodeGenMember("Template")]
        internal JsonElement TemplateJson { get; set; }

        [CodeGenMember("Parameters")]
        internal JsonElement ParametersJson { get; set; }

        /// <summary>
        /// Template
        /// </summary>
        public string Template
        {
            get => TemplateJson.ToString();
            set => TemplateJson = JsonDocument.Parse(value).RootElement;
        }

        /// <summary>
        /// Parameters
        /// </summary>
        public string Parameters
        {
            get => ParametersJson.ToString();
            set => ParametersJson = JsonDocument.Parse(value).RootElement;
        }
    }
}
