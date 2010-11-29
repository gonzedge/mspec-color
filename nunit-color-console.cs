using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class NUnitColorConsole {

    // NUnit 2.4.7 displays the summary on 1 line
    static string SummaryLine  = @"^Tests run: (\d+), Failures: (\d+), Not run: (\d+), Time: .* seconds$";

    // NUnit 2.5.8 displays the summary on 2 lines
    static string SummaryLine1 = @"^Tests run: (\d+), Errors: (\d+), Failures: (\d+), Inconclusive: \d+, Time: .* seconds$";
    static string SummaryLine2 = @"^  Not run: \d+, Invalid: \d+, Ignored: (\d+), Skipped: \d+$";

    public static void Main(string[] args) {
        var process = GetProcess(args);
        process.Start();
        ProcessOutput(process);
        process.WaitForExit();
    }

    static string GetNunitConsoleCommand() {
        if (Environment.GetEnvironmentVariable("NUNIT_CONSOLE") != null)
            return Environment.GetEnvironmentVariable("NUNIT_CONSOLE");
        else
            return "nunit-console";
    }

    static Process GetProcess(string[] args) {
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName               = GetNunitConsoleCommand();
        process.StartInfo.Arguments              = String.Join(" ", args);
        process.StartInfo.UseShellExecute        = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow         = true;
        process.StartInfo.WorkingDirectory       = System.IO.Directory.GetCurrentDirectory();
        return process;
    }

    static void ProcessOutput(Process process) {
        bool summaryHasPrinted = false;
        int timesOutputWasNull = 0;
        int waitTime           = 100;
        int maxTimesToWait     = 30; // wait 3 seconds for some output
        bool testCaseFailures  = false;
        bool testsNotRun       = false;
        String output          = null;
        Match  match           = null;
        String summary         = null;
        while ((output = process.StandardOutput.ReadLine()) != null || (process.HasExited == false)) {
            match = null;

            // If we're no longer reading any input and the summary has been printed ... let's get 
            // ready to stop if things stop responding!
            if (output == null && summaryHasPrinted) {
                Console.WriteLine("!!! summary has printed and we got no output ...");
                timesOutputWasNull++;
                if (timesOutputWasNull >= maxTimesToWait)
                    break; // nunit-console is probably done!
                else {
                    System.Threading.Thread.Sleep(waitTime);
                    continue; 
                }
            } else
                timesOutputWasNull = 0;

            // We're printing out the summary line.  Color it based on if everything passed, there were failures, or nothing ran
            //
            //   Tests run: 7, Failures: 1, Not run: 8, Time: 0.411 seconds
            //
            if ((match = Regex.Match(output, SummaryLine)).Success) {
                var run     = int.Parse(match.Groups[1].Value);
                var fails   = int.Parse(match.Groups[2].Value);
                var pending = int.Parse(match.Groups[3].Value);

                if      (fails > 0) Console.ForegroundColor = ConsoleColor.Red;
                else if (run > 0)   Console.ForegroundColor = ConsoleColor.Green;
                else                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;
                summaryHasPrinted = true;

            // Some versions of NUnit console print out the summary on 2 lines, using different verbiage
            //  
            //   Tests run: 7, Errors: 0, Failures: 1, Inconclusive: 0, Time: 0.2217072 seconds
            //     Not run: 8, Invalid: 0, Ignored: 8, Skipped: 0
            //   
            } else if ((match = Regex.Match(output, SummaryLine1)).Success) {
                summary = output; // we can't print out the results until we read the second line of the summary

            // The second line of the summary (in some versions of NUnit console)
            } else if ((match = Regex.Match(output, SummaryLine2)).Success) {
                var summaryMatch = Regex.Match(summary, SummaryLine1);

                var run     = int.Parse(summaryMatch.Groups[1].Value);
                var errors  = int.Parse(summaryMatch.Groups[2].Value);
                var fails   = int.Parse(summaryMatch.Groups[3].Value);
                var pending = int.Parse(match.Groups[1].Value);

                if      (errors > 0 || fails > 0) Console.ForegroundColor = ConsoleColor.Red;
                else if (run > 0)                 Console.ForegroundColor = ConsoleColor.Green;
                else                              Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(summary);
                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;
                summaryHasPrinted = true;

            // The following lines are all going to be the tests that were not run, so set testsNotRun = true
            } else if (output == "Tests not run:" || output == "Tests Not Run:") {
                Console.WriteLine(output);
                testsNotRun = true;

            // The following lines are all going to be the test case failures
            } else if (output == "Test Case Failures:" || output == "Errors and Failures:") {
                Console.WriteLine(output);
                testCaseFailures = true;

            // We're printing out the section of [Ignore] tests (tests not run)
            } else if (testsNotRun) {
                match = Regex.Match(output, @"^(\d+)\) ([^:]+) : (.*)$");
                if (match.Success) {
                    var testNumber   = match.Groups[1].Value;
                    var testName     = match.Groups[2].Value;
                    var ignoreReason = match.Groups[3].Value;

                    // on some versions of NUnit console, it displays 1) Ignored : TestName
                    if (testName == "Ignored") {
                        testName     = ignoreReason;
                        ignoreReason = "";
                    }

                    // Set a default [Ignore] reason if the user didn't specify one
                    if (ignoreReason.Trim().Length == 0)
                        ignoreReason = "Pending (Not Yet Implemented)";

                    Console.Write("{0}) ", testNumber);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(testName);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" : {0}", ignoreReason);
                }

            // We're printing out the section of failures
            } else if (testCaseFailures) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;

            // None of the conditions hit ... simply print out the line!
            } else {
                Console.WriteLine(output);
            }
        }
    }
}
