using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;
using Server.Hiding.hide;
using Scripts.Skills.Utility.Hiding;

namespace Server.SkillHandlers
{
	public class Hiding
	{
        private static bool m_CombatOverride;

        private static Timer timer;

        public static bool CombatOverride
		{
			get{ return m_CombatOverride; }
			set{ m_CombatOverride = value; }
		}

        public long m_StartHiding { get; set; }

        public static void Initialize()
		{
			SkillInfo.Table[21].Callback = new SkillUseCallback( OnUse );
		}

        private class HideTimer : Timer
        {
            private Mobile m;
            TimeSpan returnTimer;

            private int counter = 0;

            public HideTimer(Mobile m, TimeSpan delay) : base(delay, delay, 3)
            {
                this.m = m;
                Priority = TimerPriority.TwoFiftyMS;
            }
            

            protected override void OnTick()
            {
                if (m == null || m.Deleted)
                {
                    this.Stop();
                    return;
                }

                if (m.Spell != null)
                {
                    m.SendLocalizedMessage(501238); // You are busy doing something else and cannot hide.
                    this.Stop();
                    return;
                }

                if (Core.ML && m.Target != null)
                {
                    Targeting.Target.Cancel(m);
                }

                double bonus = 0.0;

                BaseHouse house = BaseHouse.FindHouseAt(m);

                if (house != null && house.IsFriend(m))
                {
                    bonus = 100.0;
                }
                else if (!Core.AOS)
                {
                    if (house == null)
                        house = BaseHouse.FindHouseAt(new Point3D(m.X - 1, m.Y, 127), m.Map, 16);

                    if (house == null)
                        house = BaseHouse.FindHouseAt(new Point3D(m.X + 1, m.Y, 127), m.Map, 16);

                    if (house == null)
                        house = BaseHouse.FindHouseAt(new Point3D(m.X, m.Y - 1, 127), m.Map, 16);

                    if (house == null)
                        house = BaseHouse.FindHouseAt(new Point3D(m.X, m.Y + 1, 127), m.Map, 16);

                    if (house != null)
                        bonus = 50.0;
                }

                //int range = 18 - (int)(m.Skills[SkillName.Hiding].Value / 10);
                int range = Math.Min((int)((100 - m.Skills[SkillName.Hiding].Value) / 2) + 8, 18);  //Cap of 18 not OSI-exact, intentional difference

                bool badCombat = (!m_CombatOverride && m.Combatant != null && m.InRange(m.Combatant.Location, range) && m.Combatant.InLOS(m));
                bool ok = (!badCombat /*&& m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus )*/ );

                /* old hiding system
                if ( ok )
                {
                    if ( !m_CombatOverride )
                    {
                        foreach ( Mobile check in m.GetMobilesInRange( range ) )
                        {
                            if ( check.InLOS( m ) && check.Combatant == m )
                            {
                                badCombat = true;
                                ok = false;
                                break;
                            }
                        }
                    }

                    ok = ( !badCombat && m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus ) );
                }
                */

                //check if mobile is in warmode
                if (ok)
                {
                    if (!m_CombatOverride)
                    {
                        if (m.Warmode == true)
                        {
                            badCombat = true;
                            ok = false;
                        }
                    }

                    ok = (!badCombat && m.CheckSkill(SkillName.Hiding, 0.0 - bonus, 100.0 - bonus));
                }

                if (badCombat)
                {
                    m.RevealingAction();

                    m.LocalOverheadMessage(MessageType.Regular, 0x22, 501237); // You can't seem to hide right now.

                    Stop();
                    return;
                }
                else
                {
                    if (ok)
                    {
                        /*
                        m.Hidden = true;
                        m.Warmode = false;
                        m.LocalOverheadMessage(MessageType.Regular, 0x1F4, 501240); // You have hidden yourself well.
                        */
                    }
                    else
                    {
                        m.RevealingAction();

                        m.LocalOverheadMessage(MessageType.Regular, 0x22, 501241); // You can't seem to hide here.
                    }

                    Stop();
                    return;
                }
            }

            public void Tick()
            {
                OnTick();
            }
        }

        public static TimeSpan OnUse( Mobile m )
		{
            
            Hide hide = new HidingHide(m);

            hide.TryToHide();

            return TimeSpan.FromSeconds(0.25);
		}

        private class HidingHide : Hide
        {
            private Mobile m;
            public HidingHide( Mobile m ) : base( m )
            {
                this.m = m;
            }

            public override void OnHide()
            {
                m.Hidden = true;
                m.Warmode = false;
                m.LocalOverheadMessage(MessageType.Regular, 0x1F4, 501240); // You have hidden yourself well.               
                FinishSequence();
            }

            public override void OnHiderHurt()
            {
                if (IsHiding)
                    Disturb(DisturbType.Hurt);
            }
        }
    }
}