using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class MirroredSoftware : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MirroredSoftware))
        {
            Name = "Mirrored Software",
            Description = "I'm You, Two",
            LongDescription = "Acts as 2 (+2 per stack) copies of a random active module. Switches at every combat encounter.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mirrored_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mirrored_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Mirrored Software " + h.ReturnTierLabel();
            h.LabelDescription = "Acts as 2 (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ") copies of\na random active module.\nSwitches at every combat encounter.";

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;

            GameObject VFX_Popup = new GameObject("VFX_Popup");
            FakePrefab.MarkAsFakePrefab(VFX_Popup);
            FakePrefab.DontDestroyOnLoad(VFX_Popup);
            var tk2d = VFX_Popup.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Module_T1_Collection;

            AdditionalBraveLight braveLight = VFX_Popup.gameObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = tk2d.sprite.WorldCenter;
            braveLight.LightColor = Color.white;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            tk2d.renderer.material.SetFloat("_Fade", 0f);

            VFX = VFX_Popup;
        }
        public static int ID;
        public static GameObject VFX;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += OEC;
            modulePrinter.OnRoomCleared += ORC;
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= OEC;
            modulePrinter.OnRoomCleared -= ORC;
        }

        public IEnumerator DoFlashyVFX(DefaultModule properties)
        {
            var player = this.Stored_Core.Owner;

            Vector2 playerPos = player.sprite.WorldCenter;

            var VFX_Object = UnityEngine.Object.Instantiate(VFX, playerPos, Quaternion.identity).GetComponent<tk2dBaseSprite>();
            VFX_Object.collection = GlobalModuleStorage.ReturnModule(properties).sprite.Collection;

            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(properties).sprite.spriteId);

            var light = VFX_Object.GetComponent<AdditionalBraveLight>();
            light.LightColor = GlobalModuleStorage.ReturnModule(properties).BraveLight.LightColor;

            Vector2 offset = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2.5f, 3.0f));
            float e = 0;
            while (e < 1)
            {
                float t = Toolbox.SinLerpTValue(e);

                VFX_Object.transform.position = Vector2.Lerp(playerPos, playerPos + offset, t);
                VFX_Object.renderer.material.SetFloat("_Fade", t);
                light.LightIntensity = Mathf.Lerp(0, 5, t);
                light.LightRadius = Mathf.Lerp(0, 3, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            while (e < 1)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            Vector2 p = VFX_Object.transform.PositionVector2();
            float d = UnityEngine.Random.Range(0.7f, 1.5f);
            while (e < d)
            {
                float t = Toolbox.SinLerpTValue(e / d);
                VFX_Object.transform.position = Vector2.Lerp(p, player.sprite.WorldCenter, t);
                light.LightIntensity = Mathf.Lerp(5, 1, t);
                light.LightRadius = Mathf.Lerp(3, 1, t);
                VFX_Object.renderer.material.SetFloat("_Fade", 1-t);

                e += BraveTime.DeltaTime;
                yield return null;
            }
            LootEngine.DoDefaultSynergyPoof(player.sprite.WorldCenter);
            Destroy(VFX_Object.gameObject);
            yield break;
        }

        public IEnumerator DoFlashyVFX_Destroy(DefaultModule properties)
        {
            var player = this.Stored_Core.Owner;
            Vector2 playerPos = player.sprite.WorldCenter;

            var VFX_Object = UnityEngine.Object.Instantiate(VFX, playerPos, Quaternion.identity).GetComponent<tk2dBaseSprite>();
            VFX_Object.collection = GlobalModuleStorage.ReturnModule(properties).sprite.Collection;
            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(properties).sprite.spriteId);
            var light = VFX_Object.GetComponent<AdditionalBraveLight>();
            light.LightColor = properties.BraveLight.LightColor;

            Vector2 offset = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2.5f, 3.0f));
            float e = 0;
            while (e < 2)
            {
                float t = Toolbox.SinLerpTValueFull(e/2);
                float t1 = Toolbox.SinLerpTValue(e/2);

                VFX_Object.transform.position = Vector2.Lerp(playerPos, playerPos + offset, t1);
                VFX_Object.renderer.material.SetFloat("_Fade", t);
                light.LightIntensity = Mathf.Lerp(0, 7, t);
                light.LightRadius = Mathf.Lerp(0, 3, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(VFX_Object.gameObject);
            yield break;
        }

        public void OEC(ModulePrinterCore printer, Dungeonator.RoomHandler roomHandler, PlayerController player)
        {
            int rolls = 5;
            bool found = false;
            while (found == false && rolls > 0)
            {
                var c = BraveUtility.RandomElement<ModulePrinterCore.ModuleContainer>(printer.ModuleContainers);
                if (c.LabelName != this.LabelName)
                {
                    found = !found;
                    c.FakeCount.Add(new Tuple<string, int>("Mirror", this.ReturnStack(printer)*2));
                    c.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);
                    AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", player.gameObject);
                    for (int i = 0; i < this.ReturnStack(printer) * 2; i++)
                    {
                        GameManager.Instance.StartCoroutine(DoFlashyVFX(c.defaultModule));
                    }
                }
                else
                {
                    rolls--;
                }
            }
        }
        public void ORC(ModulePrinterCore printer, PlayerController player, Dungeonator.RoomHandler roomHandler)
        {
            foreach (var help in printer.ModuleContainers)
            {
                for (int i = 0; i < help.FakeCount.Count; i++)
                {
                    var faker = help.FakeCount[i];
                    if (faker.First == "Mirror")
                    {
                        help.FakeCount.Remove(help.FakeCount[i]);
                        help.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);
                        for (int r = 0; r < this.ReturnStack(printer) * 2; r++)
                        {
                            GameManager.Instance.StartCoroutine(DoFlashyVFX_Destroy(help.defaultModule));
                        }
                    }
                }
            }
        }
    }
}

