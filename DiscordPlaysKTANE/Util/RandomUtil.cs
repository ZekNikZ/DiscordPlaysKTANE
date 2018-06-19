using System;

namespace DiscordPlaysKTANE {
    public class RandomUtil {
        private static Random random;

        public static void InitRandom(int? seed = null) {
            if (seed == null) {
                random = new Random();

            } else {
                random = new Random((int)seed);
            }
        }

        public static int Int() {
            return random.Next();
        }

        public static int Int(int upper) {
            return random.Next(upper);
        }

        public static int Int(int lower, int upper) {
            return random.Next(lower, upper);
        }

        public static bool Bool() {
            return random.Next() == 0;
        }

        public static double Double() {
            return random.NextDouble();
        }
    }
}