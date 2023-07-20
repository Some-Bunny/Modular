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
    public class SusBoxes
    {
        public static void Init()
        {
            InitSusBox(PerformanceCore.PerformanceDriveID, "PerfBoxMDLR");
            InitSusBox(CloseQuartersCore.CQCID, "CQCBoxMDLR");
            InitSusBox(AllocationCore.AllocationCoreID, "AllocBoxMDLR");

        }
        public static void InitSusBox(int ID, string name)
        {
            GameObject obj = PrefabBuilder.BuildObject("Sus_Box");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("box_sussybox"));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0);
            mat.SetFloat("_EmissivePower", 0);
            tk2d.renderer.material = mat;
            
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            var switche = obj.AddComponent<SusBoxController>();
            Module.Strings.Core.Set("#MDLR_CRATE_sus"+ name + "_DEF", "Woah, there actually seems to be something in here.");
            Module.Strings.Core.Set("#MDLR_CRATE_sus"+ name + "_ACC", StaticColorHexes.AddColorToLabelString("Take it out.", StaticColorHexes.Green_Hex));
            Module.Strings.Core.Set("#MDLR_CRATE_sus"+ name + "_CAN", StaticColorHexes.AddColorToLabelString("Leave it.", StaticColorHexes.Green_Hex));
            switche.acceptOptionKey = "#MDLR_CRATE_sus" + name + "_ACC";
            switche.displayTextKey = "#MDLR_CRATE_sus" + name + "_DEF";
            switche.declineOptionKey = "#MDLR_CRATE_sus" + name + "_CAN";
            switche.talkPoint = obj.transform;
            switche.PickupID = ID;
            obj.CreateFastBody(new IntVector2(28, 16), new IntVector2(0, -3));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(name, obj);
        }
    }

   

    public class SusBoxController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Start(){ }
        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_parentRoom = room;
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
            bool canUse = true;
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
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(PickupID).gameObject, interactor.sprite.WorldBottomCenter, Vector2.zero, 0, true, true);
                this.m_parentRoom.DeregisterInteractable(this);
                this.m_useCount++;
            }
            yield break;
        }



        public int PickupID = ConfidenceCore.ConfidenceCoreID;


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
