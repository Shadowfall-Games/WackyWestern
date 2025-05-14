using UnityEngine.Splines;
using UnityEngine.VFX;

namespace Item.ExplosiveObject.Dynamite
{
    public class DynamiteSparkle : ExplosiveObjectView
    {
        private readonly VisualEffect _sparkleVFX;
        private readonly SplineAnimate _splineAnimate;
        
        public DynamiteSparkle(float vfxLifetime, float objectLifetime, VisualEffect explosionEffect, VisualEffect sparkleVfx, SplineAnimate splineAnimate)
            : base(vfxLifetime, objectLifetime, explosionEffect)
        {
            _sparkleVFX = sparkleVfx;
            _splineAnimate = splineAnimate;
        }

        public void ActivateSparkle()
        {
            _sparkleVFX.Play();
            _splineAnimate.Play();
        }
    }
}