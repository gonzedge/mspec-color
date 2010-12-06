using System;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class MSpecColor {

    static bool         allTestsPassed       = false;
    static bool         summaryHasPrinted    = false;
    static int          msToWaitAfterSummary = 2000;
    static DateTime     lastLinePrintedAt    = DateTime.MinValue;
    static Process      process              = null;
    static Thread       outputProcessor      = null;
    static ConsoleColor originalColor;

    public static void Main(string[] args) {
        originalColor = Console.ForegroundColor;
	
	StartProcess(args);
        StartThread();
        WaitForExit();
	ResetConsoleColorBackToOriginal();
    }

    static void ResetConsoleColorBackToOriginal() {
        Console.ForegroundColor = originalColor;
	Console.WriteLine(""); // write the color to the console
    }

    static void StartThread() {
        outputProcessor = new Thread(ProcessOutput);
        outputProcessor.Start();
    }

    // Wait for the background Thread to either:
    //
    //  * finish processing output
    //  * or wait for some duration after the summary has been printed, then exit
    //
    // Sometimes when we Read() the process's STDOUT, it blocks for ever so 
    // we put this in a background thread and only wait for a certain amount of time 
    // after the summary has printed.
    //
    static void WaitForExit() {
        while (outputProcessor.IsAlive && ! summaryHasPrinted)
            Thread.Sleep(100); // hang out and wait for the summary to print (or the thread to finish)

        while (summaryHasPrinted && outputProcessor.IsAlive && MsSinceLastLinePrinted < msToWaitAfterSummary)
            Thread.Sleep(100); // hang out and wait for X milliseconds to have passed since the last time mspec.exe output a line

        // Everything should be done!  Let's tell the Thread that we're aborting it and give it 1 second to finish up ... then exit!
        if (outputProcessor.IsAlive) {
            outputProcessor.Abort();
            Thread.Sleep(1000);
        }

        // Exit with non-0 exit code unless all of the tests passed
	ResetConsoleColorBackToOriginal();
        if (allTestsPassed)
            Environment.Exit(0);
        else
            Environment.Exit(1);
    }

    static long MsSinceLastLinePrinted { get { return (DateTime.Now.Subtract(lastLinePrintedAt).Ticks / 10000); }}

    static string GetMspecConsoleCommand() {
        if (Environment.GetEnvironmentVariable("MSPEC_PATH") != null)
            return Environment.GetEnvironmentVariable("MSPEC_PATH");
        else
            return "mspec";
    }

    static void StartProcess(string[] args) {
        process = new System.Diagnostics.Process();
        process.StartInfo.FileName               = GetMspecConsoleCommand();
        process.StartInfo.Arguments              = String.Join(" ", args);
        process.StartInfo.UseShellExecute        = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow         = true;
        process.StartInfo.WorkingDirectory       = System.IO.Directory.GetCurrentDirectory();
        process.Start();
    }

    static void ProcessOutput() {
	int failed     = 0;
	int pending    = 0;
	int passed     = 0;
        String output  = null;
        while ((output = process.StandardOutput.ReadLine()) != null) {
            lastLinePrintedAt = DateTime.Now;

	    // Failed example
	    // All lines after this up until another spec or a blank line should be red too
	    if (output.StartsWith("» ") && output.EndsWith("(FAIL)")) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(output);
		failed++;

	    // Pending example
	    } else if (output.StartsWith("» ") && output.EndsWith("(NOT IMPLEMENTED)")) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;
		pending++;

	    // If it's not a (FAIL) and it's not (NOT IMPLEMENTED), then it's green!
	    } else if (output.StartsWith("» ")) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;
		passed++;

	    // Looks like we're printing out the summary line(s).
	    // Color the rest of the lines based on whether there were any failures / pending examples.
	    } else if (output.StartsWith("Contexts: ")) {
		if (failed > 0)
		    Console.ForegroundColor = ConsoleColor.Red;
		else if (pending > 0)
		    Console.ForegroundColor = ConsoleColor.Yellow;
		else if (passed > 0)
		    Console.ForegroundColor = ConsoleColor.Green;
		else
		    Console.ForegroundColor = ConsoleColor.Yellow; // no specs?

                Console.WriteLine(output);

            // None of the conditions hit ... simply print out the line!
            } else {
                Console.WriteLine(output);
            }
        }
    }
}
