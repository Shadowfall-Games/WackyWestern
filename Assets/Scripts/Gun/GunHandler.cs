using Grabbing;
using Player.Hand;
using UnityEngine;

namespace Gun
{
    public class GunHandler : GrabbedObject
    {
        [SerializeField] private Gun _gun;
        [SerializeField] private GunView _gunView;

        private void Awake()
        {
            _gun.Init(destroyCancellationToken);
            _gunView.Init(_gun);
        }

        private void OnEnable() => _gun.OnEnable();

        private void OnDisable()
        {
            _gun.OnDisable();
        }

        private void OnDestroy() => _gunView.OnDestroy();

        private void Update()
        {
            _gun.Update();
        }

        public override void Grab(HandContact handContact, bool isLeftHand)
        {
            base.Grab(handContact, isLeftHand);

            _gun.SetCanShoot(true);
        }
        
        public override void Drop()
        {
            base.Drop();
            _gun.SetCanShoot(false);
        }
    }
}
