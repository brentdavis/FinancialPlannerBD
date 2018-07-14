using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlannerBD.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public DateTime Created { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public string Code { get; set; }
        public bool Accepted { get; set; }
    }
}