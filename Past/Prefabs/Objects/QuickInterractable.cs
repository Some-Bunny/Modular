using Dungeonator;
using HutongGames.PlayMaker.Actions;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class QuickInterractableController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        private RoomHandler parentRoom;
        public string Interact_String = "Hi :)";
        public Transform talkPoint;
        public bool DebugReach = false;
        public bool UsesTransformDist = false;

        public void Start()
        {
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
            if (UsesTransformDist)
            {
                return Vector2.Distance(point, talkPoint.PositionVector2()) / 1.5f * (DebugReach == true ? 15 : 1);
            }
            if (base.sprite == null)
            {
                return 100f;
            }
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, v) / 1.5f * (DebugReach == true ? 15 : 1);
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }
        public void Interact(PlayerController interactor)
        {
            string targetDisplayKey = this.Interact_String;
            TextBoxManager.ShowThoughtBubble(interactor.sprite.WorldCenter + new Vector2(1,1), this.talkPoint, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
        }
        public void OnEnteredRange(PlayerController interactor)
        {
            TextBoxManager.ClearTextBox(this.talkPoint);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }
        public void OnExitRange(PlayerController interactor)
        {
            TextBoxManager.ClearTextBox(this.talkPoint);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
        }
    }

}
