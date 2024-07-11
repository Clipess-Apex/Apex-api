using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;



namespace Clipess.DBClient.Repositories
{
    public class EFProjectRepository : IProjectRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFProjectRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<Project> GetProjects()
        {
            return DbContext.Projects.Where(e => !e.Deleted).ToList();
        }
        public async Task<Project> GetProject(int ProjectId)
        {
            return await DbContext.Projects
                 .FirstOrDefaultAsync(e => e.ProjectId == ProjectId && !e.Deleted);
        }

        public List<FormProject> GetFormProjects()
        {
            return DbContext.Projects
                .Where(x => !x.Deleted && !(x.ProjectStatusId == 3))
                .Select(x => new FormProject
            {
                ProjectId = x.ProjectId,
                ProjectName = x.ProjectName
            })
            .ToList();
        }



        public void PostProject(Project project)
        {
            var createdProject = new Project
            {
                ProjectName = project.ProjectName,
                ClientId = project.ClientId,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Budget = project.Budget,
                ProjectStatusId = 1,
                DocumentURL = project.DocumentURL,
                Deleted = false,
                CreatedDate = DateTime.UtcNow
            };

            DbContext.Projects.Add(createdProject);
            DbContext.SaveChanges();
        }

        public void DeleteProject(Project project)
        {
            // Assuming DbContext is your instance of DbContext
            var existingProject = DbContext.Projects.FirstOrDefault(d => d.ProjectId == project.ProjectId && !d.Deleted);

            if (existingProject != null)
            {

                // Update existing project properties with new values
                existingProject.Deleted = true;

                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Project not found", nameof(project));
            }
        }

