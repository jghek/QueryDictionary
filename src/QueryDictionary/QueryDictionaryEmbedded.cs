using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QueryDictionary
{
	public class QueryDictionaryEmbedded : QueryDictionary
	{
		public Assembly Assembly { get; set; }
		public string Extension { get; set; }
		public string NamespacePrefix { get; set; }
		private object _lock = new object();

		public static QueryDictionaryEmbedded LoadAssemblyOfTypeWithSqlQueries<T>(
			string namespacePrefix = null,
			string extension = ".sql",
			string headerStartText = "-- START TEST CODE",
			string headerEndText = "-- END TEST CODE",
			string headerReplacementText = null,
			Func<Query, bool> predicate = null,
			Func<Query, Query> mutator = null) =>
			LoadAssemblyOfType<T>(namespacePrefix, extension, headerStartText, headerEndText, headerReplacementText, predicate, mutator);

		public static QueryDictionaryEmbedded LoadAssemblyOfType<T>(
			string namespacePrefix = null,
			string extension = null,
			string headerStartText = null,
			string headerEndText = null,
			string headerReplacementText = null,
			Func<Query, bool> predicate = null,
			Func<Query, Query> mutator = null)
		{
			var q = new QueryDictionaryEmbedded()
			{
				Assembly = typeof(T).Assembly,
				NamespacePrefix = namespacePrefix,
				Extension = extension,
				HeaderReplacement = headerReplacementText ?? string.Empty,
				HeaderStartText = headerStartText,
				HeaderEndText = headerEndText,
				RemoveHeader = !string.IsNullOrEmpty(headerStartText) && !string.IsNullOrEmpty(headerEndText),
				Predicate = predicate,
				Mutator = mutator
			};

			q.Load();
			return q;
		}

		public override void Load()
		{
			int start = NamespacePrefix?.Length ?? 0;
			int end = Extension?.Length ?? 0;

			lock (_lock)
			{
				Queries.Clear();

				var queries = Assembly.GetManifestResourceNames()
					.Where(o => (Extension is null || o.EndsWith(Extension)) && (NamespacePrefix is null || o.StartsWith(NamespacePrefix)))
					.Select(o => new Query(o, o.Substring(start, o.Length - start - end), getManifest(o)));

				AddRange(queries);
			}
		}

		private string getManifest(string name)
		{
			using (var stream = Assembly.GetManifestResourceStream(name) ?? throw new InvalidOperationException($"Resource '{name}' not found in assembly '{Assembly.FullName}'."))
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}
	}
}