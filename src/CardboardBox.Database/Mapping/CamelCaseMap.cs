using Dapper.FluentMap.Conventions;
using System.Text.RegularExpressions;

namespace CardboardBox.Database.Mapping;

/// <summary>
/// The camel_case naming convention for Dapper Fluent
/// </summary>
public class CamelCaseMap : Convention
{
	/// <summary>
	/// The camel_case naming convention for Dapper Fluent
	/// </summary>
	public CamelCaseMap()
	{
		Properties()
			.Configure(c =>
				c.Transform(s =>
					Regex.Replace(
						input: s,
						pattern: "([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])",
						replacement: "$1$3_$2$4"
					).ToLower()
				)
			);
	}
}
