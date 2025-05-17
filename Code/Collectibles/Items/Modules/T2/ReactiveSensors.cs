using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class ReactiveSensors : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ReactiveSensors))
        {
            Name = "Reactive Sensors",
            Description = "Get In, Get Out",
            LongDescription = "If an enemy gets too close, releases a massive shockwave that stuns, pushes and harms enemies in a large redius (+Damage, Push Force and Detection Range per stack). Can deflect enemy projectiles within a small radius. Recharges over 15 seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("reactiveshanner_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("reactiveshanner_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Reactive Sensors" + h.ReturnTierLabel();
            h.LabelDescription = "If an enemy gets too close, releases a massive shockwave\nthat stuns, pushes and harms enemies\nin a large radius.(" + StaticColorHexes.AddColorToLabelString("+Damage, Push Force and Detection Range", StaticColorHexes.Light_Orange_Hex) + ")\nCan deflect enemy projectiles.\nRecharges after 15 seconds.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            h.EnergyConsumption = 1;
            h.AdditionalWeightMultiplier = 0.85f;

            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.RETALIATION);
            h.AddModuleTag(BaseModuleTags.DEFENSIVE);


            h.AddToGlobalStorage();
            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("battery_charge_006"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("BatteryAnimation").GetComponent<tk2dSpriteAnimation>();
            BatteryObject = VFX;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject BatteryObject;
        private HeatIndicatorController ring;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnRoomCleared += ORC;
            player.StartCoroutine(EnterCharge(player));
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            ProcessRing(player, false);
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.OnRoomCleared -= ORC;
            if (extantBattery != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lasthuman_torch_01", player.gameObject);
                extantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("finish");
                extantBattery = null;
            }
        }

        public void ORC(ModulePrinterCore modulePrinter, PlayerController player, RoomHandler room) 
        {
            currentState = State.EnteredCharged;
            RechargeTime = 16;
            ProcessRing(player, true);

            if (extantBattery != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_lasthuman_torch_01", player.gameObject);
                extantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("finish");
                extantBattery = null;
            }
        }

        public float ReturnRange(ModulePrinterCore core)
        {
            return 4f + (this.ReturnStack(core));
        }

        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            if (currentState == State.Active) { return; }
            if (RechargeTime < 15)
            {
                RechargeTime += BraveTime.DeltaTime;   
                return;
            }
            if (currentState != State.EnteredCharged)
            {
                currentState = State.EnteredCharged;
                extantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("finish");
                extantBattery = null;
                AkSoundEngine.PostEvent("Play_BOSS_lasthuman_torch_01", player.gameObject);
                ProcessRing(player, true);
                extantBattery = null;
            }
            if (player.CurrentRoom == null) { return; }
            List<AIActor> enemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            if (enemies == null || enemies.Count == 0) { return; }
            foreach (var enemy in enemies)
            {
                if (enemy && Vector2.Distance(enemy.sprite.WorldCenter, player.sprite.WorldCenter) < ReturnRange(modulePrinter) + 0.25f)
                {
                    currentState = State.Active;
                    ProcessRing(player, false);
                    player.StartCoroutine(KABOOM(modulePrinter, player));
                    RechargeTime = 0;
                }
            }
        }
        
        private void ProcessRing(PlayerController player, bool SetActive = true)
        {
            if (ring == null)
            {
                ring = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), player.transform.position, Quaternion.identity, player.transform)).GetComponent<HeatIndicatorController>();
                ring.CurrentRadius = ReturnRange(Stored_Core);
                ring.CurrentColor = new Color(20, 255, 152).WithAlpha(0.02f);
                ring.IsFire = false;
                ring.GetComponent<MeshRenderer>().material.SetFloat("_PxWidth", 0.1f);
                ring.transform.position = player.sprite.WorldCenter.ToVector3ZUp(100);
                //ring.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            }
            ring.StartCoroutine(DoRingLerp(ring, SetActive ? 4 : 0, !SetActive));
        }

        public IEnumerator DoRingLerp(HeatIndicatorController a, float to, bool Destroys = false)
        {
            float e = 0;
            float afafaffa = a.CurrentRadius;
            while (e < 1)
            {
                if (a == null) { yield break; }
                e += BraveTime.DeltaTime;
                a.CurrentRadius = Mathf.Lerp(afafaffa, to, Toolbox.SinLerpTValue(e));
                yield return null;
            }
            if (Destroys == true) { Destroy(a.gameObject); }
            yield break;
        }

        public IEnumerator KABOOM(ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            float e = 0;
            AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_charge_01", player.gameObject);
            while (e < 1.5f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_BOSS_RatMech_Stomp_01", player.gameObject);
            Exploder.DoDistortionWave(player.sprite.WorldBottomCenter * ConfigManager.DistortionWaveMultiplier, 20 * ConfigManager.DistortionWaveMultiplier, 0.125f, 8, 0.2f);
            for (int I = 0; I < 240; I++)
            {
                GlobalSparksDoer.DoSingleParticle(player.sprite.WorldCenter, Toolbox.GetUnitOnCircle((float)I * 1.5f, UnityEngine.Random.Range(1.1f, 6.0f) * 3), null, 0.9f, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }         
            Exploder.DoRadialPush(player.sprite.WorldCenter, 60 * stack, 8);
            Exploder.DoRadialKnockback(player.sprite.WorldCenter, 60 * stack, 8);
            Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 8);
            ApplyActionToNearbyEnemies(modulePrinterCore , player.sprite.WorldCenter, 8f, player.CurrentRoom);

            foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
            {
                if (proj != null)
                {
                    AIActor enemy = proj.Owner as AIActor;
                    if (proj.GetComponent<BasicBeamController>() == null)
                    {
                        if (Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), player.sprite.WorldCenter) < 8 && proj.Owner != null && proj.Owner == enemy)
                        {
                            FistReflectBullet(proj, player.gameActor, proj.baseData.speed *= 2f, (proj.sprite.WorldCenter - player.transform.PositionVector2()).ToAngle(), 1f, proj.IsBlackBullet ? (5 + proj.baseData.speed) * 2 : 5 + proj.baseData.speed, 0f);
                        }
                    }
                }
            }

            player.StartCoroutine(EnterCharge(player));
            yield break;
        }

        public static void FistReflectBullet(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float ReflectAngle, float scaleModifier = 1f, float damageModifier = 10f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            Vector2 Point1 = Toolbox.GetUnitOnCircle(ReflectAngle, 1);
            p.Direction = Point1;

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
            }
            if (p.Owner && p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }
            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }
            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }
            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }
            if (p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies)
            {
                p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
            }
            p.baseData.damage = damageModifier;
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }


        public void ApplyActionToNearbyEnemies(ModulePrinterCore modulePrinterCore, Vector2 position, float radius, RoomHandler room)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            List<AIActor> a = new List<AIActor>();
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        AIActor aI = room.activeEnemies[i];
                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            if (aI.behaviorSpeculator)
                            {
                                if (aI.behaviorSpeculator.ImmuneToStun == false) { aI.behaviorSpeculator.Stun(1.5f); }
                            }
                            aI.healthHaver.ApplyDamage(90 * stack, aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }
        public IEnumerator EnterCharge(PlayerController player) 
        {
            //if (currentState == State.Inactive) { return; }
            if (extantBattery != null) { Destroy(extantBattery); }
            var eaextantBattery = player.PlayEffectOnActor(BatteryObject, new Vector3(2, 0f));
            eaextantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("docharge");
            extantBattery = eaextantBattery;
            currentState = State.Inactive;
            yield break;
        }

        public GameObject extantBattery;
        public State currentState = State.NOLLA;
        public enum State
        {
            Active,
            EnteredCharged,
            Inactive,
            NOLLA
        }
        public float RechargeTime;
    }
}

