using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlannerBD.Models
{

    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        //public virtual ICollection<Bank> Banks { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Household()
        {
            Accounts = new HashSet<Account>();
            //Banks = new HashSet<Bank>();
            Budgets = new HashSet<Budget>();
            Invitations = new HashSet<Invitation>();
            Users = new HashSet<ApplicationUser>();
        }

    }
}