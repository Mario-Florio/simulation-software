
namespace Tests;

public class TestAPI
{
	public static void Suite(string Name, Action Tests, string Filter = "")
	{
		bool Run = Name.ToLower().Trim().Contains(Filter.ToLower().Trim());

		if (Run)
		{
			Console.WriteLine();
			Console.WriteLine($"{Name}");
			Tests();
		}
	}
	public static void It(string Name, Action Fn, bool PrintError = false)
	{
		try
		{
			Fn();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{Name}: PASS"); 
		}
		catch (Exception e)
		{

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"{Name}: FAIL");
			if (PrintError) Console.WriteLine(e);
		}
		Console.ResetColor();
	}
	public static void Assert(bool Condition)
	{
		if (!Condition) throw new Exception();
	}
}
