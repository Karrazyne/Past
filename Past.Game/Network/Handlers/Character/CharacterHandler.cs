using Past.Common.Data;
using Past.Common.Database.Record;
using Past.Common.Utils;
using Past.Game.Engine;
using Past.Game.Network.Handlers.Chat;
using Past.Game.Network.Handlers.Context.Roleplay;
using Past.Game.Network.Handlers.Inventory;
using Past.Game.Network.Handlers.PvP;
using Past.Protocol.Enums;
using Past.Protocol.Messages;
using Past.Protocol.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Past.Game.Helper;

namespace Past.Game.Network.Handlers.Character
{
    public class CharacterHandler
    {
        public static void HandleCharactersListRequestMessage(Client client, CharactersListRequestMessage message)
        {
            if (client.Account.Characters.Count < 0)
            {
                client.Send(new CharactersListMessage(false, true, new CharacterBaseInformations[0]));
            }
            else
            {
                SendCharactersListMessage(client, false);
            }
        }

        public static void HandleCharacterCreationRequestMessage(Client client, CharacterCreationRequestMessage message)
        {
            if (client.Account.Characters.Count >= 5)
            {
                client.Send(new CharacterCreationResultMessage((sbyte)CharacterCreationResultEnum.ERR_TOO_MANY_CHARACTERS));
            }
            else
            {
                if (CharacterRecord.NameExist(message.name)) //need to add check for non allowed char
                {
                    client.Send(new CharacterCreationResultMessage((sbyte)CharacterCreationResultEnum.ERR_NAME_ALREADY_EXISTS));
                }
                else
                {
                    Breed breed = Breed.Breeds[(BreedEnum)message.breed];
                    string look = message.sex == false ? breed.MaleLook : breed.FemaleLook;
                    CharacterRecord character = new CharacterRecord(client.Account.Id, message.name, 1, 0, (BreedEnum)message.breed, message.colors.Distinct().Count() != 1 ? look.Insert(look.IndexOf("||") + 1, $"1={message.colors[0]},2={message.colors[1]},3={message.colors[2]},4={message.colors[3]},5={message.colors[4]}") : look, message.sex, Config.StartMap != 0 ? Config.StartMap : breed.StartMapId, Config.StartCellId != 0 ? Config.StartCellId : breed.StartDisposition.cellId, Config.StartDirection != 0 ? (DirectionsEnum)Config.StartDirection : (DirectionsEnum)breed.StartDisposition.direction, AlignmentSideEnum.ALIGNMENT_NEUTRAL, 0, 0, false, 0, 0, 0, DateTime.Now);
                    character.Create();
                    client.Account.Characters = CharacterRecord.ReturnCharacters(client.Account.Id);
                    new List<CharacterSpellRecord>(new CharacterSpellRecord[] { new CharacterSpellRecord(client.Account.Characters.FirstOrDefault(chara => chara.Name == character.Name).Id, 64, 0, 1), new CharacterSpellRecord(client.Account.Characters.FirstOrDefault(chara => chara.Name == character.Name).Id, 65, breed.BreedSpellsId[0], 1), new CharacterSpellRecord(client.Account.Characters.FirstOrDefault(chara => chara.Name == character.Name).Id, 66, breed.BreedSpellsId[1], 1), new CharacterSpellRecord(client.Account.Characters.FirstOrDefault(chara => chara.Name == character.Name).Id, 67, breed.BreedSpellsId[2], 1) }).ForEach(spell => spell.Create());
                    client.Send(new CharacterCreationResultMessage((sbyte)CharacterCreationResultEnum.OK));
                    SendCharactersListMessage(client, false);
                }
            }
        }

        public static void HandleCharacterNameSuggestionRequestMessage(Client client, CharacterNameSuggestionRequestMessage message)
        {
            client.Send(new CharacterNameSuggestionSuccessMessage(Functions.RandomName()));
        }

