using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlannerBD.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Budget Budgets { get; set; }
    }
}