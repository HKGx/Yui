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
using Yui.Entities;
using Yui.Extensions;

namespace Yui.Modules.ModerationCommands
{
    [Group("reactionmessage"), Aliases("rm", "reaction-message", "rmessage", "reactionm")]
    public class ReactionMessage : BaseCommandModule
    {
        private SharedData _data;
        private Random _random;

        public ReactionMessage(SharedData data, Random random, HttpClient client)
        {
            _data = data;
            _random = random;
        }

        [GroupCommand, RequireBotPermissions(Permissions.SendMessages | Permissions.UseExternalEmojis | Permissions.AddReactions)]
        public async Task CreateReactionMessage(CommandContext ctx, IEnumerable<DiscordRole> roles,
            IEnumerable<DiscordEmoji> emojis)
        {
            if (!GuildUtilities.IsAdmin(ctx, _data))
                return;
            var trans = ctx.Guild.GetTranslation(_data);
            var discordRoles = roles.ToList();
            var discordEmojis = emojis.ToList();
            if (discordRoles.Count() > 20 || discordEmojis.Count() > 20 || discordEmojis.Count() != discordRoles.Count())
            {
                await ctx.RespondAsync(trans.ReactionCommandOverLimitText);
                return;
            }

            var find = (discordEmojis.Find(x => !YuiToolbox.YToolbox.Emojis.Contains(x)));
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
            var dict = new ConcurrentBag<EmojiToRole>();
            for (var i = 0; i < discordRoles.Count(); i++)
            {
                var role = discordRoles[i];
                var emoji = discordEmojis[i];
                dict.Add(new EmojiToRole
                {
                    Id = emoji.Id,
                    IsGuild = emoji.Id > 0,
                    Name = emoji.Name,
                    Role = role.Id
                });
                embed.AddField(role.IsMentionable ? role.Name : role.Mention, emoji.ToString(), true);

            }

            var msg = await ctx.RespondAsync(embed: embed);
            foreach (var emoji in discordEmojis)
            {
                await msg.CreateReactionAsync(emoji);
            }

            foreach (var guild in _data.Guilds)
            {
                if (guild.Id != ctx.Guild.Id) continue;
                guild.ReactionMessages.Add(new Entities.ReactionMessage
                {
                    EmojiToRole = dict,
                    MessageId = msg.Id,
                    Channel = msg.ChannelId
                });
            }
            await _data.SaveGuildsAsync();
            
        }
        
    }
}