using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace QueryDictionary.Test
{
    public class TestQueryStore
    {
        [Fact]
        public void Load_GivenAFilename_ReturnsQueryStore()
        {
            var d = QueryDictionaryXml.ForFile(@"Queries.xml");

            d.Should().BeAssignableTo<QueryDictionary>();
            d.Should().BeAssignableTo<IQueryDictionary>();
            d.Should().BeAssignableTo<IReadOnlyDictionary<string, string>>();
        }

        [Fact]
        public void Load_GivenAStream_ReturnsQueryStore()
        {
			using (FileStream s = new FileStream(@"Queries.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var d = new QueryDictionaryXml() { Stream = s };

				d.Should().BeAssignableTo<QueryDictionary>();
                d.Should().BeAssignableTo<IQueryDictionary>();
                d.Should().BeAssignableTo<IReadOnlyDictionary<string, string>>();
            }
        }

		[Fact]
		public void Load_GivenAFilename_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryXml.ForFile(@"Queries.xml");

			d.Count.Should().Be(6);
			d.ContainsKey("Modify").Should().Be(true);
			d["Modify"].Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public void Load_GivenAFoldername_ReturnsQueryStore()
		{
			var f = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SqlQueries");
			var d = QueryDictionaryFolder.LoadSqlServerQueryFolder(f);

			d.Should().BeAssignableTo<QueryDictionary>();
			d.Should().BeAssignableTo<IQueryDictionary>();
			d.Should().BeAssignableTo<IReadOnlyDictionary<string, string>>();
		}

		[Fact]
		public void LoadEmbeddedFolder_GivenNothing_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryEmbedded.LoadAssemblyOfType<TestQueryStore>("QueryDictionary.Test.", ".sql");

			d.Count.Should().Be(3);
			d.Should().ContainKey("Embedded.Template");
			d.Should().ContainKey("Embedded2.Template1");
			d.Should().ContainKey("Embedded2.Template2");
		}

		[Fact]
		public void LoadEmbeddedFolder_GivenExtensionAndTwoLevels_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryEmbedded.LoadAssemblyOfType<TestQueryStore>(
				extension: ".sql",
				namespacePrefix: "QueryDictionary.Test."
			);

			d.Count.Should().Be(3);
			d.Should().ContainKey("Embedded.Template");
			d["Embedded.Template"].Should().Contain("-- START TEST CODE");

			d.Should().ContainKey("Embedded2.Template1");
			d["Embedded2.Template1"].Should().Contain("-- START TEST CODE");

			d.Should().ContainKey("Embedded2.Template2");
			d["Embedded2.Template2"].Should().Contain("-- START TEST CODE");
		}

		[Fact]
		public void LoadEmbeddedFolder_WithHeaderFilter_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryEmbedded.LoadAssemblyOfTypeWithSqlQueries<TestQueryStore>(
				namespacePrefix: "QueryDictionary.Test."
			);

			d.Count.Should().Be(3);
			d.Should().ContainKey("Embedded.Template");
			d["Embedded.Template"].Should().NotContain("-- START TEST CODE");

			d.Should().ContainKey("Embedded2.Template1");
			d["Embedded2.Template1"].Should().NotContain("-- START TEST CODE");

			d.Should().ContainKey("Embedded2.Template2");
			d["Embedded2.Template2"].Should().NotContain("-- START TEST CODE");
		}

		[Fact]
		public void LoadEmbeddedFolder_GivenEmbedFolderWithPrefix_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryEmbedded.LoadAssemblyOfType<TestQueryStore>(
				extension: ".sql",
				namespacePrefix: "QueryDictionary.Test.Embedded2."
			);

			d.Count.Should().Be(2);
			d.Should().ContainKey("Template1");
			d.Should().ContainKey("Template2");
		}

		[Fact]
		public void LoadEmbeddedFolder_GivenEmbedFolder2_ReturnsQueryStoreWithValues()
		{
			var d = QueryDictionaryEmbedded.LoadAssemblyOfType<TestQueryStore>(
				extension: ".sql",
				namespacePrefix: "QueryDictionary.Test.",
				mutator: q => new Query(q.RawKey, q.Key.Replace(".", "/"), q.Value)
			);

			d.Count.Should().Be(3);
			d.Should().ContainKey("Embedded/Template");
			d.Should().ContainKey("Embedded2/Template1");
			d.Should().ContainKey("Embedded2/Template2");
		}
    }
}
