using System;
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

            ParticleSystem.MainModule slashPS = ((ParticleSystem)__instance.GetField("_sparklesPS")).main;

            sparkleParticlesCount = 150 * Mathf.FloorToInt(slashMulti);
            slashPS.maxParticles = 150 * Mathf.FloorToInt(slashMulti * 2f);

            ParticleSystem.MainModule exploPS = ((ParticleSystem)__instance.GetField("_explosionPS")).main;

            explosionParticlesCount = 150 * Mathf.FloorToInt(exploMulti);
            exploPS.maxParticles = 150 * Mathf.FloorToInt(exploMulti * 2f);
        }
    }
}
