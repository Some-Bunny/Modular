using Alexandria.ItemAPI;
using Alexandria.NPCAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Controllers
{
    public class StuffedToy
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Modular_Fumo");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("module_fumo_001"));
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;
            var fumo =  obj.AddComponent<Fumo_Dot_MP4>();
            fumo.PainSound = "Play_ToySqueak";
            fumo.SpriteName_Def = "module_fumo_001";
            fumo.SpriteName_Squish = "module_fumo_002";


            obj.CreateFastBody(IntVector2.Zero, IntVector2.Zero);
            Fumo = obj;
            Init2();
            Init3();
            new Hook(typeof(Foyer).GetMethod("CheckHeroStatue", BindingFlags.Instance | BindingFlags.NonPublic), typeof(StuffedToy).GetMethod("CheckHeroStatueHook"));
        }

        public static void Init2()
        {
            GameObject obj = PrefabBuilder.BuildObject("Bunny_Fumo");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("me_1"));
            tk2d.sprite.usesOverrideMaterial = true;
            tk2d.renderer.material = new Material(StaticShaders.Default_Shader);
            var fumo = obj.AddComponent<Fumo_Dot_MP4>();
            fumo.PainSound = "Play_Suffering";
            fumo.SpriteName_Def = "me_1";
            fumo.SpriteName_Squish = "me_2";
            obj.CreateFastBody(IntVector2.Zero, IntVector2.Zero);
            fumo.isDev = true;
            Fumo_2 = obj;

        }
        public static void Init3()
        {
            GameObject obj = PrefabBuilder.BuildObject("Alt_Fumo");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("modulealt_fumo_001"));
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
            mat.SetFloat("_EmissiveColorPower", 5);
            mat.SetFloat("_EmissivePower", 3);
            tk2d.renderer.material = mat;
            var fumo = obj.AddComponent<Fumo_Dot_MP4>();
            fumo.PainSound = "Play_ToySqueak";
            fumo.SpriteName_Def = "modulealt_fumo_001";
            fumo.SpriteName_Squish = "modulealt_fumo_002";
            obj.CreateFastBody(IntVector2.Zero, IntVector2.Zero);

            Fumo_3 = obj;

        }
        public static GameObject Fumo;
        public static GameObject Fumo_2;
        public static GameObject Fumo_3;


        public static void CheckHeroStatueHook(Action<Foyer> orig, Foyer foyer)
        {
            orig(foyer);
            if (CanSpawnFumo1() == true)
            {
                UnityEngine.Object.Instantiate(Fumo, new Vector3(32.125f, 30.5f, 35.125f), Quaternion.identity);
            }
            if (CanSpawnFumo2() == true)
            {
                UnityEngine.Object.Instantiate(Fumo_3, new Vector3(31.125f, 31.5f, 35.125f), Quaternion.identity);
            }
            if (CanSpawnBnnuy() == true)
            {
                UnityEngine.Object.Instantiate(Fumo_2, new Vector3(32.5f, 32.25f, 35.125f), Quaternion.identity);
            }
        }

        public static bool CanSpawnFumo1()
        {
            var Manager = SaveAPI.AdvancedGameStatsManager.Instance;
            if (Manager == null) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_FLOOR_3) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_RAT_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST) == false) { return false; }

            return true;
        }
        public static bool CanSpawnFumo2()
        {
            var Manager = SaveAPI.AdvancedGameStatsManager.Instance;
            if (Manager == null) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_FLOOR_3) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_RAT_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST_ALT_SKIN) == false) { return false; }

            return true;
        }
        public static bool CanSpawnBnnuy()
        {
            var Manager = SaveAPI.AdvancedGameStatsManager.Instance;
            if (Manager == null) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_FLOOR_3) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_RAT_AS_MODULAR) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST_ALT_SKIN) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST) == false) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.PAST_MASTERY) == false) { return false; }
            return true;
        }
        public class Fumo_Dot_MP4 : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
        {
            public void Start()
            {
                this.transform.PositionVector2().GetAbsoluteRoom().RegisterInteractable(this);
            }
            public void ConfigureOnPlacement(RoomHandler room){}

            public Transform talkPoint;

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

            public string PainSound;
            public string SpriteName_Def;
            public string SpriteName_Squish;
            public bool isDev = false;
            public void Interact(PlayerController interactor)
            {
                if (isSquished == true) { return; }
                base.StartCoroutine(this.Squish(interactor));
            }
            private IEnumerator Squish(PlayerController interactor)
            {
                isSquished = true;
                AkSoundEngine.PostEvent(PainSound, interactor.gameObject);
                AkSoundEngine.PostEvent("Play_ToySqueak", interactor.gameObject);
                this.GetComponent<tk2dBaseSprite>().SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(SpriteName_Squish));
                float e = 0;
                while (e < (isDev ? 1 : 0.5f))
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                this.GetComponent<tk2dBaseSprite>().SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(SpriteName_Def));
                if (isDev == true)
                {
                    TextBoxManager.ShowTextBox(this.sprite.WorldCenter + new Vector2(1.25f, 1f), this.gameObject.transform, 2.5f,BraveUtility.RandomElement<string>(ThankYous), Alexandria.NPCAPI.ShopAPI.ReturnVoiceBox(ShopAPI.VoiceBoxes.SER_MANUEL), false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
                    e = 0;
                    while (e < 3)
                    {
                        e += BraveTime.DeltaTime;
                        yield return null;
                    }
                }
                isSquished = false;
                yield break;
            }
            private bool isSquished;
            public void OnEnteredRange(PlayerController interactor)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
            }
            public void OnExitRange(PlayerController interactor)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
            }
            private static List<string> ThankYous = new List<string>()
            {
                "Thanks for playing my mod!",
                "Thank you for playing Modular!",
                "Hope you enjoyed!",
                "Merry Christmas!",
                "Hope it was fun!",
                "Many thanks for giving this mod a try!"
            };
        }
    }
}
