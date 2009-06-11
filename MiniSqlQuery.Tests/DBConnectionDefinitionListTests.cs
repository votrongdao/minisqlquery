using System;
using MiniSqlQuery.Core;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MiniSqlQuery.Tests
{
	[TestFixture]
	public class DbConnectionDefinitionListTests
	{
		[Test]
		public void Can_Deserialize_a_DbConnectionDefinitionList_from_XML_string()
		{
			string xml =
				@"<DbConnectionDefinitionList xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <DefaultName>default</DefaultName>
  <Definitions>
    <DbConnectionDefinition>
      <Name>nm1</Name>
      <ConnectionString>cs1</ConnectionString>
    </DbConnectionDefinition>
    <DbConnectionDefinition>
      <Name>nm2</Name>
      <ConnectionString>cs2</ConnectionString>
    </DbConnectionDefinition>
  </Definitions>
</DbConnectionDefinitionList>";

			DbConnectionDefinitionList definitionList = DbConnectionDefinitionList.FromXml(xml);
			Assert.That(definitionList.DefaultName, Is.EqualTo("default"));
			Assert.That(definitionList.Definitions[0].Name, Is.EqualTo("nm1"));
			Assert.That(definitionList.Definitions[0].ConnectionString, Is.EqualTo("cs1"));
			Assert.That(definitionList.Definitions[1].Name, Is.EqualTo("nm2"));
		}

		[Test]
		public void Can_serialize_a_DbConnectionDefinitionList()
		{
			DbConnectionDefinitionList definitionList = new DbConnectionDefinitionList();
			definitionList.DefaultName = "def";
			definitionList.Definitions = new[]
			                             {
			                             	new DbConnectionDefinition {ConnectionString = "cs1", Name = "nm1", ProviderName = "p1"},
			                             	new DbConnectionDefinition {ConnectionString = "cs2", Name = "nm2", ProviderName = "p2"}
			                             };
			string xml = definitionList.ToXml();
			Console.WriteLine(xml);
			Assert.That(xml, Text.Contains("<DefaultName>def</DefaultName>"));
			Assert.That(xml, Text.Contains("<Name>nm1</Name>"));
			Assert.That(xml, Text.Contains("<ConnectionString>cs1</ConnectionString>"));
			Assert.That(xml, Text.Contains("<ProviderName>p1</ProviderName>"));
			Assert.That(xml, Text.Contains("<Name>nm2</Name>"));
			Assert.That(xml, Text.Contains("<ConnectionString>cs2</ConnectionString>"));
			Assert.That(xml, Text.Contains("<ProviderName>p2</ProviderName>"));
		}

		[Test]
		public void Can_upgrade_from_old_style()
		{
			ConnectionDefinition[] connDefs = ConnectionDefinition.Parse(new[]
				 {
         			"Connection1^sql^Server=.; Database=master; Integrated Security=SSPI",
         			"Connection2^oracle^Server=.; Database=fred; user=bob; password=p"
				 });

			DbConnectionDefinitionList definitionList = DbConnectionDefinitionList.Upgrade(connDefs, "newDefault");

			Assert.That(definitionList.DefaultName, Is.EqualTo("newDefault"));
			Assert.That(definitionList.Definitions.LongLength, Is.EqualTo(connDefs.Length));

			Assert.That(definitionList.Definitions[0].ConnectionString, Is.EqualTo("Server=.; Database=master; Integrated Security=SSPI"));
			Assert.That(definitionList.Definitions[0].Name, Is.EqualTo("Connection1"));
			Assert.That(definitionList.Definitions[0].ProviderName, Is.EqualTo("sql"));

			Assert.That(definitionList.Definitions[1].ConnectionString, Is.EqualTo("Server=.; Database=fred; user=bob; password=p"));
			Assert.That(definitionList.Definitions[1].Name, Is.EqualTo("Connection2"));
			Assert.That(definitionList.Definitions[1].ProviderName, Is.EqualTo("oracle"));
		}
	}
}