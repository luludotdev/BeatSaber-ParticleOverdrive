using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ParticleOverdrive.Controllers
{
    interface IGlobalController
    {
        bool Enabled { get; set; }

        void Init(bool state);

        void OnSceneChange(Scene scene);
    }
}
