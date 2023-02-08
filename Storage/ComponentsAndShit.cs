using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod
{
    public class MaintainDamageOnPierce : MonoBehaviour
    {
        //Thanks Nevernamed! :D
        public MaintainDamageOnPierce()
        {
            this.damageMultOnPierce = 1f;
            this.AmountOfPiercesBeforeFalloff = -1f;
        }
        public void Start()
        {
            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile)
            {
                SpeculativeRigidbody specRigidbody = this.m_projectile.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePierce));
            }
        }
        private void HandlePierce(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (AmountOfPiercesBeforeFalloff != 0)
            {
                AmountOfPiercesBeforeFalloff--;
                FieldInfo field = typeof(Projectile).GetField("m_hasPierced", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(myRigidbody.projectile, false);
                if (OnPierce != null) { OnPierce(m_projectile, otherRigidbody); }
                if (myRigidbody) 
                {
                    myRigidbody.StartCoroutine(FrameDelay());
                }
            }
        }
        public IEnumerator FrameDelay()
        {
            yield return null;
            if (m_projectile == null) { yield break; }
            m_projectile.projectile.baseData.damage *= this.damageMultOnPierce;
            yield break;
        }

        public Action<Projectile, SpeculativeRigidbody> OnPierce;
        public float AmountOfPiercesBeforeFalloff;
        public float damageMultOnPierce;
        private Projectile m_projectile;
    }
}
