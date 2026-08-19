using Src.Core.Entities;
using Src.Core.Ports;
using Src.Adapters;

namespace Tests.Suites;

public class ValueTests
{
	public static void Run(string SuiteName)
	{
		TestAPI.Suite("Value Tests", () =>
		{
			var state = new State();

			TestAPI.It("Evaluates Accurately (literal)", () =>
			{
				var literal = 5.0;
				var value = new Literal(literal);
				
				TestAPI.Assert(value.Evaluate(state.Context).Equals(literal));
			});

			TestAPI.It("Evaluates Accurately (reference)", () =>
			{
				var value = new Reference(state.ARef);
				
				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A));
			});

			TestAPI.It("Evaluates Accurately (modifiers)", () =>
			{
				var value = new Reference(state.ARef).AddModifier(new Multiplier(state.BRef));

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A * state.B));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators)", () =>
			{
				var value = new Reference(state.CRef) + new Reference(state.DRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.C + state.D));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators)", () =>
			{
				var value = new Reference(state.CRef) / new Literal(state.A);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.C / state.A));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators)", () =>
			{
				var value = new Reference(state.CRef) - state.ARef;

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.C - state.A));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators)", () =>
			{
				var value = new Reference(state.BRef) * state.C;

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.B * state.C));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators)", () =>
			{
				var value = new Reference(state.CRef) % state.ARef;

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.C % state.A));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators PEMDAS)", () =>
			{
				var value = (new Reference(state.BRef) + state.C) / 100;

				TestAPI.Assert(value.Evaluate(state.Context).Equals((state.B + state.C) / 100));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators PEMDAS)", () =>
			{
				var value = new Reference(state.BRef) + state.C / 100;

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.B + state.C / 100));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - operand)", () =>
			{
				var value = state.A - new Reference(state.CRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A - state.C));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - operand)", () =>
			{
				var value = state.B + new Reference(state.DRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.B + state.D));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - operand)", () =>
			{
				var value = state.A * new Reference(state.CRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A * state.C));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - operand)", () =>
			{
				var value = state.A / new Reference(state.CRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A / state.C));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - operand)", () =>
			{
				var value = state.A % new Reference(state.CRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(state.A % state.C));
			});

			TestAPI.It("Evaluates Accurately (overloaded operators - negates)", () =>
			{
				var value = -new Reference(state.CRef);

				TestAPI.Assert(value.Evaluate(state.Context).Equals(-state.C));
			});

			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) > new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(GreaterThan)));
			});

			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) >= new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(GreaterThanOrEqual)));
			});

			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) < new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(LesserThan)));
			});

			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) <= new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(LesserThanOrEqual)));
			});
			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) == new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(EqualTo)));
			});

			TestAPI.It("Composes Conditions (overloaded operators - conditions)", () =>
			{
				var value = new Reference(state.CRef) != new Literal(state.C);

				TestAPI.Assert(value.GetType().Equals(typeof(NotEqualTo)));
			});
		});
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
