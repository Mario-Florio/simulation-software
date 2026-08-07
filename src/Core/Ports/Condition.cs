using Src.Core.Entities;

namespace Src.Core.Ports;

public abstract class Condition
{
	public enum ChainType { AND, OR }

	protected ITerm _base;
	protected ITerm _comparator;
	protected Condition? _condition;
	protected ChainType _chainType = ChainType.AND;

	public ITerm Base { get => _base; }
	public ITerm Comparator { get => _comparator; }
	public Condition? condition { get => _condition; }

	public Condition(ITerm baseVal, ITerm comparator, ChainType chainType = ChainType.AND)
	{
		_base = baseVal;
		_comparator = comparator;
		_chainType = chainType;
	}

	public bool Resolve(IStateContext stateContext)
	{
		var result = false;

		var baseVal = _base.Resolve(stateContext);
		var comparatorVal = _comparator.Resolve(stateContext);

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

	protected abstract bool _Resolve(double baseVal, double comparatorVal);

	public interface ITerm
	{
		public double? Resolve(IStateContext stateContext);
	}

	public class Term : ITerm
	{
		private double _val;

		public Term(double val)
		{ _val = val; }

		public double? Resolve(IStateContext stateContext)
		{ return _val; }
	}
	public class ReferenceTerm : ITerm
	{
		private Guid _valRef;

		public ReferenceTerm(Guid valRef)
		{ _valRef = valRef; }

		public double? Resolve(IStateContext stateContext)
		{ return stateContext.GetValue(_valRef); }
	}
}

public class GreaterThan : Condition
{
	public GreaterThan(ITerm baseVal, ITerm comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal > comparatorVal; }
}

public class GreaterThanOrEqual : Condition
{
	public GreaterThanOrEqual(ITerm baseVal, ITerm comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal >= comparatorVal; }
}


public class LesserThan : Condition
{
	public LesserThan(ITerm baseVal, ITerm comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal < comparatorVal; }
}

public class LesserThanOrEqual : Condition
{
	public LesserThanOrEqual(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal <= comparatorVal; }
}

public class EqualTo : Condition
{
	public EqualTo(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal == comparatorVal; }
}

