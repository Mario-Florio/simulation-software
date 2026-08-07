using Src.Core.Entities;

namespace Src.Core.Ports;

public interface IScheduler
{
	public void Load(List<Transformation> transformations);
	public bool Has();
	public Transformation Next(IStateContext stateContext, ITracer.ISpan span);
}

