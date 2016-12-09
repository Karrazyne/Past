using Past.Game.Engine;
using Past.Protocol.Enums;
using Past.Protocol.Messages;

namespace Past.Game.Network.Handlers.Guild
{
    class GuildeHandler
    {
        public static void HandleGuildCreationValidMessage(Client client, GuildCreationValidMessage message)
        {
            if (client.Character.Guild != null)
                return;

            var Guilde = new GuildEngine()
            {
                Name = message.guildName,
                Level = 200,
                Xp = 354658435241,
                GuildEmblems = message.guildEmblem
            };
            client.Character.Guild = Guilde;
            client.Character.GuildRank = 1;

            client.Send(new GuildCreationResultMessage((sbyte)GuildCreationResultEnum.GUILD_CREATE_OK));
            client.Character.Guild.AddCharacter(client.Character);
            client.Send(new GuildJoinedMessage(Guilde.Name, Guilde.GuildEmblems, (uint)client.Character.GuildRank));
            /*client.Send(new GuildInformationsMembersMessage(new[]
            {
                new GuildMember(client.Character.Id, client.Character.Name, client.Character.Level, (sbyte)client.Character.Breed, client.Character.Sex,  1, 0, 5, 1, 1, (sbyte)client.Character.AlignmentSide, (ushort)Functions.ReturnUnixTimeStamp(client.Character.LastUsage.Value)), 
            }));*/
            client.Send(new GuildInformationsMembersMessage(Guilde.GetGuildMembers()));
            client.Send(new GuildInformationsGeneralMessage(true, (sbyte)Guilde.Level, Guilde.Xp + 120, Guilde.Xp, Guilde.Xp + 120));
        }
    }
}
