using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace QueryDictionary
{

	public abstract class QueryDictionary : IQueryDictionary
	{
		public bool RemoveHeader { get; set; }
		public string HeaderStartText { get; set; }
		public string HeaderEndText { get; set; }
		public string HeaderReplacement { get; set; } = string.Empty;
		public Func<Query, bool> Predicate { get; set; }
		public Func<Query, Query> Mutator { get; set; }
		public abstract void Load();
		protected IDictionary<string, string> Queries { get; } = new Dictionary<string, string>();

		public IEnumerable<string> Keys => Queries.Keys;
		public IEnumerable<string> Values => Queries.Values;
		public int Count => Queries.Count;

		public string this[string key] => Queries[key];

		internal void Add(Query query)
		{
			if (RemoveHeader)
			{
				if (string.IsNullOrEmpty(HeaderStartText))
					throw new InvalidOperationException("HeaderStartText must be set when RemoveHeader is true.");
				if (string.IsNullOrEmpty(HeaderEndText))
					throw new InvalidOperationException("HeaderEndText must be set when RemoveHeader is true.");

				query.Value = removeHeader(query.Value);
			}

			if (Predicate != null)
				if (!Predicate(query))
					return;

			if (Mutator != null)
				query = Mutator(query);

			Queries.Add(query.Key, query.Value);
		}

		private string removeHeader(string queryValue)
		{
			int start = queryValue.IndexOf(HeaderStartText, StringComparison.OrdinalIgnoreCase);
			int end = queryValue.IndexOf(HeaderEndText, StringComparison.OrdinalIgnoreCase);

			if (end > -1)
			{
				string before = string.Empty;

				if (start > -1)
					before = queryValue.Substring(0, start).Trim();


				string after = queryValue.Substring(end + HeaderEndText.Length, queryValue.Length - end - HeaderEndText.Length).Trim();
				return string.Concat(before, HeaderReplacement ?? string.Empty, after);
			}

			return queryValue;
		}

		public bool ContainsKey(string key) => Queries.ContainsKey(key);
		public bool TryGetValue(string key, out string value) => Queries.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Queries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Queries).GetEnumerator();
	}
}