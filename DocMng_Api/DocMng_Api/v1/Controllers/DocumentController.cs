
using DocMng_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.DocumentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocMng_Api.v1.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        public DocumentController()
        {
        }

        [Route("GetWatermark")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] WatermarkModel doc)
        {
            if (doc.File != null && doc.File.Length > 0)
            {
                try
                {

                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(doc.File.FileName);
                    string extension = Path.GetExtension(doc.File.FileName);
                    string newFileName = $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                    string originalFilePath = Path.Combine(uploadPath, newFileName);
                    using (var stream = new FileStream(originalFilePath, FileMode.Create))
                    {
                        await doc.File.CopyToAsync(stream);
                    }

                    string resultFilePath = Path.Combine(uploadPath, "Processed_" + $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMddHHmmss}{extension}");

                    var pdfService = new Pdf();
                    pdfService.AddDiagonalWatermarks(originalFilePath, resultFilePath, doc.topText, doc.middleText);

                    var resultStream = new FileStream(resultFilePath, FileMode.Open, FileAccess.Read);
                    return File(resultStream, "application/octet-stream", Path.GetFileName(resultFilePath));
                }
                catch (Exception ex)
                {
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, $"오류가 발생했습니다");
        }
    }
}
