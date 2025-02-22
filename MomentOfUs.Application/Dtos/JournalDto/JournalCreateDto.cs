using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Application.Dtos.JournalDto
{
    public record JournalCreateDto(string Title, string? PhotoUrl); 
   
}