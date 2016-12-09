using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Past.Common.Utils;
using Past.Game.Network;
using Past.Protocol.Types;

namespace Past.Game.Engine
{
    public class GuildEngine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GuildEmblem GuildEmblems { get; set; }
        public double Xp { get; set; }
        public int Level { get; set; }
        public string CharactersString { get; set; }

        public List<CharacterEngine> Characters = new List<CharacterEngine>();
        public List<int> CharactersId = new List<int>();

        public void ParseCharacter()
        {
            var Elements = CharactersString.Split(',');
            foreach (var elem in Elements)
            {
                CharacterEngine Character = null;
                foreach (var c in Server.Clients.Where(c => c.Character.Id == int.Parse(elem)))
                {
                    Character = c.Character;
                }

                if (Character == null) continue;

                CharactersId.Add(int.Parse(elem));
                Characters.Add(Character);
                Character.Guild = this;
            }
        }

        public void AddCharacter(CharacterEngine Character)
        {
            if (Character == null)
                return;
            Characters.Add(Character);
            CharactersId.Add(Character.Id);
            CharactersString = string.Join(",", CharactersId);
        }

        public void RemoveCharacter(CharacterEngine Character)
        {
            if (Character == null)
                return;
            Characters.Remove(Character);
            CharactersId.Remove(Character.Id);
            CharactersString = string.Join(",", CharactersId);
        }

        public GuildMember[] GetGuildMembers()
        {
            return Characters.Select(elem => new GuildMember()
            {
                alignmentSide = (sbyte)elem.AlignmentSide,
                breed = (sbyte)elem.Breed,
                connected = 1,
                experienceGivenPercent = 0,
                givenExperience = 0,
                hoursSinceLastConnection = (ushort)Functions.ReturnUnixTimeStamp(elem.LastUsage.Value),
                level = elem.Level,
                name = elem.Name,
                rank = (short)elem.GuildRank,
                sex = elem.Sex
            }).ToArray();
        }
    }
}
