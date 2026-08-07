using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Src.Core.Ports;

public interface ITracer
{
	public Guid ID { get; }

	public ISpan StartSpan(string name);
	public ISpan StartSpan(string name, ISpan span);
	public void EndSpan(ISpan span);

	public interface ISpan
	{
		public string Name { get; }
		public Guid ID { get; }
		public Guid TraceID { get; }
		public Guid? ParentID { get; }
		public TimeSpan Elapsed { get; }
		public ReadOnlyDictionary<string, object> Attributes { get; }
		public List<IEvent> Events { get; }

		public void End();
		public IEvent AddEvent(string name, Dictionary<string, object> attributes);
		public void AddAttribute(string name, object value);
	}

	public interface IEvent
	{
		public string Name { get; }
		public long Timestamp { get; }
		public ReadOnlyDictionary<string, object> Attributes { get; }

		public void AddAttribute(string name, object value);
	}
}

