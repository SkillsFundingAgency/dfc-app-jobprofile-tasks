namespace DFC.App.JobProfileTasks.FunctionalTests.Support.Interface
{
    internal interface IGeneralSupport
    {
        string RandomString(int length);

        byte[] ConvertObjectToByteArray(object obj);

        T GetResource<T>(string resourceName);
    }
}
