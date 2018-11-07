using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = ParticleOverdrive.Misc.Logger;

namespace ParticleOverdrive.Controllers
{
    public class WorldParticleController : MonoBehaviour, IGlobalController
    {
        private Camera _camera { get => Camera.main; }

        private ParticleSystem _dustPS;
        public ParticleSystem DustPS
        {
            get
            {
                if (_dustPS == null)
                    _dustPS = Find();

                return _dustPS;
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                string action = value ? "Enabling" : "Disabling";
                Logger.Debug($"{action} world particles!");

                _enabled = value;
                DustPS.gameObject.SetActive(_enabled);
            }
        }

        public void Init(bool state)
        {
            DontDestroyOnLoad(this);
            Enabled = state;
        }

        private ParticleSystem Find()
        {
            Transform transform = _camera.transform.Find("DustPS");
            return transform.GetComponent<ParticleSystem>();
        }

        public void OnSceneChange(Scene scene)
        {
            Logger.Debug("Scene change triggered!");
            StartCoroutine(SceneChangeHandler());
        }

        private IEnumerator SceneChangeHandler()
        {
            if (_dustPS == null)
            {
                Logger.Debug("ParticleSystem is null, checking for new one...");

                while (_dustPS == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    _dustPS = Find();
                }

                Logger.Debug("Found new ParticleSystem!");
            }

            Enabled = _enabled;
        }
    }
}
