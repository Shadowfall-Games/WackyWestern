using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Utils
{
    public class ResetLevel : MonoBehaviour
    {
        private InputSystem _inputSystem;

        [Inject]
        private void Construct(InputSystem inputSystem) =>  _inputSystem = inputSystem;

        private void Update()
        {
            if (_inputSystem.Player.ResetLevel.IsPressed()) SceneManager.LoadScene(0);
        }

        private void OnDisable()
        {
            _inputSystem.Player.Disable();
        }
    }
}
