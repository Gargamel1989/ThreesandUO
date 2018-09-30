using System;
using System.Collections.Generic;
using Server.Engines.Equipement_Requirement;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x26C2, 0x26CC )]
	public class CompositeBow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MovingShot; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return Core.ML ? 13 : 15; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 25; } }
		public override float MlSpeed{ get{ return 4.00f; } }

		public override int OldStrengthReq{ get{ return 45; } }

		public override int OldMinDamage
		{
			get
			{
				int dmg = 15;
				double dmgBonus = 0;
				
				if (Resource2 != null)
				{
					//Check how much the damage bonus is on the Composite Bow
					switch ( Resource2 )
					{
						case CraftResource.DullCopper:		dmg += 2; break;
						case CraftResource.ShadowIron:		dmg += 4; break;
						case CraftResource.Copper:			dmg += 6; break;
						case CraftResource.Bronze:			dmg += 8; break;
						case CraftResource.Gold:			dmg += 10; break;
						case CraftResource.Agapite:			dmg += 12; break;
						case CraftResource.Verite:			dmg += 14; break;
						case CraftResource.Valorite:
						{
							dmgBonus = (CraftAttributeInfo.Valorite.SmithingRequirement / 2);
							Console.WriteLine("Composite bow damage bonus form resource2: {0}%", dmgBonus);
							break;
						}
					}

					return dmg;
				}
				
				return dmg;
			}
		}
		public override int OldMaxDamage{ get{ return 17; } }
		public override int OldSpeed{ get{ return 25; } }

		public override int DefMaxRange{ get{ return 10; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }
		
		public override List<RequiredSkill> RequiredSkills
		{
			get
			{
				double skillvalue = 50.0;

				switch (Resource2)
				{
					case CraftResource.DullCopper:
					{
						skillvalue += (CraftAttributeInfo.DullCopper.SmithingRequirement / 2);
						break;
					}
					case CraftResource.Valorite:
					{
						skillvalue += (CraftAttributeInfo.Valorite.SmithingRequirement / 2);
						break;
					}
				}
				Console.WriteLine("Composite bow skillvalue = {0}%", skillvalue);

				RequiredSkill skill = new RequiredSkill(SkillName.Archery, skillvalue);
				
				List<RequiredSkill> reqSkills = new List<RequiredSkill>();
				reqSkills.Add(skill);
				
				return reqSkills;
			}
		}

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public CompositeBow() : base( 0x26C2 )
		{
			Weight = 5.0;
		}

		public CompositeBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}