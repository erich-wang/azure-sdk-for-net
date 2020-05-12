using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;

namespace Azure.Management.Resource.Tests.Helper
{
    public class Helper
    {
        ///// <summary>
        ///// Reference to the first HTTP handler (which is the start of send HTTP
        ///// pipeline).
        ///// </summary>
        //protected HttpMessageHandler FirstMessageHandler { get; set; }
        ///// <summary>
        ///// Get the HTTP pipelines for the given service client.
        ///// </summary>
        ///// <returns>The client's HTTP pipeline.</returns>
        //public virtual IEnumerable<HttpMessageHandler> HttpMessageHandlers
        //{
        //    get
        //    {
        //        var handler = FirstMessageHandler;

        //        while (handler != null)
        //        {
        //            yield return handler;

        //            DelegatingHandler delegating = handler as DelegatingHandler;
        //            handler = delegating != null ? delegating.InnerHandler : null;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Sets retry policy for the client.
        ///// </summary>
        ///// <param name="retryPolicy">Retry policy to set.</param>
        //public virtual void SetRetryPolicy(RetryPolicy retryPolicy)
        //{
        //    if (retryPolicy == null)
        //    {
        //        retryPolicy = new RetryPolicy<TransientErrorIgnoreStrategy>(0);
        //    }

        //    RetryDelegatingHandler delegatingHandler =
        //        HttpMessageHandlers.OfType<RetryDelegatingHandler>().FirstOrDefault();
        //    if (delegatingHandler != null)
        //    {
        //        delegatingHandler.RetryPolicy = retryPolicy;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(Microsoft.Rest.ClientRuntime.Properties.Resources.ExceptionRetryHandlerMissing);
        //    }
        //}


        /// <summary>
        /// Equality comparison for locations returned by resource management
        /// </summary>
        /// <param name="expected">The expected location</param>
        /// <param name="actual">The actual location returned by resource management</param>
        /// <returns>true if the locations are equivalent, otherwise false</returns>
        public static bool LocationsAreEqual(string expected, string actual)
        {
            bool result = string.Equals(expected, actual, System.StringComparison.OrdinalIgnoreCase);
            if (!result && !string.IsNullOrEmpty(expected))
            {
                string normalizedLocation = expected.ToLower().Replace(" ", null);
                result = string.Equals(normalizedLocation, actual, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}
