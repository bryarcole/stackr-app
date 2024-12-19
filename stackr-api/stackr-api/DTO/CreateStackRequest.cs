using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stackr_Api.DTO
{
    public record CreateStackRequest
    {
        public string Title { get; init; }
    }
}