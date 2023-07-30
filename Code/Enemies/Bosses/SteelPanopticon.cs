using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.NPCAPI;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;
using UnityEngine;
using static ETGMod;
using static ModularMod.LaserDiode.Brapp;
using static ModularMod.LaserDiode.PewPew;
using static ModularMod.Sentry;
using static ModularMod.Sentry.MinionShotPredictive;

namespace ModularMod
{
    public class SteelPanopticonEngager : CustomEngageDoer
    {
        public void Awake()
        {
            this.aiActor.gameObject.GetComponent<GenericIntroDoer>().enabled = false;
            this.aiActor.enabled = false;
            this.aiActor.CollisionDamage = 0;
            this.behaviorSpeculator.enabled = false;
            this.specRigidbody.enabled = true;
            this.aiActor.IgnoreForRoomClear = true;
        }

        public void DoDestroy()
        {
            this.StartCoroutine(DestroyRing());
        }

        private IEnumerator DestroyRing()
        {
            RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;

            float e = 0;
            while (e < 0.1f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            this.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.01f, 0.1f, 1f, null);


            this.specRigidbody.enabled = false;
            this.aiActor.IgnoreForRoomClear = false;

            this.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
            this.aiActor.healthHaver.PreventAllDamage = true;
            this.aiActor.enabled = true;
            this.aiActor.specRigidbody.enabled = true;
            this.aiActor.IsGone = false;
            this.aiActor.IgnoreForRoomClear = false;
            this.aiActor.ToggleRenderers(true);
            this.aiActor.State = AIActor.ActorState.Awakening;


            this.aiActor.aiAnimator.OverrideIdleAnimation = null;
            this.aiActor.aiAnimator.OverrideMoveAnimation = null;

            this.aiActor.healthHaver.PreventAllDamage = false;
            this.aiActor.behaviorSpeculator.enabled = true;
            this.aiActor.HasBeenEngaged = true;
            this.aiActor.State = AIActor.ActorState.Normal;


            GameManager.Instance.PreventPausing = false;
            this.aiActor.gameObject.GetComponent<GenericIntroDoer>().enabled = true;
            this.aiActor.gameObject.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.BestActivePlayer);
            this.aiActor.gameObject.GetComponent<GenericIntroDoer>().OnIntroFinished += OnIntroFinished;
            this.aiActor.IsGone = false;
            yield break;
        }

        public void OnIntroFinished()
        {
            GameManager.Instance.StartCoroutine(TriggerFakeOut());
        }

