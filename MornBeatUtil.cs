namespace MornBeat
{
    internal static class MornBeatUtil
    {
        internal static double InverseLerp(double a, double b, double value)
        {
            var dif = b - a;
            return (value - a) / dif;
        }

        internal static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        internal static bool BitEqual(this int self, int flag)
        {
            return (self & flag) == flag;
        }

        internal static int BitAdd(this int self, int flag)
        {
            return self | flag;
        }

        internal static int BitRemove(this int self, int flag)
        {
            return self & ~flag;
        }

        internal static int BitXor(this int self, int flag)
        {
            return self ^ flag;
        }
    }
}