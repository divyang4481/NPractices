using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using PagedList;

namespace NPractices.Mvc.JqGrid
{
    /// <summary>
    /// This type is designed to conform to the structure required by the JqGrid JavaScript component. 
    /// It has all of the properties required by the grid. When this type is serialized to JSON, the resulting 
    /// JSON will be in the structure expected by the grid when it fetches pages of data via AJAX calls.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jq",
        Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jq",
        Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
    [DataContract]
    public class JqGridData
    {
        /// <summary>
        /// The number of pages which should be displayed in the paging controls at the bottom of the grid.
        /// </summary>
        [DataMember(Name = "total")]
        public int Total { get; set; }

        /// <summary>
        /// The current page number which should be highlighted in the paging controls at the bottom of the grid.
        /// </summary>
        [DataMember(Name = "page")]
        public int Page { get; set; }

        /// <summary>
        /// The total number of records in the entire data set, not just the portion returned in Rows.
        /// </summary>
        [DataMember(Name = "records")]
        public int Records { get; set; }

        /// <summary>
        /// See the JqGrid documentation for repeatitems. This property controls how the grid interprets the Rows 
        /// property. When set false, Rows is presumed to contain a list of objects where the property name is 
        /// the grid column name in the property value is the value which should be displayed in the grid. When 
        /// set true, the ID and the non-ID values would be stored separately.
        /// </summary>
        [DataMember(Name = "repeatitems")]
        public static bool RepeatItems
        {
            get { return false; }
        }

        /// <summary>
        /// The data that will actually be displayed in the grid.
        /// </summary>
        [DataMember(Name = "rows")]
        public IEnumerable Rows { get; set; }

        /// <summary>
        /// Arbitrary data to be returned to the grid along with the row data. Leave null if not using. Must be serializable to JSON!
        /// </summary>
        [DataMember(Name = "userdata")]
        public object UserData { get; set; }
    }

    public static class JqGridExtensions
    {
        /// <summary>
        /// Converts a paged list into a format suitable for the JqGrid component, when 
        /// serialized to JSON. Use this method when returning data that the JqGrid component will 
        /// fetch via AJAX. Take the result of this method, and then serialize to JSON. This method 
        /// will also apply paging to the data.
        /// </summary>
        /// <typeparam name="T">The type of the element in baseList. Note that this type should be 
        /// an anonymous type or a simple, named type with no possibility of a cycle in the object 
        /// graph. The default JSON serializer will throw an exception if the object graph it is 
        /// serializing contains cycles.</typeparam>
        /// <param name="list">The list of records to display in the grid.</param>
        /// <param name="userData">Arbitrary data to be returned to the grid along with the row data. 
        /// Leave null if not using. Must be serializable to JSON!</param>
        /// <returns>A structure containing all of the fields required by the JqGrid. You can then call 
        /// a JSON serializer, passing this result.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the grid component we use.")]
        public static JqGridData ToJqGridData<T>(this IPagedList<T> list, object userData = null)
        {
            return new JqGridData()
            {
                Page = list.PageNumber,
                Records = list.TotalItemCount,
                Rows = list,
                Total = list.PageCount,
                UserData = userData
            };
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the grid component we use.")]
        public static JqGridData ToJqGridData<T>(this IList<T> list, object userData = null)
        {
            return new JqGridData()
            {
                Page = 1,
                Records = list.Count,
                Rows = list,
                Total = 1,
                UserData = userData
            };
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jq",
            Justification = "JqGrid is the correct name of the grid component we use.")]
        public static JsonNetResult ToJsonResult(this JqGridData data)
        {
            var r = new JsonNetResult { Data = data };
            r.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            r.SerializerSettings.Converters.Add(new StringEnumConverter());
            return r;
        }
    }
}
