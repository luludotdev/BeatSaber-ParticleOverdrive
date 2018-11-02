using System;

namespace ParticleOverdrive.Misc
{
    static class Logger
    {
        public static void Log(object data)
        {
            Console.WriteLine($"[Particle Overdrive] {data}");
        }

        public static void Debug(object data)
        {
#if DEBUG
            Console.WriteLine($"[Particle Overdrive] {data}");
#endif
        }
    }
}
