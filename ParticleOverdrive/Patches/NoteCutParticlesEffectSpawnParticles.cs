using System;
using System.Linq;
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
            float slashMulti = Plugin.SlashParticleMultiplier;
            float exploMulti = Plugin.ExplosionParticleMultiplier;

            ParticleSystem[] slashPS = (ParticleSystem[])__instance.GetField("_sparklesPS");
            foreach (ParticleSystem ps in slashPS)
            {
                ParticleSystem.MainModule main = ps.main;
                main.maxParticles = 150 * Mathf.FloorToInt(slashMulti * 2f);
            }

            sparkleParticlesCount = 150 * Mathf.FloorToInt(slashMulti);

            ParticleSystem.MainModule exploPS = ((ParticleSystem)__instance.GetField("_explosionPS")).main;

            explosionParticlesCount = 150 * Mathf.FloorToInt(exploMulti);
            exploPS.maxParticles = 150 * Mathf.FloorToInt(exploMulti * 2f);
        }
    }
}
