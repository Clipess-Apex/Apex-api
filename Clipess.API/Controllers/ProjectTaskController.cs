using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Clipess.API.Controllers
{

    //PROJECT CONTROLLER

    [ApiController]
    [Route("api/project")]
    
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
     

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [Route("GetProjects")]
        [HttpGet]
        public async Task<ActionResult> GetProjects()
        {
            try
            {
                var project = _projectRepository.GetProjects();
                if (project != null)
                {
                    return Ok(project);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("GetProject")]
        [HttpGet]
        public async Task<ActionResult<Project>> GetProject([FromQuery] int ProjectId)
        {
            try
            {
                var result = await _projectRepository.GetProject(ProjectId);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("GetFormProjects")]
        [HttpGet]
        public async Task<ActionResult<List<FormProject>>> GetFormProjects()
        {
            try
            {
                var result = _projectRepository.GetFormProjects();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("PostProject")]
        [HttpPost]
        public async Task<IActionResult> PostProject([FromBody] Project project)
        {
            try
            {
                if (project == null)
                {
                    return BadRequest("Project data is null");
                }

                _projectRepository.PostProject(project);

                return Ok(new { message = "Project added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create project");
            }
        }

        [Route("PutProject")]
        [HttpPut]
        public async Task<ActionResult<Project>> SaveProject([FromBody] Project project)
        {
            try
            {
                _projectRepository.SaveProject(project);

                return Ok(new { message = "Project updated successfully." });


            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
        [Route("UpdateProjectStatus")]
        [HttpPut]
        public async Task<ActionResult<ProjectTask>> UpdateProjectStatus([FromBody] Project project)
        {
            try
            {
                _projectRepository.UpdateProjectStatus(project);
                return Ok(new { message = "Project status updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [Route("DeleteProject")]
        [HttpDelete]
        public async Task<ActionResult<Project>> DeleteProject([FromQuery] int ProjectId)
        {
            try
            {
                var projectToDelete = await _projectRepository.GetProject(ProjectId);

                if (projectToDelete == null)
                {
                    return NotFound($"Project with Id = {ProjectId} not found");
                }

                _projectRepository.DeleteProject(projectToDelete);
                return Ok(new { message = "Project deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

    }

    //TASK CONTROLLER

    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }


        [Route("GetTasks")]
        [HttpGet]
        public async Task<ActionResult> GetTasks()
        {
            try
            {
                var task = _taskRepository.GetTasks();
                if (task != null)
                {
                    return Ok(task);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("GetEmployeeTasks")]
        [HttpGet]
        public async Task<ActionResult> GetEmployeeTasks()
        {
            try
            {
                var task = _taskRepository.GetEmployeeTasks();
                if (task != null)
                {
                    return Ok(task);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("GetTask")]
        [HttpGet]
        public async Task<ActionResult<ProjectTask>> GetTask([FromQuery] int TaskId)
        {
            try
            {
                var result = await _taskRepository.GetTask(TaskId);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }



        
        [Route("GetFormTasks")]
        [HttpGet]
        public async Task<ActionResult<List<FormProjectTask>>> GetFormTasks()
        {
            try
            {
                var result = _taskRepository.GetFormTasks();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("GetFormUsers")]
        [HttpGet]
        public async Task<ActionResult<List<FormUser>>> GetFormUsers()
        {
            try
            {
                var result = _taskRepository.GetFormUsers();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("PostTask")]
        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] ProjectTask task)
        {
            try
            {
                if (task == null)
                {
                    return BadRequest("Task data is null");
                }

                // Save the task to the repository or database
                _taskRepository.PostTask(task); 
                return Ok(new { message = "Task added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create task");
            }
        }

        [Route("PutTask")]
        [HttpPut]
        public async Task<ActionResult<ProjectTask>> SaveTask([FromQuery] int TaskId,[FromBody] ProjectTask task)
        {
            try
            {
                if (TaskId != task.TaskId)
                {
                    return BadRequest("Task ID mismatch");
                }

                var taskToUpdate = await _taskRepository.GetTask(TaskId);

                if (taskToUpdate == null)
                {
                    return NotFound($"Task with Id = {TaskId} not found");
                }

                _taskRepository.SaveTask(task);
                return Ok(new { message = "Task updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }
        [Route("AssignTask")]
        [HttpPut]
        public async Task<ActionResult<ProjectTask>> AssignTask([FromBody] ProjectTask task)
        {
            try
            {

                _taskRepository.AssignTask(task);
                return Ok(new { message = "Task assigned successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [Route("UpdateTaskStatus")]
        [HttpPut]
        public async Task<ActionResult<ProjectTask>> UpdateTaskStatus([FromBody] ProjectTask task)
        {
            try
            {

                _taskRepository.UpdateTaskStatus(task);
                return Ok(new { message = "Task Status updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        } 

        [Route("DeleteTask")]
        [HttpDelete]
        public async Task<ActionResult<ProjectTask>> DeleteTask([FromQuery] int TaskId)
        {
            try
            {
                var taskToDelete = await _taskRepository.GetTask(TaskId);

                if (taskToDelete == null)
                {
                    return NotFound($"Task with Id = {TaskId} not found");
                }

                _taskRepository.DeleteTask(taskToDelete);
                return Ok(new { message = "Task deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }

    //TEAM CONTROLLER

    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;
       

        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [Route("GetTeams")]
        [HttpGet]
        public async Task<ActionResult> GetTeams()
        {
            try
            {
                var team = _teamRepository.GetTeams();
                if (team != null)
                {
                    return Ok(team);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route ("GetTeam")]
        [HttpGet]
        public async Task<ActionResult<Team>> GetTeam([FromQuery] int TeamId)
        {
            try
            {
                var result = await _teamRepository.GetTeam(TeamId);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("GetFormTeams")]
        [HttpGet]
        public async Task<ActionResult<List<FormTeam>>> GetFormTeams()
        {
            try
            {
                var result = _teamRepository.GetFormTeams();

                if (result == null )
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("PostTeam")]
        [HttpPost]
        public async Task<IActionResult> PostTeam([FromBody] Team team)
        {
            try
            {
                if (team == null)
                {
                    return BadRequest("Team data is null");
                }

                // Save the team to the repository or database
                _teamRepository.PostTeam(team);
                return Ok(new { message = "Team added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create team");
            }
        }

        [Route("PutTeam")]
        [HttpPut]
        public async Task<ActionResult<Team>> SaveTeam([FromQuery] int TeamId,[FromBody] Team team)
        {
            try
            {
                

                var teamToUpdate = await _teamRepository.GetTeam(TeamId);

                if (teamToUpdate == null)
                {
                    return NotFound($"Team with Id = {TeamId} not found");
                }

                _teamRepository.SaveTeam(team);
                return Ok(new { message = "Team updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [Route("DeleteTeam")]
        [HttpDelete]
       
        public async Task<ActionResult<Team>> DeleteTeam([FromQuery] int TeamId)
        {
            try
            {
                var teamToDelete = await _teamRepository.GetTeam(TeamId);

                if (teamToDelete == null)
                {
                    return NotFound($"Team with Id = {TeamId} not found");
                }

                _teamRepository.DeleteTeam(teamToDelete);
                return Ok(new { message = "Team deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }

    //CLIENT CONTROLLER

    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
     

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [Route("GetClients")]
        [HttpGet]
        public async Task<ActionResult> GetClients()
        {
            try
            {
                var client = _clientRepository.GetClients();
                if (client != null)
                {
                    return Ok(client);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("GetClient")]
        [HttpGet]
        public async Task<ActionResult<Client>> GetClient([FromQuery] int ClientId)
        {
            try
            {
                var result =  await _clientRepository.GetClient(ClientId);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("GetFormClients")]
        [HttpGet]
        public async Task<ActionResult<List<FormClient>>> GetFormClients()
        {
            try
            {
                var result = _clientRepository.GetFormClients();

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [Route("PostClient")]
        [HttpPost]
        public async Task<IActionResult> PostClient([FromBody] Client client)
        {
            try
            {
                if (client == null)
                {
                    return BadRequest("Client data is null");
                }

                // Save the client to the repository or database
                _clientRepository.PostClient(client);
                return Ok(new { message = "Client added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create Client");
            }
        }


        [Route("PutClient")]
        [HttpPut]
        public async Task<ActionResult<Client>> SaveClient([FromBody] Client client)
        {
            try
            {

                _clientRepository.SaveClient(client);
                return Ok(new { message = "Client updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [Route("DeleteClient")]
        [HttpDelete]
        public async Task<ActionResult<Client>> DeleteClient([FromQuery] int ClientId)
        {
            try
            {
                var clientToDelete = await _clientRepository.GetClient(ClientId);

                if (clientToDelete == null)
                {
                    return NotFound($"Client with Id = {ClientId} not found");
                }

                _clientRepository.DeleteClient(clientToDelete);
                return Ok(new { message = "Client deleted successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }


    }
}
