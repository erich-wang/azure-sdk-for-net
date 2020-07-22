# Develop Tests for .NET

## This document shows you how to develop .NET SDK test cases.

---

## Getting Started

### Prerequisites

You will need the following values to authenticate to Azure

-   **Subscription ID**
-   **Client ID**
-   **Client Secret**
-   **Tenant ID**

### Setting Environment Variables

After you obtained the values, you need to set the following values as your environment variables

For Track 1 Tests:

* `TEST_CSM_ORGID_AUTHENTICATION` : SubscriptionId=<-Subscription ID->;ServicePrincipal=<-Client ID->;ServicePrincipalSecret=<-Client Secret->;AADTenant=<-Tenant ID->;Environment=Prod;HttpRecorderMode=<-Record-> or <-Playback->;
* `AZURE_TEST_MODE` : `Record` or `Playback`
* `SUBSCRIPTION_ID`

For Track 2 Tests:

* `AZURE_TEST_MODE` : `Record` or `Playback`
* `AZURE_USER_NAME` : Your Azure Account
* `CLIENT_ID`
* `CLIENT_SECRET`
* `TENANT_ID`
* `SUBSCRIPTION_ID`

### Start Coding Tests
Here are some tips for the test case coding  
- Add Configuration in test.csproj if you want to use other services.
`<TestHelperProjects>（Rbac1.6;Resources201705;Compute201912;Network202004;Storage201906）</TestHelperProjects>`
- Create a test base class inherit `ManagementRecordedTestBase`.
- Create a `Initialize` method in base class which prepares the required management client.
- Each test class constructor will have a `isAsync` parameter to run test under Async method or sync method.  
- Use NUnit instead of XUnit.
  - Create a `[Setup]` method in each test class, call `Initialize` method to setup test environment.
  - Create a `[TearDown]` method in each test class, call `CleanupResourceGroupsAsync` method to cleanup test rsources.
- Use `WaitForCompletionAsync` if the operation returns `Operation<xx>`.  
- Make sure every async method get its `await`.
- Some common methods may already placed in `Azure.Core.TestFramework.RecordedTestBase`, check it first.

### Test Mode
- Record Tests  
If you set the test mode to `Record`, the test framework will automaticly record the test result to testcase.json file and put them under  
`<sdk repo>\artifacts\bin\Azure.ResourceManager.RP.Tests\Debug\<>\SessionRecords`. Copy the json file to test project for further use.
- Playback Tests  
If you set the test mode to `Playback`, the testframework will use the recorded file to mock the test.  