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
		public int LevelsToInclude { get; set; }

		public static QueryDictionaryEmbedded LoadAssemblyOfTypeWithSqlQueries<T>(
			string extension = ".sql",
			int levelsToInclude = 1,
			string headerStartText = "-- START TEST CODE\r\n",
			string headerEndText = "-- END TEST CODE\r\n",
			string headerReplacementText = "\r\n",
			Func<Query, bool> predicate = null,
			Func<Query, Query> mutator = null) =>
			LoadAssemblyOfType<T>(extension, levelsToInclude, headerStartText, headerEndText, headerReplacementText, predicate, mutator);

		public static QueryDictionaryEmbedded LoadAssemblyOfType<T>(
			string extension = null,
			int levelsToInclude = 1,
			string headerStartText = null,
			string headerEndText = null,
			string headerReplacementText = null,
			Func<Query, bool> predicate = null,
			Func<Query, Query> mutator = null)
		{
			if (levelsToInclude < 1)
				throw new ArgumentOutOfRangeException(nameof(levelsToInclude), "Levels to include must be greater than 0.");

			var q = new QueryDictionaryEmbedded()
			{
				Assembly = typeof(T).Assembly,
				LevelsToInclude = levelsToInclude,
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
			Queries.Clear();

			var queries = Assembly.GetManifestResourceNames()
				.Where(o => Extension is null || o.EndsWith(Extension))
				.Select(o => new Query(o, getKey(o), getManifest(o)));

			foreach (var query in queries)
				Add(query);
		}

		private string getKey(string resourceName)
		{
			var parts = resourceName.Split('.');

			if (parts.Length < 2)
				throw new IndexOutOfRangeException("Resource name should contain at least 3 parts.");

			int l = LevelsToInclude + 2 > parts.Length ? parts.Length - 2 : LevelsToInclude;

			var name = string.Join(".", parts.Skip(parts.Length - l - 1).Take(l));
			return name;
		}

		private string getManifest(string name)
		{
			using (var stream = Assembly.GetManifestResourceStream(name) ?? throw new InvalidOperationException($"Resource '{name}' not found in assembly '{Assembly.FullName}'."))
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}
	}
}