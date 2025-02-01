using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set;}="";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}