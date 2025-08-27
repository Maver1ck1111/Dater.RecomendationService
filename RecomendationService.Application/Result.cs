using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomendationService.Application
{
    public class Result
    {
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
