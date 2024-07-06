using Clipess.DBClient.EntityModels;


namespace Clipess.DBClient.Contracts
{
    public interface ITaskPdfGenerationRepository
    {
        Task<List<ProjectTaskPdf>> GetTaskReport(List<int> taskId, int EmployeeID);
    }
}
