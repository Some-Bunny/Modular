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
    public class BarterSoftware : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BarterSoftware))
        {
            Name = "Barter Software",
            Description = "With Gold",
            LongDescription = "Adds 1 Pierce, Reduce Damage by 20% (-20% per stack hyperbolically) But each enemy pierced increases projectile damage by 2x (+0.5x per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("swindler_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("swindler_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Barter Software " + h.ReturnTierLabel();
            h.LabelDescription = "Reduces all prices by 33%.\nTaking damage breaks this module\n(" + StaticColorHexes.AddColorToLabelString("+Extra hits before breaking", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
            Alexandria.NPCAPI.CustomDiscountManager.DiscountsToAdd.Add(new Alexandria.NPCAPI.ShopDiscount()
            {
                ItemIsValidForDiscount = IsValid,
                CanDiscountCondition = CanDisc,
                IdentificationKey = "Barter_Module",
                PriceMultiplier = 0.66f
            });

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("swindler_t2_module"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("SwindlerVFXAnimation").GetComponent<tk2dSpriteAnimation>();
            BarterObject = VFX;
            ID = h.PickupObjectId;
        }

        public static GameObject BarterObject;

        public static bool CanDisc()
        {
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasModule(BarterSoftware.ID) != null) { return true; }
            }
            return false;
        }
        public static bool IsValid(ShopItemController shopItemController)
        {
            return true;
        }

        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamaged += OD;
        }
        public void OD(ModulePrinterCore modulePrinter, PlayerController player)
        {
            int c = this.ReturnStack(modulePrinter);
            if (c == 1)
            {
                var fx = player.PlayEffectOnActor(BarterObject, new Vector3(0, 2.25f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(this.ModularIsAltSkin() == false ? "destroy" : "destroy_alt");
                fx.GetComponent<tk2dSpriteAnimator>().m_onDestroyAction = () =>
                {
                    AkSoundEngine.PostEvent("Play_WPN_claw_blast_03", player.gameObject);
                    player.PlayEffectOnActor(StaticExplosionDatas.explosiveRoundsExplosion.effect, new Vector3(0, 2.25f));
                };
            }
            else if (c > 1)
            {
                var fx = player.PlayEffectOnActor(BarterObject, new Vector3(0, 2.25f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(this.ModularIsAltSkin() == false ? "break_one" : "break_one_alt");
            }
            modulePrinter.RemoveModule(BarterSoftware.ID);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamaged -= OD;
        }
    }
}

