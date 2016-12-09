using Past.Common.Data;
using Past.Game.Network;
using Past.Protocol;
using Past.Protocol.Messages;
using System.Collections.Generic;
using System.Linq;

namespace Past.Game.Engine
{
    public class MapEngine
    {
        private readonly List<Client> _clients;

        public MapEngine()
        {
            _clients = new List<Client>();
        }

        public void Send(NetworkMessage message)
        {
            _clients.ForEach(client => client.Send(message));
        }

        public void SendGameRolePlayShowActorMessage(Client client)
        {
            foreach (var _client in _clients)
            {
                client.Send(new GameRolePlayShowActorMessage(_client.Character.GetGameRolePlayCharacterInformations()));
            }
        }

        public void SendCharacterLevelUpInformation(Client client)
        {
            foreach (var _client in _clients.Where(_client => _client != client))
            {
                Send(new CharacterLevelUpInformationMessage(client.Character.Level, client.Character.Name, client.Character.Id, 0));
            }
        }

        public void AddClient(Client client)
        {
            lock (_clients)
            {
                if (!_clients.Contains(client))
                {
                    _clients.Add(client);
                    SendGameRolePlayShowActorMessage(client);
                    Send(new GameRolePlayShowActorMessage(client.Character.GetGameRolePlayCharacterInformations()));
                }
            }
        }

        public void RemoveClient(Client client)
        {
            lock (_clients)
            {
                if (_clients.Contains(client))
                {
                    _clients.Remove(client);
                    Send(new GameContextRemoveElementMessage(client.Character.Id));
                }
            }
        }

        public static void Initialize()
        {
            foreach (var map in Map.Maps.Values)
            {
                map.Instance = new MapEngine();
            }
        }
    }
}
