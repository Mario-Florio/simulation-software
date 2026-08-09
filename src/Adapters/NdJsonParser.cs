using System.IO;
using System.Text.Json.Nodes;

namespace Src.Adapters;

public class NdJsonParser
{
	public static List<JsonNode?> Parse(string fileName)
	{
		if (!File.Exists(fileName)) throw new FileNotFoundException($"File {fileName} not found.");

		var nodes = new List<JsonNode?>();

		foreach (string line in File.ReadLines(fileName))
		{
			if (string.IsNullOrWhiteSpace(line)) continue;

			JsonNode? node = JsonNode.Parse(line);
			nodes.Add(node);
		}

		return nodes;
	}
}

