using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    class BowEnchtingTool : Item
    {

        [Constructable]
        public BowEnchtingTool() : base(0x1EB8)
        {
            Weight = 1.0;
        }

        public BowEnchtingTool(Serial serial) : base(serial)
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
            from.SendMessage("Select a bow you which to enchant");

            from.Target = new BownTarget(this);
        }

        private class BownTarget : Target
        {
            public BownTarget(BowEnchtingTool item) : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is Bow)
                {
                    BaseWeapon bow = (BaseWeapon)targ;
                    if ( !bow.Resource2.HasValue)
                        from.Target = new InternalTarget(bow);
                    else
                    {
                        from.SendMessage("This bow is already enchanted");
                    }

                }
            }


            private class InternalTarget : Target
            {
                private BaseWeapon i_bow;
                public InternalTarget(BaseWeapon b) : base(10, false, TargetFlags.None)
                {
                    i_bow = b;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    int amount = 0;
                    Type type;
                    Container ourPack = from.Backpack;
                    if (targeted is IronIngot)
                    {
                        IronIngot res = (IronIngot)targeted;
                        from.SendMessage("Iron selected");
                        i_bow.MaxDamage += 5;
                        i_bow.Hue = res.Hue;
                        i_bow.Resource2 = CraftResource.Iron;
                    }
                    if (targeted is BronzeIngot)
                    {
                        BronzeIngot res = (BronzeIngot)targeted;
                        from.SendMessage("Bronze selected");
                        i_bow.MaxDamage += 10;
                        i_bow.Hue = res.Hue;
                        i_bow.Resource2 = CraftResource.Bronze;
                        if (res.Amount > 1)
                        {
                            res.Amount -= 1;
                        }
                        else
                        {
                            res.Delete();
                        }
                    }
                }
            }

        } 
    }
}
