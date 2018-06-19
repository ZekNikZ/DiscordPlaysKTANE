using System;
namespace DiscordPlaysKTANE {
    public static class Debug {
        public static void Log(object message) {
            Console.WriteLine(message);
        }

        public static void LogFormat(string format, params object[] args) {
            Console.WriteLine(String.Format(format, args));
        }
    }
}
