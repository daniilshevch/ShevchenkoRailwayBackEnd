using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public List<Image> Images { get; set; } = null!;

    }
}
