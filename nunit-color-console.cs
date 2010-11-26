using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class NUnitColorConsole {

    public static void Main(string[] args) {
        var process = GetProcess(args);
        process.Start();
        ProcessOutput(process);
        process.WaitForExit();
    }
    
    static Process GetProcess(string[] args) {
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName               = "nunit-console";
        process.StartInfo.Arguments              = String.Join(" ", args);
        process.StartInfo.UseShellExecute        = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow         = true;
        process.StartInfo.WorkingDirectory       = System.IO.Directory.GetCurrentDirectory();
        return process;
    }

    static void ProcessOutput(Process process) {
        bool testCaseFailures = false;
        bool testsNotRun      = false;
        String output         = null;
        Match  match          = null;
        while ((output = process.StandardOutput.ReadLine()) != null || (process.HasExited == false)) {
            match = null;

            // We're printing out the summary line.  Color it based on if everything passed, there were failures, or nothing ran
            if ((match = Regex.Match(output, @"^Tests run: (\d+), Failures: (\d+), Not run: (\d+), Time: .* seconds$")).Success) {
                var run     = int.Parse(match.Groups[1].Value);
                var fails   = int.Parse(match.Groups[2].Value);
                var pending = int.Parse(match.Groups[3].Value);

                if      (fails > 0) Console.ForegroundColor = ConsoleColor.Red;
                else if (run > 0)   Console.ForegroundColor = ConsoleColor.Green;
                else                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(output);
                Console.ForegroundColor = ConsoleColor.White;

            // The following lines are all going to be the tests that were not run, so set testsNotRun = true
            } else if (output == "Tests not run:") {
                Console.WriteLine(output);
                testsNotRun = true;

            // The following lines are all going to be the test case failures
            } else if (output == "Test Case Failures:") {
                Console.WriteLine(output);
                testCaseFailures = true;

            // We're printing out the section of [Ignore] tests (tests not run)
            } else if (testsNotRun) {
                match = Regex.Match(output, @"^(\d+)\) ([^:]+) : (.*)$");
                if (match.Success) {
                    var testNumber   = match.Groups[1].Value;
                    var testName     = match.Groups[2].Value;
                    var ignoreReason = match.Groups[3].Value;

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
