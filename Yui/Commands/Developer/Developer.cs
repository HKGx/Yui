using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Yui.Commands.Developer
{
    public class Developer : CommandModule
    {
        public Developer(SharedData data, Random random, HttpClient client) : base(data, random, client)
        {
        }

        [Command("end")]
        public async Task EndSession(CommandContext ctx)
        {
            if (ctx.Member.Id != ctx.Client.CurrentApplication.Owner.Id)
            {
                return;
            }
            Data.Cts.Cancel();
        }
        [Command("eval")]
        public async Task EvalAsync(CommandContext ctx, [RemainingText] string code)
        {
            if (ctx.Member.Id != ctx.Client.CurrentApplication.Owner.Id)
            {
                return;
            }

            if (!code.StartsWith("```"))
            {
                return;
            }

            if (!code.EndsWith("```"))
            {
                return;
            }
            code = code.TrimStart('`').TrimEnd('`');
            var imports = new List<string>
            {
                "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http",
                "System.Net.Http.Headers","System.IO","System.Reflection", "System.Text", "System.Text.RegularExpressions",
                "System.Threading.Tasks",
                "DSharpPlus", "DSharpPlus.CommandsNext", "DSharpPlus.Entities", "DSharpPlus.EventArgs", "DSharpPlus.Exceptions",
                "Yui.Entities", "Yui",
            };
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location));
                var globals = new ExecData(ctx, Data);
            var script = CSharpScript.Create(code, ScriptOptions.Default.AddImports(imports).AddReferences(references),
                typeof(ExecData));
            var sw = Stopwatch.StartNew();
            var compilation = script.Compile();
            sw.Stop();
            

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
                .AddField("Execution time: ", sw2.ElapsedMilliseconds + "ms")
                .AddField("Type: ",
                    scriptExec.ReturnValue == null ? "none" : scriptExec.ReturnValue.GetType().ToString());
            await ctx.RespondAsync(embed: embed);

            #endregion
        }
        
    }
}