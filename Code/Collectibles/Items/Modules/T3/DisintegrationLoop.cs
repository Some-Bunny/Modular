using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using JuneLib.Status;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class DisintegrationLoop : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(DisintegrationLoop))
        {
            Name = "Disintegration Loop",
            Description = "Obliterated",
            LongDescription = "Kills grant targeted orbital strikes, up to 25 cached strikes. Cached strikes are automatically used in combat one by one. Strike timings scale with player stats and ignore damage caps. (+Increased Strike Damage And Speed per stack)",
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("disintegrationloop_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("disintegrationloop_t3_alt_module");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Disintegration Loop " + h.ReturnTierLabel();
            h.LabelDescription = $"Kills grant targeted orbital strikes, up to 25 cached strikes.\nCached strikes are automatically used in combat one by one.\nStrike timings scale with player stats and ignore damage caps.\n({StaticColorHexes.AddColorToLabelString("+Increased Strike Damage And Speed")})";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            ID = h.PickupObjectId;



          
        }


        public static int ID;

        public int CachedArtillery = 0;



        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            player.OnKilledEnemy += OKE;
            printer.OnFrameUpdate += OFU;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.OnKilledEnemy -= OKE;
            modulePrinter.OnFrameUpdate -= OFU;
        }
        public void OKE(PlayerController player)
        {
            if (CachedArtillery >= 25) { return; }
            CachedArtillery++;

        }
        private AIActor GetSimplifiedNewTarget(ModulePrinterCore modulePrinter)
        {
            List<AIActor> enmL = new List<AIActor>();
            (modulePrinter.Owner as PlayerController).CurrentRoom?.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref enmL);
            if (enmL == null) { return null; }
            if (enmL.Count == 0) { return null; }
            enmL.RemoveAll(self => self.State != AIActor.ActorState.Normal);
            enmL.RemoveAll(self => self.specRigidbody == null);
            enmL = enmL.OrderByDescending(self => self.healthHaver.currentHealth).ToList();


            if (enmL.Count == 0) { return null; }
            enmL.RemoveAll(self => self.healthHaver.IsDead);
            enmL.RemoveAll(self => self.healthHaver.vulnerable == false);
            enmL.RemoveAll(self => self.spriteAnimator.QueryInvulnerabilityFrame() == true);
            if (enmL.Count == 0) { return null; }
            AIActor t = null;
            if (enmL.Count == 1)
            {
                return enmL[0];
            }
            for (int i = 0; i < enmL.Count; i++)
            {
                t = enmL[i]; break;
            }
            return t;
        }



        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            if (CachedArtillery > 0)
            {
                if (player.CurrentRoom != null)
                {
                    if (player.IsInCombat)
                    {
                        if (Strike == null)
                        {
                            var ai = GetSimplifiedNewTarget(modulePrinter);
                            if (ai != null)
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_supplydrop_activate_01", player.gameObject);
                                Strike = GameManager.Instance.StartCoroutine(DoArtillery(ai));
                            }
                        }
                    }
                }
            }
        }


        private Coroutine Strike;
        public IEnumerator DoArtillery(AIActor aIActor)
        {
            currentTarget = aIActor;
            float c = this.ReturnStack(Stored_Core);
            float M = Stored_Core.ModularGunController.GetRateOfFire(2.75f - (0.25f * c)) / Stored_Core.Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
            float M_1 = Stored_Core.ModularGunController.GetReload(1.75f - (0.25f * c))/ Stored_Core.Owner.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);

            float _ = BraveUtility.RandomAngle();
            GameManager.Instance.StartCoroutine(DoLaser( _, M));
            GameManager.Instance.StartCoroutine(DoLaser(_ + 120, M));
            GameManager.Instance.StartCoroutine(DoLaser(_ + 240, M));
            float t = 0f;
            while (t < M)
            {
                t += BraveTime.DeltaTime;
                yield return null;
            }
            if (aIActor)
            {
                CachedArtillery--;
                var b = aIActor.specRigidbody.UnitBottomCenter;

                for (float i = 0; i < 200; i++)
                {
                    Vector3 pos = Vector3.Lerp(Vector3.zero, new Vector3(0, 30), (float)i / (float)_);
                    GlobalSparksDoer.DoSingleParticle(b.ToVector3ZUp() + new Vector3(0, i * 0.1f, 0), Vector3.down, 0.125f, 1, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                    GlobalSparksDoer.DoSingleParticle(b.ToVector3ZUp() + new Vector3(0, i * 0.1f, 0), Vector3.down, null, 10, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }

                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Cannon_01", aIActor.gameObject);

                var rpoh = SpawnManager.SpawnProjectile(Guns.Prototype_Railgun.DefaultModule.chargeProjectiles[1].Projectile.gameObject, 
                    Vector2.zero, Quaternion.identity).GetComponent<Projectile>();
                if (rpoh != null)
                {
                    rpoh.ignoreDamageCaps = true;
                    rpoh.baseData.damage = 30 + (25 * this.ReturnStack(Stored_Core));
                    rpoh.baseData.speed = 0;
                    rpoh.UpdateSpeed();
                    rpoh.Owner = this.Stored_Core.Owner;
                    rpoh.Shooter = this.Stored_Core.Owner.specRigidbody;
                    rpoh.OnWillKillEnemy += (_1, _2) =>
                    {
                        GameManager.Instance.StartCoroutine(HandleEnemyDeath(_2.aiActor));
                    };
                    this.Stored_Core.Owner.DoPostProcessProjectile(rpoh);
                    rpoh.ForceCollision(aIActor.specRigidbody, new LinearCastResult() 
                    {
                        CollidedX = true,
                        CollidedY = true,
                        Contact = aIActor.specRigidbody.UnitCenter,
                        MyPixelCollider = rpoh.specRigidbody.PixelColliders[0],
                        NewPixelsToMove = new IntVector2(0, 0),
                        Normal = Vector2.zero,
                        OtherPixelCollider = aIActor.specRigidbody.PixelColliders[0],
                        Overlap = true,
                        TimeUsed = 0,
                    });
                    rpoh.DieInAir(true);

                    var ___ = EasyLight.Create(aIActor.specRigidbody.UnitCenter, null, Color.yellow, 1, 10, false, 10, 0, 1, false, false);
                }
            }
            t = 0;


            while (t < M_1)
            {
                t += BraveTime.DeltaTime;
                yield return null;
            }
            Strike = null;
            yield break;
        }
        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            tk2dSprite.usesOverrideMaterial = true;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            bool flag2 = tk2dSprite.renderer.material.shader.name.Contains("ColorEmissive");
            if (flag2)
            {
            }
            return gameObject2.transform;
        }

        public IEnumerator DoLaser(float Rot, float LaserTime)
        {
            GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);
            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.transform.position = currentTarget.sprite.WorldBottomCenter;
            component2.transform.localRotation = Quaternion.Euler(0f, 0f, 90);
            component2.UpdateZDepth();
            Vector3 _ = Toolbox.GetUnitOnCircleVec3(Rot, 10);
            Vector2 __ = Vector3.zero;
            component2.color = Color.red;
            float e = 0f;
            float t = 0f;

            
            while (e < LaserTime)
            {
                if (currentTarget != null)
                {
                    __ = Vector3.MoveTowards(component2.transform.position, currentTarget.specRigidbody.UnitBottomCenter, 30);
                }
                else
                {
                    currentTarget = GetSimplifiedNewTarget(Stored_Core);
                }

                t = e / LaserTime;
                float t2 = Mathf.Clamp01(t * 2.5f);
                if (UnityEngine.Random.value < 0.33f) { GlobalSparksDoer.DoSingleParticle(component2.transform.position, Vector3.up, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING); }
                e += BraveTime.DeltaTime;
                component2.transform.position = (__ + Vector2.Lerp(_, Vector3.zero, t * t)).ToVector3ZUp() + new Vector3(0, Mathf.Lerp(100, 0, t2 * t2));
                component2.dimensions = new Vector2(3200, 1f);
                component2.HeightOffGround = -2;
                component2.renderer.gameObject.layer = 23;
                component2.UpdateZDepth();
                yield return null;
            }
            Destroy(gameObject);
            yield break;
        }
        private AIActor currentTarget;
        private IEnumerator HandleEnemyDeath(AIActor target)
        {
            var __ = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            __.ignoreList = new List<SpeculativeRigidbody> { this.Stored_Core.Owner.specRigidbody };
            Exploder.Explode(target.transform.position, __, Vector2.zero);
            CachedArtillery++;
            target.EraseFromExistenceWithRewards(false);
            Transform copyTransform = this.CreateEmptySprite(target);
            tk2dSprite copySprite = copyTransform.GetComponentInChildren<tk2dSprite>();
            float elapsed = 0f;
            float duration = 1f;
            copySprite.renderer.material.DisableKeyword("TINTING_OFF");
            copySprite.renderer.material.EnableKeyword("TINTING_ON");
            copySprite.renderer.material.DisableKeyword("EMISSIVE_OFF");
            copySprite.renderer.material.EnableKeyword("EMISSIVE_ON");
            copySprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
            copySprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
            copySprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 5f);
            copySprite.renderer.material.SetFloat("_EmissiveColorPower", 1f);
            int emId = Shader.PropertyToID("_EmissivePower");
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / duration;
                copySprite.renderer.material.SetFloat(emId, Mathf.Lerp(1f, 10f, t));
                copySprite.renderer.material.SetFloat("_BurnAmount", t);
                copyTransform.position += Vector3.up * BraveTime.DeltaTime * 1f;
                yield return null;
            }
            UnityEngine.Object.Destroy(copyTransform.gameObject);
            yield break;
        }

    }
}

