using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public string FieldName { get; }
        public object InvalidValue { get; }
        public BadRequestException(string fieldName, object invalidValue, string message)
            : base($"{message} Field: {fieldName}, Value: {invalidValue}")
        {
            FieldName = fieldName;
            InvalidValue = invalidValue;
        }
        public BadRequestException(string message) : base(message) { }

    }
}