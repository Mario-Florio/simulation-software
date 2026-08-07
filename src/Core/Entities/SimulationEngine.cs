using Src.Core.Ports;

namespace Src.Core.Entities;

public class SimulationEngine
{
	private Dictionary<Guid, IState> _simState = new();
	private int _tick = 0;

	public int Tick { get => _tick; }

	public void Run(int duration)
	{
		Console.WriteLine($"Simulation State Count: {_simState.Count}");

		for (; _tick < duration; _tick++)
		{ Console.WriteLine(_tick); }

		foreach (var (stateRef, state) in _simState)
			Console.WriteLine($"State Ref: {stateRef}, Name: {state.Name}, Value: {state.Value}");
	}
	public Guid AddState(IState state)
	{
		var UID = Guid.NewGuid();
		_simState.Add(UID, state);
		return UID;
	}
}

