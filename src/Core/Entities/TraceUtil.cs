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
}
