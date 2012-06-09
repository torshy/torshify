namespace Torshify
{
    internal enum ToplistSpecialRegion
    {
        Everywhere = 0,
        User = 1
    }

    public class ToplistRegion
    {
        public static readonly int Norway = Generate('N', 'O');
        public static readonly int Sweden = Generate('S', 'E');
        public static readonly int Finland = Generate('F', 'I');
        public static readonly int France = Generate('F', 'R');
        public static readonly int Netherlands = Generate('N', 'L');
        public static readonly int UnitedKingdom = Generate('U', 'K');

        public static int Generate(char first, char second)
        {
            return char.ToUpper(first) << 8 | char.ToUpper(second);
        }
    }
}