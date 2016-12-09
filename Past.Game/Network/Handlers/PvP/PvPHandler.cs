using Past.Game.Network.Handlers.Character;
using Past.Protocol.Messages;

namespace Past.Game.Network.Handlers.PvP
{
    public class PvPHandler
    {
        public static void HandleGetPVPActivationCostMessage(Client client, GetPVPActivationCostMessage message)
        {
            client.Send(new PVPActivationCostMessage(client.Character.PvPActivationCost));
        }

        public static void HandleSetEnablePVPRequestMessage(Client client, SetEnablePVPRequestMessage message)
        {
            client.Character.PvPEnabled = message.enable;
            client.Character.CurrentMap.Send(new GameRolePlayShowActorMessage(client.Character.GetGameRolePlayCharacterInformations()));
            if (!message.enable)
            {
                client.Character.Honor -= (ushort)client.Character.PvPActivationCost;
            }
            CharacterHandler.SendCharacterStatsListMessage(client);
        }

        public static void SendAlignmentRankUpdateMessage(Client client, sbyte alignmentRank)
        {
            client.Send(new AlignmentRankUpdateMessage(alignmentRank, false));
        }

        public static void SendAlignmentSubAreasListMessage(Client client)
        {
            client.Send(new AlignmentSubAreasListMessage(new short[0], new short[0]));
        }
    }
}
