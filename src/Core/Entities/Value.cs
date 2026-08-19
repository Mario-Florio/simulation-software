using Src.Core.Ports;

namespace Src.Core.Entities;

#pragma warning disable CS0660
#pragma warning disable CS0661

public abstract class Value
{
	protected Modifier? _modifier = null;

	public Modifier? Modifier { get => _modifier; }

	public abstract Value Copy();
	public abstract double? Evaluate(IStateContext stateContext);

	public Value AddModifier(Modifier modifier)
	{
		if (_modifier == null) _modifier = modifier;
		else _modifier.AddModifier(modifier);
		return this;
	}

	public abstract Dictionary<string, object> ToDict();

	public static Value operator -
		(Value value, Value subtractor) => value.AddModifier(new Subtractor(subtractor));
	public static Value operator -
		(Value value, double subtractor) => value.AddModifier(new Subtractor(new Literal(subtractor)));
	public static Value operator -
		(Value value, Guid subtractor) => value.AddModifier(new Subtractor(new Reference(subtractor)));
	public static Value operator -
		(double value, Value subtractor) => new Literal(value).AddModifier(new Subtractor(subtractor));
	public static Value operator -
		(Guid value, Value subtractor) => new Reference(value).AddModifier(new Subtractor(subtractor));

	public static Value operator +
		(Value value, Value addend) => value.AddModifier(new Addend(addend));
	public static Value operator +
		(Value value, double addend) => value.AddModifier(new Addend(new Literal(addend)));
	public static Value operator +
		(Value value, Guid addend) => value.AddModifier(new Addend(new Reference(addend)));
	public static Value operator +
		(double value, Value addend) => new Literal(value).AddModifier(new Addend(addend));
	public static Value operator +
		(Guid value, Value addend) => new Reference(value).AddModifier(new Addend(addend));

	public static Value operator *
		(Value value, Value multiplier) => value.AddModifier(new Multiplier(multiplier));
	public static Value operator *
		(Value value, double multiplier) => value.AddModifier(new Multiplier(new Literal(multiplier)));
	public static Value operator *
		(Value value, Guid multiplier) => value.AddModifier(new Multiplier(new Reference(multiplier)));
	public static Value operator *
		(double value, Value multiplier) => new Literal(value).AddModifier(new Multiplier(multiplier));
	public static Value operator *
		(Guid value, Value multiplier) => new Reference(value).AddModifier(new Multiplier(multiplier));

	public static Value operator /
		(Value value, Value divisor) => value.AddModifier(new Divisor(divisor));
	public static Value operator /
		(Value value, double divisor) => value.AddModifier(new Divisor(new Literal(divisor)));
	public static Value operator /
		(Value value, Guid divisor) => value.AddModifier(new Divisor(new Reference(divisor)));
	public static Value operator /
		(double value, Value divisor) => new Literal(value).AddModifier(new Divisor(divisor));
	public static Value operator /
		(Guid value, Value divisor) => new Reference(value).AddModifier(new Divisor(divisor));

	public static Value operator %
		(Value value, Value modulus) => value.AddModifier(new Modulus(modulus));
	public static Value operator %
		(Value value, double modulus) => value.AddModifier(new Modulus(new Literal(modulus)));
	public static Value operator %
		(Value value, Guid modulus) => value.AddModifier(new Modulus(new Reference(modulus)));
	public static Value operator %
		(double value, Value modulus) => new Literal(value).AddModifier(new Modulus(modulus));
	public static Value operator %
		(Guid value, Value modulus) => new Reference(value).AddModifier(new Modulus(modulus));

	public static Value operator -
		(Value value) => value.AddModifier(new Multiplier(new Literal(-1)));

	public static Condition operator ==
		(Value baseVal, Value comparator) => new EqualTo(baseVal, comparator);
	public static Condition operator !=
		(Value baseVal, Value comparator) => new NotEqualTo(baseVal, comparator);

	public static Condition operator >
		(Value baseVal, Value comparator) => new GreaterThan(baseVal, comparator);
	public static Condition operator >=
		(Value baseVal, Value comparator) => new GreaterThanOrEqual(baseVal, comparator);
	public static Condition operator <
		(Value baseVal, Value comparator) => new LesserThan(baseVal, comparator);
	public static Condition operator <=
		(Value baseVal, Value comparator) => new LesserThanOrEqual(baseVal, comparator);
}

#pragma warning restore CS0660
#pragma warning restore CS0661

public class Literal : Value
{
	private double _val;

	public double Value { get => _val; }

	public Literal(double val)
	{ _val = val; }

	public override Value Copy()
	{  return new Literal(_val); }

	public override double? Evaluate(IStateContext stateContext)
	{ return _modifier == null ? _val : _modifier.Modify(_val, stateContext); }

	public override Dictionary<string, object> ToDict()
	{
		return new Dictionary<string, object>()
		{
			["Value (literal)"] = _val,
			["Modifier"] = _modifier == null ? "null" : _modifier.ToDict()
		};
	}
}

public class Reference : Value
{
	private Guid _valRef;

	public Guid Value { get => _valRef; }

	public Reference(Guid valRef)
	{ _valRef = valRef; }

	public override Value Copy()
	{ return new Reference(_valRef); }

	public override double? Evaluate(IStateContext stateContext)
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

