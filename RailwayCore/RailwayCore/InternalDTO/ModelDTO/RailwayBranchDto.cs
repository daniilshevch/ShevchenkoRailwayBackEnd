using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class RailwayBranchDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Office_Location { get; set; } = null!;
    }
}
