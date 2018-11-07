using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = ParticleOverdrive.Misc.Logger;

namespace ParticleOverdrive
{
    public class WorldEffectController : MonoBehaviour
    {
        private Camera _camera { get => Camera.main; }

        private ParticleSystem _dustPS;
        public ParticleSystem DustPS
        {
            get
            {
                if (_dustPS == null)
                    _dustPS = FindDustPS();

                return _dustPS;
            }
        }

        private bool _dustEnabled;
        public bool DustParticles
        {
            get => _dustEnabled;
            set
            {
                _dustEnabled = value;
                SetDustParticles();
            }
        }

        public void Init(bool state)
        {
            DontDestroyOnLoad(this);
            DustParticles = state;
        }

        private ParticleSystem FindDustPS()
        {
            Transform dustPSTransform = _camera.transform.Find("DustPS");
            ParticleSystem ps = dustPSTransform.GetComponent<ParticleSystem>();

            return ps;
        }

        private void SetDustParticles()
        {
            DustPS.gameObject.SetActive(_dustEnabled);
        }

        public void OnSceneChange(Scene scene)
        {
            StartCoroutine(SceneChangeHandler());
        }

        private IEnumerator SceneChangeHandler()
        {
            if (_dustPS == null)
            {
                while (_dustPS == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    _dustPS = FindDustPS();
                }
            }

            SetDustParticles();
        }
    }
}
