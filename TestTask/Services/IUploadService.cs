namespace TestTask.Services
{
    public interface IUploadService 
    {
         Task Upload(string json, string UserID);
    }
}
