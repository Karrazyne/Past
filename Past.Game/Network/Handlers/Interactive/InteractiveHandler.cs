using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Past.Common.Utils;
using Past.Protocol.Messages;

namespace Past.Game.Network.Handlers.Interactive
{
    class InteractiveHandler
    {
        public static void HandleInteractiveUseRequestMessage(Client client, InteractiveUseRequestMessage message)
        {
            ConsoleUtils.Write(ConsoleUtils.Type.DEBUG, message.elemId.ToString());
            client.Send(new InteractiveUsedMessage(client.Character.Id, message.elemId, message.skillId, 1));
            client.Send(new InteractiveUseEndedMessage(message.elemId, message.skillId));
        }
    }
}
