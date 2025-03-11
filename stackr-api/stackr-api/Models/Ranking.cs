using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stackr_Api.Models
{
    public class Ranking
    {
        public int Id {get; set; }
        public int ListId {get; set; }
        public int ItemId {get; set; }
        public int Rank {get; set; }
        
    }
}