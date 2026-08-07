namespace Src.Core.Ports;

public interface IState
{
	public string Name { get; }
	public double Value { get; }
	public double Maximum { get; }
	public double Minimum { get; }
	public double Change(double amount);
}

