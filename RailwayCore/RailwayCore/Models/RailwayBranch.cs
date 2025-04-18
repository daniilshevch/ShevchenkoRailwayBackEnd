using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace RailwayCore.Models
{
    public class RailwayBranch //Філія залізниці
    {
        [Key]
        public int Id { get; set; } //Числовий ідентифікатор філії
        [MaxLength(40)]
        public string Title { get; set; } = null!; //Назва філії
        [MaxLength(30)]
        public string Office_Location { get; set; } = null!; //Локація головного офісу
        public List<Station> Stations { get; set; } = new List<Station>(); //Станції, які прив'язані до філії
        public List<TrainRoute> Train_Routes { get; set; } = new List<TrainRoute>(); //Маршрути, які обслуговує філія
    }
}
