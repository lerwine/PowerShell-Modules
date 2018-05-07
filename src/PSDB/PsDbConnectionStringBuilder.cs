using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace PSDB
{
    public class PsDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        public const string Keyword_Database = "Database";

        public PsDbConnectionStringBuilder() { }

        public static IEnumerable<string> GetSupportKeywords()
        {
            yield return Keyword_Database;
        }
        private static string MapKeyword(string keyword)
        {
            
            string tk = keyword.Trim();
            return GetSupportKeywords().Where(k => String.Compare(k, tk, true) == 0).DefaultIfEmpty(tk.ToLower()).First();
        }

        public override object this[string keyword]
        {
            get
            {
                if (keyword == null)
                    throw new ArgumentNullException("keyword");

                if (keyword.Trim().Length == 0)
                    throw new ArgumentException("Keyword cannot be empty.", "keyword");

                return base[MapKeyword(keyword)];
            }

            set
            {
                if (keyword == null)
                    throw new ArgumentNullException("keyword");

                if (keyword.Trim().Length == 0)
                    throw new ArgumentException("Keyword cannot be empty.", "keyword");

                base[MapKeyword(keyword)] = value;
            }
        }

        public string Database
        {
            get { return this[Keyword_Database] as string; }
        }
    }
}