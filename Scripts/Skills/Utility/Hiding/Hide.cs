using System;
using Server;


namespace Scripts.Skills.Utility.Hiding
{
    public enum HidingState
    {
        None = 0,
        TryingToHide = 1,    // We are in the process of hiding (that is, waiting GetHideTime()). Hiding may be interupted in this state.
    }
    public abstract class Hide : IHiding
    {
        private HidingState m_state;
        private HidingTimer m_HidingTimer;
        private Mobile m_hider;
        private long m_StartHideTime;
        TimeSpan HideDelay;

        public Mobile Hider { get { return m_hider; } }

        public Hide( Mobile m)
        {
            this.m_hider = m;
        }

        public bool TryToHide()
        {
            m_StartHideTime = Core.TickCount;

            if (m_state == HidingState.TryingToHide)
                Disturb(DisturbType.NewHide);

            if(!m_hider.CheckAlive())
            {
                return false;
            }else if (m_hider.Frozen || m_hider.Paralyzed)
            {
                m_hider.SendMessage("You cannot hide while frozen.");
            }else if(m_hider.Hiding == null && m_hider.CheckHiding(this) && CheckHiding() && m_hider.Region.OnBeginHding(m_hider, this))
            {
                m_state = HidingState.TryingToHide;
                m_hider.Hiding = this;
                HideDelay = TimeSpan.FromSeconds(3.0);

                m_HidingTimer = new HidingTimer(this, HideDelay);

                OnBeginHide();

                if (HideDelay > TimeSpan.Zero)
                {
                    m_HidingTimer.Start();
                }
                else
                {
                    m_HidingTimer.Tick();
                }
                return true;
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

                OnDisturb(type, true);

                if (m_HidingTimer != null)
                    m_HidingTimer.Stop();
            }
        }

        public virtual void FinishSequence()
        {
            Console.WriteLine("FinishedSequence");
            m_state = HidingState.None;
            

            if (m_hider.Hiding == this)
            {
                Console.WriteLine("set m_hider.hiding = null");
                m_hider.Hiding = null;
            }else
            {
                Console.WriteLine("Hiding =/= this");
            }
                
            //succesfully hide
            m_hider.Hidden = true;
        }

        public bool IsHiding
        {
            get
            {
                return m_state == HidingState.TryingToHide;
            }
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
            throw new NotImplementedException();
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
                this.m_Hiding = Hiding;

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (m_Hiding == null && m_Hiding.m_hider == null)
                {
                    return;
                }

                else if (m_Hiding.m_state == HidingState.TryingToHide && m_Hiding.m_hider.UseSkill(SkillName.Hiding))
                {
                    m_Hiding.m_HidingTimer = null;
                    m_Hiding.OnHide();
                    m_Hiding.m_HidingTimer = null;

                }else if( m_Hiding.m_HidingTimer == null)
                {
                    Console.WriteLine("m_Hiding.m_HidngTimer = null");
                }
            }

            public void Tick()
            {
                OnTick();
            }
        }
    }
}
