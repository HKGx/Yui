using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Yui.Extensions;

namespace Yui.Modules.DevCommands
{
    [Group("dev"), Aliases("d")]
    public class Dev : BaseCommandModule
    {
        private SharedData _data;
        private Random _random;
        private HttpClient _http;
        public Dev(SharedData data, Random random, HttpClient client)
        {
            _data = data;
            _random = random;
            _http = client;
        }

        [GroupCommand]
        public async Task GetDevAsync(CommandContext ctx)
        {
            
            var trans = ctx.Guild.GetTranslation(_data);
            var devUser = (await ctx.Client.GetCurrentApplicationAsync()).Owner;
            var text = trans.GetDevText.Replace("{{devName}}", $"{devUser.Username}#{devUser.Discriminator}") +
                       ":heart:";
            await ctx.RespondAsync(text);
        }

        [Command("update"), RequireOwner]
        public async Task UpdateBotAsync(CommandContext ctx)
        {
            var trans = ctx.Guild.GetTranslation(_data);
            if (!File.Exists(Directory.GetCurrentDirectory() + "/update.sh"))
            {
                await ctx.RespondAsync(trans.UpdateBotNoFileText);
                return;
            }
            var file = "update.sh";
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "#/bin/bash",
                    Arguments = $"{file} {Process.GetCurrentProcess().Id}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            await ctx.RespondAsync(trans.UpdateBotStartingText);
            _data.CTS.Cancel();

        }
        [Command("rtrans"), RequireOwner]
        public async Task ReloadTranslationsAsync(CommandContext ctx)
        {
            await _data.ReloadTranslationsAsync();
            await ctx.RespondAsync(ctx.Guild.GetTranslation(_data).ReloadedTranslationsText);
        }

        [Command("rhugs"), RequireOwner]
        public async Task ReloadHugsAsync(CommandContext ctx)
        {
            _data.ReloadHugs();
            await ctx.RespondAsync(ctx.Guild.GetTranslation(_data).ReloadedHugsText);
        }

        [Command("rguilds"), RequireOwner]
        public async Task ReloadGuildsAsync(CommandContext ctx)
        {
            await _data.LoadGuildsAsync();
            await ctx.RespondAsync(ctx.Guild.GetTranslation(_data).ReloadedGuildsText);
        }

        [Command("addhug"), RequireOwner]
        public async Task AddHugAsync(CommandContext ctx, params string[] urls)
        {
            foreach (var url in urls)
            {
                if (!url.EndsWith(".gif"))
                    return;
                var name = "hug-" + url.Split('/')[url.Split('/').Length - 1];
                var hugName = Directory.GetCurrentDirectory() + "\\hugs\\" + name;
                using (var stream = await _http.GetStreamAsync(url))
                {
                    using (var fs = File.Create(hugName))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
            }
        }

        [Command("eval"), RequireOwner]
        public async Task EvalAsync(CommandContext ctx, [RemainingText] string code)
        {
            if (!code.StartsWith("```"))
                return;
            if (!code.EndsWith("```"))
                return;
            code = code.TrimStart('`').TrimEnd('`');

            #region script compilation info
            var imports = new List<string>
            {
                "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http",
                "System.Net.Http.Headers", "System.Reflection", "System.Text", "System.Text.RegularExpressions",
                "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Entities",
                "DSharpPlus.EventArgs", "DSharpPlus.Exceptions",
                "Yui.Entities", "Yui", "Yui.Extensions"
            };
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location));

            #endregion

            #region  create compilation

            var globals = new ExecutionData(ctx, _data);
            var script = CSharpScript.Create(code, ScriptOptions.Default.AddImports(imports).AddReferences(references),
                typeof(ExecutionData));
            var sw = Stopwatch.StartNew();
            var compilation = script.Compile();
            sw.Stop();

            #endregion

            #region return compilation errors

            DiscordEmbedBuilder embed;
            if (compilation.Any(s => s.Severity == DiagnosticSeverity.Error))
            {
                embed = new DiscordEmbedBuilder();
                foreach (var xd in compilation.Take(3))
                {
                    var ls = xd.Location.GetLineSpan();
                    embed.AddField(
                        string.Concat("Error at ", ls.StartLinePosition.Line.ToString("#,##0"), ", ",
                            ls.StartLinePosition.Character.ToString("#,##0")), Formatter.InlineCode(xd.GetMessage()));
                }

                if (compilation.Length > 3)
                {
                    embed.AddField("Some errors ommited",
                        string.Concat((compilation.Length - 3).ToString("#,##0"), " more errors not displayed"));
                }

                await ctx.RespondAsync(embed: embed);
                return;
            }

            #endregion

            #region run script

            Exception runEx = null;
            ScriptState<object> scriptExec = null;
            var sw2 = Stopwatch.StartNew();
            try
            {
                scriptExec = await script.RunAsync(globals).ConfigureAwait(false);
                runEx = scriptExec.Exception;
            }
            catch (Exception ex)
            {
                runEx = ex;
            }

            sw2.Stop();

            #endregion

            #region return runtime errors

            if (runEx != null)
            {
                embed = new DiscordEmbedBuilder
                {
                    Title = $"Execution failed after {sw.ElapsedMilliseconds} ms with",
                    Description = runEx.ToString()
                };
                await ctx.RespondAsync(embed: embed);
                return;
            }

            #endregion

            #region return succesful run

            embed = new DiscordEmbedBuilder
            {
                Title = "Eval is successful",
            };
            embed.AddField("Returned: ",
                    scriptExec.ReturnValue == null ? "no value" : scriptExec.ReturnValue.ToString())
                .AddField("Compilation time: ", sw.ElapsedMilliseconds + "ms")
                .AddField("Execution time: ", sw2.ElapsedMilliseconds + "ms");
            await ctx.RespondAsync(embed: embed);

            #endregion
        }
    }
}