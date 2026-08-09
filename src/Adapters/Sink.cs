using System.IO;

namespace Src.Adapters;

public interface ISink
{
	public void Out(object obj);
}

public class FileSink : ISink
{
	private string _path;

	public FileSink(string? path = null)
	{
		if (path == null) path = Path.GetRandomFileName();

		_path = path;
	}
	public FileSink(
		string? path = null,
		string ext = ".txt"
	) {
		if (path == null) path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

		_path = path + ext;
	}

	public void Out(object obj)
	{
		try
		{
			if (!File.Exists(_path))
			{
				File.Create(_path).Dispose();
				Console.WriteLine($"File {_path} created.");
			}

			Console.WriteLine($"Appending to file {_path}...");
			File.AppendAllText(_path, obj.ToString() ?? "null");
			Console.WriteLine($"Write completed -  see file at `{Path.GetFullPath(_path)}`.");
		}

		catch (UnauthorizedAccessException ex)
		{ Console.WriteLine($"Access denied: {ex.Message}"); }

		catch (IOException ex)
		{ Console.WriteLine($"I/O error: {ex.Message}"); }
	}

	public void Clean()
	{ File.Delete(_path); }
}

