using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IProjectRepository
    {
        List<Project> GetProjects();
        Task<Project> GetProject(int ProjectId);
        List<FormProject> GetFormProjects();
        void PostProject(Project project);
        void SaveProject(Project project);
        void UpdateProjectStatus(Project project);
        void DeleteProject(Project project);
    }

    public interface ITaskRepository
    {
        List<ProjectTask> GetTasks();
        List<ProjectTask> GetEmployeeTasks();
        Task<ProjectTask> GetTask(int TaskId);
        List<FormProjectTask> GetFormTasks();
        List<FormUser> GetFormUsers();
        Task<FormUser> GetFormUser(int EmployeeID);
        void PostTask(ProjectTask task);
        void SaveTask(ProjectTask task);
        void AssignTask(ProjectTask task);
        void UpdateTaskStatus(ProjectTask task);
        void DeleteTask(ProjectTask task);

        
    }

    public interface ITeamRepository
    {
        List<Team> GetTeams();
        Task<Team> GetTeam(int TeamId);
        List<FormTeam> GetFormTeams();
        void PostTeam(Team team);
        void SaveTeam(Team team);
        void DeleteTeam(Team team);
    }

    public interface IClientRepository
    {
        List<Client> GetClients();
        Task<Client> GetClient(int ClientId);
        List<FormClient> GetFormClients();
        void PostClient(Client client);
        void SaveClient(Client client);
        void DeleteClient(Client client);
    }
}
