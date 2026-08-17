using Tests.Suites;

namespace Tests;

public class Program
{
	public static void Main(string[] args)
	{
		_Run(string.Join(" ", args));
	}
	private static void _Run(string SuiteName = "")
	{
		ValueTests.Run(SuiteName);
		TransformationTests.Run(SuiteName);
		TransformationResolverTests.Run(SuiteName);
		SchedulerTests.Run(SuiteName);
	}
}

