using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clipess.API.Controllers
{
    [Route("api/Report")]
    [ApiController]
    public class InventoryReportController : ControllerBase
    {
        private readonly IInventoryReportRepository _inventoryReportRepository;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public InventoryReportController(IInventoryReportRepository inventoryReportRepository, IConfiguration configuration)
        {
            _inventoryReportRepository = inventoryReportRepository;
            _configuration = configuration;
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["Cloudinary:CloudName"],
                ApiKey = _configuration["Cloudinary:ApiKey"],
                ApiSecret = _configuration["Cloudinary:ApiSecret"]
            });
        }

        [HttpGet("{inventoryTypeId}")]
        public async Task<IActionResult> GenerateReport(int inventoryTypeId)
        {
            var inventoryType = _inventoryReportRepository.GetInventoryTypeById(inventoryTypeId);
            if (inventoryType == null)
            {
                return NotFound("Inventory type not found.");
            }

            var inventories = _inventoryReportRepository.GetInventoriesByType(inventoryTypeId).ToList();

            var document = Document.Create(container =>
            {

               
                container.Page(page =>
                {    
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.Grey.Lighten2);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Header()
                    .Text("clipess pvt(Ltd)")
                    .FontSize(10).FontColor(Colors.Black).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                    .AlignLeft();


                    page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(8);

                        x.Item().Text("Inventory Report").AlignCenter().Bold().FontSize(36).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .FontColor(Colors.Black);

                        x.Item().PaddingVertical(8);

                       

                        //x.Item().PaddingVertical(1);
                        var length = inventories.Count();

                        x.Item().Text($"Inventory Type: {inventoryType.InventoryTypeName}").Bold().FontFamily("Roboto", "Calibri", "Noto Color Emoji");
                        x.Item().Text($"Total Inventories:{length}").Bold().FontFamily("Roboto", "Calibri", "Noto Color Emoji");






                        x.Item().PaddingVertical(5);

                        

                        //x.Item().PaddingVertical(1);

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
                                    //.PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();


                            }

                          

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); 
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(3); 
                                columns.RelativeColumn(3); 

                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).ExtendHorizontal().Text("No.").AlignCenter();
                                header.Cell().Element(CellStyle).ExtendHorizontal().Text("Inventory Name").AlignCenter();
                                header.Cell().Element(CellStyle).ExtendHorizontal().Text("Assigned Employee").AlignCenter();
                                header.Cell().Element(CellStyle).ExtendHorizontal().Text("Created Date").AlignCenter();
                               

                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                            });
                            int counter = 1;

                            foreach (var inventory in inventories)
                            {
                                table.Cell().Element(CellStyle).Text(counter.ToString());
                                table.Cell().Element(CellStyle).Text(inventory.InventoryName);
                                table.Cell().Element(CellStyle).Text(inventory.Employee?.FirstName + " " + inventory.Employee?.LastName);
                                table.Cell().Element(CellStyle).Text(inventory.CreatedDate.ToString("yyyy-MM-dd"));

                                IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);

                                counter++;
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


            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            var pdfContent = stream.ToArray();

            var uploadStream = new MemoryStream(pdfContent);
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription($"{inventoryType.InventoryTypeName}.pdf", uploadStream),
                PublicId = $"inventory_reports/{inventoryType.InventoryTypeName}_{DateTime.UtcNow.Ticks}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                inventoryType.ReportUrl = uploadResult.SecureUrl.ToString();
                _inventoryReportRepository.SaveChanges();
                return Ok(new { ReportUrl = uploadResult.SecureUrl });
            }
            else
            {
                return StatusCode((int)uploadResult.StatusCode, "Failed to upload PDF.");
            }
        }
    }
}


/*
 using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;


namespace Clipess.API.Controllers
{
    [Route("api/pdfGeneration")]
    [ApiController]
    public class PdfGenerationController : ControllerBase
    {
        private readonly IPdfGenerationRepository _pdfGenerationRepository;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        public PdfGenerationController(IPdfGenerationRepository pdfGenerationRepository, IConfiguration configuration)
        {
            _pdfGenerationRepository = pdfGenerationRepository;
            _configuration = configuration;
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["CloudinarySettings:CloudName"],
                ApiKey = _configuration["CloudinarySettings:ApiKey"],
                ApiSecret = _configuration["CloudinarySettings:ApiSecret"]
            });

        }

        private string FormatDuration(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours}h {minutes}m";
        }


        [HttpGet]
        [Route("GenerateHelloWorldPdf")]
        public IActionResult GenerateHelloWorldPdf([FromQuery] int employeeId, string month)
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

            // Generate the PDF document
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

                        x.Item().Text("Attendanc Report").AlignCenter().Bold().FontSize(36).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .FontColor(Colors.Black);

                        x.Item().PaddingVertical(8);

                        x.Item().Text("Monthly Record").AlignLeft().FontSize(21).FontFamily("Roboto", "Calibri", "Noto Color Emoji")
                        .FontColor(Colors.Black).SemiBold();

                        //x.Item().PaddingVertical(1);

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

                        //x.Item().PaddingVertical(1);

                        x.Item()
                        .Padding(5)
                        .MinimalBox()
                        .Border(1).Table( table =>
                        {
                            IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                            {
                                return container
                                    .DefaultTextStyle(x => x.FontSize(11))
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .PaddingVertical(10)
                                    .BorderColor(backgroundColor)
                                    //.PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                                
                                
                            }

                            //TextStyle CellTextStyle()
                            //{
                            //    return TextStyle.Default.FontSize(12); // Adjust the font size here
                            //}

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
            var selectedEmployeeId = " " ;
            if (monthlyEntry == null)
            {
                fileName = "No user";
                selectedEmployeeId = " No Employee";
            }
            else
            {
                fileName = $"{monthlyEntry.FirstName}{monthlyEntry.LastName}{monthlyEntry.Month}_Attendance_Report";
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
                // Handle upload error
                Console.WriteLine($"Error uploading PDF: {uploadResult.Error.Message}");
                return BadRequest("Failed to upload PDF to Cloudinary");
            }

            return Ok(new { message = "PDF uploaded successfully!", url = uploadResult.SecureUrl });
        }
    }
}

























/*[Route("add")]
[HttpPost]

public async Task<IActionResult> AddIdForGenerateReport([FromBody] InventoryReport inventoryReport)
{
    try
    {
        if (inventoryReport == null)
        {
            return BadRequest("Inventory type name is missing.");
        }


        var reportNew = new InventoryReport
        {


            EmployeeId = inventoryReport.EmployeeId,
            InventoryTypeId = inventoryReport.InventoryTypeId,




        };



        // Add the newly created reprt details
        _inventoryReportRepository.AddIdForGenerateReport(inventoryReport);

        // Save changes to the database
        _inventoryReportRepository.SaveChanges();

        // Return a response indicating successful creation
        return CreatedAtAction(nameof(AddIdForGenerateReport), new { reportId = inventoryReport.ReportId }, inventoryReport);
    }
    catch (Exception ex)
    {
        // Log the exception
        _logger.Error($"An error occurred in: {nameof(AddIdForGenerateReport)}, exception: {ex.Message}.");

        // Return a response indicating failure
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}
*/


