using System;
using Server;


namespace Scripts.Skills.Utility.Hiding
{
    public abstract class Hide : IHiding
    {
        
        public bool IsHiding
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void OnHiderHurt()
        {
            throw new NotImplementedException();
        }

        public void OnHiderKilled()
        {
            throw new NotImplementedException();
        }
    }
}
