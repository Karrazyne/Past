using System.Collections.Generic;
using System.Linq.Expressions;
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
            var cellId = GetCellId(client, message.mapId, message.teleporterType);
            
            client.Character.Teleport(message.mapId, cellId);
            client.Send(new LeaveDialogMessage());
        }

        private static short GetCellId(Client client, int mapId, sbyte teleporterType)
        {
            short cellId;
            switch (teleporterType)
            {
                case 0: // Zaaps
                    switch (mapId)
                    {
                        case 2323:
                        case 139265:
                            cellId = 314;
                            break;
                        case 800:
                            cellId = 300;
                            break;
                        case 138543:
                            cellId = 215;
                            break;
                        case 147768:
                            cellId = 242;
                            break;
                        case 141588:
                            cellId = 313;
                            break;
                        case 148744:
                            cellId = 143;
                            break;
                        case 133896:
                        case 2567:
                            cellId = 235;
                            break;
                        case 1797:
                            cellId = 287;
                            break;
                        case 3844:
                            cellId = 254;
                            break;
                        case 132096:
                            cellId = 206;
                            break;
                        case 131597:
                            cellId = 355;
                            break;
                        case 5142:
                            cellId = 467;
                            break;
                        case 131608:
                            cellId = 381;
                            break;
                        case 17932:
                            cellId = 116;
                            break;
                        case 13060:
                            cellId = 173;
                            break;
                        case 12054:
                            cellId = 329;
                            break;
                        case 8991:
                            cellId = 228;
                            break;
                        case 13605:
                            cellId = 227;
                            break;
                        case 15654:
                            cellId = 259;
                            break;
                        case 15153:
                            cellId = 327;
                            break;
                        case 143372:
                            cellId = 257;
                            break;
                        case 144419:
                            cellId = 216;
                            break;
                        case 154642:
                            cellId = 271;
                            break;
                        default:
                            cellId = client.Character.CellId;
                            break;
                    }
                    break;
                case 1: //Zaapis
                    switch (mapId)
                    {
                        case 147767:
                            cellId = 257;
                            break;
                        case 147768:
                            cellId = 183;
                            break;
                        default:
                            cellId = client.Character.CellId;
                            break;
                    }
                    break;
                default:
                    cellId = client.Character.CellId;
                    break;
            }

            return cellId;
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

            var ZaapisElemId = new List<int>()
            {
                448783,
                450236
            };

            var ZaapisMapId = new List<int>()
            {
                147767, 
                147768
            };

            var ZaapCost = new List<short>();

            for(var i = 0; i < ZaapMapId.Count; i++)
                ZaapCost.Add(0);

            if (ZaapElemId.Contains(elemId))
            {
                client.Send(new ZaapListMessage(0, ZaapMapId.ToArray(), ZaapSubAreaId.ToArray(), ZaapCost.ToArray(), 2323));
            }
            if (ZaapisElemId.Contains(elemId))
            {
                client.Send(new ZaapListMessage(1, ZaapisMapId.ToArray(), new short[] {513, 513}, new short[] {0,0}, 2323 ));
            }
        }
    }
}
