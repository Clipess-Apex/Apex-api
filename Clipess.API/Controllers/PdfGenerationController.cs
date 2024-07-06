using Clipess.DBClient.Contracts;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hangfire;
using log4net;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [Route("api/pdfGeneration")]
    [ApiController]
    public class PdfGenerationController : ControllerBase
    {
        private readonly IPdfGenerationRepository _pdfGenerationRepository;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public static ILog _logger;
        public PdfGenerationController(IPdfGenerationRepository pdfGenerationRepository, IConfiguration configuration, IBackgroundJobClient backgroundJobClient)
        {
            _pdfGenerationRepository = pdfGenerationRepository;
            _configuration = configuration;
            _backgroundJobClient = backgroundJobClient;

            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["CloudinarySettings:CloudName"],
                ApiKey = _configuration["CloudinarySettings:ApiKey"],
                ApiSecret = _configuration["CloudinarySettings:ApiSecret"]
            });
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        private string FormatDuration(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours}h {minutes}m";
        }


        [HttpGet]
        [Route("GenerateEmployeeAttendancePDF")]
        public  void GenerateEmployeeAttendancePDF()
        {
            string month = DateTime.UtcNow.ToString("yyyy-MM");
            
            var allEmployeeIds = _pdfGenerationRepository.GetEmployee().ToList();

            foreach (var employeeId in allEmployeeIds)
            {
                var monthlyTimeEntries = _pdfGenerationRepository.GetMonthlyTimeEntries(employeeId, month).Select(x => new
                {
                    x.EmployeeId,
                    x.Month,
                    x.AllocatedDuration,
                    CompletedDuration = x.CompletedDuration ?? 0,
                    FirstName = x.Employee.FirstName ?? null,
                    LastName = x.Employee.LastName ?? null,

                }).ToList(); ;
                var dailyTimeEntries = _pdfGenerationRepository.GetDailyTimeEntries(employeeId, month).Select(x => new
                {
                    x.EmployeeId,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    CheckIn = x.CheckIn.HasValue ? x.CheckIn.Value.ToString("hh:mm tt") : null,
                    CheckOut = x.CheckOut.HasValue ? x.CheckOut.Value.ToString("hh:mm tt") : null,
                    LunchIn = x.LunchIn.HasValue ? x.LunchIn.Value.ToString("hh:mm tt") : null,
                    LunchOut = x.LunchOut.HasValue ? x.LunchOut.Value.ToString("hh:mm tt") : null,
                    TotalDuration = x.TotalDuration ?? 0,
                }).ToList(); ;

                string pdfGeneratedTime = DateTime.Now.ToString("HH:mm:ss");

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(14));

                        page.Header()
                        .Text($"Clipess Pvt Ltd - PDF Generated Time: {pdfGeneratedTime}")
                        .FontSize(10).FontColor(Colors.Green.Darken3).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .AlignRight();


                        page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(8);

                            x.Item().Text("Attendanc Report").AlignCenter().Bold().FontSize(36).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                            .FontColor(Colors.Black);

                            x.Item().PaddingVertical(8);

                            x.Item().Text("Monthly Record").AlignLeft().FontSize(21).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                            .FontColor(Colors.Black).SemiBold();

                            foreach (var entry in monthlyTimeEntries)
                            {
                                x.Item().Text($"Employee ID: {entry.EmployeeId}");
                                x.Item().Text($"Name: {entry.FirstName} {entry.LastName}");
                                x.Item().Text($"Month: {entry.Month}");
                                x.Item().Text($"Allocated Duration: {FormatDuration(entry.AllocatedDuration)}");
                                x.Item().Text($"Completed Duration: {FormatDuration(entry.CompletedDuration)}");
                            }

                            x.Item().PaddingVertical(5);

                            x.Item().Text("Daily Records").AlignLeft().FontSize(21).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                           .FontColor(Colors.Black).SemiBold();

                            x.Item()
                            .Padding(5)
                            .MinimalBox()
                            .Border(1).Table(table =>
                            {
                                IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                                {
                                    return container
                                        .DefaultTextStyle(x => x.FontSize(11))
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .PaddingVertical(10)
                                        .BorderColor(backgroundColor)
                                        .AlignCenter()
                                        .AlignMiddle();
                                }

                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(90);
                                    columns.ConstantColumn(73);
                                    columns.ConstantColumn(73);
                                    columns.ConstantColumn(73);
                                    columns.ConstantColumn(73);
                                    columns.ConstantColumn(85);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Date").AlignCenter();
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Check-In").AlignCenter();
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Lunch-In").AlignCenter();
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Lunch-Out").AlignCenter();
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Check-Out").AlignCenter();
                                    header.Cell().Element(CellStyle).ExtendHorizontal().Text("Total Duration").AlignCenter();

                                    IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                                });
                                foreach (var entry in dailyTimeEntries)
                                {
                                    table.Cell().Element(CellStyle).Text(entry.Date);
                                    table.Cell().Element(CellStyle).Text(entry.CheckIn);
                                    table.Cell().Element(CellStyle).Text(entry.LunchIn);
                                    table.Cell().Element(CellStyle).Text(entry.LunchOut);
                                    table.Cell().Element(CellStyle).Text(entry.CheckOut);
                                    table.Cell().Element(CellStyle).Text(FormatDuration(entry.TotalDuration));

                                    IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White).ShowOnce();
                                }
                            });
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

                var monthlyEntry = monthlyTimeEntries.FirstOrDefault();
                string fileName = " ";
                var selectedEmployeeId = " ";

                if (monthlyEntry == null)
                {
                    fileName = "No user";
                    selectedEmployeeId = " No Employee";
                }
                else
                {
                    fileName = $"{monthlyEntry.FirstName}_{monthlyEntry.LastName}_{monthlyEntry.Month}_Attendance_Report";
                    selectedEmployeeId = monthlyEntry.EmployeeId.ToString();
                }

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                var pdfContent = stream.ToArray();

                var uploadStream = new MemoryStream(pdfContent);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, uploadStream),
                    PublicId = $"clipess_Employee_{selectedEmployeeId}/{fileName}",
                };

                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.Error != null)
                {
                    Console.WriteLine($"Error uploading PDF: {uploadResult.Error.Message}");
                }
                var savePdfResult = _pdfGenerationRepository.SaveAttendancePdf(employeeId, month, uploadResult.SecureUrl.ToString());
           }
            string currentTime = DateTime.Now.ToString("HH:mm:ss"); 
        }

        [HttpGet]
        [Route("GetMonthlyPdfByEmployee")]
        public async Task<IActionResult> GetMonthlyPdfByEmployee(int employeeId, string month)
        {
            try
            {
                var monthlyTimeEntry = _pdfGenerationRepository.GetMonthlyPdfByEmployee(employeeId, month);

                if (monthlyTimeEntry == null)
                {
                    return NoContent();
                }
                return Ok(monthlyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyPdfByEmployee)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }
    }
}
