using Past.Game.Network.Handlers.Basic;
using Past.Protocol.Enums;
using Past.Protocol.Messages;
using System;
using System.Linq;
using Past.Common.Utils;

namespace Past.Game.Network.Handlers.Context
{
    public class ContextHandler
    {
        public static void HandleGameContextCreateRequestMessage(Client client, GameContextCreateRequestMessage message)
        {
            SendGameContextDestroyMessage(client);
            SendGameContextCreateMessage(client, GameContextEnum.ROLE_PLAY);
            SendCurrentMapMessage(client, client.Character.CurrentMapId);
        }

        public static void HandleGameMapMovementRequestMessage(Client client, GameMapMovementRequestMessage message)
        {
            client.Character.CellId = (short)(message.keyMovements.Last() & 4095);
            client.Character.Direction = (DirectionsEnum)(message.keyMovements.Last() >> 12);
            client.Character.CurrentMap.Send(new GameMapMovementMessage(client.Character.Id, message.keyMovements));
            client.Character.CurrentMap.Send(new GameContextRefreshEntityLookMessage(client.Character.Id, client.Character.EntityLook));
        }

        public static void HandleGameMapMovementConfirmMessage(Client client, GameMapMovementConfirmMessage message)
        {
            BasicHandler.SendBasicNoOperationMessage(client);
        }

        public static void HandleGameMapChangeOrientationRequestMessage(Client client, GameMapChangeOrientationRequestMessage message)
        {
            if (Enum.IsDefined(typeof(DirectionsEnum), message.direction))
            {
                client.Character.Direction = (DirectionsEnum)message.direction;
                client.Character.CurrentMap.Send(new GameMapChangeOrientationMessage(client.Character.Id, message.direction));
                client.Character.CurrentMap.Send(new GameContextRefreshEntityLookMessage(client.Character.Id, client.Character.EntityLook));
            }
        }

        public static void SendGameContextCreateMessage(Client client, GameContextEnum context)
        {
            client.Send(new GameContextCreateMessage((sbyte)context));
        }

        public static void SendGameContextDestroyMessage(Client client)
        {
            client.Send(new GameContextDestroyMessage());
        }

        public static void SendCurrentMapMessage(Client client, int mapId)
        {
            client.Send(new CurrentMapMessage(mapId));
        }
    }
}
