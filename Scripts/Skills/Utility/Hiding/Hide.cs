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
        HidingState m_state;
        HidingTimer m_HidingTimer;
        Mobile m_hider;

        public bool IsHiding
        {
            get
            {
                return m_state == HidingState.TryingToHide;
            }
        }

        public void OnHiderHurt()
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

        public void Disturb(DisturbType type)
        {
            Disturb(type, false);
        }

        private void Disturb(DisturbType type, bool ResistAble)
        {
            if ( m_state == HidingState.TryingToHide)
            {
                m_state = HidingState.None;

                OnDisturb(type, true);

                if (m_HidingTimer != null)
                    m_HidingTimer.Stop();
            }
        }

        private void OnDisturb(DisturbType type, bool v)
        {
            throw new NotImplementedException();
        }

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
                    m_Hiding.m_hider.Hidden = true;
                    m_Hiding.m_hider.Warmode = false;

                }
            }

            public void Tick()
            {
                OnTick();
            }
        }
    }
}
