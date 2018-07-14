﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlannerBD.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool Reconciled { get; set; }
        public decimal ReconciledAmount { get; set; }

        public virtual Account Account { get; set; }
    }
}