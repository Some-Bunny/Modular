using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using AmmonomiconAPI;
using UnityEngine;
using System.Collections;

namespace ModularMod.Code.UI
{
    public class AmmonomiconSetup
    {
        public static void Initialize()
        {

            AmmonomiconAPI.UIBuilder.BuildBookmark("mdl", "Modules", new ModuleAmmonomiconPageController(),
                new SpriteContainer(StaticCollections.AmmonomiconUIAtlas)
                {
                    AppearFrames = new string[] { "bookmark_mdl_001", "bookmark_mdl_002", "bookmark_mdl_003", "bookmark_mdl_004" },
                    SelectFrames = new string[] { "bookmark_mdl_select_001", "bookmark_mdl_select_002", "bookmark_mdl_select_003" },
                    HoverFrame = "bookmark_mdl_hover_001",
                    SelectHoverFrame = "bookmark_mdl_select_hover_001"
                }, Assembly.GetExecutingAssembly());

            //AmmonomiconAPI.CustomActions.OnPreEquipmentPageBuild += BuildEquipmentPage;
            AmmonomiconAPI.CustomActions.OnDeathPageFinalizing += BuilDeathRightPage;
        }


        public static void BuilDeathRightPage(AmmonomiconPageRenderer ammonomiconPageRenderer, List<tk2dBaseSprite> tk2DBaseSprites)
        {
            AmmonomiconDeathPageController component = ammonomiconPageRenderer.guiManager.GetComponent<AmmonomiconDeathPageController>();
            dfScrollPanel component2 = component.transform.Find("Scroll Panel").Find("Footer").Find("ScrollItemsPanel").GetComponent<dfScrollPanel>();
            dfPanel component3 = component2.transform.Find("AllItemsPanel").GetComponent<dfPanel>();

            var player = AmmonomiconAPI.HelperTools.LastPlayerOpenedUI();
            if (player)
            {
                var _ = component2.Find("ActiveMods");
                var __ = component2.Find("ModulePanel");

                var ___ = component2.Find("InActiveMods");
                var ____ = component2.Find("InModulePanel");

                if (_ == null)
                {
                    var header_2 = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsHeader, component2.transform);
                    header_2.name = "ActiveMods";
                    var label_2 = header_2.GetComponentInChildren<dfLabel>();
                    label_2.isLocalized = false;
                    label_2.localizationKey = "";
                    label_2.Text = "Active";
                    var translator = label_2.gameObject.GetComponent<ConditionalTranslator>();
                    translator.enabled = false;
                    header_2.ZOrder = 1;
                    _ = header_2;
                    label_2.ResetLayout();

                    component2.controls.Add(label_2);

                    component3.ResetLayout();


                    header_2.ResetLayout();


                    var modulePanel = UnityEngine.Object.Instantiate(component3, component2.transform);
                    modulePanel.gameObject.name = "ModulePanel";
                    for (int i = 0; i < modulePanel.transform.childCount; i++)
                    {
                        UnityEngine.Object.Destroy(modulePanel.transform.GetChild(0).gameObject);
                    }
                    component2.controls.Add(modulePanel);
                    modulePanel.ZOrder = 2; 
                    __ = modulePanel;


                    var header_3 = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsHeader, component2.transform);
                    header_3.name = "InActiveMods";
                    var label_3 = header_3.GetComponentInChildren<dfLabel>();
                    label_3.isLocalized = false;
                    label_3.localizationKey = "";
                    label_3.Text = "Inactive";
                    translator = label_3.gameObject.GetComponent<ConditionalTranslator>();
                    translator.enabled = false;
                    header_3.ZOrder = 3;
                    ___ = header_3;
                    label_3.ResetLayout();
                    component2.controls.Add(label_2);
                    header_3.CenterChildControls();

                    modulePanel = UnityEngine.Object.Instantiate(component3, component2.transform);
                    modulePanel.gameObject.name = "InModulePanel";
                    for (int i = 0; i < modulePanel.transform.childCount; i++)
                    {
                        UnityEngine.Object.Destroy(modulePanel.transform.GetChild(0).gameObject);
                    }
                    component2.controls.Add(modulePanel);
                    modulePanel.ZOrder = 4;
                    ____ = modulePanel;

                }
                component2.AutoLayout = true;
                component2.onChildControlInvalidatedLayout();
                component2.flowDirection = dfScrollPanel.LayoutDirection.Vertical;
                component2.anchorStyle = dfAnchorStyle.Top;
                component2.ForceUpdateCachedParentTransform();
                component2.AutoArrange();

