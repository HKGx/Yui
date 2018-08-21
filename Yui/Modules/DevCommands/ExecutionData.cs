using System.Net.Http;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Yui.Modules
{
    public class ExecutionData
    {
        public CommandContext Context { get; }
        public DiscordGuild Guild => Context.Guild;
        public HttpClient Http => Context.Services.GetService<HttpClient>();
        public SharedData Data { get; }
        public YuiToolbox Toolbox = YuiToolbox.YToolbox;
        public ExecutionData(CommandContext ctx, SharedData data)
        {
            Context = ctx;
            Data = data;
        }
    }
}