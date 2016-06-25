using System;
using Past.Protocol.IO;

namespace Past.Protocol.Types
{
    public class ObjectEffectInteger : ObjectEffect
    {
        public short value;
        public new const short Id = 70;
        public override short TypeId
        {
            get { return Id; }
        }
        public ObjectEffectInteger()
        {
        }
        public ObjectEffectInteger(short actionId, short value) : base(actionId)
        {
            this.value = value;
        }

        public override void Serialize(IDataWriter writer)
        {
            base.Serialize(writer);
            writer.WriteShort(value);

        }
        public override void Deserialize(IDataReader reader)
        {
            base.Deserialize(reader);
            value = reader.ReadShort();
            if (value < 0)
                throw new Exception("Forbidden value on value = " + value + ", it doesn't respect the following condition : value < 0");
        }
    }
}

