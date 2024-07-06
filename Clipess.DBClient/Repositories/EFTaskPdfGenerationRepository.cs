using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using System.Threading.Tasks;


namespace Clipess.DBClient.Repositories
{
    public class EFTaskPdfGenerationRepository : ITaskPdfGenerationRepository
    {
        public EFDbContext _DbContext { get; set; }

        private readonly ITaskRepository _taskRepository;
        public EFTaskPdfGenerationRepository(EFDbContext dbContext, ITaskRepository taskRepository)
        {
            _DbContext = dbContext;
            _taskRepository = taskRepository;
        }

        public async Task<List<ProjectTaskPdf>> GetTaskReport(List<int> taskId,int EmployeeID)
        {
            var finalResultsArray = new List<ProjectTaskPdf >();
            foreach (var item in taskId)
            {

                var getTask = await _taskRepository.GetTask(item);
                if (getTask == null)
                {
                    //throw new Exception("No Monthly Records Availavle");
                    return null;
                }
                string TaskStatus="To Do";

                if (getTask.TaskStatusId == 1)
                {
                    TaskStatus = "To Do";
                } else if (getTask.TaskStatusId == 2)
                {
                    TaskStatus = "In Progress";
                }else if (getTask.TaskStatusId == 3)
                {
                    TaskStatus = "Done";
                }
                string assignedDate = "";
                if(getTask.AssignedDate != null)
                {
                     assignedDate = getTask.AssignedDate.Value.ToString("yyyy-MM-dd");
                }



                var finalResults = new ProjectTaskPdf
                {
                    TaskId = getTask.TaskId,
                    TaskName = getTask.TaskName,
                    StartDate = getTask.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = getTask.EndDate.ToString("yyyy-MM-dd"),
                    AssignedDate = assignedDate,
                    CreatedDate = getTask.CreatedDate.ToString("yyyy-MM-dd"),
                    TaskStatus = TaskStatus,
                    
                };
                finalResultsArray.Add(finalResults);
            }
            return finalResultsArray;
            

            
        }

        
}
}
