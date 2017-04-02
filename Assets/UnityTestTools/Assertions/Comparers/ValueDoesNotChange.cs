namespace Assets.UnityTestTools.Assertions.Comparers
{
    public class ValueDoesNotChange : ActionBase
    {
        private object m_Value;

        protected override bool Compare(object a)
        {
            if (m_Value == null)
                m_Value = a;
            if (!m_Value.Equals(a))
                return false;
            return true;
        }
    }
}
