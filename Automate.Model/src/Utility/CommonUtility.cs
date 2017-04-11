namespace Automate.Model.Utility
{
    public static class CommonUtility
    {
        private static long _globalId = 0;

        public static long GetUid()
        {
            return _globalId++;
        }
    }
}