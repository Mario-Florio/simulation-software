using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;

namespace Tests.Suites;

public class TransformationTests
{
	public static void Run(string SuiteName)
	{
		TestAPI.Suite("Transformation", () =>
		{
			var state = new State();

			TestAPI.It("Increases State (static)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta);
			});
			TestAPI.It("Decreases State (static)", () =>
			{
				var current = state.A;
				var delta = -1.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta);

				transformation.Execute(state.Context);
	
				TestAPI.Assert(state.A == current + delta);
			});
			TestAPI.It("Increases State (dynamic)", () =>
			{
				var current = state.A;
				var delta = state.B;
				var transformation = new Transformation()
					.AddChange(state.ARef, state.BRef);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta);
			});
			TestAPI.It("Chains Transformations Accurately (static)", () =>
			{
				var current = state.A;
				var deltaA = 2.0;
				var deltaB = -4.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, deltaA)
					.AddChange(state.ARef, deltaB);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + deltaA + deltaB);
			});
			TestAPI.It("Chains Transformations Accurately (dynamic)", () =>
			{
				var current = state.A;
				var deltaA = state.B;
				var deltaB = state.C;
				var transformation = new Transformation()
					.AddChange(state.ARef, state.BRef)
					.AddChange(state.ARef, state.CRef);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + deltaA + deltaB);
			});
		}, SuiteName);

		TestAPI.Suite("Transformation Modifiers", () =>
		{
			var state = new State();

			TestAPI.It("Applies Multiplier (static)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierVal = 2.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Multiplier(multiplierVal));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta * multiplierVal);
			});
			TestAPI.It("Applies Multiplier (dynamic)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierVal = state.B;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Multiplier(state.BRef));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta * multiplierVal);
			});
			TestAPI.It("Applies Divisor (static)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var divisorVal = 2.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Divisor(divisorVal));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta / divisorVal);
			});
			TestAPI.It("Applies Divisor (dynamic)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var divisorVal = state.B;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Divisor(state.BRef));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta / divisorVal);
			});
			TestAPI.It("Applies Modulus (static)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var modulusVal = 2.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Modulus(modulusVal));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta % modulusVal);
			});
			TestAPI.It("Applies Modulus (dynamic)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var modulusVal = state.B;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta, new Modulus(state.BRef));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta % modulusVal);
			});
			TestAPI.It("Applies Modifier Chain (static)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierAVal = 5.0;
				var divisorAVal = 2.0;
				var multiplierBVal = 0.3;
				var divisorBVal = 1.7;

				var transformation = new Transformation()
					.AddChange(
						state.ARef, 
						delta,
						new Multiplier(multiplierAVal)
							.AddModifier(new Divisor(divisorAVal))
							.AddModifier(new Multiplier(multiplierBVal))
							.AddModifier(new Divisor(divisorBVal))
					);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == (current + delta
					* multiplierAVal
					/ divisorAVal
					* multiplierBVal
					/ divisorBVal));
			});
			TestAPI.It("Applies Modifier Chain (dynamic)", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierAVal = state.B;
				var divisorAVal = state.C;
				var multiplierBVal = 3.0;
				var multiplierCVal = state.D;
				var divisorBVal = state.E;

				var transformation = new Transformation()
					.AddChange(
						state.ARef,
						delta,
						new Multiplier(state.BRef)
							.AddModifier(new Divisor(state.CRef))
							.AddModifier(new Multiplier(multiplierBVal))
							.AddModifier(new Multiplier(state.DRef))
							.AddModifier(new Divisor(state.ERef))
					);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == (current + delta
					* multiplierAVal
					/ divisorAVal
					* multiplierBVal
					* multiplierCVal
					/ divisorBVal));
			});
			TestAPI.It("Applies Modifier Chain (static) 2", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierAVal = 5.0;
				var divisorAVal = 2.0;
				var multiplierBVal = 0.3;
				var divisorBVal = 1.7;

				var transformation = new Transformation()
					.AddChange(
						state.ARef, 
						delta,
						new Multiplier(multiplierAVal)
							.AddModifier(new Divisor(divisorAVal)
								.AddModifier(new Multiplier(multiplierBVal)))
							.AddModifier(new Divisor(divisorBVal))
					);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == (current + delta
					* multiplierAVal
					/ divisorAVal
					* multiplierBVal
					/ divisorBVal));
			});
			TestAPI.It("Applies Modifier Chain (dynamic) 2", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var multiplierAVal = state.B;
				var divisorAVal = state.C;
				var multiplierBVal = 3.0;
				var multiplierCVal = state.D;
				var divisorBVal = state.E;

				var transformation = new Transformation()
					.AddChange(
						state.ARef, 
						delta,
						new Multiplier(state.BRef)
							.AddModifier(new Divisor(state.CRef))
							.AddModifier(new Multiplier(multiplierBVal)
								.AddModifier(new Multiplier(state.DRef)))
							.AddModifier(new Divisor(state.ERef))
					);

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == (current + delta
					* multiplierAVal
					/ divisorAVal
					* multiplierBVal
					* multiplierCVal
					/ divisorBVal));
			});
		}, SuiteName);

		TestAPI.Suite("Transformation Conditions", () =>
		{
			var state = new State();

			TestAPI.It("Applies Transformation If Condition Is True", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta)
					.AddCondition(new GreaterThan(new Literal(100.0), new Literal(0.0)));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta);
			});
			TestAPI.It("Doesn't Apply Transformation If Condition Is False", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta)
					.AddCondition(new GreaterThan(new Literal(0.00),
								      new Literal(100.0)));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current);
			});
			TestAPI.It("Applies Condition Chain Correctly", () =>
			{
				var current = state.A;
				var delta = 1.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, delta)
					.AddCondition(new GreaterThan(new Literal(0.00),
								      new Literal(100.0),
								      Condition.ChainType.OR))
					.AddCondition(new GreaterThan(new Literal(100.00),
								      new Literal(0.0)));

				transformation.Execute(state.Context);

				TestAPI.Assert(state.A == current + delta);
			});
		}, SuiteName);

		TestAPI.Suite("Transformation Resolution Policy", () =>
		{
			var state = new State();

			TestAPI.It("Applies Fixed Policy By Default", () =>
			{
				var currentA = state.A;
				var currentB = state.B;
				var deltaA = -1.5;
				var deltaB = 2.0;
				var transformation = new Transformation()
					.AddChange(state.ARef, deltaA)
					.AddChange(state.BRef, deltaB);

				transformation.Execute(state.Context);


				TestAPI.Assert(state.A == currentA + deltaA);
				TestAPI.Assert(state.B == currentB + deltaB);
			});
			TestAPI.It("Scaled Policy Is Applied Accurately", () =>
			{
				var currentA = state.A;
				var currentB = state.B;
				var deltaA = -1.5;
				var deltaB = 2.0;
				var scale = 0.5;
				var transformation = new Transformation(Transformation.PolicyType.SCALED)
					.AddChange(state.ARef, deltaA)
					.AddChange(state.BRef, deltaB);

				transformation.Scale = scale;

				transformation.Execute(state.Context);


				TestAPI.Assert(state.A == currentA + deltaA * scale);
				TestAPI.Assert(state.B == currentB + deltaB * scale);
			});
		}, SuiteName);

	}

	private class State
	{
		private IState _variableA = new Variable("A", 1.0);
		private Guid _variableARef = Guid.NewGuid();
		private IState _variableB = new Variable("B", 3.0);
		private Guid _variableBRef = Guid.NewGuid();
		private IState _variableC = new Variable("C", 2.0);
		private Guid _variableCRef = Guid.NewGuid();
		private IState _variableD = new Variable("D", 0.3);
		private Guid _variableDRef = Guid.NewGuid();
		private IState _variableE = new Variable("E", 1.3);
		private Guid _variableERef = Guid.NewGuid();

		public double A { get => _variableA.Value; }
		public double B { get => _variableB.Value; }
		public double C { get => _variableC.Value; }
		public double D { get => _variableD.Value; }
		public double E { get => _variableE.Value; }
		public Guid ARef { get => _variableARef; }
		public Guid BRef { get => _variableBRef; }
		public Guid CRef { get => _variableCRef; }
		public Guid DRef { get => _variableDRef; }
		public Guid ERef { get => _variableERef; }

		public IPrivilegedStateContext Context = null!;

		public State()
		{
			var state = new Dictionary<Guid, IState>();

			state.Add(_variableARef, _variableA);
			state.Add(_variableBRef, _variableB);
			state.Add(_variableCRef, _variableC);
			state.Add(_variableDRef, _variableD);
			state.Add(_variableERef, _variableE);

			Context = new StateContext(state);
		}
	}
}

