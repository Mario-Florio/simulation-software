using System;

namespace Src.Core.Entities;

public abstract class Modifier
{
	protected Value _val;
	protected Modifier? _modifier;

	public Modifier(Value val)
	{ _val = val; }

	public Modifier(double literalVal)
	{ _val = new Literal(literalVal); }

	public Modifier(Guid reference)
	{ _val = new Reference(reference); }

	public double? Modify(double baseVal, IStateContext stateContext)
	{
		double? modifierValue = _val.Evaluate(stateContext);

		if (modifierValue == null) return null;

		var result = _Modify(baseVal, (double)modifierValue);

		if (_modifier != null)
		{
			var childRes = _modifier.Modify(result, stateContext);
			result = childRes == null ? result : (double)childRes!;
		}

		return result;
	}
	public Modifier AddModifier(Modifier modifier)
	{
		if (_modifier == null) _modifier = modifier;
		else _modifier.AddModifier(modifier);
		return this;
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
			["Type (concrete)"] = concreteTypeName,
			["Value"] = _val.ToDict(),
			["Modifier"] = _modifier == null ? "null" : _modifier.ToDict()
		};
	}

	protected abstract double _Modify(double baseVal, double modifierVal);
}

public class Multiplier : Modifier
{
	public Multiplier(Value val) : base(val)
	{}
	public Multiplier(double literal) : base(literal)
	{}
	public Multiplier(Guid reference) : base(reference)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal * modifierVal; }
}

public class Divisor : Modifier
{
	public Divisor(Value val) : base(val)
	{}
	public Divisor(double literal) : base(literal)
	{}
	public Divisor(Guid reference) : base(reference)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal / modifierVal; }
}

public class Modulus : Modifier
{
	public Modulus(Value val) : base(val)
	{}
	public Modulus(double literal) : base(literal)
	{}
	public Modulus(Guid reference) : base(reference)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal % modifierVal; }
}

public class Subtractor : Modifier
{
	public Subtractor(Value val) : base(val)
	{}
	public Subtractor(double literal) : base(literal)
	{}
	public Subtractor(Guid reference) : base(reference)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal - modifierVal; }
}

public class Addend : Modifier
{
	public Addend(Value val) : base(val)
	{}
	public Addend(double literal) : base(literal)
	{}
	public Addend(Guid reference) : base(reference)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal + modifierVal; }
}

