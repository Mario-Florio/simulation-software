using System;
using Src.Core.Entities;

namespace Src.Core.Ports;

public abstract class Condition
{
	public enum ChainType { AND, OR }

	protected Guid _id = Guid.NewGuid();
	protected Value _base;
	protected Value _comparator;
	protected Condition? _condition;
	protected ChainType _chainType = ChainType.AND;

	public Guid ID { get => _id; }
	public Value Base { get => _base; }
	public Value Comparator { get => _comparator; }
	public Condition? condition { get => _condition; }

	public Condition(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
	{
		_base = baseVal;
		_comparator = comparator;
		_chainType = chainType;
	}

	public bool Resolve(IStateContext stateContext)
	{
		var result = false;

		var baseVal = _base.Evaluate(stateContext);
		var comparatorVal = _comparator.Evaluate(stateContext);

		if (baseVal == null || comparatorVal == null) return result;

		result = _Resolve((double)baseVal!, (double)comparatorVal!);

		if (_condition == null) return result;

		if (_chainType == ChainType.AND) result = result && _condition.Resolve(stateContext);
		if (_chainType == ChainType.OR) result = result || _condition.Resolve(stateContext);

		return result;
	}
	public void AddCondition(Condition condition)
	{
		if (_condition == null) _condition = condition;
		else _condition.AddCondition(condition);
	}
	public Dictionary<string, object> ToDict()
	{
		var concreteTypeFullName = GetType().ToString();
		int lastNamespace = concreteTypeFullName.LastIndexOf('.');
		var concreteTypeName = (lastNamespace != -1)
			? concreteTypeFullName.Substring(lastNamespace + 1)
			: concreteTypeFullName;

		return new Dictionary<string, object>()
		{
			["ID"] = _id,
			["Type (concrete)"] = concreteTypeName,
			["Base"] = _base.ToDict(),
			["Comparator"] = _comparator.ToDict(),
			["Condition"] = _condition == null ? "null" : _condition.ID
		};
	}

	protected abstract bool _Resolve(double baseVal, double comparatorVal);
}

public class GreaterThan : Condition
{
	public GreaterThan(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal > comparatorVal; }
}

public class GreaterThanOrEqual : Condition
{
	public GreaterThanOrEqual(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal >= comparatorVal; }
}


public class LesserThan : Condition
{
	public LesserThan(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal < comparatorVal; }
}

public class LesserThanOrEqual : Condition
{
	public LesserThanOrEqual(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal <= comparatorVal; }
}

public class EqualTo : Condition
{
	public EqualTo(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal == comparatorVal; }
}

public class NotEqualTo : Condition
{
	public NotEqualTo(Value baseVal, Value comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal != comparatorVal; }
}

