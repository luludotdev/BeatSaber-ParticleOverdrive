using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using IllusionPlugin;
using Logger = ParticleOverdrive.Misc.Logger;
using ParticleOverdrive.UI;

namespace ParticleOverdrive
{
    public class Plugin : IPlugin
    {
        public string Name => "Particle Overdive";
        public string Version => "0.1.0";

        private static readonly string[] menuEnv = { "Init", "Menu" };
        private static readonly string[] gameEnv = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };

        public static WorldEffectController _controller;

        public static readonly string ModPrefsKey = "ParticleOverdrive";

        public static float SlashParticleMultiplier;
        public static float ExplosionParticleMultiplier;

        public void OnApplicationStart()
        {
            SlashParticleMultiplier = ModPrefs.GetFloat(ModPrefsKey, "slashParticleMultiplier", 1, true);
            ExplosionParticleMultiplier = ModPrefs.GetFloat(ModPrefsKey, "explosionParticleMultiplier", 1, true);

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
            if (_controller == null)
            {
                GameObject controllerObj = new GameObject("WorldEffectController");
                _controller = controllerObj.AddComponent<WorldEffectController>();

                bool state = ModPrefs.GetBool(ModPrefsKey, "dustParticles", true, true);
                _controller.Init(state);
            }

            if (scene.name == "Menu")
                PluginUI.CreateSettingsUI();

            if (menuEnv.Contains(scene.name) || gameEnv.Contains(scene.name))
                _controller.OnSceneChange(scene);
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
