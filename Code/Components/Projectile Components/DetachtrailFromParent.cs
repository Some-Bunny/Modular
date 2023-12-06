using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class DetachTrailFromParent : MonoBehaviour
    {
        public void DoDetach(float DetachTime = 0)
        {
            if (this.gameObject)
            {
                var transform = this.gameObject.transform.Find(trail_child_object_name);
                if (transform)
                {
                    transform.parent = null;
                    Destroy(transform.gameObject, DetachTime);
                }
            }
        }

        public void OnDestroy()
        {
            DoDetach(WaitTime);
        }
        public float WaitTime = 2.5f;
        public string trail_child_object_name = "trail object";
    }

}
