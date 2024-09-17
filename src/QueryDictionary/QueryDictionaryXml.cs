using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace QueryDictionary
{
	public class QueryDictionaryXml : QueryDictionary
	{
		public Stream Stream { get; set; }
		public string XPathQuery { get; set; } = "//Query";
		public string XPathNameSubQuery { get; set; } = "@Name";
		public string XPathValueSubQuery { get; set; } = ".";

		public static QueryDictionaryXml ForFile(string fileName)
		{
			using (FileStream s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var q = new QueryDictionaryXml() { Stream = s };
				q.Load();
				return q;
			}
		}

		public override void Load()
		{
			lock (Lock)
			{
				Queries.Clear();
				var doc = XDocument.Load(Stream);
				var elements = doc.XPathSelectElements(XPathQuery);

				foreach (XElement e in elements)
				{
					var name = getXPathResult(e, XPathNameSubQuery);
					var value = getXPathResult(e, XPathValueSubQuery);
					Add(new Query(name, name, value));
				}
			}
		}

		private static string getXPathResult(XElement e, string xPath)
		{
			var result = e.XPathEvaluate(xPath);
			if (result is IEnumerable<object> enumerable)
			{
				var first = enumerable.FirstOrDefault();
				if (first is XAttribute attribute)
					return attribute.Value;
				else if (first is XElement element)
					return element.Value;
			}

			return null;
		}
	}
}