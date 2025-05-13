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
    public class BigSwitch
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BigSwitch_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("table_001"));
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(124, 186, 55, 255));
            mat.SetFloat("_EmissiveColorPower", 4);
            mat.SetFloat("_EmissivePower", 3);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            tk2d.renderer.material = mat;
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            var switche= obj.AddComponent<SwitchController>();

            Module.Strings.Core.Set("#MDLR_BIG_SWITCH_DEF", "The override power switch to the main Shipyard.");
            Module.Strings.Core.Set("#MDLR_BIG_SWITCH_ACC", StaticColorHexes.AddColorToLabelString("<Flip it.>", StaticColorHexes.Green_Hex));
            Module.Strings.Core.Set("#MDLR_BIG_SWITCH_CAN", StaticColorHexes.AddColorToLabelString("<Leave it.>", StaticColorHexes.Red_Color_Hex));
            switche.displayTextKey = "#MDLR_BIG_SWITCH_DEF";
            switche.acceptOptionKey = "#MDLR_BIG_SWITCH_ACC";
            switche.declineOptionKey = "#MDLR_BIG_SWITCH_CAN";
            switche.talkPoint = obj.transform;

            obj.CreateFastBody(new IntVector2(60, 16), new IntVector2(2, -2));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BigSwitch_MDLR", obj);
        }
    }

   

    public class SwitchController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Start()
        {

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
                AkSoundEngine.PostEvent("Play_ENM_darken_world_01", this.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_moondoor_close_01", GameManager.Instance.BestActivePlayer.gameObject);
                this.sprite.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("table_002"));
                GlobalMessageRadio.BroadcastMessage("ShutDown");
                this.StartCoroutine(DoShut());
                this.m_useCount++;
            }
            yield break;
        }

        public IEnumerator DoShut()
        {
            float e = 0;
            while (e < 3) { e += BraveTime.DeltaTime; yield return null; }
            Toolbox.NotifyCustom("A DOOR OPENS...", "", this.sprite.spriteId, this.sprite.collection, UINotificationController.NotificationColor.SILVER, true);

            yield return null;
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