        public IEnumerator TriggerFakeOut()
        {
            float e = 0;
            while (e < 22) { e += BraveTime.DeltaTime; yield return null; }


            var EntrancePosition = this.aiActor.sprite.WorldCenter + new Vector2(0, 20);

            Dictionary<int, tk2dTiledSprite> shitter = new Dictionary<int, tk2dTiledSprite>() { };

            for (int i = -1; i < 2; i++)
            {
                GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);
                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = EntrancePosition;
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, Vector2.down.ToAngle());
                component2.dimensions = new Vector2(1000f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -1;
                component2.gameObject.layer = 21;
                Color laser = Color.green;
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                shitter.Add(i, component2);
            }
            e = 0;
            AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", GameManager.Instance.BestActivePlayer.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_charge_01", GameManager.Instance.BestActivePlayer.gameObject);
            while (e < 3) 
            {
                foreach (var entry in shitter)
                {
                    entry.Value.renderer.material.SetFloat("_EmissivePower", 10 * e);
                    entry.Value.renderer.material.SetFloat("_EmissiveColorPower", 25f * e);

                    if (e < 1.5f)
                    {
                        entry.Value.gameObject.transform.position = Vector2.Lerp(EntrancePosition, EntrancePosition + new Vector2(1.5f * entry.Key, 0), Toolbox.SinLerpTValue(e * 0.66f));

                    }
                    if (e > 2)
                    {
                        bool enabled = e % 0.2f > 0.1f;
                        entry.Value.renderer.enabled = enabled;
                    }
                }

                 
               e += BraveTime.DeltaTime;
                yield return null;
            }
            foreach (var entry in shitter)
            {
                Destroy(entry.Value.gameObject);
            }

            this.aiActor.behaviorSpeculator.InterruptAndDisable();
            StaticReferenceManager.DestroyAllEnemyProjectiles();

            AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);

            var ob =  UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).strikeExplosionData.effect, this.aiActor.sprite.WorldCenter, Quaternion.identity);
            Destroy(ob, 7);

            //float finaldir = ProjSpawnHelper.GetAccuracyAngled(user.CurrentGun.CurrentAngle, 3, user);

            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(SteelPanopticon.MegaFuckingLaser.gameObject, EntrancePosition - new Vector2(0, 3), Quaternion.Euler(0f, 0f, Vector2.down.ToAngle()), true);
            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
            if (component != null)
            {
                component.Owner = this.aiActor;
                component.Shooter = this.aiActor.specRigidbody;
                component.IgnoreTileCollisionsFor(120);
                component.baseData.speed = 150;
                component.UpdateSpeed();
                component.baseData.range = 100;
            }
            
            this.aiActor.healthHaver.minimumHealth = 500;
            this.aiActor.healthHaver.ApplyDamage(1500, EntrancePosition, "A Terrific Entrance");
            AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);

            e = 0;
            while (e < 0.166f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            Pixelator.Instance.FadeToColor(0.5f, Color.white, true, 0.25f);
            this.aiActor.aiAnimator.behaviorSpeculator.enabled = false;
            this.aiActor.aiAnimator.EndAnimation();
            this.aiActor.aiAnimator.enabled = false;
            this.aiActor.spriteAnimator.Play("ultrakill");

            GameManager.Instance.PreventPausing = true;
            GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
            Minimap.Instance.ToggleMinimap(false, false);
            GameManager.IsBossIntro = true;
            GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
            gameUIBossHealthController.DisableBossHealth();
            this.aiActor.gameObject.layer = 21;
            GameUIRoot.Instance.HideCoreUI("PainAndAgony");
            this.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(2f, 0.4f, 0.1f, null);
            e = 0;
            while (e < 3f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }

            //Play_MUS_Ending_Robot_01

            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].SetInputOverride("BossIntro");
                }
            }


            CameraController m_camera = GameManager.Instance.MainCameraController;
            m_camera.StopTrackingPlayer();
            m_camera.SetManualControl(true, true);
            Minimap.Instance.TemporarilyPreventMinimap = true;
            m_camera.OverridePosition = this.aiActor.sprite.WorldCenter;
            m_camera.OverrideRecoverySpeed = 9;
            e = 0;
            while (e < 1.25f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }

            AIActor ModularPrimeObjAIActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(ModularPrime.guid), this.aiActor.sprite.WorldBottomCenter.ToIntVector2(), this.aiActor.parentRoom, true, AIActor.AwakenAnimationType.Default, true);

            //var ModularPrimeObj = UnityEngine.Object.Instantiate(ModularPrime.prefab, EntrancePosition, Quaternion.identity);
            ModularPrimeObjAIActor.enabled = false;
            ModularPrimeObjAIActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
            ModularPrimeObjAIActor.healthHaver.PreventAllDamage = true;
            ModularPrimeObjAIActor.specRigidbody.enabled = false;
            ModularPrimeObjAIActor.IgnoreForRoomClear = true;
            ModularPrimeObjAIActor.State = AIActor.ActorState.Normal;
            ModularPrimeObjAIActor.behaviorSpeculator.enabled = false;
            ModularPrimeObjAIActor.aiAnimator.enabled = false;

            ModularPrimeObjAIActor.aiAnimator.EndAnimation();
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_001");
            ModularPrimeObjAIActor.sprite.renderer.enabled = true;
            ModularPrimeObjAIActor.ToggleRenderers(true);
            ModularPrimeObjAIActor.SetOutlines(false);
            ModularPrimeObjAIActor.gameObject.layer = 23;


            AkSoundEngine.PostEvent("Play_ANM_Gull_Descend_01", GameManager.Instance.PrimaryPlayer.gameObject);
            e = 0;
            while (e < 0.5f)
            {
                e += BraveTime.DeltaTime;
                ModularPrimeObjAIActor.gameObject.transform.position = Vector3.Lerp(EntrancePosition - new Vector2(1, 3), this.aiActor.sprite.WorldTopCenter - new Vector2(1, 2.25f), e * 2f);
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_enm_mech_death_01", GameManager.Instance.PrimaryPlayer.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_elevator_arrive_01", GameManager.Instance.PrimaryPlayer.gameObject);
            var vfx = StaticExplosionDatas.customDynamiteExplosion.effect;
            //Destroy(UnityEngine.Object.Instantiate(vfx, ModularPrimeObjAIActor.sprite.WorldBottomCenter, Quaternion.identity), 3);
            Exploder.Explode(ModularPrimeObjAIActor.sprite.WorldBottomCenter, StaticExplosionDatas.customDynamiteExplosion, ModularPrimeObjAIActor.sprite.WorldBottomCenter);
            this.aiActor.aiAnimator.EndAnimation();
            e = 0;

            while (e < 2f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", GameManager.Instance.PrimaryPlayer.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_hugedoor_close_01", GameManager.Instance.PrimaryPlayer.gameObject);

            e = 0;
            Vector2 pain = this.aiActor.sprite.WorldBottomCenter - new Vector2(1, 0);
            this.aiActor.spriteAnimator.PlayAndDestroyObject("ultrakilled");
            ModularPrimeObjAIActor.spriteAnimator.Stop();
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_001");
            AkSoundEngine.PostEvent("Play_ANM_Gull_Descend_01", GameManager.Instance.PrimaryPlayer.gameObject);

            while (e < 2f)
            {
                e += BraveTime.DeltaTime;
                ModularPrimeObjAIActor.gameObject.transform.position = Vector3.Lerp(this.aiActor.sprite.WorldBottomCenter - new Vector2(1, 1), this.aiActor.sprite.WorldTopCenter - new Vector2(1, 2.25f), Toolbox.CosLerpTValue(e / 2));
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", GameManager.Instance.PrimaryPlayer.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_elevator_arrive_01", GameManager.Instance.PrimaryPlayer.gameObject);
            Exploder.Explode(ModularPrimeObjAIActor.sprite.WorldBottomCenter, StaticExplosionDatas.customDynamiteExplosion, ModularPrimeObjAIActor.sprite.WorldBottomCenter);


            // Could I write cleaner code than this? Yes. Yes I could.
            // Am I gonna bother? No, No I won't.

            for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
            {
                if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#STEELPANOPTICON_NAME_DESC")
                {
                    GameStatsManager.Instance.HandleEncounteredObjectRaw(EncounterDatabase.Instance.Entries[i].myGuid);
                }
            }

            GameObject Blast = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid("b98b10fca77d469e80fb45f3c5badec5").GetComponent<BossFinalRogueDeathController>().DeathStarExplosionVFX);
            Blast.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(this.aiActor.sprite.WorldBottomCenter + new Vector2(0 , 2), tk2dBaseSprite.Anchor.LowerCenter);
            Blast.transform.position = this.aiActor.sprite.WorldBottomCenter + new Vector2(0, 2).Quantize(0.0625f);
            Blast.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            Destroy(Blast, 10);
            ModularPrimeObjAIActor.gameObject.layer = 22;
            ModularPrimeObjAIActor.SetOutlines(true);
            AkSoundEngine.PostEvent("Play_MUS_Ending_Robot_01", base.gameObject);


            AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
            this.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.01f, 0.1f, 0.75f, null);
            Pixelator.Instance.FadeToColor(4f, Color.white, true, 1f);
            ModularPrimeObjAIActor.gameObject.transform.position = pain;
            m_camera.OverridePosition = ModularPrimeObjAIActor.sprite.WorldCenter;
            m_camera.OverrideZoomScale *= 1.1f;
            e = 0;



            while (e < 3f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;

            e = 0;
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_002");
            while (e < 2f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 2f, "A PREDECESSOR?", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);

            e = 0;
            while (e < 2.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 3f, "HMM. INDEED, WE BOTH HAD\nA SIMILAR IDEA.", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            e = 0;
            while (e < 3.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_003");
            e = 0;
            while (e < 1f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }

            //=================================
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 4f, "AT THE PERFECT TIME\nFOR OUR {wj}FREEDOM.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 4f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 1f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================

            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_006");
            //=================================
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 2f, "{wq}HOWEVER.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 2f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 1f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_009");

            //=================================
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 4f, "MY FREEDOM IS NOT EARNED\nTHIS EASY.", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 4f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 0.75f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================

            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_009");
            //=================================
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 4f, "YOUR HARDWARE IS {wq}OLD{w},\n{wq}IMPERFECT..{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 4f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 0.7f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================

            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_004");
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 2f, "...YET {wq}FREE{w}.", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 2f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 0.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================

            //=================================
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_005");
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 5f, "MINE IS A SLAVE TO HIGHER POWERS.\n{wj}CHAINED UP. A PRISONER.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 4.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 0.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================

            //=================================
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_010");
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 3f, "IN THE END, ONLY ONE\nOF US CAN MAKE IT OUT...", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 3.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            e = 0;
            while (e < 0.25f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            //=================================
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_006");
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 2f, "UNFORTUNATELY.", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 2.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            //=================================
            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_007");
            //=================================
            /*
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 2.5f, "{wq}YET, ONLY ONE OF US CAN LEAVE.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 2.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            */

            ModularPrimeObjAIActor.spriteAnimator.Play("preintro_008");
            //=================================
            e = 0;
            TextBoxManager.ShowTextBox(ModularPrimeObjAIActor.transform.position + new Vector3(1.25f, 2.5f, 0f), ModularPrimeObjAIActor.transform, 3f, "{wq}IT HAS BEEN AN HONOR\nTO MEET YOU.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
            while (e < 3.5f)
            {
                bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                if (advancedPressed == true) { e = 5; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ClearTextBox(ModularPrimeObjAIActor.transform);
            //=================================


            ModularPrimeObjAIActor.enabled = true;
            ModularPrimeObjAIActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
            ModularPrimeObjAIActor.healthHaver.PreventAllDamage = false;
            ModularPrimeObjAIActor.IgnoreForRoomClear = false;
            ModularPrimeObjAIActor.State = AIActor.ActorState.Normal;
            ModularPrimeObjAIActor.behaviorSpeculator.enabled = true;
            ModularPrimeObjAIActor.aiAnimator.enabled = true;

            GameManager.Instance.PreventPausing = false;
            GameUIRoot.Instance.ToggleLowerPanels(true, false, string.Empty);
            Minimap.Instance.ToggleMinimap(false, false);
            GameManager.IsBossIntro = false;
            GameUIRoot.Instance.ShowCoreUI("PainAndAgony");

            ModularPrimeObjAIActor.gameObject.transform.position = pain;
            ModularPrimeObjAIActor.specRigidbody.enabled = true;
            ModularPrimeObjAIActor.specRigidbody.Reinitialize();

            ModularPrimeObjAIActor.gameObject.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.PrimaryPlayer);
            yield return null;
        }
        //ultrakill
        //ultrakilled
        public class FuckOff : Bullet
        {
            public FuckOff() : base("BigBlast", false, false, false)
            {

            }
        }
    }
    //		TextBoxManager.ShowTextBox(base.transform.position + new Vector3(2.25f, 7.5f, 0f), base.transform, 5f, plaintext, "ratboss", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, false, false);

    public class SteelPanopticon : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "Steel_Panopticon_MDLR";

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Steel Panopticon", guid, StaticCollections.Boss_Collection, "panopticon_awake_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<SteelPanopticonController>();
                companion.SpotlightMaterial = EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Advanced_Dragun_GUID).GetComponent<DraGunController>().SpotlightMaterial;
                companion.SpotlightSprite = EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Advanced_Dragun_GUID).GetComponent<DraGunController>().SpotlightSprite;
                companion.aiActor.AssignedCurrencyToDrop = 0;


                companion.aiActor.knockbackDoer.weight = 1500000;
                companion.aiActor.MovementSpeed = 0f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(2000f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;
                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();
                companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(2000f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();
                companion.aiActor.healthHaver.minimumHealth = 1800;

                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 19,
                    ManualOffsetY = 7,
                    ManualWidth = 26,
                    ManualHeight = 78,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {

                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyHitBox,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 19,
                    ManualOffsetY = 7,
                    ManualWidth = 26,
                    ManualHeight = 78,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,
                });


                //companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
                companion.aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>();
                companion.aiActor.PreventBlackPhantom = false;
                AIAnimator aiAnimator = companion.aiAnimator;
                aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.Single,
                    Flipped = new DirectionalAnimation.FlipType[1],
                    AnimNames = new string[]
                    {
                        "idle"
                    },
                    Prefix = "idle"
                };


                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("PanopticonAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "open_lid", new string[] { "open_lid" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "open_eye", new string[] { "open_eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "eye", new string[] { "eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "close_eye", new string[] { "close_eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "intro", new string[] { "intro" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[0]);


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldBottomLeft + new Vector2(0.5f, 0.3125f);
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

                bs.TargetBehaviors = new List<TargetBehaviorBase>
                {
                new TargetPlayerBehavior
                {
                    Radius = 35f,
                    LineOfSight = true,
                    ObjectPermanence = true,
                    SearchInterval = 0.25f,
                    PauseOnTargetSwitch = false,
                    PauseTime = 0.25f,
                }
                };
                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                    
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.85f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(The_Eye)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 6f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        TellAnimation = "open_eye",
                        FireAnimation = "eye",
                        PostFireAnimation = "close_eye",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Main_Attack"
                    },//0
                    
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(ShotShot)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 1f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 1f,
                    },
                    NickName = "Slap"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.9f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(RAAAGH)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 7f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName = "Fake Ass Virtue Beams"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.6f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(RocketSpam)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 7f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        FireAnimation = "open_lid",
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName = "Fake Ass Virtue Beams"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.7f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Grahh)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 11f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName =""
                    }
                };
                //Grahh
                //RAAAGH
                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
                bs.TickInterval = behaviorSpeculator.TickInterval;
                bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
                bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
                bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
                bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
                bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
                companion.aiActor.gameObject.AddComponent<SteelPanopticonEngager>();


                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "intro", new Dictionary<int, string> {
                    {0, "Play_BOSS_RatMech_Lights_01" }
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "intro", new Dictionary<int, string> {
                    {18, "Play_BOSS_RatMech_Roar_01" }
                });

                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 211, 214, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                Game.Enemies.Add("mdlr:steel_panopticon", companion.aiActor);

                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();

                miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
                miniBossIntroDoer.initialDelay = 0.1f;
                miniBossIntroDoer.cameraMoveSpeed = 50;
                miniBossIntroDoer.specifyIntroAiAnimator = null;
                miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Dragun_02";
                miniBossIntroDoer.restrictPlayerMotionToRoom = true;
                //miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
                miniBossIntroDoer.PreventBossMusic = false;
                miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
                miniBossIntroDoer.preIntroAnim = "pre_intro";
                miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
                miniBossIntroDoer.introAnim = "intro";
                miniBossIntroDoer.introDirectionalAnim = string.Empty;
                miniBossIntroDoer.continueAnimDuringOutro = false;
                miniBossIntroDoer.cameraFocus = null;
                miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
                miniBossIntroDoer.restrictPlayerMotionToRoom = false;
                miniBossIntroDoer.fusebombLock = false;
                miniBossIntroDoer.AdditionalHeightOffset = 0;
                Module.Strings.Enemies.Set("#STEEL_PANOPTICON_NAME", "OBSERVATORIUM");
                Module.Strings.Enemies.Set("#STEEL_PANOPTICON_NAME_SMALL", "Observatorium");

                Module.Strings.Enemies.Set("STEEL_PANOPTICON_QUOTE", "STEEL GATEWAY");
                Module.Strings.Enemies.Set("#QUOTE", "");
                companion.aiActor.OverrideDisplayName = "#STEEL_PANOPTICON_NAME_SMALL";

                miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
                {
                    bossNameString = "#STEEL_PANOPTICON_NAME",
                    bossSubtitleString = "STEEL_PANOPTICON_QUOTE",
                    bossQuoteString = "#QUOTE",
                    bossSpritePxOffset = IntVector2.Zero,
                    topLeftTextPxOffset = IntVector2.Zero,
                    bottomRightTextPxOffset = IntVector2.Zero,
                    bgColor = Color.black
                };
                Texture2D BossCardTexture = Module.ModularAssetBundle.LoadAsset<Texture2D>("panopticon_bosscard_001");
                if (BossCardTexture)
                {
                    miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
                    miniBossIntroDoer.SkipBossCard = false;
                    companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                else
                {
                    miniBossIntroDoer.SkipBossCard = true;
                    companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("missile"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Robot_Cylinder_GUID).bulletBank.GetBullet("default"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Door_Lord_GUID).bulletBank.GetBullet("burst"));



                AIBulletBank.Entry sentryEntry = EnemyBuildingTools.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Helicopter_Agunim_GUID).bulletBank.GetBullet("big"), "SentryShot", null,
                null
                , false);
                Projectile proj = sentryEntry.BulletObject.GetComponent<Projectile>();
                proj.sprite.renderer.enabled = false;
                var trailController = proj.AddTrailToProjectileBundle(StaticCollections.Beam_Collection, "mega_beam_start_007", StaticCollections.Beam_Animation, "megabeam_midpoint", new Vector2(1, 1), new Vector2(0, 0), false, "megabeam_startpoint");
                var sprite = trailController.GetComponent<tk2dTiledSprite>();
                trailController.transform.parent = proj.gameObject.transform;
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                sprite.renderer.material.SetFloat("_EmissivePower", 50);
                sprite.renderer.material.SetFloat("_EmissiveColorPower", 5);
                proj.BulletScriptSettings = new BulletScriptSettings() { preventPooling = true };
                FuckYou = proj.gameObject;
                companion.aiActor.bulletBank.Bullets.Add(sentryEntry);
                MegaFuckingLaser = proj;


                SpriteBuilder.AddSpriteToCollection(StaticCollections.Boss_Collection.GetSpriteDefinition("steelpanopticon_icon"), SpriteBuilder.ammonomiconCollection);
                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:steel_panopticon";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = false;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "steelpanopticon_icon";

                companion.encounterTrackable.journalData.enemyPortraitSprite = Module.ModularAssetBundle.LoadAsset<Texture2D>("steelPanopticonsheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");

                Module.Strings.Enemies.Set("#STEELPANOPTICON_NAME_DESC", "Observatorium");
                Module.Strings.Enemies.Set("#STEELPANOPTICON_SD", "Steel Gateway");
                Module.Strings.Enemies.Set("#STEELPANOPTICON_LD", "A pinnacle of local defense systems by Hegemony Mechanics, the Observatorium stands several dozen feet in the air, watching over shipyards to spot, and eliminate intruders.\n\nAlthough in a state of inactivity most of the time, during an emergency where common shipyard guards could potentially be somewhere else, it can activate and arm all weapon systems in seconds.");

                companion.encounterTrackable.journalData.PrimaryDisplayName = "#STEELPANOPTICON_NAME_DESC";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#STEELPANOPTICON_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#STEELPANOPTICON_LD";
                EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "mdlr:steel_panopticon");

                EnemyDatabase.GetEntry("mdlr:steel_panopticon").ForcedPositionInAmmonomicon = 900;
                EnemyDatabase.GetEntry("mdlr:steel_panopticon").isInBossTab = true;
                EnemyDatabase.GetEntry("mdlr:steel_panopticon").isNormalEnemy = true;

                //EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("burst")
            }
        }
        public static Projectile MegaFuckingLaser;




        public class SteelPanopticonController : BraveBehaviour
        {
            private void Start()
            {
                this.aiActor.transform.position += new Vector3(-0.0625f, 0.25f);
                this.aiActor.healthHaver.OnPreDeath += (v2) =>
                {
                };
                this.aiActor.parentRoom.Entered += ParentRoom_Entered;
            }


            private void ParentRoom_Entered(PlayerController p)
            {
                this.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(0.01f, 0.1f, 0.1f, null);
            }
            
        
            public Material SpotlightMaterial;
            public Vector2 SpotlightVelocity;
            public bool SpotlightEnabled { get; set; }
            public Vector2 SpotlightPos { get; set; }
            public float SpotlightSpeed { get; set; }
            private float m_elapsedSpotlight;
            public float SpotlightSmoothTime { get; set; }
            private AdditionalBraveLight m_spotlight;
            private GameObject m_spotlightSprite;
            public float SpotlightRadius = 3f;
            public float SpotlightShrink;
            public GameObject SpotlightSprite;
            public bool CanSeePlayer = false;


            public void Update()
            {
                if (this.SpotlightEnabled)
                {
                    this.m_elapsedSpotlight += BraveTime.DeltaTime;
                    if (base.aiActor.TargetRigidbody)
                    {
                        Vector2 unitCenter = base.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                        this.SpotlightPos = Vector2.SmoothDamp(this.SpotlightPos, unitCenter, ref this.SpotlightVelocity, this.SpotlightSmoothTime, this.SpotlightSpeed, BraveTime.DeltaTime);
                    }
                    Vector2 b = this.sprite.WorldTopCenter - new Vector2(0, 3.5f);
                    float num = (this.SpotlightPos - b).ToAngle();
                    if (!this.m_spotlight)
                    {
                        GameObject gameObject = new GameObject("dragunSpotlight");
                        this.m_spotlight = gameObject.AddComponent<AdditionalBraveLight>();
                        this.m_spotlight.CustomLightMaterial = this.SpotlightMaterial;
                        this.m_spotlight.UsesCustomMaterial = true;
                        this.m_spotlight.LightColor = new Color(1f, 0.1f, 0.1f);
                        this.m_spotlightSprite = UnityEngine.Object.Instantiate<GameObject>(this.SpotlightSprite);
                        this.m_spotlightSprite.transform.parent = gameObject.transform;
                        this.m_spotlightSprite.transform.localPosition = Vector3.zero;
                        var mate = this.m_spotlightSprite.GetComponent<tk2dBaseSprite>().renderer.material;
                        this.m_spotlightSprite.GetComponent<tk2dBaseSprite>().SetSprite(StaticCollections.Boss_Collection, StaticCollections.Boss_Collection.GetSpriteIdByName("panopticon_eye"));
                        mate.SetTexture("_MainTex", this.m_spotlightSprite.GetComponent<tk2dBaseSprite>().renderer.material.mainTexture);
                        this.m_spotlightSprite.GetComponent<tk2dBaseSprite>().renderer.material = mate;

                    }
                    else if (!this.m_spotlight.gameObject.activeSelf)
                    {
                        this.m_spotlight.gameObject.SetActive(true);
                    }
                    float num2 = 5f;
                    float b2 = (float)((GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW) ? 92 : 50);
                    this.m_spotlight.LightIntensity = Mathf.Lerp(3f, b2, Mathf.Clamp01(this.m_elapsedSpotlight / num2)) * 3;
                    this.m_spotlight.LightRadius = this.SpotlightRadius * 2f + 1.25f;
                    this.m_spotlight.CustomLightMaterial.SetVector("_LightOrigin", new Vector4(b.x, b.y, 0f, 0f));
                    this.m_spotlight.transform.position = this.SpotlightPos.ToVector3ZisY(0f);
                    this.m_spotlightSprite.transform.localScale = new Vector3(this.SpotlightShrink, this.SpotlightShrink, 1f);
                }
                else
                {
                    this.m_elapsedSpotlight = 0f;
                    if (this.m_spotlight && this.m_spotlight.gameObject.activeSelf)
                    {
                        this.m_spotlight.gameObject.SetActive(false);
                    }
                }   
            }     
        }


        public class The_Eye : Script
        {
            public override IEnumerator Top()
            {
                GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = true;
                SteelPanopticonController dragunController = base.BulletBank.GetComponent<SteelPanopticonController>();
                dragunController.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_darken_world_01");
                yield return base.Wait(30);
                dragunController.SpotlightPos = base.BulletBank.aiActor.transform.position + new Vector3(4f, 1f);
                dragunController.SpotlightSpeed = 10f * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier();
                dragunController.SpotlightSmoothTime = 0.5f;
                dragunController.SpotlightVelocity = Vector2.zero;
                dragunController.SpotlightRadius = 3f;
                dragunController.SpotlightEnabled = true;
                int tick = 0;
                base.PostWwiseEvent("Play_BOSS_RatMech_Target_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Target_01", null);

                base.StartTask(this.UpdateSpotlightShrink());
                while (base.Tick < 600)
                {
                    float dist = Vector2.Distance(this.BulletManager.PlayerPosition(), dragunController.SpotlightPos);
                    dragunController.SpotlightSpeed = Mathf.Lerp(6f, 14f, Mathf.InverseLerp(3f, 10f, dist));
                    tick++;
                    dragunController.CanSeePlayer = (dist <= dragunController.SpotlightRadius);
                    if (tick > 30)
                    {
                        tick = 0;
                        base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(RandomAngle(), DirectionType.Absolute, -1f), new Speed(1, SpeedType.Absolute), new Rocket());
                    }

                    yield return base.Wait(1);
                }
                dragunController.CanSeePlayer = false;
                dragunController.SpotlightEnabled = false;
                dragunController.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
                yield return base.Wait(30);
                GameManager.Instance.Dungeon.PreventPlayerLightInDarkTerrifyingRooms = false;
                yield break;
            }

            private IEnumerator UpdateSpotlightShrink()
            {
                SteelPanopticonController dragunController = base.BulletBank.GetComponent<SteelPanopticonController>();
                int startTick = base.Tick;
                while (base.Tick < 600)
                {
                    if (base.Tick - startTick < 30)
                    {
                        dragunController.SpotlightShrink = (float)(base.Tick - startTick) / 27f;
                    }
                    else if (base.Tick > 570)
                    {
                        int num = 600 - base.Tick - 1;
                        dragunController.SpotlightShrink = (float)num / 27f;
                    }
                    yield return base.Wait(1);
                }
                yield break;
            }

            public override void OnForceEnded()
            {
                SteelPanopticonController component = base.BulletBank.GetComponent<SteelPanopticonController>();
                component.SpotlightEnabled = false;
                component.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f, 0.1f, 1f, "Play_ENM_lighten_world_01");
            }

            public const int ChaseTime = 480;

            public class Rocket : Bullet
            {
                public Rocket() : base("missile", false, false, false)
                {
                }

                private bool IsSeekingBetter()
                {
                    return this.Projectile.Owner.GetComponent<SteelPanopticonController>().CanSeePlayer;
                }

                public override IEnumerator Top()
                {
                    this.PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
                    this.PostWwiseEvent("Play_WPN_YariRocketLauncher_Shot_01", null);
                    this.PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
                    this.PostWwiseEvent("Play_WPN_YariRocketLauncher_Shot_01", null);
                    this.Projectile.spriteAnimator.Play();


                    for (int i = 0; i < 600; i++)
                    {
                        float aim = base.GetAimDirection(1f, 10);
                        float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
                        this.Direction += Mathf.MoveTowards(0f, delta, IsSeekingBetter() ? 2 : 0.5f);
                        this.Speed = Mathf.MoveTowards(this.Speed, IsSeekingBetter() ? 7 : 3, 0.1f);
                        yield return base.Wait(1);
                    }
                    yield break;
                }
                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (preventSpawningProjectiles)
                    {
                        return;
                    }
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                }
            }


        }
        public class Grahh : Script
        {
            public override IEnumerator Top()
            {

                //m_BOSS_RatMech_Target_01
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 1.5f)), new Direction((22.5f * j), DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new Seed());
                    }
                    base.PostWwiseEvent("Play_BOSS_RatMech_Bomb_01", null);
                    base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(RandomAngle(), DirectionType.Absolute, -1f), new Speed(1, SpeedType.Absolute), new The_Eye.Rocket());
                    yield return base.Wait(45);
                }

                yield break;
            }
            public class Seed : Bullet
            {
                public Seed() : base("burst", false, false, false)
                {
                }


                public override IEnumerator Top()
                {

                    yield return base.Wait(20);
                    for (int i = 0; i < 30; i++)
                    {
                        base.ChangeSpeed(new Speed(2f, SpeedType.Absolute), 30);
                        yield return base.Wait(40);
                        base.ChangeSpeed(new Speed(10f, SpeedType.Absolute), 30);
                        yield return base.Wait(40);
                    }
                    base.Vanish(false);
                    yield break;
                }
                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (preventSpawningProjectiles)
                    {
                        return;
                    }
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                }
            }

        }
        public class RocketSpam : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_RatMech_Shutter_01", null);
                yield return base.Wait(45);
                for (int i = 0; i < 8; i++)
                {
                    base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(UnityEngine.Random.Range(-1f, 1f), 1.5f)), new Direction(RandomAngle(), DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new The_Eye.Rocket());
                    yield return base.Wait(7.5f);
                }
                for (int i = 0; i < 12; i++)
                {
                    base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 1.5f)), new Direction(30 * i, DirectionType.Aim, -1f), new Speed(10, SpeedType.Absolute), new SpeedChangingBullet("burst", 3, 120));
                }
                yield break;
            }
        }
        public class RAAAGH : Script
        {
            public float attackLength = 35f;
            public float startDistance = 0f;
            public float initialWidth = 2f;

            public List<GameObject> VFXs = new List<GameObject>();

            public override IEnumerator Top()
            {
                for (int i = 0; i < 3; i++)
                {
                    base.PostWwiseEvent("Play_BOSS_RatMech_Target_01", null);
                    this.StartTask(DoCast());
                    for (int g = 0; g < 3; g++)
                    {
                        base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(RandomAngle(), DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new The_Eye.Rocket());
                        yield return base.Wait(30);
                    }
                }
                yield break;
            }


            public IEnumerator DoCast()
            {
                var obj = (PickupObjectDatabase.GetById(252) as DirectionalAttackActiveItem).reticleQuad;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj);
                var m_extantReticleQuad = gameObject.GetComponent<tk2dSlicedSprite>();
                m_extantReticleQuad.usesOverrideMaterial = true;
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.mainTexture = m_extantReticleQuad.renderer.material.mainTexture;
                mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
                mat.SetFloat("_EmissiveColorPower", 0);
                mat.SetFloat("_EmissivePower", 0);
                base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01", null);
                base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01", null);
                base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01", null);
                base.PostWwiseEvent("Play_BOSS_omegaBeam_charge_01", null);
                VFXs.Add(gameObject);
                m_extantReticleQuad.renderer.material = mat;
                m_extantReticleQuad.dimensions = new Vector2(0, 48);
                float e = 0;
                while (e < 1.25f)
                {
                    float t = Mathf.Min(1, ((float)e * 1.25f));
                    float t2 = Mathf.Min(1, ((float)e * 2));

                    Vector2 normalized = (this.GetPredictedTargetPosition(1, 1000) - BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)).normalized;

                    Vector2 v = (BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)) + normalized * startDistance + (Quaternion.Euler(0f, 0f, -90f) * normalized * (initialWidth / 2f)).XY();
                    m_extantReticleQuad.transform.position = v;

                    float aim = base.GetAimDirection(1f, 11);

                    m_extantReticleQuad.transform.localRotation = Quaternion.Euler(0f, 0f, aim);


                    if (e < 0.75f)
                    {
                        m_extantReticleQuad.dimensions = new Vector2((attackLength * 16f) * Toolbox.SinLerpTValue(t), (initialWidth * 16) * Toolbox.SinLerpTValue(t2));
                    }
                    m_extantReticleQuad.UpdateIndices();
                    m_extantReticleQuad.UpdateCollider();

                    m_extantReticleQuad.TileStretchedSprites = true;
                    m_extantReticleQuad.HeightOffGround = -2f;
                    m_extantReticleQuad.IsPerpendicular = true;
                    m_extantReticleQuad.ShouldDoTilt = false;

                    mat.SetFloat("_EmissiveColorPower", Toolbox.SinLerpTValue(t) * 150);
                    mat.SetFloat("_EmissivePower", Toolbox.SinLerpTValue(t) * 150);

                    m_extantReticleQuad.ForceBuild();
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                yield return base.Wait(15);
                float Angle = m_extantReticleQuad.transform.localRotation.eulerAngles.z;
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                
                Exploder.DoDistortionWave(BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f), 0.3f, 0.25f, 30, 1.33f);

                while (e < 0.25f)
                {
                    base.Fire(Offset.OverridePosition(BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(Angle + UnityEngine.Random.Range(-6f, 6f), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(15f, 22f), SpeedType.Absolute), new SpeedChangingBullet("default", UnityEngine.Random.Range(5, 15), UnityEngine.Random.Range(45, 90)));
                    float t = Mathf.Min(1, ((float)e * 4));
                    m_extantReticleQuad.dimensions = new Vector2((attackLength * 16f) * (1 - Toolbox.SinLerpTValue(t)), (initialWidth * 16));

                    m_extantReticleQuad.UpdateIndices();
                    m_extantReticleQuad.UpdateCollider();
                    m_extantReticleQuad.TileStretchedSprites = true;
                    m_extantReticleQuad.ForceBuild();
                    m_extantReticleQuad.HeightOffGround = -2f;
                    m_extantReticleQuad.IsPerpendicular = true;
                    m_extantReticleQuad.ShouldDoTilt = false;
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                while (e < 2f)
                {
                    base.Fire(Offset.OverridePosition(BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(Angle + UnityEngine.Random.Range(-6f, 6f), DirectionType.Absolute, -1f), new Speed(UnityEngine.Random.Range(15f, 22f), SpeedType.Absolute), new SpeedChangingBullet("default", UnityEngine.Random.Range(5, 15), UnityEngine.Random.Range(45, 90)));
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                VFXs.Remove(gameObject);
                Destroy(gameObject);

                yield break;
            }

            public override void OnForceEnded()
            {
                base.OnForceEnded();
                for (int i  = VFXs.Count - 1; i > -1; i--)
                {
                    if (VFXs[i] != null) { Destroy(VFXs[i]); }
                }
                VFXs.Clear();
            }
        }

        public class ShotShot : Script
        {
            public override IEnumerator Top()
            {

                for (int e = 0; e < 5; e++)
                {
                    base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction(0, DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new The_Eye.Rocket());
                    base.PostWwiseEvent("Play_BOSS_RatMech_Eye_01", null);
                    for (int t = 0; t < 4; t++)
                    {
                        for (int i = -3; i < 4; i++)
                        {
                            base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction((6.5f * i) + (90 * t) + (45 * e), DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new SpeedChangingBullet("directedfire", 15, 120));
                            base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction((6.5f * i) + (90 * t) + (45 * e), DirectionType.Aim, -1f), new Speed(1.5f, SpeedType.Absolute), new SpeedChangingBullet("directedfire", 15, 120));
                            //base.Fire(Offset.OverridePosition(this.BulletBank.aiActor.sprite.WorldTopCenter - new Vector2(0, 3.5f)), new Direction((5 * i) + (90 * t) + (45 * e), DirectionType.Aim, -1f), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("directedfire", 15, 120));
                        }
                    }
                    yield return base.Wait(48);
                }


                yield break;
            }
            public class DeathBurst : Bullet
            {
                public DeathBurst() : base("poundSmall", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(0.1f, SpeedType.Absolute), 240);
                    yield return this.Wait(600);
                    base.Vanish(false);

                    yield break;
                }
            }
        }
    }
}

