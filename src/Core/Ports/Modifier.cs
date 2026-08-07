namespace Src.Core.Entities;

public abstract class Modifier
{
	protected Guid? _valueRef;
	protected double? _value;
	protected Modifier? _modifier;

	public Modifier(double value)
	{ _value = value; }

	public Modifier(Guid valueRef)
	{ _valueRef = valueRef; }

	public double? Modify(double baseVal, IStateContext stateContext)
	{
		double modifierValue;

		if (_valueRef != null)
		{
			if (!stateContext.IsNotNullRef((Guid)_valueRef!)) return null;

			modifierValue = (double)stateContext.GetValue((Guid)_valueRef!)!;
		}
		else if (_value != null)
		{ modifierValue = (double)_value!; }

		else
		{ return null; }

		var result = _Modify(baseVal, modifierValue);

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

	protected abstract double _Modify(double baseVal, double modifierVal);
}

public class Multiplier : Modifier
{
	public Multiplier(double value) : base(value)
	{}
	public Multiplier(Guid valueRef) : base(valueRef)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal * modifierVal; }
}

public class Divisor : Modifier
{
	public Divisor(double value) : base(value)
	{}
	public Divisor(Guid valueRef) : base(valueRef)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal / modifierVal; }
}

public class Modulus : Modifier
{
	public Modulus(double value) : base(value)
	{}
	public Modulus(Guid valueRef) : base(valueRef)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal % modifierVal; }
}

public class Subtractor : Modifier
{
	public Subtractor(double value) : base(value)
	{}
	public Subtractor(Guid valueRef) : base(valueRef)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal - modifierVal; }
}

public class Addend : Modifier
{
	public Addend(double value) : base(value)
	{}
	public Addend(Guid valueRef) : base(valueRef)
	{}

	protected override double _Modify(double baseVal, double modifierVal)
	{ return baseVal + modifierVal; }
}

