using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JatanWebApp.Helpers;

namespace JatanWebApp.Models
{
    /// <summary>
    /// Generic class that contains a list of entities and paging information.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the list.</typeparam>
    public class PagedList<T> : List<T>, IPagedList
    {
        /// <summary>
        /// The 1-based index of the current page.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// The maximum number of records contained in a single page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// The total number of records from the data source.
        /// </summary>
        public int TotalNumberOfRecords { get; private set; }

        /// <summary>
        /// The number of pages required to display all records from the data source.
        /// </summary>
        public int TotalNumberOfPages { get; private set; }

        /// <summary>
        /// Returns true if there is a page before this page.
        /// </summary>
        public bool HasPreviousPage { get { return PageIndex > 1; } }

        /// <summary>
        /// Returns true if there is a page after this page.
        /// </summary>
        public bool HasNextPage { get { return PageIndex < TotalNumberOfPages; } }

        /// <summary>
        /// Creates a new PagedList instance.
        /// </summary>
        /// <param name="source">The data source. This must be a sorted sequence.</param>
        /// <param name="pageIndex">The 1-based current page index.</param>
        /// <param name="pageSize">The maximum number of records contained in a single page.</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            this.PageIndex = (pageIndex < 1) ? 1 : pageIndex;
            this.PageSize = pageSize.Clamp(0, int.MaxValue);
            this.TotalNumberOfRecords = source.Count();
            this.TotalNumberOfPages = (int)Math.Ceiling((double)this.TotalNumberOfRecords / this.PageSize);
            this.AddRange(source.GetPage(this.PageIndex, this.PageSize).ToList());
        }
    }

    /// <summary>
    /// IPagedList interface
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// Gets the number of items in the current page.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The 1-based index of the current page.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// The maximum number of records contained in a single page.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// The total number of records from the data source.
        /// </summary>
        int TotalNumberOfRecords { get; }

        /// <summary>
        /// The number of pages required to display all records from the data source.
        /// </summary>
        int TotalNumberOfPages { get; }

        /// <summary>
        /// Returns true if there is a page before this page.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Returns true if there is a page after this page.
        /// </summary>
        bool HasNextPage { get; }
    }
}