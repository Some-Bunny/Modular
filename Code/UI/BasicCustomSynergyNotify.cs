using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace ModularMod
{
    public class BasicCustomSynergyNotifier
    {
        public static void Init()
        {
            //new Hook(typeof(UINotificationController).GetMethod("DoNotification", BindingFlags.Instance | BindingFlags.Public), typeof(BasicCustomSynergyNotifier).GetMethod("DoNotificationHook"));

        }

        public static void DoNotificationHook(Action<UINotificationController, EncounterTrackable, bool> orig, UINotificationController self, EncounterTrackable trackable, bool onlyIfSynergy = false)
        {
            if (trackable.GetComponent<PickupObject>() != null)
            {
                foreach (var entry in synergizing_Items)
                {
                    if (entry.item_Id == trackable.GetComponent<PickupObject>().PickupObjectId)
                    {
                        CallModularSynergy(trackable);
                        return;
                    }
                }               
            }
            orig(self, trackable, onlyIfSynergy);
        }
        public static void CallModularSynergy(EncounterTrackable trackable)
        {
            if (!trackable)
            {
                return;
            }
            NotificationParams param = new NotificationParams();
            param.EncounterGuid = trackable.EncounterGuid;
            tk2dBaseSprite component = trackable.GetComponent<tk2dBaseSprite>();
            PickupObject component2 = trackable.GetComponent<PickupObject>();
            if (component2)
            {
                param.pickupId = component2.PickupObjectId;
            }
            param.SpriteCollection = component.Collection;
            param.SpriteID = component.spriteId;
            param.HasAttachedSynergy = true;
            param.forcedColor = UINotificationController.NotificationColor.GOLD;
            param.PrimaryTitleString = component2.DisplayName;
            param.SecondaryDescriptionString = component2.encounterTrackable.journalData.NotificationPanelDescription;
            var Notif = GameUIRoot.Instance.notificationController;
            param = Notif.SetupTexts(trackable, param);

            Notif.m_queuedNotifications.Add(HandleNotification(param));
            Notif.m_queuedNotificationParams.Add(param);
            Notif.StartCoroutine(Notif.PruneQueuedNotifications());
            //GameUIRoot.Instance.notificationController.StartCoroutine(HandleNotification(param));
        }


        private static IEnumerator HandleNotification(NotificationParams notifyParams)
        {
            yield return null;
            yield return null;
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(notifyParams.pickupId))
                {
                    player.PlayEffectOnActor((GameObject)ResourceCache.Acquire("Global VFX/VFX_Synergy"), new Vector3(0f, 0.5f, 0f), true, false, false);
                }
            }
            var NotifCont = GameUIRoot.Instance.notificationController;
            NotifCont.SetupSprite(notifyParams.SpriteCollection, notifyParams.SpriteID);
            NotifCont.DescriptionLabel.ProcessMarkup = true;
            NotifCont.DescriptionLabel.ColorizeSymbols = true;
            NotifCont.NameLabel.Text = notifyParams.PrimaryTitleString.ToUpperInvariant();
            NotifCont.DescriptionLabel.Text = notifyParams.SecondaryDescriptionString;
            NotifCont.CenterLabel.Opacity = 1f;
            NotifCont.NameLabel.Opacity = 1f;
            NotifCont.DescriptionLabel.Opacity = 1f;
            NotifCont.CenterLabel.IsVisible = false;
            NotifCont.NameLabel.IsVisible = true;
            NotifCont.DescriptionLabel.IsVisible = true;
            dfSpriteAnimation component = NotifCont.BoxSprite.GetComponent<dfSpriteAnimation>();
            component.Stop();
            dfSpriteAnimation component2 = NotifCont.CrosshairSprite.GetComponent<dfSpriteAnimation>();
            component2.Stop();
            dfSpriteAnimation component3 = NotifCont.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
            component3.Stop();

            UINotificationController.NotificationColor forcedColor = notifyParams.forcedColor;
            string trackableGuid = notifyParams.EncounterGuid;
            bool isGold = forcedColor == UINotificationController.NotificationColor.GOLD || (!string.IsNullOrEmpty(trackableGuid) && GameStatsManager.Instance.QueryEncounterable(trackableGuid) == 1);
            bool isPurple = forcedColor == UINotificationController.NotificationColor.PURPLE || (!string.IsNullOrEmpty(trackableGuid) && EncounterDatabase.GetEntry(trackableGuid).usesPurpleNotifications);
            NotifCont.ToggleGoldStatus(isGold);
            NotifCont.TogglePurpleStatus(isPurple);
            bool singleLineSansSprite = notifyParams.isSingleLine;
            if (singleLineSansSprite || notifyParams.SpriteCollection == null)
            {
                NotifCont.ObjectBoxSprite.IsVisible = false;
                NotifCont.StickerSprite.IsVisible = false;
            }
            if (singleLineSansSprite)
            {
                NotifCont.CenterLabel.IsVisible = true;
                NotifCont.NameLabel.IsVisible = false;
                NotifCont.DescriptionLabel.IsVisible = false;
                NotifCont.CenterLabel.Text = NotifCont.NameLabel.Text;
            }
            else
            {
                NotifCont.NameLabel.IsVisible = true;
                NotifCont.DescriptionLabel.IsVisible = true;
                NotifCont.CenterLabel.IsVisible = false;
            }
            NotifCont.m_doingNotification = true;
            NotifCont.m_panel.IsVisible = false;
            GameUIRoot.Instance.MoveNonCoreGroupOnscreen(NotifCont.m_panel, false);
            float elapsed = 0f;
            float duration = 5f;
            bool hasPlayedAnim = false;
            if (singleLineSansSprite)
            {
                NotifCont.notificationObjectSprite.renderer.enabled = false;
                SpriteOutlineManager.ToggleOutlineRenderers(NotifCont.notificationObjectSprite, false);
            }
            while (elapsed < ((!notifyParams.HasAttachedSynergy) ? duration : (duration - 2f)))
            {
                elapsed += BraveTime.DeltaTime;
                if (!hasPlayedAnim && elapsed > 0.75f)
                {
                    NotifCont.BoxSprite.GetComponent<dfSpriteAnimation>().Clip = ((!isPurple) ? ((!isGold) ? NotifCont.SilverAnimClip : NotifCont.GoldAnimClip) : NotifCont.PurpleAnimClip);
                    hasPlayedAnim = true;
                    NotifCont.ObjectBoxSprite.Parent.GetComponent<dfSpriteAnimation>().Play();
                }
                yield return null;
                NotifCont.m_panel.IsVisible = true;
                if (!singleLineSansSprite && notifyParams.SpriteCollection != null)
                {
                    NotifCont.notificationObjectSprite.renderer.enabled = true;
                    SpriteOutlineManager.ToggleOutlineRenderers(NotifCont.notificationObjectSprite, true);
                }
            }
            if (notifyParams.HasAttachedSynergy)
            {
                EncounterDatabaseEntry encounterSource = EncounterDatabase.GetEntry(trackableGuid);
                int pickupObjectId = (encounterSource == null) ? -1 : encounterSource.pickupObjectId;
                PickupObject puo = PickupObjectDatabase.GetById(pickupObjectId);
                if (puo)
                {
                    int pID = ModulePrinterCore.ModulePrinterCoreID;
                    PickupObject puo2 = PickupObjectDatabase.GetById(pID);
                    if (puo2 && puo2.sprite)
                    {
                        NotifCont.SetupSynergySprite(puo2.sprite.Collection, puo2.sprite.spriteId);
                        elapsed = 0f;
                        duration = 4f;
                        NotifCont.notificationSynergySprite.renderer.enabled = true;
                        SpriteOutlineManager.ToggleOutlineRenderers(NotifCont.notificationSynergySprite, true);
                        dfSpriteAnimation boxSpriteAnimator = NotifCont.BoxSprite.GetComponent<dfSpriteAnimation>();
                        boxSpriteAnimator.Clip = NotifCont.SynergyTransformClip;
                        boxSpriteAnimator.Play();
                        dfSpriteAnimation crosshairSpriteAnimator = NotifCont.CrosshairSprite.GetComponent<dfSpriteAnimation>();
                        crosshairSpriteAnimator.Clip = NotifCont.SynergyCrosshairTransformClip;
                        crosshairSpriteAnimator.Play();
                        dfSpriteAnimation objectSpriteAnimator = NotifCont.ObjectBoxSprite.GetComponent<dfSpriteAnimation>();
                        objectSpriteAnimator.Clip = NotifCont.SynergyBoxTransformClip;
                        objectSpriteAnimator.Play();
                        string synergyName = ModularSynergy.Get_Synergy_Name(pickupObjectId);
                        bool synergyHasName = !string.IsNullOrEmpty(synergyName);
                        if (synergyHasName)
                        {
                            NotifCont.CenterLabel.IsVisible = true;
                            NotifCont.CenterLabel.Text = synergyName;
                        }
                        while (elapsed < duration)
                        {
                            float baseSpriteLocalX = NotifCont.notificationObjectSprite.transform.localPosition.x;
                            float synSpriteLocalX = NotifCont.notificationSynergySprite.transform.localPosition.x;
                            NotifCont.CrosshairSprite.Size = NotifCont.CrosshairSprite.SpriteInfo.sizeInPixels * 3f;
                            float p2u = NotifCont.BoxSprite.PixelsToUnits();
                            Vector3 endPosition = NotifCont.ObjectBoxSprite.GetCenter();
                            Vector3 startPosition = endPosition + new Vector3(0f, -120f * p2u, 0f);
                            Vector3 startPosition2 = endPosition;
                            Vector3 endPosition2 = endPosition + new Vector3(0f, 12f * p2u, 0f);
                            endPosition -= new Vector3(0f, 21f * p2u, 0f);
                            float t = elapsed / duration;
                            float quickT = elapsed / 1f;
                            float smoothT = Mathf.SmoothStep(0f, 1f, quickT);
                            if (synergyHasName)
                            {
                                float num = Mathf.SmoothStep(0f, 1f, elapsed / 0.5f);
                                float opacity = Mathf.SmoothStep(0f, 1f, (elapsed - 0.5f) / 0.5f);
                                NotifCont.NameLabel.Opacity = 1f - num;
                                NotifCont.DescriptionLabel.Opacity = 1f - num;
                                NotifCont.CenterLabel.Opacity = opacity;
                            }
                            Vector3 t2 = Vector3.Lerp(startPosition, endPosition, smoothT).Quantize(p2u * 3f).WithX(startPosition.x);
                            Vector3 t3 = Vector3.Lerp(startPosition2, endPosition2, smoothT).Quantize(p2u * 3f).WithX(startPosition2.x);
                            t3.y = Mathf.Max(startPosition2.y, t3.y);
                            NotifCont.notificationSynergySprite.PlaceAtPositionByAnchor(t2, tk2dBaseSprite.Anchor.MiddleCenter);
                            NotifCont.notificationSynergySprite.transform.position = NotifCont.notificationSynergySprite.transform.position + new Vector3(0f, 0f, -0.125f);
                            NotifCont.notificationObjectSprite.PlaceAtPositionByAnchor(t3, tk2dBaseSprite.Anchor.MiddleCenter);
                            NotifCont.notificationObjectSprite.transform.localPosition = NotifCont.notificationObjectSprite.transform.localPosition.WithX(baseSpriteLocalX);
                            NotifCont.notificationSynergySprite.transform.localPosition = NotifCont.notificationSynergySprite.transform.localPosition.WithX(synSpriteLocalX);
                            NotifCont.notificationSynergySprite.UpdateZDepth();
                            NotifCont.notificationObjectSprite.UpdateZDepth();
                            elapsed += BraveTime.DeltaTime;
                            yield return null;
                        }
                    }
                }
            }
            GameUIRoot.Instance.MoveNonCoreGroupOnscreen(NotifCont.m_panel, true);
            elapsed = 0f;
            duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            NotifCont.CenterLabel.Opacity = 1f;
            NotifCont.NameLabel.Opacity = 1f;
            NotifCont.DescriptionLabel.Opacity = 1f;
            NotifCont.CenterLabel.IsVisible = false;
            NotifCont.NameLabel.IsVisible = true;
            NotifCont.DescriptionLabel.IsVisible = true;
            NotifCont.DisableRenderers();
            NotifCont.m_doingNotification = false;
            yield break;
        }

        public class SynergyMarker : MonoBehaviour { }

        public static List<ModularSynergy> synergizing_Items = new List<ModularSynergy>();

        public class ModularSynergy
        {
            public ModularSynergy(string syn, string consoleName)
            {
                item_Id = Gungeon.Game.Items[consoleName].PickupObjectId;
                synergy_Name = syn;
                var obj = PickupObjectDatabase.GetById(item_Id);
                obj.gameObject.AddComponent<SynergyMarker>();
                Alexandria.ItemAPI.CustomSynergies.Add(syn, new List<string> { "mdl:modular_printer_core" }, new List<string> { consoleName }, true);

            }

            public static string Get_Synergy_Name(int ID)
            {
                foreach (var entry in synergizing_Items)
                {
                    if (entry.item_Id == ID) { return entry.synergy_Name; }
                }
                return "ERROR";
            }

            public static bool isSynergyItem(int ID)
            {
                foreach (var entry in synergizing_Items)
                {
                    if (entry.item_Id == ID) { return true; }
                }
                return false;
            }


            public bool ModuleSynergyIsAvailable(PlayerController p)
            {
                return this.PlayerHasPickup(p, item_Id);
            }

            public bool PlayerHasPickup(PlayerController p, int pickupID)
            {
                if (p && p.inventory != null && p.inventory.AllGuns != null)
                {
                    for (int i = 0; i < p.inventory.AllGuns.Count; i++)
                    {
                        if (p.inventory.AllGuns[i].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                }
                if (p)
                {
                    for (int j = 0; j < p.activeItems.Count; j++)
                    {
                        if (p.activeItems[j].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                    for (int k = 0; k < p.passiveItems.Count; k++)
                    {
                        Debug.Log("fuck5");
                        if (p.passiveItems[k].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                    if (pickupID == GlobalItemIds.Map && p.EverHadMap)
                    {
                        return true;
                    }
                }
                return false;
            }
            public int item_Id;
            public string synergy_Name;
        }
    }
}
