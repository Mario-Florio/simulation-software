using System.IO;
using System.Text.Json.Nodes;

namespace Src.Adapters;

public class NdJsonParser
{
	public static List<JsonNode?> Parse(List<string> lines)
	{
		var nodes = new List<JsonNode?>();

		foreach (var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line)) continue;

			JsonNode? node = JsonNode.Parse(line);
			nodes.Add(node);
		}

		return nodes;
	}
}

