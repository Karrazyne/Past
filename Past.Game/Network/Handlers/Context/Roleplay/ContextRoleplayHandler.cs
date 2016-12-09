using Past.Common.Database.Record;
using Past.Game.Engine;
using Past.Game.Network.Handlers.Character;
using Past.Game.Network.Handlers.Inventory;
using Past.Protocol.Enums;
using Past.Protocol.Messages;
using Past.Protocol.Types;
using System;
using System.Linq;
using Past.Common.Data;

namespace Past.Game.Network.Handlers.Context.Roleplay
{
    public class ContextRoleplayHandler
    {
        public static void HandleMapInformationsRequestMessage(Client client, MapInformationsRequestMessage message)
        {
            client.Send(new MapComplementaryInformationsDataMessage(client.Character.Map.Id, (short)client.Character.Map.SubAreaId, new HouseInformations[0], new GameRolePlayActorInformations[0], Common.Data.Interactive.GetInteractiveElements(client.Character.Map.Id), new StatedElement[0], new MapObstacle[0], new FightCommonInformations[0]));
            client.Character.CurrentMap.AddClient(client);
        }

        public static void HandleChangeMapMessage(Client client, ChangeMapMessage message)
        {
            short cell = client.Character.CellId;
            if (client.Character.Map.TopNeighbourId == message.mapId)
                cell += 532;
            if (client.Character.Map.BottomNeighbourId == message.mapId)
                cell -= 532;
            if (client.Character.Map.LeftNeighbourId == message.mapId)
                cell += 13;
            if (client.Character.Map.RightNeighbourId == message.mapId)
                cell -= 13;
            client.Character.Teleport(message.mapId, cell);
        }

        public static void HandleEmotePlayRequestMessage(Client client, EmotePlayRequestMessage message)
        {
            if (Enum.IsDefined(typeof(EmoteEnum), message.emoteId))
            {
                client.Character.CurrentMap.Send(new EmotePlayMessage(message.emoteId, 0, client.Character.Id));
            }
        }

        public static void HandleStatsUpgradeRequestMessage(Client client, StatsUpgradeRequestMessage message)
        {
            if (Enum.IsDefined(typeof(StatBoost), message.statId))
            {
                if (message.boostPoint >= 1 && client.Character.StatsPoints >= message.boostPoint)
                {
                    switch((StatBoost)message.statId)
                    {
                        case StatBoost.STRENGTH:
                            #region ...
                            /*statFloor = client.Character.BreedData.StatsPointsForStrength.Split('|', ',');
                            short strength = client.Character.Stats[StatEnum.STRENGTH].@base;
                            if (strength >= short.Parse(statFloor[0]) && strength <= short.Parse(statFloor[2])) //0 to 50
                            {
                                points = short.Parse(statFloor[1]);
                            }
                            else if (strength >= short.Parse(statFloor[2]) && strength <= short.Parse(statFloor[4])) //50 to 150
                            {
                                points = short.Parse(statFloor[3]);
                            }
                            else if (strength >= short.Parse(statFloor[4]) && strength <= short.Parse(statFloor[6])) //150 to 250
                            {
                                points = short.Parse(statFloor[5]);
                            }
                            else if (strength >= short.Parse(statFloor[6])) //250 and plus
                            {
                                points = short.Parse(statFloor[7]);
                            }*/
                            #endregion
                            client.Character.Stats[StatEnum.STRENGTH].@base += message.boostPoint;
                            client.Character.Strength = client.Character.Stats[StatEnum.STRENGTH].@base;
                            break;
                        case StatBoost.VITALITY:
                            client.Character.Stats[StatEnum.VITALITY].@base += message.boostPoint;
                            client.Character.Vitality = client.Character.Stats[StatEnum.VITALITY].@base;
                            break;
                        case StatBoost.WISDOM:
                            client.Character.Stats[StatEnum.WISDOM].@base += message.boostPoint;
                            client.Character.Wisdom = client.Character.Stats[StatEnum.WISDOM].@base;
                            break;
                        case StatBoost.CHANCE:
                            client.Character.Stats[StatEnum.CHANCE].@base += message.boostPoint;
                            client.Character.Chance = client.Character.Stats[StatEnum.CHANCE].@base;
                            break;
                        case StatBoost.AGILITY:
                            client.Character.Stats[StatEnum.AGILITY].@base += message.boostPoint;
                            client.Character.Agility = client.Character.Stats[StatEnum.AGILITY].@base;
                            break;
                        case StatBoost.INTELLIGENCE:
                            client.Character.Stats[StatEnum.INTELLIGENCE].@base += message.boostPoint;
                            client.Character.Intelligence = client.Character.Stats[StatEnum.INTELLIGENCE].@base;
                            break;
                    }
                    client.Character.StatsPoints -= message.boostPoint;
                    client.Character.Stats.Refresh();
                    client.Send(new StatsUpgradeResultMessage(message.boostPoint));
                    CharacterHandler.SendCharacterStatsListMessage(client);
                }
            }
        }

