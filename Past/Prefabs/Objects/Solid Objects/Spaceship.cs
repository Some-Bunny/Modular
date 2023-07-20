using Alexandria.PrefabAPI;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.Past.Prefabs.Objects.BigFuckOffDoor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class Spaceship
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Spaceship_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("spaceship_closed"));
            
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 206, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;

            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("SpaceshipAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            obj.AddComponent<BigAssSpaceship>();
            obj.AddComponent<FakeCorridor.Fuck_You_Youre_No_Longer_Perpendicular>();
            var t = obj.AddComponent<ImprovedAfterImage>();
            t.dashColor = new Color(0, 2, 2);
            t.shadowLifetime = 1;
            t.shadowTimeDelay = 0.01f;
            t.spawnShadows = false;
            obj.CreateFastBody(new IntVector2(202, 105), new IntVector2(11, 16));
            obj.CreateFastBody(new IntVector2(162, 63), new IntVector2(31, -121));
            obj.CreateFastBody(new IntVector2(144, 64), new IntVector2(40, 184));

            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("GiantSpaceShip_MDLR", obj);
        }

        public class BigAssSpaceship : MonoBehaviour
        {
            public tk2dBaseSprite sprite;
            public void Start()
            {
                sprite = this.GetComponent<tk2dBaseSprite>();
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "DoTakeOff" }, OnRecieveMessage);
            }
            public void OnRecieveMessage(GameObject obj, string message)
            {
                GameManager.Instance.StartCoroutine(EnterTheGungeon());
            }
            private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
            {
                Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
                return new Vector4(vector.x, vector.y, 0f, 0f);
            }
            public IEnumerator EnterTheGungeon()
            {
                Pixelator.Instance.FadeToColor(1f, Color.black, false, 0.5f);
                float e = 0;
                while (e < 1.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].SetInputOverride("goodbye!");
                        GameManager.Instance.AllPlayers[j].sprite.renderer.enabled = false;
                        SpriteOutlineManager.RemoveOutlineFromSprite(GameManager.Instance.AllPlayers[j].sprite);
                        GameManager.Instance.AllPlayers[j].m_hideGunRenderers = new OverridableBool(true) { };
                        GameManager.Instance.AllPlayers[j].ToggleShadowVisiblity(false);
                        GameManager.Instance.AllPlayers[j].ToggleGunRenderers(false);
                    }
                }
                AkSoundEngine.PostEvent("Play_OBJ_hugedoor_close_01", GameManager.Instance.BestActivePlayer.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_energycannon_loop_01", GameManager.Instance.BestActivePlayer.gameObject);

                //m_WPN_bsg_loop_01
                //m_OBJ_hugedoor_close_01
                GameManager.Instance.PreventPausing = true;
                GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
                Minimap.Instance.ToggleMinimap(false, false);
                GameManager.IsBossIntro = true;
                GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
                gameUIBossHealthController.DisableBossHealth();
                GameUIRoot.Instance.HideCoreUI("PainAndAgony");
                CameraController m_camera = GameManager.Instance.MainCameraController;
                m_camera.OverridePosition = this.GetComponent<tk2dSpriteAnimator>().sprite.WorldCenter;
                m_camera.OverrideRecoverySpeed = 100;
                m_camera.SetManualControl(true);

                this.GetComponent<tk2dSpriteAnimator>().Play("idle_closed");
                Pixelator.Instance.FadeToColor(1f, Color.black, true, 0f);
                e = 0;

                while (e < 2.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(10.25f, 12.5f, 0f), this.transform, 3.5f, "---DESTINATION SET TO: ---\nGUNYMEDE.", "golem", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);

                while (e < 2.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                Vector2 Pos = this.transform.PositionVector2();
                AkSoundEngine.PostEvent("Play_obj_bowler_ignite_01", GameManager.Instance.BestActivePlayer.gameObject);
                while (e < 1.5f)
                {
                    this.gameObject.transform.position = Vector2.Lerp(Pos, Pos + new Vector2(-5f, 0), Toolbox.SinLerpTValueFull(e / 3f));
                    m_camera.OverridePosition = this.GetComponent<tk2dSpriteAnimator>().sprite.WorldCenter;
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_ENM_cannonball_roll_01", GameManager.Instance.BestActivePlayer.gameObject);

                this.GetComponent<tk2dSpriteAnimator>().Play("take_off");

                var m_distortMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionRadius"));
                m_distortMaterial.SetFloat("_Strength", 1f);
                m_distortMaterial.SetFloat("_TimePulse", 0f);
                m_distortMaterial.SetFloat("_RadiusFactor", 0f);
                m_distortMaterial.SetVector("_WaveCenter", GetCenterPointInScreenUV(sprite.WorldCenter));
                Pixelator.Instance.RegisterAdditionalRenderPass(m_distortMaterial);
                AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Linger_01", GameManager.Instance.BestActivePlayer.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Linger_01", GameManager.Instance.BestActivePlayer.gameObject);

                while (e < 4.5f)
                {
                    Exploder.DoDistortionWave(sprite.WorldCenter, 0.1f, 0.75f, 22, 0.75f);

                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                Pos = this.transform.PositionVector2();
                AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Crackle_01", GameManager.Instance.BestActivePlayer.gameObject);
                while (e < 1f)
                {

                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                this.GetComponent<ImprovedAfterImage>().spawnShadows = true;
                Exploder.DoDistortionWave(sprite.WorldCenter, 100f, 0.25f, 30, 0.5f);
                e = 0;
                AkSoundEngine.PostEvent("Play_BOSS_spacebaby_explode_01", this.gameObject);
                while (e < 1f)
                {
                    this.gameObject.transform.position = Vector2.Lerp(Pos, Pos + new Vector2(0, 100), Toolbox.SinLerpTValue(e));
                    m_distortMaterial.SetFloat("_Strength", 0);
                    m_distortMaterial.SetFloat("_TimePulse", 0);
                    m_distortMaterial.SetFloat("_RadiusFactor",0);
                    e += BraveTime.DeltaTime;
                    yield return null;
                }

                Pixelator.Instance.FreezeFrame();
                BraveTime.RegisterTimeScaleMultiplier(0f, base.gameObject);
                float ela = 0f;
                while (ela < ConvictPastController.FREEZE_FRAME_DURATION)
                {
                    ela += GameManager.INVARIANT_DELTA_TIME;
                    yield return null;
                }
                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    if (player.HasPickupID(HighValueInfo.CoolInfoID))
                    {
                        GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, 100);
                        AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.PAST_MASTERY, true);
                    }
                }
                if (PDashTwo.P_2ParticleController.RecievedDamage == true)
                {
                    AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.PAST_MASTERY, true);
                }

                BraveTime.ClearMultiplier(base.gameObject);
                TimeTubeCreditsController ttcc = new TimeTubeCreditsController();
                Pixelator.Instance.FadeToColor(0.15f, Color.white, true, 0.15f);
                ttcc.ClearDebris();
                yield return base.StartCoroutine(ttcc.HandleTimeTubeCredits(GameManager.Instance.PrimaryPlayer.sprite.WorldCenter, false, null, -1, false));
                AmmonomiconController.Instance.OpenAmmonomicon(true, true);
                yield break;
            }
        }
    }
}
