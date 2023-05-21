using Alexandria.PrefabAPI;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class ExpressElevator
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Express_Elevator_To_Hell");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("bridge_idle_001"));
            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

            obj.CreateFastBody(new IntVector2(4, 160), new IntVector2(0, 0));
            obj.CreateFastBody(new IntVector2(4, 160), new IntVector2(252, 0));

            obj.CreateFastBody(new IntVector2(96, 16), new IntVector2(0, 0));
            obj.CreateFastBody(new IntVector2(96, 16), new IntVector2(160, 0));

            obj.CreateFastBody(new IntVector2(96, 16), new IntVector2(0, 144));
            obj.CreateFastBody(new IntVector2(96, 16), new IntVector2(160, 144));

            obj.CreateFastBody(CollisionLayer.Pickup ,new IntVector2(244, 64), new IntVector2(6, 64));


            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("ElevatorAnimationAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("elevator_idle");
            tk2dAnim.playAutomatically = true;

            tk2dAnim.sprite.usesOverrideMaterial = true;
            Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat1.mainTexture = tk2dAnim.sprite.renderer.material.mainTexture;
            mat1.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat1.SetFloat("_EmissiveColorPower", 10);
            mat1.SetFloat("_EmissivePower", 50);
            tk2dAnim.sprite.renderer.material = mat1;

            obj.AddComponent<Elevator>();

            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));


            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Express_Elevator_To_Hell_Past", obj);


            GameObject obj1 = PrefabBuilder.BuildObject("Express_Elevator_In_Hell");
            var tk2d1 = obj1.AddComponent<tk2dSprite>();
            tk2d1.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d1.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("bridge_idle_001"));

            tk2d1.sprite.usesOverrideMaterial = true;
            Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat2.mainTexture = tk2dAnim.sprite.renderer.material.mainTexture;
            mat2.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat2.SetFloat("_EmissiveColorPower", 10);
            mat2.SetFloat("_EmissivePower", 50);
            tk2d1.sprite.renderer.material = mat2;

            obj1.CreateFastBody(new IntVector2(4, 160), new IntVector2(0, 0));
            obj1.CreateFastBody(new IntVector2(4, 160), new IntVector2(252, 0));

            obj1.CreateFastBody(new IntVector2(96, 16), new IntVector2(0, 0));
            obj1.CreateFastBody(new IntVector2(96, 16), new IntVector2(160, 0));

            obj1.CreateFastBody(new IntVector2(96, 16), new IntVector2(0, 144));
            obj1.CreateFastBody(new IntVector2(96, 16), new IntVector2(160, 144));

            obj1.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));


            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Express_Elevator_To_Hell_Broke", obj1);
        }

        public class Elevator : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
        {
            private RoomHandler parentRoom;
            private bool Triggered = false;

            public void Start()
            {
                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody)
                {
                    specBody.OnPreRigidbodyCollision += (myBody, myCollider, otherbody, otherCollider) =>
                    {

                        if (myCollider.CollisionLayer == CollisionLayer.Pickup)
                        {
                            var currentRoom = GameManager.Instance.Dungeon.data.rooms[1];
                            IntVector2 pos = currentRoom.GetCenterCell();
                            if (Triggered == false)
                            {
                                var player = otherbody.gameObject.GetComponent<PlayerController>();
                                Triggered = true;
                                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
                                if (otherPlayer)
                                {
                                    otherPlayer.ReuniteWithOtherPlayer(player, false);
                                }
                                StaticReferenceManager.DestroyAllEnemyProjectiles();
                                Minimap.Instance.ToggleMinimap(false, false);
                                GameManager.IsBossIntro = true;
                                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                                {
                                    if (GameManager.Instance.AllPlayers[j])
                                    {
                                        GameManager.Instance.AllPlayers[j].SetInputOverride("BossIntro");
                                    }
                                }
                                GameManager.Instance.PreventPausing = true;
                                GameUIRoot.Instance.HideCoreUI(string.Empty);
                                GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);

                                CameraController m_camera = GameManager.Instance.MainCameraController;
                                GameManager.Instance.MainCameraController.SetManualControl(true, true);
                                GameManager.Instance.MainCameraController.OverridePosition = this.sprite.WorldCenter;

                                GameManager.Instance.StartCoroutine(DoElevator());
                            }            
                        }
                    };
                }
            }

            public IEnumerator DoElevator()
            {
                AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", this.gameObject);
                AkSoundEngine.PostEvent("Play_MachineNoises", this.gameObject);
                float e = 0;
                while (e < 1)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                while (e < 1)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_Alarm_BeepBeep", this.gameObject);
                this.spriteAnimator.Play("elevator_move");
                e = 0;
                float q = 0;
                while (e < 2)
                {
                    if (q > 0.2f)
                    {
                        q = 0;
                        foreach (PlayerController p in GameManager.Instance.AllPlayers)
                        {
                            p.WarpToPoint(p.transform.PositionVector2() - new Vector2(0, 0.0625f));
                        }
                    }
                    q += BraveTime.DeltaTime;
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                Pixelator.Instance.FadeToBlack(1.5f, false, 0f);
                while (e < 1.5)
                {
                    if (q > 0.2f)
                    {
                        q = 0;
                        foreach (PlayerController p in GameManager.Instance.AllPlayers)
                        {
                            p.WarpToPoint(p.transform.PositionVector2() - new Vector2(0, 0.0625f));
                        }
                    }
                    q += BraveTime.DeltaTime;
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                GameManager.IsBossIntro = false;
                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].ClearInputOverride("BossIntro");
                    }
                }//Stop_MachineNoises
                AkSoundEngine.PostEvent("Stop_Alarm_BeepBeep", this.gameObject);
                AkSoundEngine.PostEvent("Stop_MachineNoises", this.gameObject);

                GameManager.Instance.PreventPausing = false;
                GameManager.Instance.LoadCustomLevel(PDashTwo.PastDefinition.dungeonSceneName);

                //GameManager.Instance.LoadCustomLevel("tt_modular_ultrahard");
                yield break;
            }


            public void Update()
            {

            }

            public void ConfigureOnPlacement(RoomHandler room)
            {
                this.parentRoom = room;
            }

            public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
            {
                shouldBeFlipped = false;
                return string.Empty;
            }

            public float GetDistanceToPoint(Vector2 point)
            {
                if (base.sprite == null)
                {
                    return 100f;
                }
                Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
                return Vector2.Distance(point, v) / 1.5f;
            }

            public float GetOverrideMaxDistance()
            {
                return -1f;
            }
            public void Interact(PlayerController interactor)
            {

            }
            public void OnEnteredRange(PlayerController interactor)
            {
                //SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
            }
            public void OnExitRange(PlayerController interactor)
            {
                //SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
            }
        }
    }
}
