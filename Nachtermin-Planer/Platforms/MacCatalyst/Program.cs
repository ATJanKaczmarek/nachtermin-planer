using ObjCRuntime;
using UIKit;

namespace Nachtermin_Planer;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
		string dbPath = FileAccessHelper.GetLocalFilePath("database.db3");

		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
