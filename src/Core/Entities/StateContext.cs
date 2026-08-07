using Src.Core.Ports;

namespace Src.Core.Entities;

public interface IStateContext
{
	public bool IsNotNullRef(Guid stateRef);
	public string? GetName(Guid stateRef);
	public double? GetValue(Guid stateRef);
	public double? GetMaximum(Guid stateRef);
	public double? GetMinimum(Guid stateRef);
	public Dictionary<Guid, double> GetAll();
}

public interface IPrivilegedStateContext : IStateContext
{
	public double? Change(Guid stateRef, double amount);
}

public class StateContext : IStateContext, IPrivilegedStateContext
{
	private Dictionary<Guid, IState> _state;

	public StateContext(Dictionary<Guid, IState> state)
	{
		_state = state;
	}

	public bool IsNotNullRef(Guid stateRef)
	{
		return _IsValid(stateRef);
	}
	public string? GetName(Guid stateRef)
	{
		if (!_IsValid(stateRef)) return null;

		return _state[stateRef].Name;
	}
	public double? GetValue(Guid stateRef)
	{
		if (!_IsValid(stateRef)) return null;
	
		return _state[stateRef].Value;
	}
	public double? GetMaximum(Guid stateRef)
	{
		if (!_IsValid(stateRef)) return null;

		return _state[stateRef].Maximum;
	}
	public double? GetMinimum(Guid stateRef)
	{
		if (!_IsValid(stateRef)) return null;

		return _state[stateRef].Minimum;
	}
	public Dictionary<Guid, double> GetAll()
	{
		var state = new Dictionary<Guid, double>();
		foreach (var (stateRef, s) in _state)
		{
			state.Add(stateRef, s.Value);
		}
		return state;
	}
	public double? Change(Guid stateRef, double amount)
	{
		if (!_IsValid(stateRef)) return null;

		return _state[stateRef].Change(amount);
	}

	private bool _IsValid(Guid stateRef)
	{
		return _state.ContainsKey(stateRef);
	}
}

