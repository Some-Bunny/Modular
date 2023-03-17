using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class MinelayerSystem : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MinelayerSystem))
        {
            Name = "Minelayer System",
            Description = "Fortification Expert",
            LongDescription = "Projectile damage reduced by 66%. On destruction, projectiles now leave proximity mines that take 3 (-15% hyperbolically per stack) seconds to prime, and take 1 (-15% hyperbolically per stack) second to detonate." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("minelayer_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("minelayer_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Minelayer System " + h.ReturnTierLabel();
            h.LabelDescription = "Projectile damage reduced by 66%.\nOn destruction, projectiles now leave proximity mines that\ntake 3 (" + StaticColorHexes.AddColorToLabelString("+15% hyperbollicaly", StaticColorHexes.Light_Orange_Hex) + ") seconds to prime\n and 1 (" + StaticColorHexes.AddColorToLabelString("+15% hyperbollicaly", StaticColorHexes.Light_Orange_Hex) + ") second to detonate.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            Mine = BuildPrefab();
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject Mine;

        public static GameObject BuildPrefab()
        {
            GameObject VFX = new GameObject("Mine");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);

            VFX.SetActive(false);
            VFX.AddComponent<DebrisObject>();
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("mine_bounce_001"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("MinelayerAnimation").GetComponent<tk2dSpriteAnimation>();


            tk2dAnim.Library.GetClipByName("start").frames[0].eventInfo = "BeepBoop";
            tk2dAnim.Library.GetClipByName("start").frames[0].triggerEvent = true;

            tk2dAnim.Library.GetClipByName("boom").frames[0].eventInfo = "Kabloom";
            tk2dAnim.Library.GetClipByName("boom").frames[0].triggerEvent = true;
            var audioListener = VFX.AddComponent<AudioAnimatorListener>();
            audioListener.animationAudioEvents = new ActorAudioEvent[]
            {
                new ActorAudioEvent
                {
                    eventName = "Play_OBJ_mine_beep_01",
                    eventTag = "BeepBoop"
                },
                new ActorAudioEvent
                {
                    eventName = "Play_OBJ_metroid_roll_01",
                    eventTag = "Kabloom"
                }
            };

            var exData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            exData.damage = 15;
            CustomProximityMine proximityMine = new CustomProximityMine
            {
                explosionData = exData,
                explosionStyle = CustomProximityMine.ExplosiveStyle.PROXIMITY,
                detectRadius = 2.25f,
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 1f,
                explodeAnimName = "boom",
                idleAnimName = "start",
                MovesTowardEnemies = false,
                HomingTriggeredOnSynergy = false,
                HomingDelay = 3.25f,
                HomingRadius = 10,
                HomingSpeed = 4,
               
            };
            var boom = VFX.AddComponent<CustomProximityMine>(proximityMine);
            return VFX;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 0.33f;
            p.OnDestruction += (ONJ) =>
            {
                var obj = UnityEngine.Object.Instantiate(Mine, ONJ.sprite.WorldCenter, Quaternion.identity);    
                Vector3 vector = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1).ToVector3ZUp(0f).normalized;
                vector *= UnityEngine.Random.Range(1, 4);
                DebrisObject orAddComponent = obj.GetOrAddComponent<DebrisObject>();
                orAddComponent.additionalHeightBoost = 1.5f;
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.sprite.UpdateZDepth();
                orAddComponent.Trigger(vector.WithZ(2f), 0.5f, 1f);
                var prox = orAddComponent.gameObject.GetComponent<CustomProximityMine>();
                prox.Force_Disarm = true;
                orAddComponent.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
                orAddComponent.sprite.renderer.material.SetFloat("_EmissivePower", 0.1f);
                orAddComponent.StartCoroutine(ArmTime(prox));
            };     
        }
        public IEnumerator ArmTime(CustomProximityMine mine)
        {
            mine.explosionDelay = 1 - (1 - (1 / (1 + 0.15f * Stack())));
            float d = 3;
            d *= 1 - (3 - (3 / (1 + 0.15f * Stack())));
            float e = 0;
            while (e < d)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_BOSS_mineflayer_trigger_01", mine.gameObject);
            mine.Force_Disarm = false;
            mine.sprite.renderer.material.SetFloat("_EmissiveColorPower", 10f);
            mine.sprite.renderer.material.SetFloat("_EmissivePower", 10f);
            yield break;
        }
    }
}

