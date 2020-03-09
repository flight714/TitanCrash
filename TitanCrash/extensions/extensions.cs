using Godot;
using System;

namespace Godot
{
    
    public static class extensions
    {
        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }
        public static int ArrayMiddle(this int[] array)
        {
            int n = array.Length;
            if (n.IsEven())
            {
                return (array[n/2-1] + array[n/2]) /2;
            }
            else
            {
                return array[(n+1)/2-1];
            }
        }
        public static int GetHighest(this int[] array)
        {
            if (array.Length > 0)
            {
                int result = array[0];
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] > result)
                    {
                        result = array[i];
                    }
                }
                return result;
            }
            else
            {
                return 0;
            }
        }
        public static int GetMiddleValue(this int num)
        {
            if (num.IsEven())
            {
                return num/2;
            }
            else
            {
                return ((num-1)/2)+1;
            }
        }
    }
}
