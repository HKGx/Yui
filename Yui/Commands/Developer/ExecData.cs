using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Yui.Commands.Developer
{
    public class ExecData
    {
        // ReSharper disable once InconsistentNaming
        public CommandContext ctx { get; }
        public DiscordGuild Guild => ctx.Guild;
        public DiscordChannel Channel => ctx.Channel;
        public DiscordMessage Message => ctx.Message;
        public HttpClient Http => ctx.Services.GetService<HttpClient>();
        public SharedData Data { get; }

        public ExecData(CommandContext ctx, SharedData data)
        {
            this.ctx = ctx;
            Data = data;
        }

    }
}