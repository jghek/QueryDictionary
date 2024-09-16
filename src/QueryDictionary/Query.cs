namespace QueryDictionary
{
	public class Query
	{
		public string RawKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }

		public Query(string rawKey, string key, string value)
		{
			RawKey = rawKey;
			Key = key;
			Value = value;
		}
	}
}