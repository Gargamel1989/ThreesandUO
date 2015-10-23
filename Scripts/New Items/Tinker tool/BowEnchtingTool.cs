using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    class BowCraftingTool : Item
    {

        [Constructable]
        public BowCraftingTool() : base(0x1EB8)
        {
            Weight = 1.0;
        }

        public BowCraftingTool(Serial serial) : base(serial)
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
            public BownTarget(BowCraftingTool item) : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is Bow)
                {
                    BaseWeapon bow = (BaseWeapon)targ;
                    if (!bow.Resource2.HasValue)
                    { 
                        from.SendMessage("Select an ingot to echant the bow with");
                        from.Target = new InternalTarget(bow);
                    }
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
                    String resourceName = "";
                    Container ourPack = from.Backpack;

                    CraftResource thisResource = CraftResources.GetFromType(targeted.GetType());

                    BaseIngot bier;

                    switch (thisResource)
                    {
                        case CraftResource.Iron:
                            {
                                IronIngot res = (IronIngot)targeted;
                                resourceName = "Iron";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.Iron;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.DullCopper:
                            {
                                DullCopperIngot res = (DullCopperIngot)targeted;
                                resourceName = "DullCopper";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.DullCopper;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.ShadowIron:
                            {

                                ShadowIronIngot res = (ShadowIronIngot)targeted;
                                resourceName = "ShadowIron";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.ShadowIron;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.Copper:
                            {
                                ShadowIronIngot res = (ShadowIronIngot)targeted;
                                resourceName = "ShadowIron";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.ShadowIron;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.Bronze:
                            {
                                BronzeIngot res = (BronzeIngot)targeted;
                                resourceName = "Bronze";
                                i_bow.MaxDamage += 5;
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
                                break;
                            }
                        case CraftResource.Gold:
                            {
                                GoldIngot res = (GoldIngot)targeted;
                                resourceName = "Gold";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.Gold;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.Agapite:
                            {
                                AgapiteIngot res = (AgapiteIngot)targeted;
                                resourceName = "Agapite";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.Agapite;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.Verite:
                            {
                                VeriteIngot res = (VeriteIngot)targeted;
                                resourceName = "Verite";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.Verite;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                        case CraftResource.Valorite:
                            {
                                ValoriteIngot res = (ValoriteIngot)targeted;
                                resourceName = "Valorite";
                                i_bow.MaxDamage += 5;
                                i_bow.Hue = res.Hue;
                                i_bow.Resource2 = CraftResource.Valorite;
                                if (res.Amount > 1)
                                {
                                    res.Amount -= 1;
                                }
                                else
                                {
                                    res.Delete();
                                }
                                break;
                            }
                    }
                    from.SendMessage(resourceName + " added to your bow");
                }
            }
        } 
    }
}
