using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key) : base($"The {entityName} with identifier '{key}' was not found") { }
        public NotFoundException(string message) : base(message) { }
    }
}