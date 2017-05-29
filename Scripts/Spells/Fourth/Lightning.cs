using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fourth
{
	public class LightningSpell : TargetedMagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Lightning", "Por Ort Grav",
				239,
				9021/*,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
                */
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public LightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info, TargetFlags.Harmful )
		{
		}

		public override void OnCast()
		{
            Mobile m = target;

            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (m.IsDeadBondedPet)
            {
                Caster.SendLocalizedMessage(1060177); // You cannot heal a creature that is already dead!
            }
            else if (CheckBSequence(m))
            {
                if(Caster.CanSee(m))
                {
                    
                    /*
                    Console.WriteLine("Spellhelper.turn");
                    SpellHelper.Turn(Caster, m);
                    Console.WriteLine("SpelleHelper.CheckReflect");
                    SpellHelper.CheckReflect((int)this.Circle, Caster, ref m);

                    double toDamage = GetNewAosDamage(23, 1, 4, m);

                    m.BoltEffect(0);
                    Console.WriteLine("Do damage");
                    SpellHelper.Damage(this, m, toDamage);
                    */
                }else
                {
                    Caster.Spell.OnCasterHurt();
                }
                
            }

            FinishSequence();

        }

		public override bool DelayedDamage{ get{ return false; } }

		
	}
}