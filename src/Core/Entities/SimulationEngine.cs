
namespace Src.Core.Entities;

public class SimulationEngine
{
	private int _tick = 0;

	public int Tick { get => _tick; }

	public void Run(int duration)
	{
		for (; _tick < duration; _tick++)
		{ Console.WriteLine(_tick); }
	}
}

