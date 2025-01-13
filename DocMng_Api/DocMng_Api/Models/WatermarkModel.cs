using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocMng_Api.Models
{
    public class WatermarkModel
    {
        public IFormFile File { get; set; }
        public string topText { get; set; }
        public string middleText { get; set; }
    }
}
