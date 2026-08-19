using Src.Core.Ports;

namespace Src.Core.Entities;

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

	public static Value operator +
		(Value value, Value addend) => value.AddModifier(new Addend(addend));
	public static Value operator +
		(Value value, double addend) => value.AddModifier(new Addend(new Literal(addend)));
	public static Value operator +
		(Value value, Guid addend) => value.AddModifier(new Addend(new Reference(addend)));

	public static Value operator *
		(Value value, Value multiplier) => value.AddModifier(new Multiplier(multiplier));
	public static Value operator *
		(Value value, double multiplier) => value.AddModifier(new Multiplier(new Literal(multiplier)));
	public static Value operator *
		(Value value, Guid multiplier) => value.AddModifier(new Multiplier(new Reference(multiplier)));

	public static Value operator /
		(Value value, Value divisor) => value.AddModifier(new Divisor(divisor));
	public static Value operator /
		(Value value, double divisor) => value.AddModifier(new Divisor(new Literal(divisor)));
	public static Value operator /
		(Value value, Guid divisor) => value.AddModifier(new Divisor(new Reference(divisor)));

	public static Value operator %
		(Value value, Value modulus) => value.AddModifier(new Modulus(modulus));
	public static Value operator %
		(Value value, double modulus) => value.AddModifier(new Modulus(new Literal(modulus)));
	public static Value operator %
		(Value value, Guid modulus) => value.AddModifier(new Modulus(new Reference(modulus)));
}

public class Literal : Value
{
	private double _val;

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

