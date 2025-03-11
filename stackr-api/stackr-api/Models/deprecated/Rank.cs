using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Stackr_Api.Models
{
    public class Rank
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
    }

    public class Stack
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public List<Rank> Rankings {get; set;}
    }
}