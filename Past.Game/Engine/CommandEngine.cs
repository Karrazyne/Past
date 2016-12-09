using Past.Common.Data;
using Past.Game.Network;
using Past.Game.Network.Handlers.Basic;
using Past.Protocol.Enums;
using System.Linq;
using Past.Protocol.Messages;

namespace Past.Game.Engine
{
    public class CommandEngine
    {
        
        public static void Handle(Client client, string content)
        {
            string[] command = content.Split(' ');
            switch (command[0])
            {
                case ".help":
                    Command.Commands.Where(cmd => cmd.Value.Role <= client.Account.Role).ToList().ForEach(@cmd => BasicHandler.SendTextInformationMessage(client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 16, new string[] { $"{@cmd.Value.Name}", $"{@cmd.Value.Description}" }));
                    break;
                case ".save":
                    client.Character.Save();
                    break;
                case ".start":
                    client.Character.Teleport(client.Character.BreedData.StartMapId, client.Character.BreedData.StartDisposition.cellId);
                    break;
                case ".goname":
                    if (client.Account.Role >= GameHierarchyEnum.MODERATOR)
                    {
                        Client targetClient = Server.Clients.FirstOrDefault(target => target.Character.Name == command[1]);
                        if (targetClient != null && targetClient != client)
                        {
                            client.Character.Teleport(targetClient.Character.CurrentMapId, targetClient.Character.CellId);
                        }
                        else
                        {
                            BasicHandler.SendTextInformationMessage(client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 16, new string[] { "Error", $"Can't found the character {command[1]} !" });
                        }
                    }
                    break;
                case ".go":
                    if (client.Account.Role >= GameHierarchyEnum.MODERATOR)
                    {
                        Map map = Map.Maps.FirstOrDefault(findMap => findMap.Value.Id == int.Parse(command[1])).Value;
                        if (map != null && map.Id != client.Character.CurrentMapId)
                        {
                            client.Character.Teleport(map.Id, client.Character.CellId);
                        }
                        else
                        {
                            BasicHandler.SendTextInformationMessage(client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 16, new string[] { "Error", $"Can't found the map {command[1]} !" });
                        }
                    }
                    break;
                case ".levelup":
                    if (client.Account.Role >= GameHierarchyEnum.GAMEMASTER_PADAWAN)
                    {
                        client.Character.LevelUp();
                    }
                    break;
                case ".parcho":
                    var returnElement = string.Empty;
                    if (client.Character.Stats[StatEnum.VITALITY].@base < 101)
                    {
                        returnElement += "Vitalitée " + (101 - client.Character.Stats[StatEnum.VITALITY].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.VITALITY].@base += (short)(101 - client.Character.Stats[StatEnum.VITALITY].@base);
                    }
                    if (client.Character.Stats[StatEnum.WISDOM].@base < 101)
                    {
                        returnElement += "Sagesse " + (101 - client.Character.Stats[StatEnum.WISDOM].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.WISDOM].@base += (short)(101 - client.Character.Stats[StatEnum.WISDOM].@base);
                    }
                    if (client.Character.Stats[StatEnum.STRENGTH].@base < 101)
                    {
                        returnElement += "Force " + (101 - client.Character.Stats[StatEnum.STRENGTH].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.STRENGTH].@base += (short)(101 - client.Character.Stats[StatEnum.STRENGTH].@base);
                    }
                    if (client.Character.Stats[StatEnum.INTELLIGENCE].@base < 101)
                    {
                        returnElement += "Intelligence " + (101 - client.Character.Stats[StatEnum.INTELLIGENCE].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.INTELLIGENCE].@base += (short)(101 - client.Character.Stats[StatEnum.INTELLIGENCE].@base);
                    }
                    if (client.Character.Stats[StatEnum.CHANCE].@base < 101)
                    {
                        returnElement += "Chance " + (101 - client.Character.Stats[StatEnum.CHANCE].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.CHANCE].@base += (short)(101 - client.Character.Stats[StatEnum.CHANCE].@base);
                    }
                    if (client.Character.Stats[StatEnum.AGILITY].@base < 101)
                    {
                        returnElement += "Agilité " + (101 - client.Character.Stats[StatEnum.AGILITY].@base) +
                                          "\n";
                        client.Character.Stats[StatEnum.AGILITY].@base += (short)(101 - client.Character.Stats[StatEnum.AGILITY].@base);
                    }
                    client.Send(new TextInformationMessage(0, 0, new[] { "Vous avez été parchoté : \n" + returnElement }));
                    client.Send(new CharacterStatsListMessage(client.Character.GetCharacterCharacteristicsInformations));
                    client.Character.UpdateStats();
                    break;
                case ".guilde":
                    client.Send(new GuildCreationStartedMessage());
                    break;
                default:
                    BasicHandler.SendTextInformationMessage(client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 16, new string[] { "Error", $"Command {command[0]} not found !" });
                    break;
            }
        }
    }
}
