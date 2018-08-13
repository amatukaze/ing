namespace Sakuno.ING.Game.Models.Knowledge
{
    public static class KnownLeveling
    {
        private static readonly int[] f99exp;
        private static readonly int[] ad100exp =
        {
            1300000,1600000,1900000,2200000,2600000,3000000,
            3500000,4000000,4600000,5200000,5900000,
            6600000,7400000,8200000,9100000,10000000,
            11000000,12000000,13000000,14000000,15000000
        };
        private static readonly int[] ship100exp;
        static KnownLeveling()
        {
            //generate exp table
            f99exp = new int[99];
            int exp = 0;
            int step = 0;
            int level = 0;
            while (level < 50)
            {
                f99exp[level++] = exp;
                step += 100;
                exp += step;
            }
            while (level < 60)
            {
                f99exp[level++] = exp;
                step += 200;
                exp += step;
            }
            while (level < 70)
            {
                f99exp[level++] = exp;
                step += 300;
                exp += step;
            }
            while (level < 80)
            {
                f99exp[level++] = exp;
                step += 400;
                exp += step;
            }
            while (level < 90)
            {
                f99exp[level++] = exp;
                step += 500;
                exp += step;
            }
            f99exp[90] = exp;
            f99exp[91] = 584500;
            f99exp[92] = 606500;
            f99exp[93] = 631500;
            f99exp[94] = 661500;
            f99exp[95] = 701500;
            f99exp[96] = 761500;
            f99exp[97] = 851500;
            f99exp[98] = 1000000;

            ship100exp = new int[66];
            ship100exp[0] = 1000000;
            ship100exp[1] = exp = 1010000;
            step = 0;
            level = 2;
            while (level <= 11)
            {
                step += 1000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 16)
            {
                step += 2000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 21)
            {
                step += 3000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 31)
            {
                step += 4000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 40)
            {
                step += 5000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 45)
            {
                step += 7000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 50)
            {
                step += 8000;
                exp += step;
                ship100exp[level++] = exp;
            }
            while (level <= 55)
            {
                step += 9000;
                exp += step;
                ship100exp[level++] = exp;
            }
            exp += 250000;
            ship100exp[level++] = exp;

            int step2 = 0;
            step = 50000;
            while (level <= 65)
            {
                step2 += 10000;
                step += step2;
                exp += step;
                ship100exp[level++] = exp;
            }
        }

        public static int GetShipExp(int level)
        {
            if (level <= 99)
                return f99exp[level - 1];
            if (level <= 165)
                return ship100exp[level - 100];
            return 0;
        }

        public static int GetAdmiralExp(int level)
        {
            if (level <= 99)
                return f99exp[level - 1];
            if (level <= 120)
                return ad100exp[level - 100];
            if (level == 121)
                return 180_000_000;
            return 0;
        }

        public static readonly int MaxShipLevel = 165;
        public static readonly int MaxAdmiralLevel = 120;
    }
}
