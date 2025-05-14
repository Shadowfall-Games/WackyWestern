using Grabbing;
using Player.Hand;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Zenject;

namespace Gun
{
    public class GunHandler : GrabbedObject
    {
        [Header("Raycast start point")]
        [SerializeField] private Transform _originRay;
        
        [Header("Gun specs")]
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _rateOfFire = 0.1f;
        [SerializeField] private float _rechargeSpeed = 3;
        [SerializeField] private int _maxBulletsAmount = 20;
        [SerializeField] private bool _isMachineGun;
        
        [Header("Gun view")]
        [SerializeField] private float _vfxDuration = 0.5f;
        [SerializeField] private VisualEffect _impactExplosion;
        
        private InputSystem _inputSystem;
        
        private Gun _gun; 
        private GunView _gunView;

        [Inject]
        private void Construct(InputSystem inputSystem) =>  _inputSystem = inputSystem;

        private void Awake()
        {
            _gun = new Gun(_inputSystem, destroyCancellationToken, _originRay, _damage, _rateOfFire, _rechargeSpeed, _maxBulletsAmount, _isMachineGun);
            _gunView = new GunView(_gun, _vfxDuration, _impactExplosion);
        }

        private void OnEnable() => _gun.OnEnable();

        private void OnDisable()
        {
            _gun.OnDisable();
        }

        private void OnDestroy()
        {
            _gun.OnDestroy();
            _gunView.OnDestroy();
        }

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
