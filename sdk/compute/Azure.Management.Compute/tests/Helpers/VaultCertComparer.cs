// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azure.Management.Resources;
using Azure.Management.Resources.Models;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using System.Reflection;
using System.Threading;
using Azure.Management.Compute.Tests;
using Azure.Management.Compute;
using Azure.Management.Compute.Models;
using System.Text.Json;
using System.Net;
//using Newtonsoft.Json;

namespace Azure.Management.Compute.Tests
{
    internal class VaultCertComparer : IEqualityComparer<VaultCertificate>
    {
        public bool Equals(VaultCertificate cert1, VaultCertificate cert2)
        {
            if (cert1.CertificateStore == cert2.CertificateStore && cert1.CertificateUrl == cert2.CertificateUrl)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(VaultCertificate Cert)
        {
            return Cert.CertificateUrl.ToLower().GetHashCode();
        }
    }
}
