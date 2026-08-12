using System.Collections.ObjectModel;
using Src.Core.Ports;

namespace Src.Core.Entities;

public class SimulationEngine
{
	private Dictionary<Guid, IState> _simState = new();
	private Dictionary<Guid, IAgent> _agents = new();
	private int _tick = 0;
	private ITracer _tracer;
	private TransformationResolver _transformationResolver;
	private IScheduler _scheduler;
	private IPrivilegedStateContext _stateContext;
	private SimulationContext _simContext;

	public int Tick { get => _tick; }
	public IStateContext StateContext { get => (IStateContext)_stateContext; }

	public SimulationEngine(ITracer? tracer = null)
	{
		_tracer = tracer ?? new NullTracer();
		_transformationResolver = new TransformationResolver(_tracer);
		_scheduler = new Scheduler(_tracer);
		_stateContext = new StateContext(_simState);
		_simContext = new SimulationContext(this);
	}
	public SimulationEngine(IScheduler? scheduler = null, ITracer? tracer = null)
	{
		_tracer = tracer ?? new NullTracer();
		_transformationResolver = new TransformationResolver(_tracer);
		_scheduler = scheduler ?? new Scheduler(_tracer);
		_stateContext = new StateContext(_simState);
		_simContext = new SimulationContext(this);
	}

	public void Run(int duration)
	{
		var simulationSpan = _tracer.StartSpan("Simulation Run");

		simulationSpan.AddAttribute("State", new ReadOnlyDictionary<Guid, IState>(_simState));
		simulationSpan.AddAttribute("Agents", new ReadOnlyDictionary<Guid, IAgent>(_agents));

		for (; _tick < duration; _tick++)
		{
			var tickSpan = _tracer.StartSpan($"Tick {_tick}", simulationSpan);
			var transformations = new List<Transformation>();

			foreach (var agent in _agents.Values) transformations.AddRange(agent.Propose(_simContext));

			tickSpan.AddAttribute(
				"State Snapshot",
				_simState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value)
			);

			tickSpan.AddAttribute(
				"Transformations",
				transformations.ToDictionary(transformation => transformation.ID,
							     transformation => transformation.ToDict())
			);

			_scheduler.Load(transformations);
			while (_scheduler.Has())
			{
				var transformation = _scheduler.Next(_stateContext, tickSpan);

				_transformationResolver.Resolve(transformation, _stateContext, tickSpan);

				if (transformation.Status.Equals(Transformation.StatusState.APPROVED))
				{ transformation.Execute(_stateContext, tickSpan); }
			}

			tickSpan.End();
		}

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

