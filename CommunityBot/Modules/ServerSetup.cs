﻿using System.Threading.Tasks;
using CommunityBot.Extensions;
using CommunityBot.Features.GlobalAccounts;
using Discord;
using Discord.Commands;

namespace CommunityBot.Modules
{
    public class ServerSetup : ModuleBase<MiunieCommandContext>

    {

        [Command("offLog")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerActivivtyLogOff()
        {
            var guild = GlobalGuildAccounts.GetGuildAccount(Context.Guild);
            guild.LogChannelId = 0;
            guild.ServerActivityLog = 0;
            GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);

            await ReplyAsync("No more Logging");

        }

        /// <summary>
        /// by saying "SetLog" it will create a   channel itself, you may move and rname it
        /// by saying "SetLog ID" it will set channel "ID" as Logging Channel
        /// by saying "SetLog" again, it will turn off Logging, but will not delete it from the file
        /// </summary>
        /// <param name="logChannel"></param>
        /// <returns></returns>
        [Command("SetLog")]
        [Alias("SetLogs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerActivivtyLog(ulong logChannel = 0)
        {
            var guild = GlobalGuildAccounts.GetGuildAccount(Context.Guild);

            if (logChannel != 0)
            {
                try
                {
                    var channel = Context.Guild.GetTextChannel(logChannel);
                    guild.LogChannelId = channel.Id;
                    guild.ServerActivityLog = 1;
                    GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);

                }
                catch
                {
//
                }

                return;
            }
            switch (guild.ServerActivityLog)
            {
                case 1:
                    guild.ServerActivityLog = 0;
                    guild.LogChannelId = 0;
                    GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);


                        await ReplyAsync("No more logging any activity now");

                    return;
                case 0:
                    try
                    {
                        try
                        {
                            var tryChannel = Context.Guild.GetTextChannel(guild.LogChannelId);
                            if (tryChannel.Name != null)
                            {
                                guild.LogChannelId = tryChannel.Id;
                                guild.ServerActivityLog = 1;
                                GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);

                                await ReplyAsync(
                                    $"Now we log everything to {tryChannel.Mention}, you may rename and move it.");
                            }
                        }
                        catch
                        {

                            var channel = Context.Guild.CreateTextChannelAsync("OctoLogs");
                            guild.LogChannelId = channel.Result.Id;
                            guild.ServerActivityLog = 1;
                            GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);

                            await ReplyAsync(
                                $"Now we log everything to {channel.Result.Mention}, you may rename and move it.");
                        }
                    }
                    catch
                    {
                     //ignored
                    }
                    break;
            }
        }



        [Command("SetRoleOnJoin")]
        [Alias("RoleOnJoin")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetRoleOnJoin(string role = null)
        {

            string text;
            var guild = GlobalGuildAccounts.GetGuildAccount(Context.Guild);
            if (role == null)
            {
                guild.RoleOnJoin = null;
                text = $"No one will get role on join from me!";
            }
            else
            {
                guild.RoleOnJoin = role;
                text = $"Everyone will now be getting {role} role on join!";
            }

            GlobalGuildAccounts.SaveAccounts(Context.Guild.Id);
            await ReplyAsync(text);

        }
    }
}