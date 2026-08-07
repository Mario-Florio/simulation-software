using Src.Core.Ports;

namespace Src.Adapters;

public class Variable : IState
{
	private string _name;
	private double _value;
	private double _maximum = double.MaxValue;
	private double _minimum = 0;

	public string Name { get => _name; }
	public double Value { get => _value; }
	public double Maximum { get => _maximum; }
	public double Minimum { get => _minimum; }

	public Variable(string name, double value)
	{
		_name = name;
		_value = value;
	}
	public Variable(string name, double value, double minimum)
	{
		_name = name;
		_value = value;
		_minimum = minimum;
	}
	public Variable(string name, double value, double minimum, double maximum)
	{
		_name = name;
		_value = value;
		_minimum = minimum;
		_maximum = maximum;
	}

	public double Change(double amount)
	{ return _value += amount; }
}

