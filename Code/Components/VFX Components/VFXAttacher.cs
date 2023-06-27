using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class ShittyVFXAttacher : MonoBehaviour
    {
        public PlayerController mainPlayer;
        public GameObject gameObj = VFXStorage.VFX_Modulable;
        public tk2dSpriteAnimator VFX;
        public PickupObject g;
        public void Start()
        {
            g = this.GetComponent<PickupObject>();
            VFX = UnityEngine.Object.Instantiate(gameObj, g.sprite.WorldTopLeft, Quaternion.identity).GetComponent<tk2dSpriteAnimator>();
            VFX.gameObject.transform.parent = g.gameObject.transform;
            VFX.Play(mainPlayer.IsUsingAlternateCostume ? "start_alt" : "start");
        }
        public void OnDestroy()
        {
            VFX.PlayAndDestroyObject(mainPlayer.IsUsingAlternateCostume ? "break_alt" : "break");
        }
    }
}
