using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Seventh
{
	public class ChainLightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Chain Lightning", "Vas Ort Grav",
				209,
				9022,
				false,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

        public override void SelectTarget()
        {
            Caster.Target = new InternalSphereTarget(this);
        }

        public override void OnSphereCast()
        {
            if (SpellTarget != null)
            {
                if (SpellTarget is IPoint3D)
                {
                    Target((IPoint3D)SpellTarget);
                }
                else
                {
                    Caster.SendAsciiMessage("Invalid Target");
                }
            }
            FinishSequence();
        }

	    public ChainLightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
            else if (!CheckLineOfSight(p))
            {
                this.DoFizzle();
                Caster.SendAsciiMessage("Target is not in line of sight");
            }
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				bool playerVsPlayer = false;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 2 );

					foreach ( Mobile m in eable )
					{
						if ( Core.AOS && m == Caster )
							continue;

						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						{
							if ( Core.AOS && !Caster.InLOS( m ) )
								continue;

							targets.Add( m );

							if ( m.Player )
								playerVsPlayer = true;
						}
					}

					eable.Free();
				}

				double damage;

				if ( Core.AOS )
					damage = GetNewAosDamage( 51, 1, 5, playerVsPlayer );
				else
					damage = Utility.Random( 27, 22 );

				if ( targets.Count > 0 )
				{
					if ( Core.AOS && targets.Count > 2 )
						damage = (damage * 2) / targets.Count;
					else if ( !Core.AOS )
						damage /= targets.Count;

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						double toDeal = damage;

						if ( !Core.AOS && CheckResisted( m ) )
						{
							toDeal *= 0.5;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

					Caster.DoHarmful( m );
					SpellHelper.Damage( this, m, toDeal, 0, 0, 0, 0, 100 );

					m.BoltEffect( 0 );
					}
				}
				else
				{
					Caster.PlaySound ( 0x29 );
				}
			}

			FinishSequence();
		}

        private class InternalSphereTarget : Target
        {
            private ChainLightningSpell m_Owner;

            public InternalSphereTarget(ChainLightningSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.Harmful)
            {
                m_Owner = owner;
                m_Owner.Caster.SendAsciiMessage("Select target...");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                {
                    m_Owner.SpellTarget = o;
                    m_Owner.CastSpell();
                }
                else
                {
                    m_Owner.Caster.SendAsciiMessage("Invalid Target");
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_Owner.SpellTarget == null)
                {
                    m_Owner.Caster.SendAsciiMessage("Targeting cancelled.");
                }
            }
        }

		private class InternalTarget : Target
		{
			private ChainLightningSpell m_Owner;

			public InternalTarget( ChainLightningSpell owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}