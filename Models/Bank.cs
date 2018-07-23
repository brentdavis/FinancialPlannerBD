﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlannerBD.Models
{
    
    public class Bank
    {
        public int Id { get; set; }
        //public int HouseholdId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }

        //public virtual Household Household { get; set; }
        public virtual ICollection<Account> Account { get; set; }

        public Bank()
        {
            Account = new HashSet<Account>();
        }
    }
}