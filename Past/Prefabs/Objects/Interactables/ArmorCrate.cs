using Alexandria.PrefabAPI;
using Brave.BulletScript;
using Dungeonator;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.FakeCorridor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class ArmorCrate
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("ArmorCrate_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Crate_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("crate_idle_001"));
            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = StaticCollections.Crate_Animation;
            tk2d.sprite.usesOverrideMaterial = true;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            tk2d.renderer.material = mat;
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            var switche= obj.AddComponent<ArmorCrateController>();
            Module.Strings.Core.Set("#MDLR_CRATE_DEF", "A 'Goliath' class Modular repair crate. Take some?");
            Module.Strings.Core.Set("#MDLR_CRATE_ACC", StaticColorHexes.AddColorToLabelString("Take the repairs.", StaticColorHexes.Green_Hex));
            Module.Strings.Core.Set("#MDLR_CRATE_CAN", StaticColorHexes.AddColorToLabelString("Leave it.", StaticColorHexes.Light_Orange_Hex));
            switche.acceptOptionKey = "#MDLR_CRATE_ACC";
            switche.displayTextKey = "#MDLR_CRATE_DEF";
            switche.declineOptionKey = "#MDLR_CRATE_CAN";
            switche.talkPoint = obj.transform;

            obj.CreateFastBody(new IntVector2(26, 22), new IntVector2(1, 1));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("ArmorCrate_MDLR", obj);
        }
    }

   

    public class ArmorCrateController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Start()
        {
            this.spriteAnimator.Play("cratealt_idle");
        }


        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_parentRoom = room;
            this.RegisterMinimapIcon();
        }
        public void RegisterMinimapIcon()
        {
            //this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
        }
        private RoomHandler m_parentRoom;
        //private GameObject m_instanceMinimapIcon;
        public Transform talkPoint;
        private int m_useCount = 0;

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

            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                return;
            }
            if (this.m_useCount > 0)
            {
                return;
            }
            base.StartCoroutine(this.HandleShrineConversation(interactor));
        }

        private IEnumerator HandleSpentText(PlayerController interactor)
        {
            TextBoxManager.ShowInfoBox(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(this.spentOptionKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(this.declineOptionKey), string.Empty);
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            yield break;
        }

        private IEnumerator HandleShrineConversation(PlayerController interactor)
        {
            string targetDisplayKey = this.displayTextKey;
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            bool canUse = true;//this.CheckCosts(interactor);
            if (canUse)
            {
                string text = StringTableManager.GetString(this.acceptOptionKey);

                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, text, StringTableManager.GetString(this.declineOptionKey));
            }
            else
            {
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(this.declineOptionKey), string.Empty);
            }
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            if (canUse && selectedResponse == 0)
            {
                interactor.PlayEffectOnActor(VFXStorage.HealingSparklesVFX, new Vector3(0, 0));
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", interactor.gameObject);
                interactor.healthHaver.Armor += 3;
                this.m_parentRoom.DeregisterInteractable(this);
                this.spriteAnimator.Play("cratealt_close");
                this.m_useCount++;
            }
            yield break;
        }



        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
        }

        public string displayTextKey;

        public string acceptOptionKey;

        public string declineOptionKey;

        public string spentOptionKey = "#SHRINE_GENERIC_SPENT";
    }

}
