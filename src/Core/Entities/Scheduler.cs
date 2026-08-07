using System.Collections.ObjectModel;
using Src.Core.Ports;

namespace Src.Core.Entities;

public interface IScheduler
{
	public void Load(List<Transformation> transformations);
	public bool Has();
	public Transformation Next(IStateContext stateContext, ITracer.ISpan tickSpan);
}

public class Scheduler : IScheduler
{
	private static NullTracer _nullTracer = new();
	private static NullTracer.Span _nullSpan = new("null", _nullTracer);

	private List<Transformation> _transformations = new();
	private ITracer _tracer = _nullTracer;

	public Scheduler()
	{}

	public Scheduler(ITracer tracer)
	{ _tracer = tracer; }

	public void Load(List<Transformation> transformations)
	{ _transformations.AddRange(transformations); }

	public bool Has()
	{ return _transformations.Count > 0; }

	public Transformation Next(IStateContext stateContext, ITracer.ISpan? tickSpan = null)
	{
		if (tickSpan == null) tickSpan = _nullSpan;

		var nextRef = _GetNextRef(stateContext);
		var nextIdx = _GetNextTransformationIdx(nextRef, stateContext);

		var transformation = _transformations[nextIdx];
		_transformations.RemoveAt(nextIdx);

		return transformation;
	}

	private bool _TransformationsContainsChange(Guid stateRef)
	{
		return _transformations.Any(entry => entry.Changes.Any(changeEntry => changeEntry.Key == stateRef));
	}
	private Guid _GetNextRef(IStateContext stateContext)
	{
		var stateCopy = stateContext.GetAll();
		Guid smallestValFromMinRef = Guid.NewGuid();
		double smallestValFromMin = double.PositiveInfinity;

		foreach (var (stateRef, value) in stateCopy)
		{
			if (!stateContext.IsNotNullRef(stateRef)) continue;

			var valFromMin = value - (double)stateContext.GetMinimum(stateRef)!;

			if (_TransformationsContainsChange(stateRef) && valFromMin <= smallestValFromMin)
			{
				smallestValFromMinRef = stateRef;
				smallestValFromMin = valFromMin;
			}
		}

		return smallestValFromMinRef;
	}
	private int _GetNextTransformationIdx(Guid stateRef, IStateContext stateContext)
	{
		int greatestChangeIdx = 0;

		for (int i = 0; i < _transformations.Count; i++)
		{
			var t = _transformations[i];
			double greatestIncreaseValue = double.MinValue;

			foreach (var (sRef, change) in t.Changes)
			{
				if (sRef != stateRef) continue;

				if (change.Resolve(stateContext) > greatestIncreaseValue)
				{
					var changeResult = change.Resolve(stateContext);
					if (changeResult == null) continue;

					greatestIncreaseValue = (double)changeResult!;
					greatestChangeIdx = i;
				}
			}
		}

		return greatestChangeIdx;
	}
}

