using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class UserProfileImageDto
    {
        public byte[] Image_Data { get; set; } = null!;
        public int User_Id { get; set; } 
    }
}
