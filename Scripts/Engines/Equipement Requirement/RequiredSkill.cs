namespace Server.Engines.Equipement_Requirement
{
    public class RequiredSkill
    {
        private SkillName _RequiredSkill;
        private double _MinSkill;

        public RequiredSkill(SkillName requiredSkill, double minSkill)
        {
            _RequiredSkill = requiredSkill;
            _MinSkill = minSkill;
        }

        public SkillName RequiredSkillName
        {
            get { return _RequiredSkill; }
            set { _RequiredSkill = value; } 
        }

        public double MinSkillLevel
        {
            get { return _MinSkill; } 
            set { _MinSkill = value; }
        }
    }
}