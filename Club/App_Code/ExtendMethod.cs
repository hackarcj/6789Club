using System;

namespace Club
{
    public static class ExtendMethod
    {
        /// <summary>
        /// 转int类型扩展
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 转bool类型扩展
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool ToBit(this int sender)
        {
            var a = false;
            try
            {
                if (sender > 0)
                {
                    a = true;
                }
                return a;
            }
            catch (Exception ex)
            {
                return a;
            }                        
        }

    }
}