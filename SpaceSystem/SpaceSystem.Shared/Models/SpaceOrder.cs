using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceSystem.Shared.Models
{
    public class SpaceOrder
    {
        public string AgencyName { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
    }
}
