using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    class BowCraftingTool : Item
    {

        [Constructable]
        public BowCraftingTool()
        {
            Name = "BowCrafting Toolkit";
            Weight = 1.0;
            Movable = true;
            ItemID = 7864;
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
                    Container ourPack = from.Backpack;
                    bool worked = false;

                    CraftResource thisResource = CraftResources.GetFromType(targeted.GetType());

                    switch (thisResource)
                    {
                        case CraftResource.Iron:
                            {
                                worked = checking(i_bow, thisResource, from, 50, 5, targeted);
                                break;
                            }
                        case CraftResource.DullCopper:
                            {
                                worked = checking(i_bow, thisResource, from, 70, 6, targeted);
                                break;
                            }
                        case CraftResource.ShadowIron:
                            {
                                worked = checking(i_bow, thisResource, from, 80, 7, targeted);
                                break;
                            }
                        case CraftResource.Copper:
                            {
                                worked = checking(i_bow, thisResource, from, 90, 8, targeted);
                                break;
                            }
                        case CraftResource.Bronze:
                            {

                                worked = checking(i_bow, thisResource, from, 100, 10, targeted);
                                break;

                            }
                        case CraftResource.Gold:
                            {
                                worked = checking(i_bow, thisResource, from, 100, 10, targeted);
                                break;
                            }
                        case CraftResource.Agapite:
                            {
                                worked = checking(i_bow, thisResource, from, 100, 10, targeted);
                                break;
                            }
                        case CraftResource.Verite:
                            {
                                worked = checking(i_bow, thisResource, from, 100, 10, targeted);
                                break;
                            }
                        case CraftResource.Valorite:
                            {
                                worked = checking(i_bow, thisResource, from, 100, 10, targeted);
                                break;
                            }
                    }

                    if ( worked == true)
                    {
                        from.SendMessage("You have enchanted your bow with" + CraftResources.GetName(thisResource));
                    }
                    else
                    {
                        from.SendMessage("Not enough skill in blacksmithing");
                    }
                }
            }

            public static bool checking(BaseWeapon weapon, CraftResource resource, Mobile from, int reqSkill, int damageBonus, object targeted)
            {
                if (from.CheckSkill(SkillName.Blacksmith, reqSkill, 100))
                {
                    Item res = (Item)targeted;
                    weapon.Hue = CraftResources.GetHue(resource);
                    
                    if (res.Amount > 1)
                    {
                        res.Amount -= 1;
                    }
                    else
                    {
                        res.Delete();
                    }
                    return true;
                }
                else
                {
                    
                    return false;
                }
            }
        } 
    }
}
