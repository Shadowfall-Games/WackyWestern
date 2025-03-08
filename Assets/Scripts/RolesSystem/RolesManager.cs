using UnityEngine;

public class RolesManager : MonoBehaviour
{
    public enum Roles
    {
        Sheriff,
        Miner,
        Barman,
        Shepherd,
        Bandit,
        Priest,
        Gunsmith,
        Banker,
        Tradesman,
        Hunter,
        Gravedigger,
        Townsman
    }

    [SerializeField] private Roles _roles;
}