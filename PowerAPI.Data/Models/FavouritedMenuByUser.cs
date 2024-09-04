using System;
using System.Collections.Generic;


namespace PowerAPI.Data.Models
{
    public partial class FavouritedMenuByUser
    {
        public string CompanyId { get; set; }

        public string DivisionId { get; set; }

        public string DepartmentId { get; set; }

        public string Username { get; set; }

        public int MenuId { get; set; }

        public DateTime? EntryDate { get; set; }

        public virtual MainMenu Menu { get; set; }

        public virtual Users User { get; set; }
    }
}