        public static void HandleSpellUpgradeRequestMessage(Client client, SpellUpgradeRequestMessage message)
        {
            CharacterSpellRecord spellRecord = client.Character.Spells.FirstOrDefault(spell => spell.SpellId == message.spellId);
            if (spellRecord != null && client.Character.SpellsPoints > 0)
            {
                client.Character.SpellsPoints--;
                client.Send(new SpellUpgradeSuccessMessage(spellRecord.SpellId, spellRecord.Level++));
                CharacterHandler.SendCharacterStatsListMessage(client);
                InventoryHandler.SendSpellListMessage(client);
            }
            else
            {
                client.Send(new SpellUpgradeFailureMessage());
            }
        }

        public static void HandleSpellMoveMessage(Client client, SpellMoveMessage message)
        {
            CharacterSpellRecord spellRecord = client.Character.Spells.FirstOrDefault(spell => spell.SpellId == message.spellId);
            if (spellRecord != null)
            {
                byte oldPosition = spellRecord.Position;
                byte newPosition = message.position;
                CharacterSpellRecord spell2Record = client.Character.Spells.FirstOrDefault(x => x.Position == newPosition);
                if (spell2Record != null)
                {
                    spellRecord.Position = newPosition;
                    spell2Record.Position = oldPosition;
                    client.Send(new SpellMovementMessage((short)spellRecord.SpellId, newPosition));
                    client.Send(new SpellMovementMessage((short)spell2Record.SpellId, oldPosition));
                }
                else
                {
                    spellRecord.Position = newPosition;
                    client.Send(new SpellMovementMessage((short)spellRecord.SpellId, newPosition));
                }
            }
        }

        public static void HandlePartyInvitationRequestMessage(Client client, PartyInvitationRequestMessage message)
        {
            Client targetClient = Server.Clients.FirstOrDefault(target => target.Character.Name == message.name);
            if (targetClient != null && targetClient != client)
            {
                if (client.Character.Party != null)
                {
                    //invite
                }
                else
                {
                    client.Character.Party = new PartyEngine(client, targetClient);
                }
            }
            else
            {
                client.Send(new PartyCannotJoinErrorMessage((sbyte)PartyJoinErrorEnum.PARTY_JOIN_ERROR_PLAYER_NOT_FOUND));
            }
        }

        public static void HandlePartyAcceptInvitationMessage(Client client, PartyAcceptInvitationMessage message)
        {
        }

        public static void HandleGuidedModeQuitRequestMessage(Client client, GuidedModeQuitRequestMessage message)
        {

        }

        public static void HandleGuidedModeReturnRequestMessage(Client client, GuidedModeReturnRequestMessage message)
        {

        }

        public static void SendEmoteListMessage(Client client, sbyte[] emoteIds)
        {
            client.Send(new EmoteListMessage(emoteIds));
        }
    }
}
