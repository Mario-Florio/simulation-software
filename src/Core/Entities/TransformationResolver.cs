using Src.Core.Ports;

namespace Src.Core.Entities;

public class TransformationResolver
{
	private static NullTracer _nullTracer = new();
	private static NullTracer.Span _nullSpan = new("null", _nullTracer);

	private ITracer _tracer = _nullTracer;

	public TransformationResolver()
	{}

	public TransformationResolver(ITracer tracer)
	{ _tracer = tracer; }

	public void Resolve(
		Transformation transformation,
		IPrivilegedStateContext stateContext,
		ITracer.ISpan? tickSpan = null
	) {
		if (tickSpan == null) tickSpan = _nullSpan;

		var guiltyChanges = new List<Guid>();

		foreach (var (stateRef, change) in transformation.Changes)
		{
			if (!stateContext.IsNotNullRef(stateRef)) continue;

			var value = (double)stateContext.GetValue(stateRef)!;
			var delta = (double)change.Resolve((IStateContext)stateContext)!;
			var max = (double)stateContext.GetMaximum(stateRef)!;
			var min = (double)stateContext.GetMinimum(stateRef)!;

			if (transformation.Policy.Equals(Transformation.PolicyType.FIXED))
			{
				var changeIsValid = _ChangeIsValid(value, delta, min, max);

				transformation.Status = changeIsValid ?
					Transformation.StatusState.APPROVED : Transformation.StatusState.REJECTED;

				if (!changeIsValid) guiltyChanges.Add(change.ID);
			}
			else if (transformation.Policy.Equals(Transformation.PolicyType.SCALED))
			{
				transformation.Scale = Math.Min(transformation.Scale, _GetScale(value, delta, min, max));

				if (transformation.Scale == 0.0)
				{
					transformation.Status = Transformation.StatusState.REJECTED;
					guiltyChanges.Add(change.ID);
				}
				else
				{ transformation.Status = Transformation.StatusState.APPROVED; }
			}
		}

		tickSpan.AddEvent(
			transformation.Status == Transformation.StatusState.APPROVED ?
				"Transformation Approved" : "Transformation Rejected",
				_GetEventDict(transformation.ID, guiltyChanges)
		);
	}

	private bool _ChangeIsValid(double value, double delta, double min, double max)
	{
		if (max < value + delta) return false;
		if (min > value + delta) return false;

		return true;
	}
	private double _GetScale(double value, double delta, double min, double max)
	{
		if (value + delta > max) return (max - value) / delta;
		if (value + delta < min) return (min - value) / delta;
		return 1.0;
	}
	private Dictionary<string, object> _GetEventDict(Guid transformationID, List<Guid> guiltyChanges)
	{
		return new Dictionary<string, object>()
		{
			["Transformation ID"] = transformationID,
			["Guilty Changes"] = guiltyChanges
		};
	}
}

