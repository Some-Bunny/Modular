﻿using Alexandria.PrefabAPI;
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
    public class SecretSwitch
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("SecretSwitche_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("switch1"));


            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Shader);
            tk2d.renderer.material = mat;

            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            var switche= obj.AddComponent<SecretSwitchController>();
            Module.Strings.Core.Set("#MDLR_SWITCH_DEF", "A switch. Flip it..?");
            Module.Strings.Core.Set("#MDLR_SWITCH_ACC", StaticColorHexes.AddColorToLabelString("Flip it.", StaticColorHexes.Red_Color_Hex));
            Module.Strings.Core.Set("#MDLR_SWITCH_CAN", StaticColorHexes.AddColorToLabelString("Leave it.", StaticColorHexes.Green_Hex));
            switche.acceptOptionKey = "#MDLR_SWITCH_ACC";
            switche.displayTextKey = "#MDLR_SWITCH_DEF";
            switche.declineOptionKey = "#MDLR_SWITCH_CAN";
            switche.talkPoint = obj.transform;

            obj.CreateFastBody(new IntVector2(0, 0), new IntVector2(0,0));
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Switch_MDLR", obj);
        }
    }

   

    public class SecretSwitchController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
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
        }
        private RoomHandler m_parentRoom;
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
            return Vector2.Distance(point, v) / 2f;
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
                this.sprite.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("switch2"));
                GlobalMessageRadio.BroadcastMessage("SwitchFlipped");
                AkSoundEngine.PostEvent("Play_OBJ_chain_switch_01", interactor.gameObject);
                this.m_parentRoom.DeregisterInteractable(this);
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
