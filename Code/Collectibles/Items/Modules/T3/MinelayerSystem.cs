﻿using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            LongDescription = "Projectile damage reduced by 75%. Increases fire rate by 25% and massively reduces spread. Projectiles now leave proximity mines on impact that take 3 (-33% hyperbolically per stack) seconds to prime, and take 1 (-25% hyperbolically per stack) second to detonate. (+Explosion Damage per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
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
            h.LabelDescription = "Projectile damage reduced by 70%.\nIncreases fire rate by 33% and massively reduces spread.\nProjectiles now leave proximity mines on impact that\ntake 3 (" + StaticColorHexes.AddColorToLabelString("-33% hyperbollicaly", StaticColorHexes.Light_Orange_Hex) + ") seconds to prime\n and 1 (" + StaticColorHexes.AddColorToLabelString("-25% hyperbollicaly", StaticColorHexes.Light_Orange_Hex) + ") second to detonate.\n("+StaticColorHexes.AddColorToLabelString("+Explosion Damage")+")";

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            Mine = BuildPrefab();
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject Mine;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.005f) { return; }
            p.baseData.damage *= 0.25f;
            p.baseData.force *= 1.5f;

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
                orAddComponent.StartCoroutine(ArmTime(prox, 1));
            };
        }


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

            data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            data.damage = 15;
            data.force = 0;
            CustomProximityMine proximityMine = new CustomProximityMine
            {
                explosionData = data,
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

        public static ExplosionData data;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                Accuracy_Process = ProcessAccuracy,
                ChargeSpeed_Process = ProcessFireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 0.66f;
        }

        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 0.33f;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.damage *= 0.3f;
            p.baseData.force *= 1.5f;

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
                var ex = StaticExplosionDatas.CopyFields(data);
                ex.damage = 10 + (5* Stack());
                prox.explosionData = ex;
                orAddComponent.sprite.renderer.material.SetFloat("_EmissiveColorPower", 0.1f);
                orAddComponent.sprite.renderer.material.SetFloat("_EmissivePower", 0.1f);
                orAddComponent.StartCoroutine(ArmTime(prox, null));
            };     
        }
        public IEnumerator ArmTime(CustomProximityMine mine, float? armTime)
        {
            mine.explosionDelay = 1.25f - (1.25f - (1.25f / (1 + 0.25f * (armTime ?? Stack()))));
            float d1 = 3.5f;
            d1 *= (3.5f - (3.5f / (1 + 0.33f * (armTime ?? Stack()))));


            float e = 0;
            while (e < d1)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            DebrisObject orAddComponent = mine.GetComponent<DebrisObject>();
            if (orAddComponent) { Destroy(orAddComponent); }
            AkSoundEngine.PostEvent("Play_BOSS_mineflayer_trigger_01", mine.gameObject);

            mine.Force_Disarm = false;
            mine.explosionData.ignoreList = new List<SpeculativeRigidbody>()
            {

            };
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                mine.explosionData.ignoreList.Add(player.specRigidbody);
            }
            mine.sprite.renderer.material.SetFloat("_EmissiveColorPower", 15f);
            mine.sprite.renderer.material.SetFloat("_EmissivePower", 15f);
            Destroy(mine.gameObject, 20);
            yield break;
        }
    }
}

