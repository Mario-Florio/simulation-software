using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;

namespace Tests.Suites;

public class SchedulerTests
{
	public static void Run(string SuiteName)
	{
		TestAPI.Suite("Scheduler", () =>
		{
			TestAPI.It("Schedules Transformations That Restore Lowest Quantity State First", () =>
			{
				var scheduler = new Scheduler();
				var state = new Dictionary<Guid, IState>();
				var stateContext = new StateContext(state);
				var tickSpan = new NullTracer.Span("Tick Span", new NullTracer());

				var aRef = Guid.NewGuid();
				var bRef = Guid.NewGuid();
				var cRef = Guid.NewGuid();
				state.Add(aRef, new Variable("A", 0.0));
				state.Add(bRef, new Variable("B", 1.0));
				state.Add(cRef, new Variable("C", 2.0));

				var transformations = new List<Transformation>();
				transformations.Add(new Transformation("Last").AddChange(cRef, 1.0));
				transformations.Add(new Transformation("Second").AddChange(bRef, 1.0));
				transformations.Add(new Transformation("First").AddChange(aRef, 1.0));

				scheduler.Load(transformations);

				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "First");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Second");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Last");
			});
			TestAPI.It("Schedules Transformations That Restore Lowest Quantity State First (Relative to State Minimum)", () =>
			{
				var scheduler = new Scheduler();
				var state = new Dictionary<Guid, IState>();
				var stateContext = new StateContext(state);
				var tickSpan = new NullTracer.Span("Tick Span", new NullTracer());

				var aRef = Guid.NewGuid();
				var bRef = Guid.NewGuid();
				var cRef = Guid.NewGuid();
				state.Add(aRef, new Variable("A", 0.0));
				state.Add(bRef, new Variable("B", 1.0, double.NegativeInfinity));
				state.Add(cRef, new Variable("C", 2.0));

				var transformations = new List<Transformation>();
				transformations.Add(new Transformation("Last").AddChange(bRef, 1.0));
				transformations.Add(new Transformation("Second").AddChange(cRef, 1.0));
				transformations.Add(new Transformation("First").AddChange(aRef, 1.0));

				scheduler.Load(transformations);

				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "First");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Second");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Last");
			});
			TestAPI.It(
				"Schedules Transformations That Recovers The Most Depleted State With The Greatest Increase First",
				() =>
			{
				var scheduler = new Scheduler();
				var state = new Dictionary<Guid, IState>();
				var stateContext = new StateContext(state);
				var tickSpan = new NullTracer.Span("Tick Span", new NullTracer());

				var aRef = Guid.NewGuid();
				var bRef = Guid.NewGuid();
				var cRef = Guid.NewGuid();
				state.Add(aRef, new Variable("A", 0.0));
				state.Add(bRef, new Variable("B", 1.0, double.NegativeInfinity));
				state.Add(cRef, new Variable("C", 2.0));

				var transformations = new List<Transformation>();
				transformations.Add(new Transformation("Last").AddChange(cRef, 1.0));
				transformations.Add(new Transformation("Second").AddChange(cRef, 10.0));
				transformations.Add(new Transformation("First").AddChange(aRef, 1.0));

				scheduler.Load(transformations);

				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "First");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Second");
				TestAPI.Assert(scheduler.Next(stateContext, tickSpan).Name == "Last");
			});
		}, SuiteName);
	}
}

