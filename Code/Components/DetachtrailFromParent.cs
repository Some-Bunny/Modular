using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class DetachTrailFromParent : MonoBehaviour
    {
        public void OnDestroy()
        {
            if (this.gameObject)
            {
                var transform = this.gameObject.transform.Find(trail_child_object_name);
                if (transform)
                {
                    transform.parent = null;
                    Destroy(transform.gameObject, 2.5f);
                }
            }
        }
        public string trail_child_object_name = "trail object";
    }
}
