using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using ParticleOverdrive.Misc;

namespace ParticleOverdrive.Patches
{
    [HarmonyPatch(typeof(NoteCutParticlesEffect))]
    [HarmonyPatch("SpawnParticles")]
    class NoteCutParticlesEffectSpawnParticles
    {
        static void Prefix(ref NoteCutParticlesEffect __instance, ref int sparkleParticlesCount, ref int explosionParticlesCount)
        {
            float multi = Plugin.ParticleMultiplier;

            ParticleSystem.MainModule main = ((ParticleSystem)__instance.GetField("_sparklesPS")).main;

            sparkleParticlesCount = 150 * Mathf.FloorToInt(multi);
            main.maxParticles = 150 * Mathf.FloorToInt(multi * 1.1f);
        }
    }
}
