using System.Collections.ObjectModel;
using System.Diagnostics;
using Src.Core.Ports;

namespace Src.Core.Entities;

public class NullTracer : ITracer
{
	private Guid _id = Guid.NewGuid();

	public Guid ID { get => _id; }

	public ITracer.ISpan StartSpan(string name)
	{ return new Span(name, this); }

	public ITracer.ISpan StartSpan(string name, ITracer.ISpan span)
	{ return new Span(name, this, span); }

	public void EndSpan(ITracer.ISpan span)
	{}

	public class Span : ITracer.ISpan
	{
		private string _name = "";
		private Guid _id = Guid.NewGuid();
		private Guid _traceId = Guid.NewGuid();
		private Guid? _parentId = null;
		private Dictionary<string, object> _attributes = new();
		private List<ITracer.IEvent> _events = new();

		public string Name { get => _name; }
		public Guid ID { get => _id; }
		public Guid TraceID { get => _traceId; }
		public Guid? ParentID { get => _parentId; }
		public TimeSpan Elapsed { get => new TimeSpan(); }
		public ReadOnlyDictionary<string, object> Attributes { get => new(_attributes); }
		public List<ITracer.IEvent> Events { get => _events; }

		public Span(string name, ITracer tracer)
		{}
		public Span(string name, ITracer tracer, ITracer.ISpan parent)
		{}

		public void End()
		{}
		public ITracer.IEvent AddEvent(string name, Dictionary<string, object> attributes)
		{ return new NullTracer.Event(name); }

		public void AddAttribute(string name, object value)
		{}
	}

	public class Event : ITracer.IEvent
	{
		private Dictionary<string, object> _attributes = new();

		public string Name { get => ""; }
		public long Timestamp { get => 0L; }
		public ReadOnlyDictionary<string, object> Attributes { get => new(_attributes); }

		public Event(string name)
		{}

		public void AddAttribute(string name, object value)
		{}
	}
}

