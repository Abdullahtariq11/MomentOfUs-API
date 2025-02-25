using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Dtos.SharedJournalDto
{
    public record SharedJournalUpdated( PermissionLevel Permission,string targetUserId);

}