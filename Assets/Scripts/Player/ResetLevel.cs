using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
    private InputSystem _inputSystem;

    private void OnEnable()
    {
        _inputSystem = new InputSystem();
        _inputSystem.Player.Enable();
    }

    private void Update()
    {
        if (_inputSystem.Player.ResetLevel.IsPressed()) SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
    }
}
