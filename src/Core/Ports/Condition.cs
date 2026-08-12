using System;
using Src.Core.Entities;

namespace Src.Core.Ports;

public abstract class Condition
{
	public enum ChainType { AND, OR }

	protected Guid _id = Guid.NewGuid();
	protected Term _base;
	protected Term _comparator;
	protected Condition? _condition;
	protected ChainType _chainType = ChainType.AND;

	public Guid ID { get => _id; }
	public Term Base { get => _base; }
	public Term Comparator { get => _comparator; }
	public Condition? condition { get => _condition; }

	public Condition(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
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
			["Condition"] = _condition == null ? "null" : _condition.ToDict()
		};
	}

	protected abstract bool _Resolve(double baseVal, double comparatorVal);

	public abstract class Term
	{
		protected Modifier? _modifier = null;

		public Modifier? Modifier { get => _modifier; }

		public abstract double? Resolve(IStateContext stateContext);

		public Term AddModifier(Modifier modifier)
		{
			if (_modifier == null) _modifier = modifier;
			else _modifier.AddModifier(modifier);
			return this;
		}

		public abstract Dictionary<string, object> ToDict();
	}
	public class ConstantTerm : Term
	{
		private double _val;

		public ConstantTerm(double val)
		{ _val = val; }

		public override double? Resolve(IStateContext stateContext)
		{ return _modifier == null ? _val : _modifier.Modify(_val, stateContext); }

		public override Dictionary<string, object> ToDict()
		{
			return new Dictionary<string, object>()
			{
				["Value"] = _val,
				["Modifier"] = _modifier == null ? "null" : _modifier.ToDict()
			};
		}
	}
	public class ReferenceTerm : Term
	{
		private Guid _valRef;

		public ReferenceTerm(Guid valRef)
		{ _valRef = valRef; }

		public override double? Resolve(IStateContext stateContext)
		{
			if (!stateContext.IsNotNullRef(_valRef)) return null;
			var val = (double)stateContext.GetValue(_valRef)!;
			return _modifier == null ? val : _modifier.Modify(val, stateContext);
		}

		public override Dictionary<string, object> ToDict()
		{
			return new Dictionary<string, object>()
			{
				["Value (reference)"] = _valRef,
				["Modifier"] = _modifier == null ? "null" : _modifier.ToDict()
			};
		}
	}
}

public class GreaterThan : Condition
{
	public GreaterThan(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal > comparatorVal; }
}

public class GreaterThanOrEqual : Condition
{
	public GreaterThanOrEqual(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
		: base(baseVal, comparator, chainType)
	{}

	protected override bool _Resolve(double baseVal, double comparatorVal)
	{ return baseVal >= comparatorVal; }
}


public class LesserThan : Condition
{
	public LesserThan(Term baseVal, Term comparator, ChainType chainType = ChainType.AND)
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

