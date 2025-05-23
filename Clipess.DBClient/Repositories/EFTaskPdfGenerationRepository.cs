﻿using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using System.Threading.Tasks;


namespace Clipess.DBClient.Repositories
{
    public class EFTaskPdfGenerationRepository : ITaskPdfGenerationRepository
    {
        public EFDbContext _DbContext { get; set; }

        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        public EFTaskPdfGenerationRepository(EFDbContext dbContext, ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _DbContext = dbContext;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<List<ProjectTaskPdf>> GetTaskReport(List<int> taskId,int EmployeeID)
        {
            var finalResultsArray = new List<ProjectTaskPdf >();
            foreach (var item in taskId)
            {
                var getProjects =  _projectRepository.GetFormProjects();
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

                string ProjectName = "";

                foreach (var data in getProjects)
                {
                    if (data.ProjectId == getTask.ProjectId)
                    {
                        ProjectName = data.ProjectName;
                    }
                }


                var finalResults = new ProjectTaskPdf
                {
                    TaskId = getTask.TaskId,
                    TaskName = getTask.TaskName,
                    Project = ProjectName,
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
