using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;
using Src.Infra;

namespace Demo;

public class Program
{
	private static string VARIABLE_A = "VARIABLE A";
	private static string VARIABLE_B = "VARIABLE B";
	private static string VARIABLE_C = "VARIABLE C";

	public static void Main(string[] args)
	{
		var logFile = $"sim-trace.{Guid.NewGuid()}.ndjson";
		var sink = new FileSink(logFile);
		var exporter = new NdJsonExporter(sink);
		var tracer = new SimpleTracer(exporter);
		var simEngine = new SimulationEngine(tracer);

		var variableARef = simEngine.AddState(new Variable("A", 50.0, 0.0, 100.0));
		var variableBRef = simEngine.AddState(new Variable("B", 50.0, 0.0));
		var variableCRef = simEngine.AddState(new Variable("C", 50.0));

		var stateDict = new Dictionary<string, Guid>();
		stateDict.Add(VARIABLE_A, variableARef);
		stateDict.Add(VARIABLE_B, variableBRef);
		stateDict.Add(VARIABLE_C, variableCRef);

		var agentARef = simEngine.AddAgent(new AgentA("Agent A", stateDict));
		var agentBRef = simEngine.AddAgent(new AgentB("Agent B", stateDict));

		simEngine.Run(5);

		var lines = new List<string>();
		foreach (var line in File.ReadLines(logFile))
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			lines.Add(line);
		}

		var nodes = NdJsonParser.Parse(lines);
		PrettyPrinter.Print(nodes);
	}

	private class AgentA : IAgent
	{
		private Guid _id = Guid.NewGuid();
		private string _name;
		private Dictionary<string, Guid> _stateDict;

		public Guid ID { get => _id; }
		public string Name { get => _name; }

		public AgentA(string name, Dictionary<string, Guid> stateDict)
		{
			_name = name;
			_stateDict = stateDict;
		}

		public List<Transformation> Propose(SimulationContext simContext)
		{
			var transformations = new List<Transformation>();

			var transformation = new Transformation("Transformation A", Transformation.PolicyType.SCALED)
				.AddChange(_stateDict[VARIABLE_A]!, 20.0)
				.AddChange(
					_stateDict[VARIABLE_B]!,
					-1.0,
					new Multiplier(_stateDict[VARIABLE_C]!)
						.AddModifier(new Divisor(100.0))
				);

			transformations.Add(transformation);

			return transformations;
		}
	}
	private class AgentB : IAgent
	{
		private Guid _id = Guid.NewGuid();
		private string _name;
		private Dictionary<string, Guid> _stateDict;

		public Guid ID { get => _id; }
		public string Name { get => _name; }

		public AgentB(string name, Dictionary<string, Guid> stateDict)
		{
			_name = name;
			_stateDict = stateDict;
		}

		public List<Transformation> Propose(SimulationContext simContext)
		{
			var transformations = new List<Transformation>();

			var transformation = new Transformation("Transformation B")
				.AddChange(_stateDict[VARIABLE_C]!, 5.0)
				.AddChange(
					_stateDict[VARIABLE_A]!,
					-1.0,
					new Multiplier(_stateDict[VARIABLE_B]!)
						.AddModifier(new Divisor(100.0))
				);

			transformations.Add(transformation);

			return transformations;
		}

	}
}
