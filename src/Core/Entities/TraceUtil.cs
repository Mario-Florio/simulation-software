namespace Src.Core.Entities;

public class TraceUtil
{
	public static Dictionary<Guid, object> GetConditionsDict(List<Transformation> transformations)
	{
		var conditions = new Dictionary<Guid, object>();
		foreach (var t in transformations)
		{
			var condition = t.Condition;
			while(condition != null)
			{
				conditions.Add(condition.ID, condition.ToDict());
				condition = condition.condition;
			}
		}
		return conditions;
	}
	public static Dictionary<Guid, object> GetChangesDict(List<Transformation> transformations)
	{
		var changes = new Dictionary<Guid, object>();
		foreach (var t in transformations)
		{
			foreach (var c in t.Changes.Values) changes.Add(c.ID, c.ToDict());
		}
		return changes;
	}
	public static Dictionary<Guid, object> GetValuesDict(List<Transformation> transformations)
	{
		var values = new Dictionary<Guid, object>();
		foreach (var t in transformations)
		{
			foreach (var c in t.Changes.Values)
			{
				values.Add(c.Delta.ID, c.Delta.ToDict());
				_WalkModTreeForValues(values, c.Delta.Modifier);
			}
		}
		return values;
	}
	public static Dictionary<Guid, object> GetModifiersDict(List<Transformation> transformations)
	{
		var modifiers = new Dictionary<Guid, object>();
		foreach (var t in transformations)
		{
			foreach (var c in t.Changes.Values)
			{
				var modifier = c.Delta.Modifier;
				_WalkModTreeForMods(modifiers, modifier);
			}
		}
		return modifiers;
	}

	private static void _WalkModTreeForMods(Dictionary<Guid, object> dict, Modifier? modifier)
	{
		if (modifier == null) return;
		
		dict.Add(modifier.ID, modifier.ToDict());

		_WalkModTreeForMods(dict, modifier.Mod);
		if (modifier.Value is not null) _WalkModTreeForMods(dict, modifier.Value.Modifier);
	}
	private static void _WalkModTreeForValues(Dictionary<Guid, object> dict, Modifier? modifier)
	{
		if (modifier == null) return;

		if (modifier.Value is not null)
		{
			dict.Add(modifier.Value.ID, modifier.Value.ToDict());
			_WalkModTreeForValues(dict, modifier.Value.Modifier);
		}
		_WalkModTreeForValues(dict, modifier.Mod);
	}
}
