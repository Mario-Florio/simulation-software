using System.Collections.ObjectModel;
using System.Diagnostics;
using Src.Core.Entities;
using Src.Core.Ports;

namespace Src.Adapters;

public class SimpleTracer : ITracer
{
	private Guid _id = Guid.NewGuid();
	private ISpanExporter _exporter = new ConsoleExporter();

	public Guid ID { get => _id; }

	public SimpleTracer()
	{}
	public SimpleTracer(ISpanExporter exporter)
	{ _exporter = exporter; }

	public ITracer.ISpan StartSpan(string name)
	{ return new Span(name, this); }

	public ITracer.ISpan StartSpan(string name, ITracer.ISpan span)
	{ return new Span(name, this, span); }

	public void EndSpan(ITracer.ISpan span)
	{ _exporter.Export(span); }

	public class Span : ITracer.ISpan
	{
		private string _name;
		private Guid _id = Guid.NewGuid();
		private ITracer _tracer;
		private ITracer.ISpan? _parent = null;
		private Stopwatch _stopwatch = Stopwatch.StartNew();
		private Dictionary<string, object> _attributes = new();
		private List<ITracer.IEvent> _events = new();

		public string Name { get => _name; }
		public Guid ID { get => _id; }
		public Guid TraceID { get => _tracer.ID; }
		public Guid? ParentID { get => _parent == null ? null : _parent.ID; }
		public TimeSpan Elapsed { get => _stopwatch.Elapsed; }
		public ReadOnlyDictionary<string, object> Attributes { get => new(_attributes); }
		public List<ITracer.IEvent> Events { get => _events; }

		public Span(string name, ITracer tracer)
		{
			_name = name;
			_tracer = tracer;
		}
		public Span(string name, ITracer tracer, ITracer.ISpan parent)
		{
			_name = name;
			_tracer = tracer;
			_parent = parent;
		}

		public void End()
		{
			_stopwatch.Stop();
			_tracer.EndSpan(this);
		}
		public ITracer.IEvent AddEvent(string name, Dictionary<string, object> attributes)
		{
			var traceEvent = new Event(name, attributes);
			_events.Add(traceEvent);
			return traceEvent;
		}
		public void AddAttribute(string name, object value)
		{
			if (_attributes.ContainsKey(name)) _attributes[name] = value;

			_attributes.Add(name, value);
		}
	}

	public class Event : ITracer.IEvent
	{
		private string _name;
		private long _timestamp = Stopwatch.GetTimestamp();
		private Dictionary<string, object> _attributes = new();

		public string Name { get => _name; }
		public long Timestamp { get => _timestamp; }
		public ReadOnlyDictionary<string, object> Attributes { get => new(_attributes); }

		public Event(string name)
		{ _name = name; }

		public Event(string name, Dictionary<string, object> attributes)
		{
			_name = name;
			_attributes = attributes;
		}

		public void AddAttribute(string name, object value)
		{
			if (_attributes.ContainsKey(name)) _attributes[name] = value;

			_attributes.Add(name, value);
		}
	}
}

