using CraftBot.Helper;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace CraftBot.Commands.Features
{
    public static class CommandPrompt
    {
        public static DiscordMessage CmdMessage = null;

        public static Process CmdProcess = null;

        public static string Output = "";

        public static Timer Timer = new Timer(1500);

        public static void InitializeCmd()
        {
            CmdProcess = Process.Start(new ProcessStartInfo()
            {
                FileName = "C:\\Windows\\System32\\cmd.exe",
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                ErrorDialog = true,
                WorkingDirectory = "C:\\",
            });
            CmdProcess.OutputDataReceived += CmdProcess_OutputDataReceived;
            CmdProcess.ErrorDataReceived += CmdProcess_OutputDataReceived;
            CmdProcess.Exited += CmdProcess_ExitedAsync;
            //ReadConsole();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
            Timer.Elapsed += Timer_ElapsedAsync;
            Timer.Start();
        }

        private static async void Timer_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
            if (CmdProcess == null || CmdMessage == null)
            {
                return;
            }

            int start = Math.Max(Output.Length - 1994, 0);
            int length = Math.Min(Output.Length, 1994);

            Output = Output.Substring(start, length);

            string messageText = "```" + Output + "```";

            if (CmdMessage.Content != messageText)
            {
                CmdMessage = await CmdMessage.ModifyAsync(messageText);
            }

        }

        private static async void CmdProcess_ExitedAsync(object sender, EventArgs e)
        {
            await CmdMessage.ModifyAsync("cmd has exited with " + CmdProcess.ExitCode);
            CmdProcess = null;
            CmdMessage = null;
            Timer.Elapsed -= Timer_ElapsedAsync;
            Timer.Stop();
        }

        private static void CmdProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) => Output += e.Data + "\n";
    }

    public partial class MainCommands : BaseCommandModule
    {
        [RequireUserGroup]
        [Command("cmd")]
        public async Task ExecuteCommand(CommandContext context, params string[] input)
        {
            if (CommandPrompt.CmdMessage == null)
            {
                CommandPrompt.CmdMessage = await context.RespondAsync("Starting the command prompt...");
                CommandPrompt.InitializeCmd();
            }
            else
            {
                string line = string.Join(" ", input);
                await CommandPrompt.CmdProcess.StandardInput.WriteLineAsync(line);
                await CommandPrompt.CmdProcess.StandardInput.FlushAsync();
                await context.Message.DeleteAsync();
            }
        }
    }
}
