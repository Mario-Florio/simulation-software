using System.Text.Json;
using Src.Core.Ports;

namespace Src.Adapters;

public interface ISpanExporter
{
	public void Export(ITracer.ISpan span);
}

public class NdJsonExporter : ISpanExporter
{
	private ISink _sink;

	public NdJsonExporter(ISink? sink = null)
	{
		if (sink == null)
			sink = new FileSink(
				Path.GetFileNameWithoutExtension(Path.GetRandomFileName()),
				".ndjson"
			);

		_sink = sink;
	}

	public void Export(ITracer.ISpan span)
	{ _sink.Out($"\n{JsonSerializer.Serialize(span)}\n"); }
}



public class ConsoleExporter : ISpanExporter
{
	private long? start = null;

	public void Export(ITracer.ISpan span)
	{
		Console.WriteLine(span.Name);
		Console.WriteLine($"\tElapsed Time: {span.Elapsed}");
		foreach (var (name, value) in span.Attributes) Console.WriteLine($"\t{name}: {_PrettyPrint(value, 1)}");

		Console.WriteLine("\tEvents:");
		foreach (var spanEvent in span.Events)
		{
			if (start == null) start = spanEvent.Timestamp;

			Console.WriteLine($"\t* {spanEvent.Name} - {(double)spanEvent.Timestamp - start}");
			foreach (var (name, value) in spanEvent.Attributes) Console.WriteLine($"\t\t{name}: {_PrettyPrint(value)}");
		}
	}

	private string _PrettyPrint(object value, int indent = 0)
	{
		var str = "";

		if (value.GetType() == typeof(Dictionary<string, double>))
		{
			var dict = (Dictionary<string, double>)value;
			var nest = dict.Count > 5;

			foreach (var (k, v) in dict)
			{
				str += nest ? '\n' : "";
				str += nest ? new string('\t', indent + 1) : ", ";
				str += $"{_PrettyPrint(k)}: {_PrettyPrint(v)}";
			}

			return str;
		}

		str += new string('\t', indent);
		str += Convert.ToString(value) ?? "NULL";
		return str;
	}
}
