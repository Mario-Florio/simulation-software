using System.Collections.ObjectModel;
using Src.Core.Ports;

namespace Src.Core.Entities;

public class Transformation
{
	private static NullTracer _nullTracer = new();
	private static NullTracer.Span _nullSpan = new("null", _nullTracer);

	public enum StatusState { APPROVED, REJECTED, PENDING }
	public enum PolicyType { FIXED, SCALED }

	private Guid _id = Guid.NewGuid();
	private string _name = "";
	private Dictionary<Guid, Change> _changes = new();
	private Condition? _condition = null;
	private PolicyType _policy = PolicyType.FIXED;

	public Guid ID { get => _id; }
	public string Name { get => _name; }
	public ReadOnlyDictionary<Guid, Change> Changes { get => new(_changes); }
	public Condition? Condition => _condition;
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

	public Transformation AddChange(Guid stateRef, Value delta)
	{
		if (_changes.ContainsKey(stateRef))
		{ _changes[stateRef].AccumulateDeltas(delta); }

		else
		{ _changes.Add(stateRef, new Change(stateRef, delta)); }

		return this;
	}
	public Transformation AddChange(Guid stateRef, double delta, Modifier? modifier = null)
	{
		var value = new Literal(delta);

		if (modifier != null) value.AddModifier(modifier);

		return AddChange(stateRef, value);
	}
	public Transformation AddChange(Guid stateRef, Guid deltaRef, Modifier? modifier = null)
	{
		var value = new Reference(deltaRef);

		if (modifier != null) value.AddModifier(modifier);

		return AddChange(stateRef, value);
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
				["Condition ID"] = _condition.ID
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
				["Change ID"] = change.ID
			});
		}
	}
	public Dictionary<string, object> ToDict()
	{
		return new Dictionary<string, object>
		{
			["ID"] = _id,
			["Name"] = _name,
			["Status"] = Status,
			["Changes"] = _changes.Values.Select(n => n.ID),
			["Condition"] = _condition == null ? "null" : _condition.ID,
			["Policy"] = _policy == PolicyType.SCALED ? "Scaled" : "Fixed",
			["Scale"] = Scale
		};
	}
}

public class Change
{
	private Guid _id = Guid.NewGuid();
	private Guid _targetRef;
	private Value _delta;

	public Guid ID { get => _id; }
	public Guid TargetRef { get => _targetRef; }
	public Value Delta { get => _delta; }

	public Change(Guid targetRef, Value delta)
	{
		_targetRef = targetRef;
		_delta = delta;
	}

	public double? Resolve(IStateContext stateContext)
	{ return _delta.Evaluate(stateContext); }

	public void AccumulateDeltas(Value delta)
	{ _delta.AddModifier(new Addend(delta)); }

	public void AddModifier(Modifier modifier)
	{ _delta.AddModifier(modifier); }

	public Dictionary<string, object> ToDict()
	{
		return new Dictionary<string, object>()
		{
			["ID"] = _id,
			["Target Reference"] = _targetRef,
			["Delta"] = _delta.ToDict()
		};
	}
}

