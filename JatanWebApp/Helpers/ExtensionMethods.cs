using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Jatan.Models;
using JatanWebApp.Models;
using JatanWebApp.Models.DAL;

namespace JatanWebApp.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the Jatan Player object from their username. Returns null if not found.
        /// </summary>
        public static Jatan.Models.Player GetPlayerFromName(this Jatan.GameLogic.GameManager manager, string userName)
        {
            return manager.Players.FirstOrDefault(p => p.Name == userName);
        }

        /// <summary>
        /// Gets the avatar path for a user identity.
        /// </summary>
        public static string GetAvatarPath(this IIdentity identity)
        {
            var userName = identity.Name;
            return DatabaseHelper.GetAvatarPathFromUsername(userName);
        }

        /// <summary>
        /// Converts a DateTime to a Unix timestamp
        /// </summary>
        public static long ToUnixTimestamp(this DateTime date)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan unixTimeSpan = date - unixEpoch;

            return (long)unixTimeSpan.TotalSeconds;
        }

        /// <summary>
        /// Encodes a string for JSON
        /// </summary>
        public static string JsonEscape(this object o)
        {
            return System.Web.Helpers.Json.Encode(o);
        }

        /// <summary>
        /// Encodes a string for JSON with option to trim quotation marks
        /// </summary>
        public static string JsonEscape(this object o, bool removeQuotes)
        {
            var encoded = System.Web.Helpers.Json.Encode(o);
            if (removeQuotes) return encoded.Trim('"');
            return encoded;
        }

        /// <summary>
        /// Returns a css color from a player color.
        /// </summary>
        public static string ToCssColor(this PlayerColor playerColor, float alpha)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            switch (playerColor)
            {
                case PlayerColor.Blue:
                    b = 255;
                    break;
                case PlayerColor.Green:
                    g = 255;
                    break;
                case PlayerColor.Red:
                    r = 255;
                    break;
                case PlayerColor.Yellow:
                    r = 255;
                    g = 255;
                    break;
            }
            if (alpha > 1) alpha = 1;
            else if (alpha < 0) alpha = 0;
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, alpha);
        }

        /// <summary>
        /// Returns a random guid string.
        /// </summary>
        public static string GetGuid(this HtmlHelper helper)
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Returns a readable timespan string (e.g. 3 hours, 24 minutes)
        /// </summary>
        public static string ToReadableString(this TimeSpan ts)
        {
            var sb = new StringBuilder();
            if (ts.Days > 0) sb.AppendFormat("{0} day{1}, ", ts.Days, ts.Days > 1 ? "s" : "");
            if (ts.Hours > 0) sb.AppendFormat("{0} hour{1}, ", ts.Hours, ts.Hours > 1 ? "s" : "");
            if (ts.Minutes > 0) sb.AppendFormat("{0} minute{1}, ", ts.Minutes, ts.Minutes > 1 ? "s" : "");
            if (ts.Seconds > 0) sb.AppendFormat("{0} second{1}", ts.Seconds, ts.Seconds > 1 ? "s" : "");
            return sb.ToString().Trim(' ', ',');
        }

        /// <summary>
        /// Limits a value to a specified range.
        /// </summary>
        public static int Clamp(this int num, int minimum, int maximum)
        {
            if (num < minimum) return minimum;
            if (num > maximum) return maximum;
            return num;
        }

        /// <summary>
        /// Gets a single page of data from a sequence. This should be the final method called on an IQueryable object.
        /// </summary>
        /// <typeparam name="TSource">The type of element in the squence.</typeparam>
        /// <param name="query">The sequence.</param>
        /// <param name="pageNumber">The 1-based index of the page to return.</param>
        /// <param name="pageSize">The number of elements in a single page.</param>
        /// <returns>Retuns a new sequnce limited by the paging parameters.</returns>
        public static IQueryable<TSource> GetPage<TSource>(this IQueryable<TSource> query, int pageNumber, int pageSize)
        {
            pageNumber = (pageNumber < 1) ? 1 : pageNumber;
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Creates a page navigation control for the specified paged model object.
        /// </summary>
        /// <param name="htmlHelper">Helper used to render HTML elements.</param>
        /// <param name="pagedList">The list which will be used with this paging control.</param>
        /// <returns>HTML of the paging control.</returns>
        public static MvcHtmlString PagerControl(this HtmlHelper htmlHelper, IPagedList pagedList)
        {
            string actionName = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<ul class=""pagination"">");

            if (pagedList.TotalNumberOfPages > 1)
            {
                if (pagedList.HasPreviousPage)
                {
                    sb.AppendFormat(@"<li>{0}</li>", htmlHelper.ActionLink("«", actionName, new { page = 1, size = pagedList.PageSize }, new { @class = "" }));    
                }
                else
                {
                    sb.Append(@"<li class=""disabled""><a href=""#"">«</a></li>");
                }

                int startingPageRangeIndex, endingPageRangeIndex;
                if (pagedList.PageIndex <= 3)
                {
                    startingPageRangeIndex = 1;
                    endingPageRangeIndex = (pagedList.TotalNumberOfPages < 5) ? pagedList.TotalNumberOfPages : 5;
                }
                else if (pagedList.PageIndex >= pagedList.TotalNumberOfPages - 2)
                {
                    startingPageRangeIndex = (pagedList.TotalNumberOfPages > 5) ? (pagedList.TotalNumberOfPages - 4) : 1;
                    endingPageRangeIndex = pagedList.TotalNumberOfPages;
                }
                else
                {
                    startingPageRangeIndex = pagedList.PageIndex - 2;
                    endingPageRangeIndex = pagedList.PageIndex + 2;
                }

                for (int i = startingPageRangeIndex; i <= endingPageRangeIndex; i++)
                {
                    if (i != pagedList.PageIndex)
                    {
                        sb.AppendFormat(@"<li>{0}</li>", htmlHelper.ActionLink(i.ToString(), actionName, new { page = i, size = pagedList.PageSize }, new { @class = "" }));
                    }
                    else
                    {
                        sb.AppendFormat(@"<li class=""active""><a href=""#"">{0}</a></li>", i);
                    }
                }

                if (pagedList.HasNextPage)
                {
                    sb.AppendFormat(@"<li>{0}</li>", htmlHelper.ActionLink("»", actionName, new {page = pagedList.TotalNumberOfPages, size = pagedList.PageSize}, new {@class = ""}));
                }
                else
                {
                    sb.Append(@"<li class=""disabled""><a href=""#"">»</a></li>");
                }
                
            }

            sb.Append(@"</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}