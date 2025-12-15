using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Core.PayType
{
    public record PayTypeShortDto(
        Guid Id,
        string Name,
        string Code);

    public record CreateUpdatePayTypeDto(
        [Required] string Name,
        [Required] string Code);

    public record PayTypeDetailsDto(
        Guid Id,
        string Name,
        string Code,
        int VendingMachinesCount);
}
