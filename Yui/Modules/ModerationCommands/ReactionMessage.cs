using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LiteDB;
using Yui.Entities;
using Yui.Extensions;

namespace Yui.Modules.ModerationCommands
{
    [Group("reactionmessage"), Aliases("rm", "reaction-message", "rmessage", "reactionm")]
    public class ReactionMessage : BaseCommandModule
    {
        private SharedData _data;
        public ReactionMessage(SharedData data)
        {
            _data = data;
        }

        [GroupCommand,
         RequireBotPermissions(Permissions.SendMessages | Permissions.UseExternalEmojis | Permissions.AddReactions)]
        public async Task CreateReactionMessage(CommandContext ctx, IEnumerable<DiscordRole> roles,
            IEnumerable<DiscordEmoji> emojis)
        {
            if (!GuildUtilities.IsAdmin(ctx))
                return;
            
            var trans = ctx.Guild.GetTranslation(_data);
            
            var discordRoles = roles.ToList();
            var discordEmojis = emojis.ToList();
            if (discordRoles.Count() > 20 || discordEmojis.Count() > 20 ||
                discordEmojis.Count() != discordRoles.Count())
            {
                await ctx.RespondAsync(trans.ReactionCommandOverLimitText);
                return;
            }

            var find = (discordEmojis.Find(x => !YuiToolbox.YToolbox.Emojis.Contains(x) && x.Id > 0));
            if (find != null)
            {
                Console.WriteLine(find);
                await ctx.RespondAsync(trans.ReactionCommandEmojiUnreachableText);
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
                    Id = emoji.Id,
                    Name = emoji.Name,
                    Role = role.Id
                });
                embed.AddField(role.Name, emoji.ToString(), true);

            }


            var msg = await ctx.RespondAsync(embed: embed);
            using (var db = new LiteDatabase("Data.db"))
            {
                var rms = db.GetCollection<Entities.ReactionMessage>();
                rms.Insert(new Entities.ReactionMessage
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