using Past.Protocol.IO;

namespace Past.Protocol.Types
{
    public class FightTeamInformations : AbstractFightTeamInformations
    {
        public FightTeamMemberInformations[] teamMembers;
        public new const short Id = 33;
        public override short TypeId
        {
            get { return Id; }
        }
        public FightTeamInformations()
        {
        }
        public FightTeamInformations(sbyte teamId, int leaderId, sbyte teamSide, FightTeamMemberInformations[] teamMembers) : base(teamId, leaderId, teamSide)
        {
            this.teamMembers = teamMembers;
        }
        public override void Serialize(IDataWriter writer)
        {
            base.Serialize(writer);
            writer.WriteUShort((ushort)teamMembers.Length);
            foreach (var entry in teamMembers)
            {
                writer.WriteShort(entry.TypeId);
                entry.Serialize(writer);
            }
        }
        public override void Deserialize(IDataReader reader)
        {
            base.Deserialize(reader);
            var limit = reader.ReadUShort();
            teamMembers = new Types.FightTeamMemberInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                teamMembers[i] = (FightTeamMemberInformations)ProtocolTypeManager.GetInstance(reader.ReadUShort());
                teamMembers[i].Deserialize(reader);
            }
        }
    }
}