using System.IO;
using System.Text.Json.Nodes;
using Src.Adapters;

namespace Src.Infra;

public class PrettyPrinter
{
	public static void Print(List<JsonNode?> nodes)
	{ Console.WriteLine(GetPrettyString(nodes)); }

	public static string GetPrettyString(List<JsonNode?> nodes, int indentAmnt = 0)
	{
		var str = "";
		foreach (var node in nodes) str += GetPrettyString(node, indentAmnt);

		return str;
	}
	public static string GetPrettyString(JsonNode? node, int indentAmnt = 0, bool prefix = false)
	{
		var str = "";
		switch (node)
		{
			case JsonObject obj:
				str += _GetPrettyString(obj, indentAmnt, prefix);
				break;

			case JsonArray arr:
				str += _GetPrettyString(arr, indentAmnt, prefix);
				break;

			case JsonValue value:
				str += _GetPrettyString(value, indentAmnt, prefix);
				break;

			case null:
				var indent = new string('\t', indentAmnt);
				str += "null";
				break;
		}
		return str;
	}
	private static string _GetPrettyString(JsonObject obj, int indentAmnt = 0, bool prefix = false)
	{
		var count = _GetCount(obj);
		var nest = count > 3 ? true : false;
		var isZero = count == 0;

		var indent = new string('\t', indentAmnt);
		var str = prefix ? indent + "{" :
				nest || isZero ? "{" : "{ ";

		var i = 0;
		foreach (var (key, val) in obj)
		{
			if (nest)
			{
				str += '\n';
				str += GetPrettyString(key, indentAmnt + 1, true);
				str += ": ";
				str += GetPrettyString(val, indentAmnt + 1);
			}
			else
			{
				str += GetPrettyString(key);
				str += ": ";
				str += GetPrettyString(val);
			}

			i++;
			if (i < obj.Count) str += ", ";
		}
		str += nest ? '\n' + indent : isZero ? "" : " ";
		str += "}";

		return str;
	}
	private static string _GetPrettyString(JsonArray arr, int indentAmnt = 0, bool prefix = false)
	{
		var count = _GetCount(arr);
		var nest = count > 3 ? true : false;
		var isZero = count == 0;

		var indent = new string('\t', indentAmnt);
		var str = prefix ? indent + "[" :
				nest || isZero ? "[" : "[ ";

		var i = 0;
		foreach (var item in arr)
		{
			if (nest)
			{
				str += '\n';
				str += GetPrettyString(item, indentAmnt + 1, true);
			}
			else
			{
				str += GetPrettyString(item);
			}

			i++;
			if (i < arr.Count) str += ", ";
		}
		str += nest ? '\n' + indent : isZero ? "" : " ";
		str += "]";

		return str;
	}
	private static string _GetPrettyString(JsonValue value, int indentAmnt = 0, bool prefix = false)
	{
		var indent = new string('\t', indentAmnt);
		var str = prefix ? indent + value.ToString() : value.ToString();
		return str;
	}
	private static int _GetCount(JsonNode? node)
	{
		int count = 0;

		switch (node)
		{
			case JsonObject obj:
				count += _GetCount(obj);
				break;

			case JsonArray arr:
				count += _GetCount(arr);
				break;

			case JsonValue value:
				break;

			case null:
				break;
		}
		return count;
	}
	private static int _GetCount(JsonObject? obj)
	{
		if (obj == null) return 0;

		var count = obj.Count;

		foreach (var (name, child) in obj) count += _GetCount(child);

		return count;
	}
	private static int _GetCount(JsonArray? arr)
	{
		if (arr == null) return 0;

		var count = arr.Count;

		foreach (var child in arr) count += _GetCount(child);

		return count;
	}

}

