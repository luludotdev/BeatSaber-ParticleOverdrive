using System;
using ParticleOverdrive.Misc;
using UnityEngine;
using IllusionPlugin;

namespace ParticleOverdrive.UI
{
    class PluginUI : MonoBehaviour
    {
        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("Particle Overdrive");

            BoolViewController dustEnabled = subMenu.AddBool("Global Dust Particles");
            dustEnabled.GetValue += (() => Plugin._particleController.Enabled);
            dustEnabled.SetValue += delegate (bool value)
            {
                Plugin._particleController.Enabled = value;
                ModPrefs.SetBool(Plugin.ModPrefsKey, "dustParticles", value);
            };

            float[] values = new float[]
            {
                0f,
                0.1f,
                0.2f,
                0.3f,
                0.4f,
                0.5f,
                0.6f,
                0.7f,
                0.8f,
                0.9f,
                1f,
                1.1f,
                1.2f,
                1.3f,
                1.4f,
                1.5f,
                1.6f,
                1.7f,
                1.8f,
                1.9f,
                2f,
                2.25f,
                2.5f,
                2.75f,
                3f,
                3.25f,
                3.5f,
                3.75f,
                4f,
                4.25f,
                4.5f,
                4.75f,
                5f,
                5.5f,
                6f,
                6.5f,
                7f,
                7.5f,
                8f,
                8.5f,
                9f,
                9.5f,
                10f,
                11f,
                12f,
                13f,
                14f,
                15f,
                16f,
                17f,
                18f,
                19f,
                20f,
                22.5f,
                25f,
                27.5f,
                30f,
                32.5f,
                35f,
                37.5f,
                40f,
                42.5f,
                45f,
                47.5f,
                50f,
                55f,
                60f,
                65f,
                70f,
                75f,
                80f,
                85f,
                90f,
                100f,
                110f,
                120f,
                130f,
                140f,
                150f,
                160f,
                170f,
                180f,
                190f,
                200f
            };

            ListViewController slashParticleCount = subMenu.AddList("Slash Particles", values);
            slashParticleCount.FormatValue += ((float value) => $"{value * 100f}%");
            slashParticleCount.GetValue += (() => Plugin.SlashParticleMultiplier);
            slashParticleCount.SetValue += delegate (float value)
            {
                Plugin.SlashParticleMultiplier = value;
                ModPrefs.SetFloat(Plugin.ModPrefsKey, "slashParticleMultiplier", value);
            };

            ListViewController exploParticleCount = subMenu.AddList("Explosion Particles", values);
            exploParticleCount.FormatValue += ((float value) => $"{value * 100f}%");
            exploParticleCount.GetValue += (() => Plugin.ExplosionParticleMultiplier);
            exploParticleCount.SetValue += delegate (float value)
            {
                Plugin.ExplosionParticleMultiplier = value;
                ModPrefs.SetFloat(Plugin.ModPrefsKey, "explosionParticleMultiplier", value);
            };
        }
    }
}
