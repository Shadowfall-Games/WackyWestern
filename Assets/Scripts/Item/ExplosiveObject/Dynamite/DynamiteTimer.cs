using System;
using System.Threading;
using UnityEngine;

namespace Item.ExplosiveObject.Dynamite
{
    public class DynamiteTimer
    {
        private readonly float _waitingTime;
        private readonly float _activationTime;

        private CancellationTokenSource _waitDestroyCts;
        private CancellationTokenSource _activateDestroyCts;
        
        private bool _isActive;
        
        public event Action Exploded;
        public event Action Activated;
        
        public DynamiteTimer(CancellationTokenSource waitDestroyCts, CancellationTokenSource activateDestroyCts, float waitingTime, float activationTime)
        {
            _waitDestroyCts = waitDestroyCts;
            _activateDestroyCts = activateDestroyCts;
            _waitingTime = waitingTime;
            _activationTime = activationTime;
        }

        public void OnDisable()
        {
            StopWaitToken();
            StopActivateToken();
        }

        public async void StartWaiting()
        {
            if (!_isActive) StopWaitToken();
            try
            {
                await Awaitable.WaitForSecondsAsync(_waitingTime, _waitDestroyCts.Token);
            }
            catch
            {
                return;
                
            }
            StartActivation();
        }

        private async void StartActivation()
        {
            _isActive = true;
            StopActivateToken();
            Activated?.Invoke();
            try
            {
                await Awaitable.WaitForSecondsAsync(_activationTime, _activateDestroyCts.Token);

            }
            catch
            {
                return;
            }
            Exploded?.Invoke();
        }

        private void StopToken(CancellationTokenSource destroyCts)
        {
            destroyCts?.Cancel();
            destroyCts?.Dispose();
        }
        
        public void StopWaitToken()
        {
            StopToken(_waitDestroyCts);
            _waitDestroyCts = new CancellationTokenSource();
        }

        private void StopActivateToken()
        {
            StopToken(_activateDestroyCts);
            _activateDestroyCts = new CancellationTokenSource();
        }
    }
}