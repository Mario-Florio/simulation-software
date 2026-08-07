using System.Collections.ObjectModel;
using Src.Core.Ports;

namespace Src.Core.Entities;

public class Transformation
{
	private static NullTracer _nullTracer = new();
	private static NullTracer.Span _nullSpan = new("null", _nullTracer);

	public enum StatusState { APPROVED, REJECTED, PENDING }
	public enum PolicyType { FIXED, SCALED }

	public static Dictionary<string, object> ToDict(Transformation transformation)
	{
		return new Dictionary<string, object>
		{
			["ID"] = transformation.ID,
			["Name"] = transformation.Name,
			["Status"] = transformation.Status,
			["Changes"] = transformation.Changes.Values,
			["Policy"] = transformation.Policy,
			["Scale"] = transformation.Scale
		};
	}

	private Guid _id = Guid.NewGuid();
	private string _name = "";
	private Dictionary<Guid, Change> _changes = new();
	private Condition? _condition = null;
	private PolicyType _policy = PolicyType.FIXED;

	public Guid ID { get => _id; }
	public string Name { get => _name; }
	public ReadOnlyDictionary<Guid, Change> Changes { get => new(_changes); }
	public StatusState Status = StatusState.PENDING;
	public PolicyType Policy { get => _policy; }
	public double Scale = 1.0;

	public Transformation(PolicyType policy = PolicyType.FIXED)
	{ _policy = policy; }

	public Transformation(string name, PolicyType policy = PolicyType.FIXED)
	{
		_name = name;
		_policy = policy;
	}

	public Transformation AddChange(Guid stateRef, double delta, Modifier? modifier = null)
	{
		if (_changes.ContainsKey(stateRef))
		{ _changes[stateRef].AccumulateDeltas(delta); }

		else

		{ _changes.Add(stateRef, new Change(delta)); }

		if (modifier != null) _changes[stateRef]!.AddModifier(modifier);

		return this;
	}
	public Transformation AddChange(Guid stateRef, Guid deltaRef, Modifier? modifier = null)
	{
		if (_changes.ContainsKey(stateRef))
		{ _changes[stateRef].AccumulateDeltas(deltaRef); }

		else

		{ _changes.Add(stateRef, new Change(deltaRef)); }

		if (modifier != null) _changes[stateRef]!.AddModifier(modifier);

		return this;
	}
	public Transformation AddCondition(Condition condition)
	{
		if (_condition == null) _condition = condition;
		else _condition!.AddCondition(condition);

		return this;
	}
	public void Execute(IPrivilegedStateContext stateContext, ITracer.ISpan? tickSpan = null)
	{
		if (tickSpan == null) tickSpan = _nullSpan;

		if (_condition != null && _condition.Resolve(stateContext) == false)
		{
			tickSpan.AddEvent("Transformation Condition Failed", new Dictionary<string, object>()
			{
				["Transformation ID"] = _id,
				["Transformation Name"] = _name,
				["Condition"] = _condition
			});

			return;
		}

		foreach (var (stateRef, change) in _changes)
		{
			var nullableChangeResult = change.Resolve(stateContext);

			if (nullableChangeResult == null) continue;

			double changeResult = (double)nullableChangeResult!;

			var initialVal = stateContext.GetValue(stateRef);

			stateContext.Change(
				stateRef,
				_policy == PolicyType.SCALED ? Scale * changeResult : changeResult
			);

			tickSpan.AddEvent("Change Executed", new Dictionary<string, object>()
			{
				["Transformation ID"] = _id,
				["Transformation Name"] = _name,
				["Scale"] = Scale,
				["Delta (scale applied)"] = _policy == PolicyType.SCALED ? Scale * changeResult : changeResult,
				["State Changed"] = stateContext.GetName(stateRef!)!,
				["State Value (after)"] = initialVal!,
				["State Value (before)"] = stateContext.GetValue(stateRef!)!,
			});
		}
	}
}

public class Change
{
	private static Dictionary<string, object> ToDict(Change change, IStateContext stateContext)
	{
		return new Dictionary<string, object>()
		{
			["Delta References"] = change.References(stateContext),
			["Delta (resolved)"] = change.Resolve(stateContext)!
		};
	}

	private double? _delta;
	private List<Guid> _stateRefs = new List<Guid>(); // Alternative source of delta.
							  // List is used to allow for accumulation of delta refs
							  // without needing immediate resolution.
	private Modifier? _modifier = null;

	public Modifier? Modifier { get => _modifier; }
	public double? Delta { get => _delta; }
	public List<string> References(IStateContext stateContext)
	{
		var refs = new List<string>();

		foreach (var stateRef in _stateRefs)
		{
			if (stateContext.IsNotNullRef(stateRef)) refs.Add(stateContext.GetName(stateRef)!);
		}

		return refs;
	}

	public Change(double delta)
	{ _delta = delta; }

	public Change(Guid stateRef)
	{ _stateRefs.Add(stateRef); }

	public Change(double delta, Modifier modifier)
	{
		_delta = delta;
		_modifier = modifier;
	}

	public Change(Guid stateRef, Modifier modifier)
	{
		_stateRefs.Add(stateRef);
		_modifier = modifier;
	}

	public double? Resolve(IStateContext stateContext)
	{
		double? result = null;

		if (_delta != null) result = _ResolveMod((double)_delta!, stateContext);

		if (_stateRefs.Count > 0)
		{

			foreach (var stateRef in _stateRefs)
			{
				if (!stateContext.IsNotNullRef(stateRef)) return result;

				var stateVal = (double)stateContext.GetValue(stateRef)!;

				var delta = result == null ? stateVal : stateVal + (double)result!;

				result = _ResolveMod(delta, stateContext);
			}
		}

		return result;
	}
	public void AccumulateDeltas(double delta)
	{ _delta += delta; }

	public void AccumulateDeltas(Guid deltaRef)
	{ _stateRefs.Add(deltaRef); }

	public void AddModifier(Modifier modifier)
	{
		if (_modifier == null) _modifier = modifier;
		else _modifier.AddModifier(modifier);
	}

	private double _ResolveMod(double delta, IStateContext stateContext)
	{
		var modifierVal = _modifier == null ? null : _modifier.Modify(delta, stateContext);
		return modifierVal == null ? delta : (double)modifierVal!;
	}
}

