using System;
using Server;
using Server.Network;
using Scripts.Skills.Utility.Hiding;

namespace Server.Hiding.hide
{
    public abstract class Hide : IHiding
    {
        private HidingState m_state;
        private HidingTimer m_HidingTimer;
        private Mobile m_hider;
        private long m_StartHideTimer;

        public Mobile Hider { get { return m_hider; } }
        public HidingState State { get { return m_state; } set {m_state = value; } }

        public Hide( Mobile m)
        {
            m_hider = m;
        }

        public bool TryToHide()
        {
            m_StartHideTimer = Core.TickCount;
            
            //This crashes when I use this.
            if (m_hider.Hiding is Hide && ((Hide)m_hider.Hiding).State == HidingState.TryingToHide)
            {
                //Distrub code
                ((Hide)m_hider.Hiding).Disturb(DisturbType.NewHide);
            }

            if (!m_hider.CheckAlive())
            {
                return false;
            }
            else if (m_hider.Frozen || m_hider.Paralyzed)
            {
                m_hider.SendMessage("You cannot hide while frozen.");
            }
            /*
            else if (Core.TickCount - m_hider.NextHideTime < 0)
            {
                m_hider.SendMessage("You have not yet recoverd from hiding");
            }
            */
            else if(m_hider.Hiding == null && m_hider.CheckHiding(this) && CheckHiding() && m_hider.Region.OnBeginHiding(m_hider, this))
            {
                m_state = HidingState.TryingToHide;
                m_hider.Hiding = this;
                TimeSpan HideDelay = this.GetHideDelay();

                
                m_HidingTimer = new HidingTimer(this, HideDelay);

                //OnBeginHide();

                if (HideDelay > TimeSpan.Zero){
                    m_HidingTimer.Start();
                }
                else{
                    m_HidingTimer.Tick();
                }

                return true;
            }
            else
            {
                return false;
            }

            return false;
        }
        
        public virtual bool CheckHiding()
        {
            return true;
        }

        public void Disturb(DisturbType type)
        {
            Disturb(type, false);
        }

        private void Disturb(DisturbType type, bool ResistAble)
        {
            if (m_state == HidingState.TryingToHide)
            {
                m_state = HidingState.None;
                m_hider.Hiding = null;
                
                OnDisturb(type, true);

                if (m_HidingTimer != null)
                    m_HidingTimer.Stop();

                m_hider.NextHideTime = Core.TickCount + (int)GetDisturbRecovery().Milliseconds;
            }
        }

        public virtual void FinishSequence()
        {
            m_state = HidingState.None;

            if (m_hider.Hiding == this)
            {
                m_hider.Hiding = null;
            }
        }

        public virtual TimeSpan GetHideRecovery()
        {
            return TimeSpan.FromSeconds(0.75);
        }

        public virtual TimeSpan GetDisturbRecovery()
        {
            double delay = 1.0 - Math.Sqrt((Core.TickCount - m_StartHideTimer));

            if ( delay < 0.2)
                delay = 0.2;

            return TimeSpan.FromSeconds(delay);
        }

        public virtual TimeSpan GetHideDelay()
        {
            TimeSpan HideDelayBase = TimeSpan.FromSeconds(3.0);
            TimeSpan baseDelay = HideDelayBase;

            return baseDelay;
        }

        public bool IsHiding
        {
            get
            {
                return m_state == HidingState.TryingToHide;
            }
        }

        public void OnConnectionChanged()
        {
            FinishSequence();
        }

        public virtual void OnHiderHurt()
        {
            if (IsHiding)
            {
				bool disturb = true;

				if ( disturb )
					Disturb( DisturbType.Hurt, false );
            }
        }

        public void OnHiderKilled()
        {
            Disturb(DisturbType.Kill);
        }

        private void OnDisturb(DisturbType type, bool v)
        {
            m_hider.SendMessage("You failed to hide.");
            
        }

        public abstract void OnHide();

        public virtual void OnBeginHide()
        { }

        private class HidingTimer : Timer
        {
            private Hide m_Hiding;

            public HidingTimer(Hide Hiding, TimeSpan HideDelay) : base(HideDelay)
            {
                m_Hiding = Hiding;

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                
                if (m_Hiding == null && m_Hiding.m_hider == null)
                {
                    return;
                }
                
                else if (m_Hiding.m_state == HidingState.TryingToHide ) //&& m_Hiding.m_hider.UseSkill(SkillName.Hiding))
                {
                    m_Hiding.m_state = HidingState.Sequencing;
                    m_Hiding.m_HidingTimer = null;
                    m_Hiding.m_hider.OnHiding(m_Hiding);
                    if (m_Hiding.m_hider.Region != null)
                        m_Hiding.m_hider.Region.OnHide(m_Hiding.m_hider, m_Hiding);
                    m_Hiding.m_hider.NextHideTime = Core.TickCount + (int)m_Hiding.GetHideRecovery().TotalMilliseconds;

                    m_Hiding.OnHide();

                    m_Hiding.m_HidingTimer = null;
                }
                
            }

            public void Tick()
            {
                OnTick();
            }
        }
    }
}
