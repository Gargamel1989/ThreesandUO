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

            }
            throw new NotImplementedException();
        }

        public void OnHiderKilled()
        {
            throw new NotImplementedException();
        }

        public void Disturb(DisturbType type)
        {
            Disturb(type, false);
        }

        private void Disturb(DisturbType type, bool v2)
        {
            throw new NotImplementedException();
        }
    }
}