        public static void HandleCharacterDeletionRequestMessage(Client client, CharacterDeletionRequestMessage message)
        {
            CharacterRecord characterRecord = client.Account.Characters.FirstOrDefault(character => character.Id == message.characterId);
            if (characterRecord != null)
            {
                if (characterRecord.Level >= 20 ? Functions.CipherSecretAnswer(message.characterId, client.Account.SecretAnswer) != message.secretAnswerHash : Functions.CipherSecretAnswer(message.characterId, "000000000000000000") != message.secretAnswerHash)
                {
                    client.Send(new CharacterDeletionErrorMessage((sbyte)CharacterDeletionErrorEnum.DEL_ERR_BAD_SECRET_ANSWER));
                }
                else
                {
                    characterRecord.Delete();
                    client.Account.Characters.Remove(characterRecord);
                    SendCharactersListMessage(client, false);
                }
            }
            else
            {
                client.Send(new CharacterDeletionErrorMessage((sbyte)CharacterDeletionErrorEnum.DEL_ERR_NO_REASON));
            }
        }

        public static void HandleCharacterSelectionMessage(Client client, CharacterSelectionMessage message)
        {
            SelectCharacter(client, message.id);
        }

        public static void HandleCharacterFirstSelectionMessage(Client client, CharacterFirstSelectionMessage message) //TODO Tutorial
        {
            SelectCharacter(client, message.id);
        }

        public static void SendCharactersListMessage(Client client, bool tutorial)
        {
            CharacterBaseInformations[] characterBaseInformations = new CharacterBaseInformations[client.Account.Characters.Count];
            for (int i = 0; i < client.Account.Characters.Count; i++)
            {
                //characterBaseInformations[i] = new CharacterBaseInformations(client.Account.Characters[i].Id, client.Account.Characters[i].Name, client.Account.Characters[i].Level, Functions.BuildEntityLook(client.Account.Characters[i].EntityLookString), (sbyte)client.Account.Characters[i].Breed, client.Account.Characters[i].Sex);
                characterBaseInformations[i] = new CharacterBaseInformations(client.Account.Characters[i].Id, client.Account.Characters[i].Name, client.Account.Characters[i].Level, /*Functions.BuildEntityLook(client.Account.Characters[i].EntityLookString)*/ GeneralHelper.BuildEntityLook(client.Account.Characters[i].EntityLookString, client.Account.Characters[i]), (sbyte)client.Account.Characters[i].Breed, client.Account.Characters[i].Sex);
            }
            client.Send(new CharactersListMessage(false, tutorial, characterBaseInformations));
        }

        public static void SendCharacterStatsListMessage(Client client)
        {
            client.Send(new CharacterStatsListMessage(client.Character.GetCharacterCharacteristicsInformations));
        }

        public static void SendSetCharacterRestrictionsMessage(Client client)
        {
            client.Send(new SetCharacterRestrictionsMessage(new ActorRestrictionsInformations(false, false, false, false, false, false, false, false, true, false, false, false, false, true, true, true, false, false, false, false, false)));
        }

        public static void SendLifePointsRegenBeginMessage(Client client, byte regenRate)
        {
            client.Send(new LifePointsRegenBeginMessage(regenRate));
        }

        public static void SendCharacterLevelUpMessage(Client client, byte level)
        {
            client.Send(new CharacterLevelUpMessage(level));
        }

        public static void SelectCharacter(Client client, int id)
        {
            CharacterRecord characterRecord = client.Account.Characters.FirstOrDefault(character => character.Id == id);
            if (characterRecord != null)
            {
                CharacterEngine character = new CharacterEngine(characterRecord, client);
                client.Character = character;
                client.Send(new CharacterSelectedSuccessMessage(character.GetCharacterBaseInformations));
                ContextRoleplayHandler.SendEmoteListMessage(client, new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 19, 21, 22, 23, 24 });
                ChatHandler.SendEnabledChannelsMessage(client, new sbyte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
                InventoryHandler.SendInventoryContentMessage(client, new ObjectItem[0], character.Kamas); //TODO Get the characters items
                InventoryHandler.SendInventoryWeightMessage(client, 0, character.MaxPods);
                InventoryHandler.SendSpellListMessage(client);
                PvPHandler.SendAlignmentRankUpdateMessage(client, 1);
                PvPHandler.SendAlignmentSubAreasListMessage(client);
                SendSetCharacterRestrictionsMessage(client);
                SendLifePointsRegenBeginMessage(client, 10);
                SendCharacterStatsListMessage(client);
                client.Character.SendLoginMessage();
                /*if (tutorial == true)
                {
                    character.CurrentMapId = 35651584; //mapid of the tutorial map
                    character.CellId = 324;
                    client.Send(new QuestStartedMessage(489)); //start the tutorial quest
                }*/
            }
            else
            {
                client.Send(new CharacterSelectedErrorMessage());
            }
        }
    }
}
