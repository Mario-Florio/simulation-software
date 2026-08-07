using Src.Core.Ports;

namespace Src.Core.Entities;

public class SimulationEngine
{
	private Dictionary<Guid, IState> _simState = new();
	private int _tick = 0;
	private ITracer _tracer;
	private TransformationResolver _transformationResolver;

	public int Tick { get => _tick; }

	public SimulationEngine(ITracer? tracer = null)
	{
		_tracer = tracer ?? new NullTracer();
		_transformationResolver = new TransformationResolver(_tracer);
	}

	public void Run(int duration)
	{
		var simulationSpan = _tracer.StartSpan("Simulation Run");

		var initialState = new Dictionary<string, double>();
		foreach (var state in _simState.Values) initialState.Add(state.Name, state.Value);
		simulationSpan.AddAttribute("Initial State", initialState);

		for (; _tick < duration; _tick++)
		{ Console.WriteLine(_tick); }

		var finalState = new Dictionary<string, double>();
		foreach (var state in _simState.Values) finalState.Add(state.Name, state.Value);
		simulationSpan.AddAttribute("Final State", finalState);

		simulationSpan.End();
	}
	public Guid AddState(IState state)
	{
		var UID = Guid.NewGuid();
		_simState.Add(UID, state);
		return UID;
	}
}

