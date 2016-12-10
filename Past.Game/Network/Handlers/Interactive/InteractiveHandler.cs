using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Types;
using Past.Common.Utils;
using Past.Protocol.Messages;

namespace Past.Game.Network.Handlers.Interactive
{
    class InteractiveHandler
    {
        public static void HandleInteractiveUseRequestMessage(Client client, InteractiveUseRequestMessage message)
        {
            ConsoleUtils.Write(ConsoleUtils.Type.DEBUG, message.elemId.ToString());
            client.Send(new InteractiveUsedMessage(client.Character.Id, message.elemId, message.skillId, 0));
            client.Send(new InteractiveUseEndedMessage(message.elemId, message.skillId));
            ParseInteractive(client, message.elemId);
        }

        public static void HandleTeleportRequestMessage(Client client, TeleportRequestMessage message)
        {
            client.Character.Teleport(message.mapId, client.Character.CellId);
            client.Send(new LeaveDialogMessage());
        }

        public static void ParseInteractive(Client client, int elemId)
        {
            var ZaapElemId = new List<int>()
            {
                433973,
                57531,
                57534,
                260872,
                437118,
                406477,
                433974,
                443366,
                1202,
                57539,
                77916,
                57272,
                57590,
                57554,
                57527,
                57573,
                1254,
                149940,
                1246,
                1261,
                60101,
                159227,
                437117,
                58211,
                443686
            };

            var ZaapMapId = new List<int>()
            {
                2323,
                800,
                138543,
                147768,
                141588,
                148744,
                139265,
                133896,
                1797,
                3844,
                132096,
                2567,
                131597,
                5142,
                131608,
                17932,
                13060,
                12054,
                8991,
                13605,
                15654,
                15153,
                143372,
                144419,
                154642
            };

            var ZaapSubAreaId = new List<short>()
            {
                95,
                30,
                70,
                513, 
                524,
                74,
                182,
                2,
                180,
                1,
                10,
                3,
                22,
                490,
                170,
                93,
                161,
                117,
                115,
                529,
                118,
                116,
                526,
                511,
                466
            };

            var ZaapCost = new List<short>();

            for(var i = 0; i < ZaapMapId.Count; i++)
                ZaapCost.Add(0);

            if (ZaapElemId.Contains(elemId))
            {
                client.Send(new ZaapListMessage(0, ZaapMapId.ToArray(), ZaapSubAreaId.ToArray(), ZaapCost.ToArray(), 2323));
            }
        }
    }
}
