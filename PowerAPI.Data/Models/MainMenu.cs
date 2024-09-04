using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class MainMenu
    {
        public int MenuId { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int? ParentId { get; set; }
        public int? MainMenuId { get; set; }

        public virtual ICollection<FavouritedMenuByUser> FavouritedMenuByUsers { get; set; } = new List<FavouritedMenuByUser>();
    }
}
