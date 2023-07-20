using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class ShittyVFXAttacher : MonoBehaviour
    {
        public bool wasUsingAltCostume;
        public GameObject gameObj = VFXStorage.VFX_Modulable;
        public tk2dSpriteAnimator VFX;
        public PickupObject g;
        private bool Dont = false;
        public void Start()
        {
            if (Dont == true) { return; }
            g = this.GetComponent<PickupObject>();
            VFX = UnityEngine.Object.Instantiate(gameObj, g.sprite.WorldTopLeft, Quaternion.identity).GetComponent<tk2dSpriteAnimator>();
            VFX.gameObject.transform.parent = g.gameObject.transform;
            VFX.Play(wasUsingAltCostume ? "start_alt" : "start");
        }

        public void CallOfTheVoid()
        {
            Destroy(this);
            Dont = true;
            if (VFX == null) { return; }
            VFX.PlayAndDestroyObject(wasUsingAltCostume ? "break_alt" : "break");
        }

        public void OnDestroy()
        {
            if (VFX == null) { return; }
            VFX.PlayAndDestroyObject(wasUsingAltCostume ? "break_alt" : "break");
        }
    }
}
