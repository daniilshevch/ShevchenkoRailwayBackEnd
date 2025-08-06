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
        public int User_Id { get; set; }
        public User User { get; set; } = null!;
        public List<Image> Images { get; set; } = null!;

    }
}
