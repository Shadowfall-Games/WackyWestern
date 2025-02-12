using Zenject;

public class GlobalInstaller :  MonoInstaller
{
    public override void InstallBindings()
    {
        InputSystem inputSystem = new InputSystem();
        inputSystem.Enable();
        Container.Bind<InputSystem>().FromInstance(inputSystem);


    }
}