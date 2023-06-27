using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class WoodenCrate
    {
        public static void Init()
        {
            //obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));


            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("WoodenCrateMDLR",
              Toolbox.GenerateDungeonPlaceable(
              new Dictionary<GameObject, float>()
              {
              { CreateObject("box_ammo", "box_", "A wooden crate.", new IntVector2(28, 27), new IntVector2(0, -4)), 1 },
              {  CreateObject("box_ammo_small", "box_small", "A wooden crate, though this one is smaller.", new IntVector2(28, 27), new IntVector2(0, -4)), 0.1f },
              }));


            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("WoodenCrateAmmoMDLR", 
            Toolbox.GenerateDungeonPlaceable(
            new Dictionary<GameObject, float>()
            {
              { CreateObject("box_ammo_1", "box_ammo", "A box filled with basic ammunition", new IntVector2(28, 27), new IntVector2(0, -4)), 1 },
              { CreateObject("box_ammo_2", "box_ammo_red", "A box filled with various munitions", new IntVector2(28, 27), new IntVector2(0, -4)), 1 },
            }));

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BustedTVMDLR", CreateObject("bustedTV", "old_robot_tv_001", "A broken television.\nI still wonder why they would bring it there...", new IntVector2(14, 12), new IntVector2(1, -4)));

            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("WoodenCrateOpenMDLR", CreateObject("box_open", "box_open", "A wooden crate.\nIt has nothing stored in it.", new IntVector2(28, 27), new IntVector2(0, -4)));


            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("HMPRIME_CART", CreateObject("hmprime_cart_mdlr", "hymprime_unit", "A H.M Prime Unit. Recently finished and designed.\nI'll see it again.", new IntVector2(38, 28), new IntVector2(1, -4)));

            var verytinycrate = CreateObject("box_metal_baby", "metal_crate_verytiny", "A baby metal container.\nIt seems to have lost it's parents.", new IntVector2(17, 20), new IntVector2(0, -4));


            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("MetalCrateMDLR",
              Toolbox.GenerateDungeonPlaceable(
              new Dictionary<GameObject, float>()
              {
              { CreateObject("box_metal", "metal_crate_small", "A metal container.\nContents: Indeterminate.", new IntVector2(20, 32), new IntVector2(0, -4)), 1 },
              {  CreateObject("box_metal_tall", "metal_crate_tall", "A large metal container.\nContents: Indeterminate.", new IntVector2(20, 40), new IntVector2(0, -4)), 0.7f },
              {  CreateObject("box_metal_short", "metal_crate_tiny", "A small metal container\nContents: Indeterminate.", new IntVector2(20, 26), new IntVector2(0, -4)), 0.5f },
              {verytinycrate, 0.1f }

              }));

            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("LongShortWoodenCrateMDLR",
            Toolbox.GenerateDungeonPlaceable(
            new Dictionary<GameObject, float>()
            {
              { CreateObject("short_longbox", "short_crate", "A long, short wooden crate.", new IntVector2(28, 16), new IntVector2(0, -4)), 1 },
            }));
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("TinyAssCrateMDLR",
            Toolbox.GenerateDungeonPlaceable(
            new Dictionary<GameObject, float>()
            {
              { CreateObject("short_longbox", "tiny_ass_crate", "A very tiny wooden crate.", new IntVector2(13, 16), new IntVector2(0, -4)), 1 },
            }));
        }


        public static GameObject CreateObject(string name, string spriteName, string Dialogue, IntVector2 size, IntVector2 offset)
        {
            GameObject obj = PrefabBuilder.BuildObject(name);
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0);
            mat.SetFloat("_EmissivePower", 0);
            tk2d.renderer.material = mat;

            obj.CreateFastBody(size, offset);
            var controller = tk2d.gameObject.AddComponent<QuickInterractableController>();
            Module.Strings.Core.Set("#MDLR_CRATE_"+name, Dialogue);
            controller.Interact_String = "#MDLR_CRATE_" + name;
            controller.talkPoint = obj.transform;
            return obj;
        }
       
    }
}
