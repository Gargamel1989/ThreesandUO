using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;

using Scripts.Skills.Utility.Hiding;

namespace Server.SkillHandlers
{
	public class Hiding
	{
        private static bool m_CombatOverride;
        

        private static Timer timer;
        public bool isHiding { get; set; }

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

        public void test()
        {

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
                counter++;
                m.Emote("Hiding iets: counter is: {0}", Convert.ToString(counter));
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
            //code before
            /*
            timer = new hiddenStatus(m, 0);

            timer.Start();
            */

            //Hide class code

            Console.WriteLine("Start hiding");
            Console.WriteLine("OnUse Hiding.cs");

            Hide hide = new HidingHide(m);

            hide.TryToHide();

            Console.WriteLine("ishidng: {0}", Convert.ToString(hide.IsHiding));
            if (hide.IsHiding)
                return TimeSpan.FromSeconds(3.0);

            return TimeSpan.Zero;
		}

        private class HidingHide : Hide
        {
            public HidingHide( Mobile m ) : base( m )
            {
            }

            public override void OnHide()
            {               
                FinishSequence();
            }

            public override void OnHiderHurt()
            {
                if (IsHiding)
                    Disturb(DisturbType.Hurt);
            }
        }

        public class hiddenStatus : Timer
        {
            int m_count;
            Mobile m;
            public hiddenStatus(Mobile m, int count) : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), count)
            {
                this.m = m;
                this.m_count = count;
            }

            protected override void OnTick()
            {
                m_count++;
                if (!(m.FindMostRecentDamageEntry(true) == null))
                {
                    var test = m.FindMostRecentDamageEntry(true);

                    DateTime damageTimer = test.LastDamage;
                    TimeSpan damgeTime = DateTime.UtcNow - damageTimer;

                    Console.WriteLine("now: {0}", damgeTime.TotalMilliseconds);
                }
                

                if ( m_count <= 10)
                {
                    m.Emote("counter: {0}", Convert.ToString(m_count));
                }
                else
                {
                    this.Stop();
                    m.Hidden = true;
                }

                if (m.Warmode == true)
                {
                    m.Emote("Warmode: {0}", Convert.ToString(m.Warmode));
                    this.Stop();
                    return;
                }
                
            }
        }
    }

    
}