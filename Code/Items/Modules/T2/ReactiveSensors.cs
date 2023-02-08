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
            LongDescription = "If an enemy gets too close, release a massive shockwave that stuns, pushes and harms enemies in a large redius (+Damage and push force per stack). Recharges over 15 seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "If an enemy gets too close, release a massive shockwave\nthat stuns, pushes and harms enemies\nin a large radius.(" + StaticColorHexes.AddColorToLabelString("+Damage and push force", StaticColorHexes.Light_Orange_Hex) + ").\nRecharges after 15 seconds.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

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

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnRoomCleared += ORC;
            EnterCharge(player);
        }

        public void ORC(ModulePrinterCore modulePrinter, PlayerController player, RoomHandler room) 
        {
            currentState = State.EnteredCharged;
            RechargeTime = 16;
            AkSoundEngine.PostEvent("Play_BOSS_lasthuman_torch_01", player.gameObject);

            if (extantBattery)
            {
                extantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("finish");
            }
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
                AkSoundEngine.PostEvent("Play_BOSS_lasthuman_torch_01", player.gameObject);

            }
            if (player.CurrentRoom == null) { return; }
            List<AIActor> enemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            if (enemies == null || enemies.Count == 0) { return; }
            foreach (var enemy in enemies)
            {
                if (enemy && Vector2.Distance(enemy.sprite.WorldCenter, player.sprite.WorldCenter) < 3.25f)
                {
                    currentState = State.Active;
                    player.StartCoroutine(KABOOM(modulePrinter, player));
                    RechargeTime = 0;
                }
            }
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
            Exploder.DoDistortionWave(player.sprite.WorldBottomCenter, 20, 0.125f, 7, 0.2f);
            for (int I = 0; I < 240; I++)
            {
                GlobalSparksDoer.DoSingleParticle(player.sprite.WorldCenter, Toolbox.GetUnitOnCircle((float)I * 1.5f, UnityEngine.Random.Range(1.1f, 6.0f) * 3), null, 0.9f, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
            }         
            Exploder.DoRadialPush(player.sprite.WorldCenter, 60 * stack, 7);
            Exploder.DoRadialKnockback(player.sprite.WorldCenter, 60 * stack, 7);
            Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 7);
            ApplyActionToNearbyEnemies(modulePrinterCore , player.sprite.WorldCenter, 7, player.CurrentRoom);
            EnterCharge(player);
            yield break;
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
                            aI.healthHaver.ApplyDamage(65 * stack, aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }

        public void EnterCharge(PlayerController player) 
        {
            if (currentState == State.Inactive) { return; }
            if (extantBattery != null) { return; }
            extantBattery = player.PlayEffectOnActor(BatteryObject, new Vector3(2, 0f));
            extantBattery.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("docharge");
            currentState = State.Inactive;
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

