using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class PaginationMetadata
    {
        public PaginationMetadata(int currentPage, int totalCount, int itemsPerPage)
        {
            CurrentPage = currentPage;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)itemsPerPage);
        }

        public int CurrentPage { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
