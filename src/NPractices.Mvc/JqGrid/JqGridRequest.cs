using System.Linq;
using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.DynamicQuery;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NPractices.Mvc.JqGrid
{
    /// <summary>
    /// represent the request from jqgrid
    /// </summary>
    public class JqGridRequest
    {
        public JqGridRequest()
        {
            Page = 1;
            Rows = 20;
            Sidx = "Id";
            Sord = "desc";
        }

        #region paging

        public int Page { get; set; }

        public int Rows { get; set; }

        #endregion

        #region sorting

        public string Sidx { get; set; }

        public string Sord { get; set; }

        #endregion

        #region filtering

        public bool _Search { get; set; }

        public string Filters { get; set; }

        #endregion

        #region treegrid

        public int? NodeId { get; set; }

        public int? ParentId { get; set; }

        public int? N_Level { get; set; }

        #endregion

        /// <summary>
        /// convert to QueryObject
        /// </summary>
        /// <param name="includeOnlyPrefix">prefix of the fields that we want to include in the QueryObject</param>
        /// <param name="excludePrefixes">prefixes of the fields that we want to exclude from the QueryObject</param>
        /// <returns></returns>
        public QueryObject ToQuery(string includeOnlyPrefix = null, string[] excludePrefixes = null)
        {
            var q = new QueryObject(new DynamicQueryObjectExecutor());
            if (!string.IsNullOrWhiteSpace(Filters))
            {
                var criteriaGroup = JsonConvert.DeserializeObject<CriteriaGroup>(Filters, new StringEnumConverter());
                if (criteriaGroup != null && criteriaGroup.Valid)
                {
                    if (!string.IsNullOrEmpty(includeOnlyPrefix))
                    {
                        //remove all which does not start with prefix
                        criteriaGroup.Criterias.RemoveAll(rule => !rule.Field.StartsWith(includeOnlyPrefix));
                        //update remains to clean prefix
                        criteriaGroup.Criterias.ForEach(
                            rule => rule.Field = rule.Field.Remove(0, includeOnlyPrefix.Length));
                    }

                    if (excludePrefixes != null && excludePrefixes.Length > 0) //remove all which does start with prefix
                        criteriaGroup.Criterias.RemoveAll(
                            rule => excludePrefixes.Any(prefix => rule.Field.StartsWith(prefix)));

                    q.Add(criteriaGroup);
                }
            }

            if (!string.IsNullOrEmpty(Sidx))
            {
                string sortJsonString = string.Format("{{'sidx':'{0}','sord':'{1}'}}", Sidx, Sord);
                var d = JsonConvert.DeserializeObject<SortDescription>(sortJsonString, new StringEnumConverter());
                q.Add(d);
            }

            return q;
        }
    }
}