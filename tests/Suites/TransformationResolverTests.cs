using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;

namespace Tests.Suites;

public class TransformationResolverTests
{
	public static void Run(string SuiteName)
	{
		TestAPI.Suite("Transformation Resolver", () =>
		{
			var transformationResolver = new TransformationResolver();
			var maximum = 10.0;
			var minimum = 0.0;
			var variable = new Variable("A", 1.0, minimum, maximum);
			var variableRef = Guid.NewGuid();
			var state = new Dictionary<Guid, IState>();
			state.Add(variableRef, variable);
			var stateContext = new StateContext(state);
			var tickSpan = new NullTracer.Span("null", new NullTracer());

			TestAPI.It("Resolves Transformation Status To 'APPROVED' If Within Valid Range", () =>
			{
				var current = variable.Value;
				var amount = -(current - variable.Minimum);

				var transformation = new Transformation()
					.AddChange(variableRef, amount);

				transformationResolver.Resolve(transformation, stateContext, tickSpan);

				TestAPI.Assert(transformation.Status.Equals(Transformation.StatusState.APPROVED));
			});
			TestAPI.It("Resolves Transformation Status To 'APPROVED' If Within Valid Range (SCALED)", () =>
			{
				var current = variable.Value;
				var amount = -(current - variable.Minimum + 1);

				var transformation = new Transformation(Transformation.PolicyType.SCALED)
					.AddChange(variableRef, amount);

				transformationResolver.Resolve(transformation, stateContext, tickSpan);

				TestAPI.Assert(transformation.Status.Equals(Transformation.StatusState.APPROVED));
			});
			TestAPI.It("Resolves Transformation Status To 'REJECTED' If Within Valid Range", () =>
			{
				var current = variable.Value;
				var amount = -(current - variable.Minimum + 1);

				var transformation = new Transformation()
					.AddChange(variableRef, amount);

				transformationResolver.Resolve(transformation, stateContext, tickSpan);

				TestAPI.Assert(transformation.Status.Equals(Transformation.StatusState.REJECTED));
			});
			TestAPI.It("Resolves Transformation Scale Appropriately", () =>
			{
				var current = variable.Value;
				var amount = (variable.Maximum - current + 1);
				var scale = _GetScale(variable, amount);

				var transformation = new Transformation(Transformation.PolicyType.SCALED)
					.AddChange(variableRef, amount);

				transformationResolver.Resolve(transformation, stateContext, tickSpan);

				TestAPI.Assert(transformation.Scale == scale);
			});
		}, SuiteName);
	}

	private static double _GetScale(IState state, double delta)
	{
		if (state.Value + delta > state.Maximum) return (state.Maximum - state.Value) / delta;
		if (state.Value + delta < state.Minimum) return (state.Minimum - state.Value) / delta;
		return 1.0;
	}
}

