using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    class BowEnchtingTool : Item
    {
        [Constructable]
        public BowEnchtingTool() : base( 0xF9F )
		{
            Weight = 1.0;
        }

        public BowEnchtingTool(Serial serial) : base( serial )
		{
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Select a bow you which to enchant"); // What should I use these scissors on?

            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private BowEnchtingTool m_Item;

            public InternalTarget ( BowEnchtingTool item) : base (2 , false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Item.Deleted)
                    return;
                if (targeted.GetType() == typeof(Bow))
                {
                    Console.Write("You selected a bow");
                }
            }

            
        }
    }
}
