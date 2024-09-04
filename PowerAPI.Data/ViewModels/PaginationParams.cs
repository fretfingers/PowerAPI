using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class PaginationParams
    {
        private const int maxItemPerPage = 50;
        private int itemsPerPage = 50;

        public int Page { get; set; } = 1;
        public int ItemsPerPage
        {
            get =>  itemsPerPage;
            set => itemsPerPage = value > maxItemPerPage ? maxItemPerPage : value;
        }
    }
}
