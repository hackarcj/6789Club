using System;

namespace Club
{
    public static class ExtendMethod
    {
        public static int ToInt(this string sender)
        {
            int result = 0;
            try
            {
                result = int.Parse(sender);
                return result;
            }
            catch (Exception ex)
            {
                return result;

            }
        }
        public static bool ToBit(this int sender)
        {
            var a = false;
            if(sender>0)
            {
                a = true;
            }
            return a;
        }

    }
}