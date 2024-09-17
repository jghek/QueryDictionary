using System;
using System.IO;

namespace QueryDictionary
{
	public class QueryDictionaryFolder : QueryDictionary
	{
		public string FolderName { get; set; }
		public string SearchPattern { get; set; }
		private object _lock = new object();

		public static QueryDictionaryFolder LoadSqlServerQueryFolder(string folderName)
		{
			var q = new QueryDictionaryFolder()
			{
				FolderName = folderName,
				SearchPattern = "*.sql",
				RemoveHeader = true,
				HeaderReplacement = string.Empty,
				HeaderStartText = "-- START TEST CODE\r\n",
				HeaderEndText = "-- END TEST CODE\r\n"
			};

			q.Load();
			return q;
		}

		public override void Load()
		{
            lock (_lock)
            {
				Queries.Clear();

				foreach (string fileName in Directory.EnumerateFiles(FolderName, SearchPattern, SearchOption.AllDirectories))
					Add(new Query(fileName, Path.GetFileNameWithoutExtension(fileName), File.ReadAllText(fileName)));
            }
		}
	}
}