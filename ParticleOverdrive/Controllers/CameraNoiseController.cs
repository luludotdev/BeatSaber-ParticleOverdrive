using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ParticleOverdrive.Misc;
using Logger = ParticleOverdrive.Misc.Logger;

namespace ParticleOverdrive.Controllers
{
    public class CameraNoiseController : MonoBehaviour, IGlobalController
    {
        private BlueNoiseDitheringUpdater _ditheringUpdater;
        public BlueNoiseDitheringUpdater DitheringUpdater
        {
            get
            {
                if (_ditheringUpdater == null)
                    _ditheringUpdater = Find();

                return _ditheringUpdater;
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                string action = value ? "Enabling" : "Disabling";
                Logger.Debug($"{action} camera noise!");

                _enabled = value;
                Set();
            }
        }

        private Texture2D _originalNoise;
        private Texture2D _newNoise;

        public void Init(bool state)
        {
            DontDestroyOnLoad(this);

            _newNoise = Texture2D.blackTexture;

            Color32[] pixels = _newNoise.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(0, 0, 0, 200);

            _newNoise.SetPixels32(pixels);
            _newNoise.Apply();

            _enabled = state;
        }

        private void Set()
        {
            BlueNoiseDithering _blueNoiseDithering = DitheringUpdater.GetField<BlueNoiseDithering>("_blueNoiseDithering");

            if (_originalNoise == null)
            {
                Logger.Debug("Noise texture unset, caching original value...");
                Texture2D orig = _blueNoiseDithering.GetField<Texture2D>("_noiseTexture");

                _originalNoise = orig;
            }

            if (_enabled)
                _blueNoiseDithering.SetField("_noiseTexture", _originalNoise);
            else
                _blueNoiseDithering.SetField("_noiseTexture", _newNoise);
        }

        private BlueNoiseDitheringUpdater Find()
        {
            GameObject helper = GameObject.Find("BlueNoiseHelper");
            return helper.GetComponent<BlueNoiseDitheringUpdater>();
        }

        public void OnSceneChange(Scene scene)
        {
            StartCoroutine(SceneChangeHandler());
        }

        private IEnumerator SceneChangeHandler()
        {
            if (_ditheringUpdater == null)
            {
                Logger.Debug("DitheringUpdater is null, checking for new one...");

                while (_ditheringUpdater == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    _ditheringUpdater = Find();
                }

                Logger.Debug("Found new DitheringUpdater!");
            }

            Set();
        }
    }
}
