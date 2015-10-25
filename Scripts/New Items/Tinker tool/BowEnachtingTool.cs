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
            from.SendMessage("Select the bow you wish to enchant.");

            from.Target = new BowTarget(this);
        }

        private class BowTarget : Target
        {
            public BowTarget(BowCraftingTool item) : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is BaseRanged)
                {
                    BaseWeapon bow = (BaseWeapon)targ;
                    if (!bow.Resource2.HasValue)
                    { 
                        from.SendMessage("Select an ingot to echant the bow with.");
                        from.Target = new InternalTarget(bow);
                    }
                    else
                    {
                        from.SendMessage("This bow is already enchanted.");
                    }

                }
                else
                {
                    from.SendMessage("That item can not be enchanted!");
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
                    //select backpack
                    Container ourPack = from.Backpack;
                    Item selectedItem = (Item)targeted;
                    CraftResource thisResource = CraftResources.GetFromType(targeted.GetType()); // select resource

                    if (selectedItem is BaseIngot)
                    {
                        //look for the resource chosen
                        switch (thisResource)
                        {
                            case CraftResource.Iron:
                                {
                                    CheckSkill(thisResource, from, 50, 5, targeted);
                                    break;
                                }
                            case CraftResource.DullCopper:
                                {
                                    CheckSkill(thisResource, from, 70, 6, targeted);
                                    break;
                                }
                            case CraftResource.ShadowIron:
                                {
                                    CheckSkill(thisResource, from, 80, 7, targeted);
                                    break;
                                }
                            case CraftResource.Copper:
                                {
                                    CheckSkill(thisResource, from, 90, 8, targeted);
                                    break;
                                }
                            case CraftResource.Bronze:
                                {
                                    CheckSkill(thisResource, from, 100, 10, targeted);
                                    break;
                                }
                            case CraftResource.Gold:
                                {
                                    CheckSkill(thisResource, from, 100, 10, targeted);
                                    break;
                                }
                            case CraftResource.Agapite:
                                {
                                    CheckSkill(thisResource, from, 100, 10, targeted);
                                    break;
                                }
                            case CraftResource.Verite:
                                {
                                    CheckSkill(thisResource, from, 100, 10, targeted);
                                    break;
                                }
                            case CraftResource.Valorite:
                                {
                                    CheckSkill(thisResource, from, 100, 10, targeted);
                                    break;
                                }
                            default:
                                // if no ingot source was found.
                                from.SendMessage("You can not use these ingots to enchant a bow.");
                                break;
                        }
                    }
                    else
                    {
                        // if the selected item is no ingot
                        from.SendMessage("You can not use that item to enchant a bow.");
                    }
                }

                public void CheckSkill(CraftResource thisResource, Mobile from, int reqSkill, int damageBonus, object targeted)
                {
                    BaseWeapon weapon = i_bow;
                    
                    if (from.CheckSkill(SkillName.Blacksmith, reqSkill, 100))
                    {
                        Item res = (Item)targeted;
                        weapon.Hue = CraftResources.GetHue(thisResource);
                        weapon.Resource2 = thisResource;

                        if (res.Amount > 1)
                        {
                            res.Amount -= 1;
                        }
                        else
                        {
                            res.Delete();
                        }

                        from.SendMessage(String.Format("You have enchanted your bow with {0}.", CraftResources.GetName(thisResource)));
                    }
                    else
                    {
                        from.SendMessage("Not enough skill in blacksmithing.");
                    }
                }
            }
        } 
    }
}
