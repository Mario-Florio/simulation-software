using Src.Core.Ports;

namespace Src.Core.Entities;

public class SimulationEngine
{
	private Dictionary<Guid, IState> _simState = new();
	private Dictionary<Guid, IAgent> _agents = new();
	private int _tick = 0;
	private ITracer _tracer;
	private TransformationResolver _transformationResolver;
	private IPrivilegedStateContext _stateContext;
	private SimulationContext _simContext;

	public int Tick { get => _tick; }
	public IStateContext StateContext { get => (IStateContext)_stateContext; }

	public SimulationEngine(ITracer? tracer = null)
	{
		_tracer = tracer ?? new NullTracer();
		_transformationResolver = new TransformationResolver(_tracer);
		_stateContext = new StateContext(_simState);
		_simContext = new SimulationContext(this);
	}

	public void Run(int duration)
	{
		var simulationSpan = _tracer.StartSpan("Simulation Run");

		var initialState = new Dictionary<string, double>();
		foreach (var state in _simState.Values) initialState.Add(state.Name, state.Value);
		simulationSpan.AddAttribute("Initial State", initialState);

		for (; _tick < duration; _tick++)
		{
			var tickSpan = _tracer.StartSpan($"Tick {_tick}", simulationSpan);
			var transformations = new List<Transformation>();

			foreach (var agent in _agents.Values) transformations.AddRange(agent.Propose(_simContext));

			tickSpan.End();
		}

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
	public Guid AddAgent(IAgent agent)
	{
		var UID = Guid.NewGuid();
		_agents.Add(UID, agent);
		return UID;
	}

}