                var core = player.PlayerHasCore();
                if (core)
                {
                    _.gameObject.SetActive(true);
                    __.gameObject.SetActive(true);
                    ___.gameObject.SetActive(true);
                    ____.gameObject.SetActive(true);

                    _.ZOrder = 1;
                    __.ZOrder = 2;
                    ___.ZOrder = 3;
                    ____.ZOrder = 4;
                    component2.AutoArrange();

                    GameManager.Instance.StartCoroutine(DoWait(ammonomiconPageRenderer, component2, player, core, __ as dfPanel, ____ as dfPanel));
                    /*
                    var scrollPanel = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.RightPageScrollPanel, component2.transform);
                    var panel = scrollPanel.GetComponent<dfPanel>();
                    panel.ZOrder = component3.ZOrder + 2;
                    panel.Width = component3.Width;


                    */
                }
                else
                {
                    _.gameObject.SetActive(false);
                    __.gameObject.SetActive(false);
                    ___.gameObject.SetActive(false);
                    ____.gameObject.SetActive(false);
                }
            }
        }
        public static IEnumerator DoWait(AmmonomiconPageRenderer ammonomiconPageRenderer, dfScrollPanel component2, PlayerController player, ModulePrinterCore core,  dfPanel __, dfPanel ____)
        {
            yield return null;
            //_.ZOrder = 1;
            __.ZOrder = 2;
            ____.ZOrder = 4;
            for (int i = 0; i < __.transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(__.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < ____.transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(____.transform.GetChild(i).gameObject);
            }

            List<tk2dBaseSprite> Sprites = new List<tk2dBaseSprite>();
            foreach (var entry in core.ModuleContainers)
            {
                AddSprites(ammonomiconPageRenderer, player, __ as dfPanel, entry.ActiveCount + entry.ReturnTemporaryCounts() + entry.ReturnFakeCounts(), entry.defaultModule.PickupObjectId, ref Sprites);
            }
            Sprites = Sprites.ttOrderBy((tk2dBaseSprite a) => a.GetBounds().size.y);
            List<tk2dBaseSprite> list2 = new List<tk2dBaseSprite>();
            ammonomiconPageRenderer.BoxArrangeItems(__ as dfPanel, Sprites, new Vector2(0f, 6f), new Vector2(6f, 3f), ref list2);
            ammonomiconPageRenderer.StartCoroutine(ammonomiconPageRenderer.HandleDeathItemsClipping(__ as dfPanel, Sprites));

            component2.ResetLayout();
            component2.AutoArrange();


            List<tk2dBaseSprite> Sprites2 = new List<tk2dBaseSprite>();

            foreach (var entry in core.ModuleContainers)
            {
                AddSprites(ammonomiconPageRenderer, player, ____ as dfPanel, entry.Count - entry.ActiveCount, entry.defaultModule.PickupObjectId, ref Sprites2);
            }
            Sprites2 = Sprites2.ttOrderBy((tk2dBaseSprite a) => a.GetBounds().size.y);
            List<tk2dBaseSprite> list3 = new List<tk2dBaseSprite>();
            ammonomiconPageRenderer.BoxArrangeItems(____ as dfPanel, Sprites2, new Vector2(0f, 6f), new Vector2(6f, 3f), ref list3);
            ammonomiconPageRenderer.StartCoroutine(ammonomiconPageRenderer.HandleDeathItemsClipping(____ as dfPanel, Sprites2));
            yield break;
        }


        public static void AddSprites(AmmonomiconPageRenderer __instance, PlayerController playerController, dfPanel component3, int amount, int itemID, ref List<tk2dBaseSprite> tk2DBaseSprites)
        {
            var pickup = GlobalModuleStorage.ReturnModule(itemID);
            for (int i = 0; i < amount; i++)
            {
                tk2dClippedSprite tk2dClippedSprite3 = __instance.AddSpriteToPage<tk2dClippedSprite>(pickup.sprite.collection, pickup.sprite.spriteId);// playerController.passiveItems[m].sprite.spriteId);
                SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>(tk2dClippedSprite3, Color.black, 0.1f, 0.01f);
                tk2dClippedSprite3.transform.parent = component3.transform;
                tk2dClippedSprite3.transform.position = component3.GetCenter();
                tk2DBaseSprites.Add(tk2dClippedSprite3);
            }
        }




        public class ModuleAmmonomiconPageController : CustomAmmonomiconPageController
        {
            public ModuleAmmonomiconPageController() : base("MODULES", 8, false, "") { }

            public override List<EncounterDatabaseEntry> GetEntriesForPage(AmmonomiconPageRenderer renderer)
            {

                List<EncounterDatabaseEntry> list2 = new List<EncounterDatabaseEntry>();


               
                return list2;
            }

            public List<EncounterDatabaseEntry> GetDatabase(DefaultModule.ModuleTier moduleTier)
            {
                List<EncounterDatabaseEntry> list2 = new List<EncounterDatabaseEntry>();
                List<KeyValuePair<int, PickupObject>> list = new List<KeyValuePair<int, PickupObject>>();
                switch (moduleTier)
                {
                    case DefaultModule.ModuleTier.Tier_1:
                        foreach (var entry in GlobalModuleStorage.all_Tier_1_Modules)
                        {
                            int key = (entry.ForcedPositionInAmmonomicon >= 0) ? entry.ForcedPositionInAmmonomicon : 1000000000;
                            list.Add(new KeyValuePair<int, PickupObject>(key, entry));
                        }
                        break;
                    case DefaultModule.ModuleTier.Tier_2:
                        foreach (var entry in GlobalModuleStorage.all_Tier_2_Modules)
                        {
                            int key = (entry.ForcedPositionInAmmonomicon >= 0) ? entry.ForcedPositionInAmmonomicon : 1000000000;
                            list.Add(new KeyValuePair<int, PickupObject>(key, entry));
                        }
                        break;
                    case DefaultModule.ModuleTier.Tier_3:
                        foreach (var entry in GlobalModuleStorage.all_Tier_3_Modules)
                        {
                            int key = (entry.ForcedPositionInAmmonomicon >= 0) ? entry.ForcedPositionInAmmonomicon : 1000000000;
                            list.Add(new KeyValuePair<int, PickupObject>(key, entry));
                        }
                        break;
                }
                list = (from e in list
                        orderby e.Key
                        select e).ToList<KeyValuePair<int, PickupObject>>();
                for (int j = 0; j < list.Count; j++)
                {
                    EncounterTrackable component4 = list[j].Value.GetComponent<EncounterTrackable>();
                    if (!component4.journalData.SuppressInAmmonomicon)
                    {
                        EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(component4.EncounterGuid);
                        if (entry != null)
                        {
                            list2.Add(entry);
                        }
                    }
                }
                return list2;
            }


            public override void InitializeItemsPageLeft(AmmonomiconPageRenderer self)
            {
                List<EncounterDatabaseEntry> entriesForPage = GetDatabase(DefaultModule.ModuleTier.Tier_1);
                List<EncounterDatabaseEntry> entriesForPage2 = GetDatabase(DefaultModule.ModuleTier.Tier_2);
                List<EncounterDatabaseEntry> entriesForPage3 = GetDatabase(DefaultModule.ModuleTier.Tier_3);


                Transform c = self.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");

                var header_1 = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsHeader, c);
                header_1.name = "Tier1";
                var label_1 = header_1.GetComponentInChildren<dfLabel>();
                label_1.isLocalized = false;
                label_1.localizationKey = "";
                label_1.Text = "Tier 1";
                var translator = label_1.gameObject.GetComponent<ConditionalTranslator>();
                translator.enabled = false;
                label_1.ZOrder = 8;

                dfPanel DefaultPanel_1 = c.Find("Guns Panel").GetComponent<dfPanel>();
                var DefaultPanel_2 = UnityEngine.Object.Instantiate(DefaultPanel_1, c);
                var DefaultPanel_3 = UnityEngine.Object.Instantiate(DefaultPanel_1, c);



                DefaultPanel_1.ZOrder = 9;
                dfPanel component2 = DefaultPanel_1.transform.GetChild(0).GetComponent<dfPanel>();
                self.StartCoroutine(self.ConstructRectanglePageLayout(component2, entriesForPage, new Vector2(12f, 20f), new Vector2(20f, 20f)));
                component2.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
                DefaultPanel_1.Height = component2.Height;
                component2.Height = DefaultPanel_1.Height;

                var header_2 = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsHeader, c);
                header_2.name = "Tier2";
                var label_2 = header_2.GetComponentInChildren<dfLabel>();
                label_2.isLocalized = false;
                label_2.localizationKey = "";
                label_2.Text = "Tier 2";
                translator = label_2.gameObject.GetComponent<ConditionalTranslator>();
                translator.enabled = false;
                label_2.ZOrder = 10;


                DefaultPanel_2.ZOrder = 11;
                component2 = DefaultPanel_2.transform.GetChild(0).GetComponent<dfPanel>();
                self.StartCoroutine(self.ConstructRectanglePageLayout(component2, entriesForPage2, new Vector2(12f, 20f), new Vector2(20f, 20f)));
                component2.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
                DefaultPanel_2.Height = component2.Height;
                component2.Height = DefaultPanel_2.Height;

                var header_3 = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsHeader, c);
                header_3.name = "Tier3";
                var label_3 = header_3.GetComponentInChildren<dfLabel>();
                label_3.isLocalized = false;
                label_3.localizationKey = "";
                label_3.Text = "Tier 3";
                translator = label_3.gameObject.GetComponent<ConditionalTranslator>();
                translator.enabled = false;
                label_3.ZOrder = 12;


                DefaultPanel_3.ZOrder = 13;
                component2 = DefaultPanel_3.transform.GetChild(0).GetComponent<dfPanel>();
                self.StartCoroutine(self.ConstructRectanglePageLayout(component2, entriesForPage3, new Vector2(12f, 20f), new Vector2(20f, 20f)));
                component2.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
                DefaultPanel_3.Height = component2.Height;
                component2.Height = DefaultPanel_3.Height;

                /*




                */


                /*



               
                */


                /*
                var obj = AmmonomiconAPI.StaticData.HeaderObject;
                var newObject = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.HeaderObject, c);
                newObject.name = "Tier1";
                var label = newObject.GetComponentInChildren<dfLabel>();
                label.isLocalized = false;
                label.localizationKey = "";
                label.Text = "Tier 1";
                var translator = label.gameObject.GetComponent<ConditionalTranslator>();
                translator.enabled = false;
                var lablePanel = newObject.GetComponent<dfPanel>();
                lablePanel.ZOrder = 9;
                var newObjectList = UnityEngine.Object.Instantiate(AmmonomiconAPI.StaticData.ItemsPanel, c);
                newObjectList.name = "Tier 1 Panel";
                newObjectList.ZOrder = 10;

                self.StartCoroutine(self.ConstructRectanglePageLayout(lablePanel, entriesForPage, new Vector2(12f, 20f), new Vector2(20f, 20f)));
                lablePanel.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
                component.Height = lablePanel.Height;
                lablePanel.Height = component.Height;
                */

                /*
                dfPanel component2 = component.transform.GetChild(0).GetComponent<dfPanel>();
                self.StartCoroutine(self.ConstructRectanglePageLayout(component2, entriesForPage, new Vector2(12f, 20f), new Vector2(20f, 20f)));
                component2.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
                component.Height = component2.Height;
                component2.Height = component.Height;
                */




            }

            public override bool ShouldBeActive()
            {
                /*
                if (GameManager.Instance.AllPlayers == null)
                {
                    return true;
                }
                foreach (var entry in GameManager.Instance.AllPlayers)
                {
                    if (entry.PlayerHasCore())
                    {
                        return true;
                    }
                }
                */
                return true;
            }
            public override void OnPageOpenedRight(AmmonomiconPageRenderer rightPage)
            {
                rightPage.SetPageDataUnknown(rightPage);
            }

        }

    }
}
