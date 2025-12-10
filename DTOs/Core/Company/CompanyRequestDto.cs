using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Core.Company
{
    public record CompanyRequestDto(
        string Name,
        string? Address = null,
        string? Phone = null,
        string? Email = null
    );
}
