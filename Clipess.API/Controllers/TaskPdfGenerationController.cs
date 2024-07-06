using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace Clipess.API.Controllers
{
    [Route("api/taskpdfGeneration")]
    [ApiController]
    public class TaskPdfGenerationController : ControllerBase
    {
        private readonly ITaskPdfGenerationRepository _pdfGenerationRepository;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        private readonly ITaskRepository _taskRepository;
        public TaskPdfGenerationController(ITaskPdfGenerationRepository pdfGenerationRepository, IConfiguration configuration, ITaskRepository taskRepository)
        {
            _pdfGenerationRepository = pdfGenerationRepository;
            _configuration = configuration;
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["TaskCloudinarySettings:CloudName"],
                ApiKey = _configuration["TaskCloudinarySettings:ApiKey"],
                ApiSecret = _configuration["TaskCloudinarySettings:ApiSecret"]
            });
            _taskRepository = taskRepository;
        }


        [HttpGet]
        [Route("GenerateTaskReportPdf")]
        public async Task<ActionResult> GenerateTaskReportPdf([FromQuery] int EmployeeId)
        {

            List<int> taskIdList = new List<int>();
            List<ProjectTask> taskList = _taskRepository.GetEmployeeTasks();
            foreach (var item in taskList)
            {
                string[] dataArray = item.SelectedUsers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Convert each substring to integer if needed
                int[] taskEmployees = Array.ConvertAll(dataArray, int.Parse);
                foreach (var data in taskEmployees)
                {
                    if(data == EmployeeId)
                    {
                        taskIdList.Add(item.TaskId);
                    }
                }

            }


            

            var getTasks = new List<ProjectTaskPdf>();
            getTasks = await _pdfGenerationRepository.GetTaskReport(taskIdList, EmployeeId);
            var employeeName = _taskRepository.GetFormUser(EmployeeId);
          
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Header()
                    .Text("Clipess@pvtLimitedCompany")
                    .FontSize(10).FontColor(Colors.Green.Darken3).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                    .AlignRight();

                    page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(8);

                        x.Item().Text("Task Report").AlignCenter().Bold().FontSize(36).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .FontColor(Colors.Black);

                        x.Item().PaddingVertical(8);

                        x.Item().Text($"Employee ID: {EmployeeId}").AlignLeft().FontSize(21).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .FontColor(Colors.Black).SemiBold();

                        

                        x.Item()
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Background(backgroundColor)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                // please be sure to call the 'header' handler!

                                header.Cell().Element(CellStyle).Text("Task ID");
                                header.Cell() .Element(CellStyle).Text("Task Name");
                                header.Cell().Element(CellStyle).Text("Start Date");
                                header.Cell().Element(CellStyle).Text("End Date");
                                header.Cell().Element(CellStyle).Text("Assigned Date");
                                header.Cell().Element(CellStyle).Text("Task Status");

                                // you can extend existing styles by creating additional methods
                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                            });

                            foreach (var task in getTasks)
                            {
                                
                                
                                table.Cell().Element(CellStyle).Text(task.TaskId);
                                table.Cell().Element(CellStyle).Text(task.TaskName);
                                table.Cell().Element(CellStyle).Text(task.StartDate);
                                table.Cell().Element(CellStyle).Text(task.EndDate);
                                table.Cell().Element(CellStyle).Text(task.AssignedDate);
                                table.Cell().Element(CellStyle).Text(task.TaskStatus);

                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White).ShowOnce();
                            }
                        });

                        x.Item().PaddingVertical(5);
                       
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            string fileName = " ";
            var selectedEmployeeId = " ";
            if (getTasks == null)
            {
                fileName = "No user";
                selectedEmployeeId = " No Employee";
            }
            else
            {
                fileName = "Task_Report";
                selectedEmployeeId = EmployeeId.ToString();
            }


            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            var pdfContent = stream.ToArray();

            var uploadStream = new MemoryStream(pdfContent);
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription($"{fileName}.pdf", uploadStream),
                PublicId = $"clipess_Employee_{selectedEmployeeId}/{fileName}_{DateTime.UtcNow.Ticks}",
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            if (uploadResult.Error != null)
            {
                // Handle upload error
                Console.WriteLine($"Error uploading PDF: {uploadResult.Error.Message}");
                return BadRequest("Failed to upload PDF to Cloudinary");
            }

            return Ok(uploadResult.SecureUrl);
        }
    }
}