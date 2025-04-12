using UnityEngine;

namespace Player.Contacts
{
    public abstract class GrabbedObject : MonoBehaviour
    {
        public abstract void Grab(HandContact handContact);

        public abstract void Drop();
    }
}