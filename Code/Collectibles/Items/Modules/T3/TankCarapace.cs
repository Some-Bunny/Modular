﻿using Alexandria.ItemAPI;
using Brave.BulletScript;
using Dungeonator;
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
using static UnityEngine.ParticleSystem;


namespace ModularMod
{
    public class TankCarapace : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TankCarapace))
        {
            Name = "Tank Carapace",
            Description = "I AM BULLETPROOF",
            LongDescription = "Grants a damage and fire rate boost (+Damage and Fire Rate per stack) the longer you have been standing still. While standing still gain defensive orbitals that break bullets,up to 8 (+4). Lose all bonuses and orbitals when you start moving again."+ "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("tankstance_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("tankstance_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Tank Carapace " + h.ReturnTierLabel();
            h.EnergyConsumption = 2;
            h.LabelDescription = "Grants a damage and fire rate boost (" + StaticColorHexes.AddColorToLabelString("+Damage and Fire Rate", StaticColorHexes.Light_Orange_Hex) + ")\nthe longer you have been standing still.\nWhile standing still, gain up to 4 (" + StaticColorHexes.AddColorToLabelString("+4", StaticColorHexes.Light_Orange_Hex) + ") defensive orbitals.\n" +StaticColorHexes.AddColorToLabelString("Lose all bonuses and orbitals when you start moving again.", StaticColorHexes.Red_Color_Hex);

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.OverrideScrapCost = 10;
            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);

            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            ID = h.PickupObjectId;


            GameObject Guon = new GameObject("Guon_Carapace");
            FakePrefab.DontDestroyOnLoad(Guon);
            FakePrefab.MarkAsFakePrefab(Guon);
            Guon.SetActive(false);
            var tk2d = Guon.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("warning_003"));
            var tk2dAnim = Guon.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = StaticCollections.Generic_VFX_Animation;
            tk2dAnim.defaultClipId = StaticCollections.Generic_VFX_Animation.GetClipIdByName("zappyguon");
            tk2dAnim.playAutomatically = true;
            var speculativeRigidbody = Guon.CreateFastBody(CollisionLayer.EnemyBulletBlocker, new IntVector2(12, 12), new IntVector2(-1, -1));
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;

            var orbital = Guon.AddComponent<PlayerOrbital>();
            orbital.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
            orbital.shouldRotate = false;
            orbital.orbitRadius = 3.2f;
            orbital.SetOrbitalTier(0);
            orbital.orbitDegreesPerSecond = 90;
            orbital.perfectOrbitalFactor = 100f;

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 2);
            mat.SetFloat("_EmissivePower", 2);
            tk2d.renderer.material = mat;

            ImprovedAfterImage yes = Guon.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.25f;
            yes.shadowTimeDelay = 0.05f;
            yes.dashColor = new Color(0, 0.4f, 0.4f, 1f);

            Guon.AddComponent<DefensiveDrone>();

            Orbital = Guon;
                
        }
        public static int ID;
        public static GameObject Orbital;
        public List<GameObject> Orbital_Count = new List<GameObject>();

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnFrameUpdate += OnUpdate;

            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessStats,
                Accuracy_Process = ProcessStats,
                ChargeSpeed_Process = ProcessStats,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectile += PPP;
            player.knockbackDoer.knockbackMultiplier = 0;
        }

        public void OnUpdate(ModulePrinterCore printer, PlayerController player)
        {
            int stack = this.ReturnStack(printer);
            c += printer.isStandingStill() ? BraveTime.DeltaTime * 1.75f : -(BraveTime.DeltaTime * 4);
            if (c < 0 && printer.isStandingStill()) { c = 0; }
            if (c > 0 && !printer.isStandingStill()) { c = 0; }

            Multiplier = Mathf.Max(1, Mathf.Min(1.5f + (0.5f * stack), 1 + ((c / 4) * stack)));
            if (c > 0.8f && Orbital_Count.Count < (4 * stack))
            {
                c = 0;
                float r = Orbital_Count.Count > 7 ? Mathf.Ceil(Orbital_Count.Count / 8) * 1.4f : 0;
                int h = (int)Mathf.Ceil(Orbital_Count.Count / 8);
                AkSoundEngine.PostEvent("Play_OBJ_daggershield_start_01", player.gameObject);
                for (int i = 0 ; i < 4; i++)
                {
                    GameObject guon = PlayerOrbitalItem.CreateOrbital(player, Orbital, false);
                    guon.GetComponent<DefensiveDrone>().Stack = stack;
                    guon.GetComponent<PlayerOrbital>().m_orbitalTier = h;
                    guon.GetComponent<PlayerOrbital>().orbitRadius = 2f + r;
                    guon.GetComponent<PlayerOrbital>().orbitDegreesPerSecond = 360 / (Mathf.Ceil(Orbital_Count.Count / 8) + 1);

                    Orbital_Count.Add(guon);
                    guon.GetComponent<PlayerOrbital>().Initialize(player);
                }
            }
            else if (c < -0.2f && Orbital_Count.Count > 0)
            {
                c = 0;
                AkSoundEngine.PostEvent("Play_OBJ_drum_break_01", player.gameObject);
                for (int i = 0; i < Orbital_Count.Count; i++)
                {
                    Destroy(Orbital_Count[i]);
                }
                Orbital_Count.Clear();
            }
        }

        private float c;

        public float Multiplier = 1;

        public float ProcessStats(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (Multiplier * 1.125f);
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.damage *= 1 + (Multiplier * 0.166f);
            p.baseData.speed *= 1 + (Multiplier * 0.25f);
            p.baseData.force *= Multiplier;
            p.UpdateSpeed();
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.knockbackDoer.knockbackMultiplier = 1;
            modulePrinter.OnFrameUpdate -= OnUpdate;          
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            player.stats.RecalculateStats(player);
            AkSoundEngine.PostEvent("Play_OBJ_drum_break_01", player.gameObject);
            for (int i = 0; i < Orbital_Count.Count; i++)
            {
                Destroy(Orbital_Count[i]);
            }
            Orbital_Count.Clear();
        }


    }
    public class DefensiveDrone : BraveBehaviour
    {
        public int Stack = 1;
        public RoomHandler room;

        public void Start()
        {
            if (ConfigManager.DoVisualEffect == true)
            {
                var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects[0].effects[0].effect, this.sprite.WorldCenter, Quaternion.identity);
                obj.transform.parent = this.transform;
                Destroy(obj, 2);
            }
        }

        public float Cooldown = 1;
        public void Update()
        {
            room = this.transform.position.GetAbsoluteRoom();
            if (Cooldown > 0) { Cooldown -= BraveTime.DeltaTime; }
            else if (room != null)
            {
                var l = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                if (l != null)
                {
                    foreach (var enemy in l)
                    {
                        if (Vector2.Distance(enemy.transform.PositionVector2(), this.transform.PositionVector2()) < 2)
                        {
                            if (ConfigManager.DoVisualEffect == true)
                            {
                                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, sprite.WorldCenter, Quaternion.identity);
                                blankObj.transform.localScale = Vector3.one * 0.2f;
                                Destroy(blankObj, 2f);
                            }
                            Cooldown = 5;
                            Exploder.DoRadialPush(sprite.WorldCenter, 80 * Stack, 2);
                            Exploder.DoRadialKnockback(sprite.WorldCenter, 80 * Stack, 2);
                            //Exploder.DoRadialMinorBreakableBreak(sprite.WorldCenter, 2);
                            ApplyActionToNearbyEnemies(sprite.WorldCenter, 3, this.transform.position.GetAbsoluteRoom(), (float)(Stack / 6) );
                        }
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            this.GetComponent<PlayerOrbital>().OnDestroy();

            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, sprite.WorldCenter, Quaternion.identity);
            blankObj.transform.localScale = Vector3.one * 0.5f;

            Destroy(blankObj, 2f);
            Exploder.DoRadialPush(sprite.WorldCenter, 120 * Stack, 3);
            Exploder.DoRadialKnockback(sprite.WorldCenter, 120 * Stack, 3);
            Exploder.DoRadialMinorBreakableBreak(sprite.WorldCenter, 3);
            ApplyActionToNearbyEnemies(sprite.WorldCenter, 5, this.transform.position.GetAbsoluteRoom(), Stack);
            base.OnDestroy();
        }
        public void ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room, float m)
        {
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
                            aI.healthHaver.ApplyDamage(5f * m, aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }
    }
}

