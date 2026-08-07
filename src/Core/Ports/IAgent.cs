namespace Src.Core.Entities;

public interface IAgent
{
	public Guid ID { get; }
	public string Name { get; }

	public List<Transformation> Propose(SimulationContext simContext);
}

