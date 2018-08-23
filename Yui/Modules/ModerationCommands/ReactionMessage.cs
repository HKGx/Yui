using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LiteDB;
using Yui.Entities.Commands;
using Yui.Entities.Database;
using Yui.Extensions;

namespace Yui.Modules.ModerationCommands
{
    [Group("reactionmessage"), Aliases("rm", "reaction-message", "rmessage", "reactionm")]
    public class ReactionMessage : CommandModule
    {
        public ReactionMessage(SharedData data, Random random, HttpClient http) : base(data, random, http)
        {
        }

        [GroupCommand,
         RequireBotPermissions(Permissions.SendMessages | Permissions.UseExternalEmojis | Permissions.AddReactions)]
        public async Task CreateReactionMessage(CommandContext ctx, IEnumerable<DiscordRole> roles,
            IEnumerable<DiscordEmoji> emojis)
        {
            if (!GuildUtilities.IsAdmin(ctx))
                return;
            
            var trans = ctx.Guild.GetTranslation(Data);
            
            var discordRoles = roles.ToList();
            var discordEmojis = emojis.ToList();
            if (discordRoles.Count() > 20 || discordEmojis.Count() > 20 ||
                discordEmojis.Count() != discordRoles.Count())
            {
                await ctx.RespondAsync(trans.ReactionCommandOverLimitText);
                return;
            }


            await ctx.Message.DeleteAsync();
            var embed = new DiscordEmbedBuilder
            {
                Title = trans.ReactionCommandEmbedNameText
            };
            var list = new List<EmojiToRole>();
            for (var i = 0; i < discordRoles.Count(); i++)
            {
                var role = discordRoles[i];
                var emoji = discordEmojis[i];
                list.Add(new EmojiToRole
                {
                    FullName = emoji.ToString(),
                    Role = role.Id
                });
                
                embed.AddField(role.Name, emoji.ToString(), true);

            }


            var msg = await ctx.RespondAsync(embed: embed);
            using (var db = new LiteDatabase("Data.db"))
            {
                var rms = db.GetCollection<Entities.Database.ReactionMessage>();
                rms.Insert(new Entities.Database.ReactionMessage
                {
                    ChannelId = ctx.Channel.Id,
                    EmojiToRole = list,
                    GuildId = ctx.Guild.Id,
                    MessageId = msg.Id
                });
            }
            foreach (var emoji in discordEmojis)
            {
                await msg.CreateReactionAsync(emoji);
            }


        }

    }
}