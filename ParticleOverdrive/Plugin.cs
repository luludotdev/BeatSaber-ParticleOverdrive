using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using Harmony;
using IllusionPlugin;
using ParticleOverdrive.Misc;
using ParticleOverdrive.UI;

namespace ParticleOverdrive
{
    public class Plugin : IPlugin
    {
        public string Name => "Particle Overdive";
        public string Version => "0.1.0";

        public static float ParticleMultiplier;

        public void OnApplicationStart()
        {
            ParticleMultiplier = ModPrefs.GetFloat("ParticleOverdrive", "particleMultiplier", 1, true);

            try
            {
                HarmonyInstance harmony = HarmonyInstance.Create("com.jackbaron.beatsaber.particleoverdrive");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.Log("This plugin requires Harmony. Make sure you " +
                    "installed the plugin properly, as the Harmony DLL should have been installed with it.");
                Console.WriteLine(e);
            }

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.name == "Menu")
                PluginUI.CreateSettingsUI();
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void OnLevelWasInitialized(int level) { }

        public void OnLevelWasLoaded(int level) { }

        public void OnUpdate() { }

        public void OnFixedUpdate() { }
    }
}
