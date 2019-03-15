using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using IllusionPlugin;
using ParticleOverdrive.UI;
using ParticleOverdrive.Controllers;
using Logger = ParticleOverdrive.Misc.Logger;

namespace ParticleOverdrive
{
    public class Plugin : IPlugin
    {
        public string Name => "Particle Overdive";
        public string Version => "0.4.0";

        private static readonly string[] env = { "Init", "MenuCore", "GameCore", "Credits" };

        public static GameObject _controller;
        public static WorldParticleController _particleController;
        public static CameraNoiseController _noiseController;

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

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "MenuViewControllers")
                PluginUI.CreateSettingsUI();
        }

        void SceneManagerOnActiveSceneChanged(Scene _, Scene scene)
        {
            Logger.Debug($"Scene Change! {scene.name}");

            if (_controller == null)
            {
                _controller = new GameObject("WorldEffectController");
                GameObject.DontDestroyOnLoad(_controller);

                _particleController = _controller.AddComponent<WorldParticleController>();
                _noiseController = _controller.AddComponent<CameraNoiseController>();

                bool particleState = ModPrefs.GetBool(ModPrefsKey, "dustParticles", true, true);
                _particleController.Init(particleState);

                bool noiseState = ModPrefs.GetBool(ModPrefsKey, "cameraNoise", true, true);
                _noiseController.Init(noiseState);
            }

            if (env.Contains(scene.name))
            {
                _particleController.OnSceneChange(scene);
                _noiseController.OnSceneChange(scene);
            }
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