        public void UpdateProjectStatus(Project project)
        {

            // Assuming DbContext is your instance of DbContext
            var existingProject = DbContext.Projects.FirstOrDefault(x => x.ProjectId == project.ProjectId && !x.Deleted);

            if (existingProject != null)
            {
                // Update existing task properties with new values
                existingProject.ProjectStatusId++;
                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Project not found", nameof(existingProject));
            }

        }
        public void SaveProject(Project project)
        {
            
            var editProject = DbContext.Projects.FirstOrDefault(d => d.ProjectId == project.ProjectId && !d.Deleted);

            if (editProject != null)
            {
                // Update existing project properties with new values
                editProject.ProjectName = project.ProjectName;
                editProject.ClientId = project.ClientId;
                editProject.Description = project.Description;
                editProject.StartDate = project.StartDate;
                editProject.EndDate = project.EndDate;
                editProject.Budget = project.Budget;
                editProject.ProjectStatusId = project.ProjectStatusId;
                editProject.DocumentURL = project.DocumentURL;
                editProject.Deleted = false;
                editProject.UpdatedDate = DateTime.UtcNow;

                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Project not found", nameof(project));
            }

            

        }


    }

    public class EFTaskRepository : ITaskRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFTaskRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<ProjectTask> GetTasks()
        {
            return DbContext.ProjectTasks.Where(e => !e.Deleted).ToList(); 
        }

        public List<ProjectTask> GetEmployeeTasks()
        {
            return DbContext.ProjectTasks.Where(e => !e.Deleted && e.Assigned).ToList();
        }
        public List<FormProjectTask> GetFormTasks()
        {
            return DbContext.ProjectTasks
                .Where(x => !x.Deleted)
                .Select(x => new FormProjectTask
                {
                    TaskId = x.TaskId,
                    TaskName = x.TaskName
                })
            .ToList();
        }
        public List<FormUser> GetFormUsers()
        {
            return DbContext.Employees.
                Where(x=>!x.Deleted)
                .Select(x => new FormUser
                {
                    EmployeeID = x.EmployeeID,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
            .ToList();
        }

        public async  Task<FormUser> GetFormUser(int EmployeeID)
        {
            var user = await DbContext.Users
                 .FirstOrDefaultAsync(e => e.EmployeeID == EmployeeID);
            return user;
        }

        public async Task<ProjectTask> GetTask(int TaskId)
        {
            return await DbContext.ProjectTasks
                 .FirstOrDefaultAsync(e => e.TaskId == TaskId && !e.Deleted);
        }

        public void PostTask(ProjectTask task)
        {

            var createdTask = new ProjectTask
            {
                TaskName = task.TaskName,
                TeamId = task.TeamId,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                ProjectId = task.ProjectId,
                Assigned = false,
                CreatedDate = DateTime.UtcNow,
                Deleted = false,
                TaskStatusId =1,
                SelectedUsers= task.SelectedUsers,
            };
            DbContext.ProjectTasks.Add(createdTask);
            DbContext.SaveChanges();
        }

        public void SaveTask(ProjectTask task)
        {
            
            var editTask = DbContext.ProjectTasks.FirstOrDefault(d => d.TaskId == task.TaskId);

            if (editTask != null)
            {
                // Update existing task properties with new values
                editTask.TaskName = task.TaskName;
                editTask.TeamId = task.TeamId;
                editTask.StartDate = task.StartDate;
                editTask.EndDate = task.EndDate;
                editTask.ProjectId = task.ProjectId;
                editTask.UpdatedDate = DateTime.UtcNow;
                DbContext.SaveChanges(); ;
            }
            else
            {
                throw new ArgumentException("Task not found", nameof(task));
            }

            
                
        }
        public void AssignTask(ProjectTask task)
        {

            // Assuming DbContext is your instance of DbContext
            var existingTask = DbContext.ProjectTasks.FirstOrDefault(x => x.TaskId == task.TaskId && !x.Deleted);

            if (existingTask != null)
            {
                // Update existing task properties with new values
                existingTask.Assigned = true;
                existingTask.AssignedDate = DateTime.UtcNow;


                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Task not found", nameof(existingTask));
            }

        }

        public void UpdateTaskStatus(ProjectTask task)
        {

            // Assuming DbContext is your instance of DbContext
            var existingTask = DbContext.ProjectTasks.FirstOrDefault(x => x.TaskId == task.TaskId  && !x.Deleted);

            if (existingTask != null)
            {
                // Update existing task properties with new values
                existingTask.TaskStatusId ++;
                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Task not found", nameof(existingTask));
            }

        } 

        public void DeleteTask(ProjectTask task)
        {

            // Assuming DbContext is your instance of DbContext
            var existingTask = DbContext.ProjectTasks.FirstOrDefault(x => x.TaskId == task.TaskId && !x.Deleted);

            if (existingTask != null)
            {
                // Update existing task properties with new values
                existingTask.Deleted = true;
                existingTask.DeletedDate = DateTime.UtcNow;


                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Task not found", nameof(existingTask));
            }

        }
    }

    public class EFTeamRepository : ITeamRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFTeamRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<Team> GetTeams()
        {
            return DbContext.Teams.Where(e => !e.Deleted).ToList(); ;
        }

        public async Task<Team> GetTeam(int TeamId)
        {
            return await DbContext.Teams
                 .FirstOrDefaultAsync(e => e.TeamId == TeamId && !e.Deleted);
        }
        public List<FormTeam> GetFormTeams()
        {
            return DbContext.Teams
                .Where(x => !x.Deleted)
                .Select(x => new FormTeam
                {
                    TeamId = x.TeamId,
                    TeamName = x.TeamName
                })
            .ToList();
        }
        public void  PostTeam(Team team)
        {

            var createdTeam = new Team
            {
                TeamName = team.TeamName,
                Description = team.Description,
                CreatedDate = DateTime.UtcNow,
                Deleted = false,
            };
            DbContext.Teams.Add(createdTeam);
            DbContext.SaveChanges();
        }

        public void SaveTeam(Team team)
        {
            
            var editTeam = DbContext.Teams.FirstOrDefault(d => d.TeamId == team.TeamId);
            if (editTeam != null)
            {
                // Update existing team properties with new values
                editTeam.TeamName = team.TeamName;
                editTeam.Description = team.Description;
                editTeam.UpdatedDate = DateTime.UtcNow;
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Team not found", nameof(team));
            }
        }

        public void DeleteTeam(Team team)
        {

            // Assuming DbContext is your instance of DbContext
            var existingTeam = DbContext.Teams.FirstOrDefault(x => x.TeamId == team.TeamId && !x.Deleted);

            if (existingTeam != null)
            {
                // Update existing team properties with new values
                existingTeam.Deleted = true;
                existingTeam.DeletedDate = DateTime.UtcNow;


                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Team not found", nameof(existingTeam));
            }

        }
    }

    public class EFClientRepository : IClientRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFClientRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<Client> GetClients()
        {
            return DbContext.Clients.Where(e => !e.Deleted).ToList();
        }

        public async Task<Client> GetClient(int ClientId)
        {
            return await DbContext.Clients
                 .FirstOrDefaultAsync(e => e.ClientId == ClientId && !e.Deleted);
        }
        public List<FormClient> GetFormClients()
        {
            return DbContext.Clients
                .Where(x => !x.Deleted)
                .Select(x => new FormClient
                {
                    ClientId = x.ClientId,
                    ClientName = x.ClientName
                })
            .ToList();
        }

        public void PostClient(Client client)
        {
            var createdClient = new Client
            {
                ClientName = client.ClientName,
                TelephoneNo = client.TelephoneNo,
                Address = client.Address,
                Email = client.Email,
                CreatedDate = DateTime.UtcNow
            };
            DbContext.Clients.Add(createdClient);
            DbContext.SaveChanges();
        }

        public void SaveClient(Client client)
        {
            
            var editClient = DbContext.Clients.FirstOrDefault(d => d.ClientId == client.ClientId && !d.Deleted);

            if (editClient != null)
            {
                
                editClient.ClientName = client.ClientName;
                editClient.TelephoneNo = client.TelephoneNo;
                editClient.Address = client.Address;
                editClient.Email = client.Email;
                editClient.UpdatedDate = DateTime.UtcNow;
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Client not found", nameof(client));
            }

            
        }

        public void DeleteClient(Client client)
        {

            // Assuming DbContext is your instance of DbContext
            var existingClient = DbContext.Clients.FirstOrDefault( x => x.ClientId == client.ClientId && !x.Deleted);

            if (existingClient != null)
            {
                // Update existing client properties with new values
                existingClient.Deleted = true;
                existingClient.DeletedDate= DateTime.UtcNow;


                // Save changes to the database
                DbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Client not found", nameof(existingClient));
            }
            
        }
    }
}
