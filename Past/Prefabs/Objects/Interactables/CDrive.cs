using Alexandria.PrefabAPI;
using Brave.BulletScript;
using Dungeonator;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.FakeCorridor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class CDrive
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("C_Drive");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("cdrivefullofmalware"));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;

            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            var switche = obj.AddComponent<SusBoxController>();
            Module.Strings.Core.Set("#MDLR_CDRIVE_DEF", "A data storage device. Likely left carelessly during the evacuation.");
            Module.Strings.Core.Set("#MDLR_CDRIVE_ACC", StaticColorHexes.AddColorToLabelString("Take out a drive.", StaticColorHexes.Green_Hex));
            Module.Strings.Core.Set("#MDLR_CDRIVE_CAN", StaticColorHexes.AddColorToLabelString("Leave it.", StaticColorHexes.Red_Color_Hex));
            switche.acceptOptionKey = "#MDLR_CDRIVE_ACC";
            switche.displayTextKey = "#MDLR_CDRIVE_DEF";
            switche.declineOptionKey = "#MDLR_CDRIVE_CAN";
            switche.talkPoint = obj.transform;
            switche.PickupID = HighValueInfo.CoolInfoID;
            obj.CreateFastBody(new IntVector2(14, 12), new IntVector2(0, -3));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("C_DRIVE_MDLR", obj);

        }
    }
}
