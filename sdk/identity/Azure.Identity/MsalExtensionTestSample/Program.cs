using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Identity;

namespace MsalExtensionTestSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string PowerShellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

            var options = new DeviceCodeCredentialOptions();
            options.EnablePersistentCache = true;
            options.ClientId = PowerShellClientId;

            var credential = new DeviceCodeCredential((code, cancelToken) => VerifyDeviceCode(code, cancelToken), options);
            var record = credential.Authenticate();
        }

        private static Task VerifyDeviceCode(DeviceCodeInfo code, CancellationToken token)
        {
            Console.WriteLine(code.UserCode);
            return Task.CompletedTask;
        }

    }
}
