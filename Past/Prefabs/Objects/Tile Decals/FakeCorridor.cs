using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class FakeCorridor
    {
        public static void Init()
        {
            CreateFakeCorridor("FakeDoor_Right_1", "fake_corridor_leftward", "fake_corridor_leftwardrightward_outlinedummy", new Vector2(-1, 4f), new Vector3(-0.5625f, 3.5f), "Locked.", new Vector3(1.75f, -1));
            CreateFakeCorridor("FakeDoor_Left_1", "fake_corridor_rightward", "fake_corridor_leftwardrightward_outlinedummy", new Vector2(2, 4f), new Vector3(1.5625f, 3.5f), "Locked.", new Vector3(-1.75f, -1));

            CreateFakeCorridor("FakeDoor_Down_1", "fake_corridor_downward", "fake_corridor_upwarddownward_outlinedummy", new Vector2(2, 0f), new Vector3(1f, -1f), "Locked.", new Vector3(1f, 2f));
            CreateFakeCorridor("FakeDoor_Up_1", "fake_corridor_upward", "fake_corridor_upwarddownward_outlinedummy", new Vector2(2, 2.5f), new Vector3(1f, 1f), "Locked.", new Vector3(1f, -2f));

        }


        public static void CreateFakeCorridor(string name, string spriteName, string OutlineObjectSprite, Vector2 spriteoffset, Vector3 objectOffset, string Dialogue, Vector3 talkpointOffset)
        {
            GameObject obj = PrefabBuilder.BuildObject(name);
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));
            tk2d.usesOverrideMaterial = true;
            tk2d.hasOffScreenCachedUpdate = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);

            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            AddChild(name, obj, OutlineObjectSprite, objectOffset, Dialogue, talkpointOffset);
            var c = StaticCollections.Past_Decorative_Object_Collection.GetSpriteDefinition(spriteName);
            c.position0.x += spriteoffset.x;
            c.position0.y += spriteoffset.y;
            c.position1.x += spriteoffset.x;
            c.position1.y += spriteoffset.y;
            c.position2.x += spriteoffset.x;
            c.position2.y += spriteoffset.y;
            c.position3.x += spriteoffset.x;
            c.position3.y += spriteoffset.y;
            obj.AddComponent<Fuck_Your_Z_Axis_Its_Now_Zero>();
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(name + "_MDLR", obj);
        }

        public static void AddChild(string name, GameObject parent, string outlineObjectSprite, Vector3 offset, string D, Vector3 offsetTalkPoint)
        {
            GameObject obj = PrefabBuilder.BuildObject(name + "_Outline");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(outlineObjectSprite));
            tk2d.usesOverrideMaterial = true;
            tk2d.hasOffScreenCachedUpdate = true;
            tk2d.IsPerpendicular = false;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;

            obj.transform.parent = parent.transform;
            var controller = tk2d.gameObject.AddComponent<QuickInterractableController>();
            Module.Strings.Core.Set("#MDLR_DOOR_" + name, D);
            controller.Interact_String = "#MDLR_DOOR_" + name;

            var talkPoint = PrefabBuilder.BuildObject(name + "_Outline_Talkpoint");
            talkPoint.transform.parent = obj.transform;
            talkPoint.transform.localPosition += offsetTalkPoint;
            //talkPoint.CreateFastBody(new IntVector2(16,16), new IntVector2(0,0));
            //talkPoint.AddComponent<Fuck_Your_Invisible_Hitbox>();

            controller.talkPoint = talkPoint.transform;
            controller.DebugReach = false;
            controller.UsesTransformDist = true;
            obj.transform.localPosition += offset;
            obj.AddComponent<Fuck_Your_Z_Axis_Its_Now_Zero>();
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            obj.CreateFastBody(new IntVector2(0,0), new IntVector2(0, 0));

        }

        public class Fuck_Your_Invisible_Hitbox : MonoBehaviour
        {

            public void Start()
            {
                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody) { specBody.ShowHitBox(); }
            }
        }

        public class Fuck_Your_Z_Axis_Its_Now_Zero : MonoBehaviour
        {
            
            public void Start()
            {
            }
            public void Update()
            {
                this.gameObject.transform.localPosition = this.gameObject.transform.localPosition.WithZ(0);
            }
        }
        public class Fuck_You_Youre_No_Longer_Perpendicular : MonoBehaviour
        {
            public void Start()
            {
                this.GetComponent<tk2dBaseSprite>().IsPerpendicular = false;
            }
        }

    }
}
