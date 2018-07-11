using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace CommandTerminal
{
    public static class BuiltinCommands
    {
        [RegisterCommand(Help = "Does nothing")]
        static void CommandNoop(CommandArg[] args) { }

        [RegisterCommand(Help = "Clears the Command Console", MaxArgCount = 0)]
        static void CommandClear(CommandArg[] args) {
            Terminal.Logger.Clear();
        }

        [RegisterCommand(Help = "Displays all Console Commands", MaxArgCount = 0)]
        static void CommandLs(CommandArg[] args) {
            foreach (var command in Terminal.Interpreter.Commands) {
                Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
            }
        }

        [RegisterCommand(Help = "Displays help documentation of a Command", MaxArgCount = 1)]
        static void CommandHelp(CommandArg[] args) {
            if (args.Length == 0) {
                CommandLs(args);
                return;
            }

            string command_name = args[0].String.ToUpper();

            if (!Terminal.Interpreter.Commands.ContainsKey(command_name)) {
                Terminal.Interpreter.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            string help = Terminal.Interpreter.Commands[command_name].help;

            if (help == null) {
                Terminal.Log("{0} does not provide any help documentation.", command_name);
            } else {
                Terminal.Log(help);
            }
        }

        [RegisterCommand(Help = "Times the execution of a Command", MinArgCount = 1)]
        static void CommandTime(CommandArg[] args) {
            var sw = new Stopwatch();
            sw.Start();

            Terminal.Interpreter.RunCommand(JoinArguments(args));

            sw.Stop();
            Terminal.Log("Time: {0}ms", (double)sw.ElapsedTicks / 10000);
        }

        [RegisterCommand(Help = "Outputs message")]
        static void CommandPrint(CommandArg[] args) {
            Terminal.Log(JoinArguments(args));
        }

    #if DEBUG
        [RegisterCommand(Help = "Outputs the StackTrace of the previous message", MaxArgCount = 0)]
        static void CommandTrace(CommandArg[] args) {
            int log_count = Terminal.Logger.Logs.Count;

            if (log_count - 2 <  0) {
                Terminal.Log("Nothing to trace.");
                return;
            }

            var log_item = Terminal.Logger.Logs[log_count - 2];

            if (log_item.stack_trace == "") {
                Terminal.Log("{0} (no trace)", log_item.message);
            } else {
                Terminal.Log(log_item.stack_trace);
            }
        }
    #endif

        [RegisterCommand(Help = "Quits running Application", MaxArgCount = 0)]
        static void CommandQuit(CommandArg[] args) {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        static string JoinArguments(CommandArg[] args) {
            var sb = new StringBuilder();
            int arg_length = args.Length;

            for (int i = 0; i < arg_length; i++) {
                sb.Append(args[i].String);

                if (i < arg_length - 1) {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}