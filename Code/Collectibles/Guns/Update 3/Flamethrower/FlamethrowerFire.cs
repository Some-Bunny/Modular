using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Collectibles.Guns.Update_3
{
    public class FlamethrowerFire : MonoBehaviour
    {
        private Projectile projectile;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.projectile.spriteAnimator.Play("flamingfire");
            this.projectile.spriteAnimator.AnimationCompleted += MyBad;

            this.projectile.baseData.speed *= UnityEngine.Random.Range(0.85f, 1.15f);
            this.projectile.UpdateSpeed();
            this.projectile.GetComponent<BounceProjModifier>().OnBounceContext += Bounce;
            this.projectile.OnHitEnemy += OHE;
            this.projectile.BossDamageMultiplier *= 1.2f; 
        }

        public void OHE(Projectile p, SpeculativeRigidbody body, bool b)
        {
            if (body && body.aiActor != null && body.aiActor.healthHaver != null)
            {
                var l = body.aiActor.healthHaver.damageTypeModifiers.Where(self => self.damageType == CoreDamageTypes.Fire && self.damageMultiplier < 1);
                if (l.Count() > 0)
                {
                    var ff = l.ToList();
                    for (int i = 0; i < ff.Count(); i++)
                    {
                        body.aiActor.healthHaver.damageTypeModifiers.Remove(ff[i]);
                    }
                    body.aiActor.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier()
                    {
                        damageMultiplier = 1.33f,
                        damageType = CoreDamageTypes.Fire
                    });
                }
                if (body.aiActor.EffectResistances != null)
                {
                    var l2 = body.aiActor.EffectResistances.Where(self => self.resistType == EffectResistanceType.Fire);
                    if (l2.Count() > 0)
                    {
                        var ff2 = body.aiActor.EffectResistances.ToList();
                        var ff = l2.ToList();
                        for (int i = 0; i < ff.Count(); i++)
                        {
                            ff2.Remove(ff[i]);
                        }
                        body.aiActor.EffectResistances = ff2.ToArray();
                    }
                }
            }
        }

        public Projectile GetProjectile()
        {
            return projectile;
        }
        public void Bounce(BounceProjModifier bounceProjModifier, SpeculativeRigidbody body)
        {
            if (Bounced == true) { return; }
            Bounced = true;
            projectile.baseData.speed *= 0.4f;
            projectile.UpdateSpeed();
        }
        bool Bounced = false;

        public void MyBad(tk2dSpriteAnimator a, tk2dSpriteAnimationClip b)
        {
            projectile.DieInAir(true, false, true, false);
        }
    }
}
