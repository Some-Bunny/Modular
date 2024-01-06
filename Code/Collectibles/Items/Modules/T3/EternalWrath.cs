using Alexandria.ItemAPI;
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
using static ModularMod.VFXStorage;
using static tk2dSpriteCollectionDefinition;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class EternalWrath : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(EternalWrath))
        {
            Name = "Eternal Wrath",
            Description = "Strong Spirit",
            LongDescription = "Cheat Death Once (+1 Revive). Cheating death grants permanent stat boosts. Module is destroyed after cheating death." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("eternalwrath_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("eternalwrath_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Eternal Wrath " + h.ReturnTierLabel();
            h.LabelDescription = "Cheat Death Once ("+StaticColorHexes.AddColorToLabelString("+1 Revive")+").\nCheating death grants "+StaticColorHexes.AddColorToLabelString("permanent", StaticColorHexes.Green_Hex) +" stat boosts.\nModule is destroyed after cheating death.";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.EnergyConsumption = 2;
            h.OverrideScrapCost = 20;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.75f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            player.healthHaver.OnPreDeath += CheatDeath;
        }
        private void CheatDeath(Vector2 finalDamageDirection)
        {
            if (Revives == 0) { return; }
            Revives--;
            var player = Stored_Core.Owner;

            player.healthHaver.Armor = 2;
            player.ToggleGunRenderers(true, "non-death");
            player.ToggleHandRenderers(true, "non-death");
            player.CurrentInputState = PlayerInputState.AllInput;
            player.spriteAnimator.enabled = true;

            Toolbox.ApplyStat(player, PlayerStats.StatType.Damage, .125f, StatModifier.ModifyMethod.ADDITIVE);
            Toolbox.ApplyStat(player, PlayerStats.StatType.Accuracy, .90f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            Toolbox.ApplyStat(player, PlayerStats.StatType.RateOfFire, .1f, StatModifier.ModifyMethod.ADDITIVE);
            Toolbox.ApplyStat(player, PlayerStats.StatType.ProjectileSpeed, .1f, StatModifier.ModifyMethod.ADDITIVE);
            Toolbox.ApplyStat(player, PlayerStats.StatType.KnockbackMultiplier, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            Pixelator.Instance.DoFinalNonFadedLayer = true;
            player.CurrentInputState = PlayerInputState.NoInput;
            GameManager.Instance.MainCameraController.SetManualControl(true, false);

            player.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unfaded"));
            GameUIRoot.Instance.ForceClearReload(player.PlayerIDX);
            GameUIRoot.Instance.notificationController.ForceHide();

            GameManager.Instance.PauseRaw();
            GameManager.Instance.PreventPausing = true;

            player.StartCoroutine(DoResurrection(player));

        }

        public IEnumerator DoResurrection(PlayerController player)
        {
            bool wasPitFalling = player.IsFalling;
            this.Stored_Core.RemoveModule(this, 1);
            Transform cameraTransform = GameManager.Instance.MainCameraController.transform;
            Vector3 cameraStartPosition = cameraTransform.position;
            Vector3 cameraEndPosition = player.CenterPosition;
            GameManager.Instance.MainCameraController.OverridePosition = cameraStartPosition;

            float e = 0;
            while (e < 0.75f)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    e += 0.05f;
                }
                e += GameManager.INVARIANT_DELTA_TIME;
                float t = e / 0.5f;
                GameManager.Instance.MainCameraController.OverridePosition = Vector3.Lerp(cameraStartPosition, cameraEndPosition, t);
                player.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                Pixelator.Instance.saturation = Mathf.Clamp01(1f - t);
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", player.gameObject);
            var text = Toolbox.GenerateText(player.transform, new Vector2(-11, 0), 0, StaticColorHexes.AddColorToLabelString("LAZARUS SYSTEM ONLINE", StaticColorHexes.Red_Color_Hex), new Color32(0,0,0,0), true, 20);
            e = 0;
            while (e < 1f)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    e += 0.05f;
                }
                e += GameManager.INVARIANT_DELTA_TIME;

                bool enabled = e % 0.25f > 0.125f;
                text.label.isVisible= enabled;
                yield return null;
            }
            GameManager.Instance.Unpause();
            GameManager.Instance.PreventPausing = false;

            player.CurrentInputState = PlayerInputState.AllInput;
            player.IsVisible = true;
            player.ToggleGunRenderers(true, "death");
            player.ToggleHandRenderers(true, "death");

            player.ToggleAttachedRenderers(true);

            player.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
            GameManager.Instance.DungeonMusicController.ResetForNewFloor(GameManager.Instance.Dungeon);
            if (player.CurrentRoom != null)
            {
                GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(player.CurrentRoom);
            }
            GameManager.Instance.ForceUnpause();
            GameManager.Instance.PreventPausing = false;
            BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
            if (wasPitFalling)
            {
                player.PitRespawn(Vector2.zero);
            }

            MourningStarVFXController.SpawnMourningStar(player.sprite.WorldBottomCenter, 0.5f);
            Pixelator.Instance.DoFinalNonFadedLayer = false;

            AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", player.gameObject);
            Destroy(text.gameObject);
            player.healthHaver.IsVulnerable = true;
            player.healthHaver.TriggerInvulnerabilityPeriod(3);
            GameManager.Instance.MainCameraController.SetManualControl(false, false);
            ExplosionData boomboom = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericSmallExplosion);//StaticExplosionDatas.genericSmallExplosion;
            boomboom.damageToPlayer = 0;
            boomboom.damageRadius = 25f;
            boomboom.damage = 750;
            boomboom.preventPlayerForce = true;
            boomboom.ignoreList.Add(player.specRigidbody);
            boomboom.playDefaultSFX = false;
            boomboom.doExplosionRing = false;
            Exploder.Explode(player.sprite.WorldCenter, boomboom, player.transform.PositionVector2());
            yield break;
        }


        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            Revives++;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.healthHaver.OnPreDeath -= CheatDeath;
        }
        public int Revives = 0;


        /*
         * 
         *             private IEnumerator HandleTrollDeath(PlayerController p)
            {
                p.healthHaver.IsVulnerable = false;

                bool wasPitFalling = p.IsFalling;
                Pixelator.Instance.DoFinalNonFadedLayer = true;
                if (p.CurrentGun)
                {
                    p.CurrentGun.CeaseAttack(false, null);
                }
                p.CurrentInputState = PlayerInputState.NoInput;
                GameManager.Instance.MainCameraController.SetManualControl(true, false);
                p.ToggleGunRenderers(false, "death");
                p.ToggleHandRenderers(false, "death");
                p.spriteAnimator.Play("death");

                PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ToggleAttachedRenderers", p, new object[] { true });
                Transform cameraTransform = GameManager.Instance.MainCameraController.transform;
                Vector3 cameraStartPosition = cameraTransform.position;
                Vector3 cameraEndPosition = p.CenterPosition;
                GameManager.Instance.MainCameraController.OverridePosition = cameraStartPosition;
                if (p.CurrentGun)
                {
                    p.CurrentGun.DespawnVFX();
                }
                yield return null;
                p.ToggleHandRenderers(false, "death");
                if (p.CurrentGun)
                {
                    p.CurrentGun.DespawnVFX();
                }
                p.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unfaded"));
                GameUIRoot.Instance.ForceClearReload(p.PlayerIDX);
                GameUIRoot.Instance.notificationController.ForceHide();
                float elapsed = 0f;
                float duration = 0.8f;
                tk2dBaseSprite spotlightSprite = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DeathShadow", ".prefab"), p.specRigidbody.UnitCenter, Quaternion.identity)).GetComponent<tk2dBaseSprite>();
                spotlightSprite.spriteAnimator.ignoreTimeScale = true;
                spotlightSprite.spriteAnimator.Play();
                tk2dSpriteAnimator whooshAnimator = spotlightSprite.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
                whooshAnimator.ignoreTimeScale = true;
                whooshAnimator.Play();
                Pixelator.Instance.CustomFade(0.6f, 0f, Color.white, Color.black, 0.1f, 0.5f);
                Pixelator.Instance.LerpToLetterbox(0.35f, 0.8f);
                BraveInput.AllowPausedRumble = true;
                p.DoVibration(Vibration.Time.Normal, Vibration.Strength.Hard);
                
                if (p.OverrideAnimationLibrary != null)
                {
                    p.OverrideAnimationLibrary = null;
                    PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ResetOverrideAnimationLibrary", p, new object[] { });
                    GameObject effect = (GameObject)BraveResources.Load("Global VFX/VFX_BulletArmor_Death", ".prefab");
                    p.PlayEffectOnActor(effect, Vector3.zero, true, false, false);
                }
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    float t = elapsed / duration;
                    GameManager.Instance.MainCameraController.OverridePosition = Vector3.Lerp(cameraStartPosition, cameraEndPosition, t);
                    p.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    spotlightSprite.color = new Color(1f, 1f, 1f, t);
                    Pixelator.Instance.saturation = Mathf.Clamp01(1f - t);
                    yield return null;
                }
                spotlightSprite.color = Color.white;
                yield return base.StartCoroutine(InvariantWait(0.4f, p));
                Transform clockhairTransform = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("Clockhair", ".prefab"))).transform;
                ClockhairController clockhair = clockhairTransform.GetComponent<ClockhairController>();
                elapsed = 0f;
                duration = clockhair.ClockhairInDuration;
                Vector3 clockhairTargetPosition = p.CenterPosition;
                Vector3 clockhairStartPosition = clockhairTargetPosition + new Vector3(-20f, 5f, 0f);
                clockhair.renderer.enabled = false;
                clockhair.spriteAnimator.Play("clockhair_intro");
                clockhair.hourAnimator.Play("hour_hand_intro");
                clockhair.minuteAnimator.Play("minute_hand_intro");
                clockhair.secondAnimator.Play("second_hand_intro");
                bool hasWobbled = false;
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    float t2 = elapsed / duration;
                    float smoothT = Mathf.SmoothStep(0f, 1f, t2);
                    Vector3 currentPosition = Vector3.Slerp(clockhairStartPosition, clockhairTargetPosition, smoothT);
                    clockhairTransform.position = currentPosition.WithZ(0f);
                    if (t2 > 0.5f)
                    {
                        clockhair.renderer.enabled = true;
                        clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                    if (t2 > 0.75f)
                    {
                        clockhair.hourAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.minuteAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.secondAnimator.GetComponent<Renderer>().enabled = true;
                        clockhair.hourAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                        clockhair.minuteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                        clockhair.secondAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    }
                    if (!hasWobbled && clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1)
                    {
                        clockhair.spriteAnimator.Play("clockhair_wobble");
                        hasWobbled = true;
                    }
                    clockhair.sprite.UpdateZDepth();
                    p.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    yield return null;
                }
                if (!hasWobbled)
                {
                    clockhair.spriteAnimator.Play("clockhair_wobble");
                }
                clockhair.SpinToSessionStart(clockhair.ClockhairSpinDuration);
                elapsed = 0f;
                duration = clockhair.ClockhairSpinDuration + clockhair.ClockhairPauseBeforeShot;
                while (elapsed < duration)
                {
                    if (GameManager.INVARIANT_DELTA_TIME == 0f)
                    {
                        elapsed += 0.05f;
                    }
                    elapsed += GameManager.INVARIANT_DELTA_TIME;
                    clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    yield return null;
                }
              
                BraveInput.AllowPausedRumble = false;
                UnityEngine.Object.Destroy(spotlightSprite.gameObject);
                UnityEngine.Object.Destroy(clockhair.gameObject);

                Pixelator.Instance.saturation = 1f;
                Pixelator.Instance.FadeToColor(0.25f, Pixelator.Instance.FadeColor, true, 0f);
                Pixelator.Instance.LerpToLetterbox(1f, 0.25f);
                Pixelator.Instance.DoFinalNonFadedLayer = false;

                p.CurrentInputState = PlayerInputState.AllInput;                
                p.IsVisible = true;
                p.ToggleGunRenderers(true, "death");
                p.ToggleHandRenderers(true, "death");
                
                PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "ToggleAttachedRenderers", p, new object[] { true });

                p.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
                GameManager.Instance.DungeonMusicController.ResetForNewFloor(GameManager.Instance.Dungeon);
                if (p.CurrentRoom != null)
                {
                    GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(p.CurrentRoom);
                }
                GameManager.Instance.ForceUnpause();
                GameManager.Instance.PreventPausing = false;
                BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                if (wasPitFalling)
                {
                    PlanetsideReflectionHelper.InvokeMethod(typeof(PlayerController), "PitRespawn", p, new object[] { Vector2.zero });
                }
                p.healthHaver.IsVulnerable = true;
                p.healthHaver.TriggerInvulnerabilityPeriod(-1f);
                OtherTools.Notify("Just Kidding :)", "Just Kidding :)", "Planetside/Resources/PerkThings/somethingtoDoWithThrownGuns", UINotificationController.NotificationColor.PURPLE, true);


                yield break;
            }

         * 
         */
    }
}

