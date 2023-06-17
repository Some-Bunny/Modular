using Alexandria.PrefabAPI;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class Dead
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Dead_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("fuckingdead_001"));
            tk2d.hasOffScreenCachedUpdate = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            obj.CreateFastBody(new IntVector2(0, 0), new IntVector2(0, 0));
            var controller = tk2d.gameObject.AddComponent<QuickInterractableController>();
            Module.Strings.Core.Set("#MDLR_CORPSE_", "You fought well.");
            controller.Interact_String = "#MDLR_CORPSE_";

            var talkPoint = PrefabBuilder.BuildObject("Talkpoint");
            talkPoint.transform.parent = obj.transform;
            talkPoint.transform.localPosition += new Vector3(2 ,2);

            controller.talkPoint = talkPoint.transform;
            obj.AddComponent<CorpseController>();

            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("DeadCorpseMDLR", obj);
        }

        public class CorpseController : MonoBehaviour
        {
            private tk2dBaseSprite sprite;
            private ParticleSystem SMOKE;
            public void Start()
            {
                sprite = base.GetComponent<tk2dBaseSprite>();
                SMOKE = UnityEngine.Object.Instantiate(CrateSpawnController.SmokeObject).GetComponent<ParticleSystem>();
                this.transform.position.GetAbsoluteRoom().RegisterInteractable(this.GetComponent<QuickInterractableController>());
            }

            public void Update()
            {
                if (SMOKE && sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value < 0.025f))
                {
                    Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
                    Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
                    Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                    ParticleSystem particleSystem = SMOKE;
                    var trails = particleSystem.trails;
                    trails.worldSpace = false;
                    var main = particleSystem.main;
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                    {
                        position = position,
                        randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                    };
                    var emission = particleSystem.emission;
                    emission.enabled = false;
                    particleSystem.gameObject.SetActive(true);
                    particleSystem.Emit(emitParams, 1);
                }
            }
        }

    }//BG_Nonsense
}
