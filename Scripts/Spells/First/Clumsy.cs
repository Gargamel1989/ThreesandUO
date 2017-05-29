using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.First
{
	public class ClumsySpell : TargetedMagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Clumsy", "Uus Jux",
				212,
				9031,
				Reagent.Bloodmoss,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public ClumsySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info, TargetFlags.Harmful )
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
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, Caster, ref m);

                SpellHelper.AddStatCurse(Caster, m, StatType.Dex);

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5002, EffectLayer.Head);
                m.PlaySound(0x1DF);

                int percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, true) * 100);
                TimeSpan length = SpellHelper.GetDuration(Caster, m);

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Clumsy, 1075831, length, m, percentage.ToString()));

                HarmfulSpell(m);
            }

            FinishSequence();
        }
	}
}