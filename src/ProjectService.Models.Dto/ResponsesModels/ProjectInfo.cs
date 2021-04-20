using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class ProjectInfo
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string ShortDescription { get; set; }
        Department Department { get; set; }
    }
}
