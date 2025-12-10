using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Authentification
{
    public record RegisterRequestDto(string Email, string PasswordHash, string Fullname, EmployeeRoleEnum Role, Guid CompanyId);
}
