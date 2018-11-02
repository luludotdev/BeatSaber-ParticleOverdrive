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
            float count = 100f;

            ParticleSystem.MainModule main = ((ParticleSystem)__instance.GetField("_sparklesPS")).main;

            sparkleParticlesCount = 150 * Mathf.FloorToInt(count);
            main.maxParticles = 150 * Mathf.FloorToInt(count * 2f);
        }
    }
}
