using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class BeamCollisionEvent : MonoBehaviour
    {
        public bool WillBeDestroyed = false;
        public bool isCollisionEvent = false;
        public Func<Projectile, bool> DetermineDestroy;
    }
}
