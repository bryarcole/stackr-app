using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stackr_Api.Models
{
    public class List
    {
        public int Id {get; set}
        public int UserId {get; set; }
        public string Name {get; set; }
        public List<Ranking> Rankings {get; set;}

        
    }
}