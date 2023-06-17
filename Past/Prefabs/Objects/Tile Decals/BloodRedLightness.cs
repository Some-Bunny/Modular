using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class BloodRedLightness
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BloodRedLight_MDLR");
            AdditionalBraveLight braveLight = obj.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = obj.transform.position;
            braveLight.LightColor = Color.red;
            braveLight.LightIntensity = 5f;
            braveLight.LightRadius = 10;
            obj.AddComponent<RedLight_Controller>();
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BloodRedLight_MDLR", obj);
        }

        public class RedLight_Controller : MonoBehaviour
        {
            private AdditionalBraveLight light;
            public void Start()
            {
                light = this.GetComponent<AdditionalBraveLight>();
            }
            private float Counter;
            public void Update()
            {
                Counter += Toolbox.SinLerpTValueFull((BraveTime.DeltaTime * 6));
                light.LightIntensity = Mathf.PingPong(Counter, 12);

            }
        }
    }
}
