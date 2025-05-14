using System.Threading;
using Grabbing;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;

namespace Item.ExplosiveObject.Dynamite
{
    public class Dynamite : ExplosiveObject
    {
        [Header("Timer settings")]
        [SerializeField] private float _waitingTime = 3;
        [SerializeField] private float _activationTime = 6;
        
        [Header("Sparkle settings")]
        [SerializeField] private VisualEffect _sparkleVFX;
        [SerializeField] private SplineAnimate _splineAnimate;
        
        private DynamiteTimer _dynamiteTimer;
        private DynamiteSparkle _dynamiteSparkle;
        private GrabbedObject _grabbedObject;
        
        private readonly CancellationTokenSource _waitDestroyCts;
        private readonly CancellationTokenSource _activateDestroyCts;

        public override void Start()
        {
            base.Start();
            
            _grabbedObject = GetComponent<GrabbedObject>();
            
            _dynamiteTimer = new DynamiteTimer(_waitDestroyCts, _activateDestroyCts, _waitingTime, _activationTime);
            _dynamiteSparkle = ExplosiveObjectView as DynamiteSparkle;

            _grabbedObject.OnGrab += _dynamiteTimer.StartWaiting;
            _grabbedObject.OnDrop += _dynamiteTimer.StopWaitToken;
            _dynamiteTimer.Activated += _dynamiteSparkle.ActivateSparkle;
            _dynamiteTimer.Exploded += Explode;
        }
        
        protected override ExplosiveObjectView CreateView()
        {
            return new DynamiteSparkle(
                VFXLifetime,
                ObjectLifetime,
                ExplosionEffect,
                _sparkleVFX,
                _splineAnimate
            );
        }

        private void OnDestroy()
        {
            _grabbedObject.OnGrab -= _dynamiteTimer.StartWaiting;
            _grabbedObject.OnDrop -= _dynamiteTimer.StopWaitToken;
            _dynamiteTimer.Exploded -= Explode;
        }

        private void OnDisable() => _dynamiteTimer.OnDisable();
        
        private void Explode() => Explode(transform.position);
    }
}