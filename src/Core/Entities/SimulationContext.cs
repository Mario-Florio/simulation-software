
namespace Src.Core.Entities;

public class SimulationContext
{
	private SimulationEngine _simEngine;

	public IStateContext StateContext { get => _simEngine.StateContext; }
	public int Tick { get => _simEngine.Tick; }

	public SimulationContext(SimulationEngine simEngine)
	{ _simEngine = simEngine; }
}

