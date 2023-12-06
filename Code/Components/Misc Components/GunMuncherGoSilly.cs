using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace ModularMod.Code.Components.Misc_Components
{
    public class GunMuncherGoSilly : MonoBehaviour
    {
        public GunberMuncherController muncherController;
        public int Jumps = 2;
        public Scrapper scrapper;
        private bool CanJump = true;
        private bool isMonologued = false;

        public void Start()
        {
            UnityEngine.Random.Range(1, 5);
        }

        public void DoJump()
        {
            if (CanJump == false) { return; }
            scrapper.gunberMunchers.Remove(muncherController);
            Jumps--;
            RemoveIcon();
            CanJump = false;
            if (Jumps == 0)
            {
                if (muncherController.RequiredNumberOfGuns == 2)
                {
                    muncherController.aiAnimator.PlayUntilFinished("activate", false, null, -1f, false);
                    this.StartCoroutine(DoTalkGreen());
                }
                else
                {
                    this.StartCoroutine(DoTalkRed());
                }
                return;
            }
            var room = muncherController.transform.position.GetAbsoluteRoom();
            if (room.IsRegistered(muncherController.GetComponent<IPlayerInteractable>()))
            {
                room.DeregisterInteractable(muncherController.GetComponent<IPlayerInteractable>());
            }
            this.StartCoroutine(DoHop());

        }
        public IEnumerator DoTalkGreen()
        {
            var room = muncherController.transform.position.GetAbsoluteRoom();
            if (room.IsRegistered(muncherController.GetComponent<IPlayerInteractable>()))
            {
                room.DeregisterInteractable(muncherController.GetComponent<IPlayerInteractable>());
            }

            float e1 = 0;
            muncherController.aiAnimator.PlayUntilFinished("activate", false, null, -1f, false);
            muncherController.GetComponent<TalkDoerLite>().ForceTimedSpeech("Ok I get it!", 0f, 2, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
            while (e1 < 2.2f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            muncherController.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
            muncherController.GetComponent<TalkDoerLite>().ForceTimedSpeech("...", 0f, 2, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
            e1 = 0;
            while (e1 < 2)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            muncherController.GetComponent<TalkDoerLite>().ForceTimedSpeech("How about...", 0f, 2, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
            e1 = 0;
            while (e1 < 3)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            muncherController.GetComponent<TalkDoerLite>().ForceTimedSpeech("Heres what I'll do.", 0f, 2.5f, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
            e1 = 0;
            while (e1 < 3f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            muncherController.aiAnimator.PlayUntilFinished("activate", false, null, -1f, false);
            e1 = 0;
            while (e1 < 6f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }


            int amount = UnityEngine.Random.Range(2, 6);
            for (int i = 0; i < amount; i++)
            {
                GameObject itemForPlayer = PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                tk2dBaseSprite component = itemForPlayer.GetComponent<tk2dBaseSprite>();
                Vector2 b = Vector2.zero;
                if (component != null)
                {
                    b = -1f * component.GetBounds().center.XY();
                }
                DebrisObject debrisObject = LootEngine.SpawnItem(itemForPlayer, muncherController.sprite.WorldCenter + b, Vector2.down, 4, true, false, false);
                debrisObject.bounceCount = 0;
                DebrisObject debrisObject2 = debrisObject;
                debrisObject2.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject2.OnGrounded, new Action<DebrisObject>(muncherController.DoSteamOnGrounded));
            }
            isMonologued = true;

            e1 = 0;
            while (e1 < 1.25f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            e1 = 0;
            muncherController.aiAnimator.PlayUntilFinished("activate", false, null, -1f, false);
            muncherController.GetComponent<TalkDoerLite>().ForceTimedSpeech("Now leave me alone.", 0f, 2, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
            while (e1 < 3f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            muncherController.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
            e1 = 0;
            while (e1 < 2)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            var currentPosition = this.transform.position;
            var newPosition = this.transform.position + Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 30);

            var Distance = Vector2.Distance(currentPosition, newPosition);
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_jump_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_blobulord_leap_01", base.gameObject);
            Exploder.DoDistortionWave(muncherController.sprite.WorldCenter, 0.2f, 0.25f, 5f, 0.5f);
            var afterImage = muncherController.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage.dashColor = new Color(0.3f, 0.3f, 0.3f);
            afterImage.shadowLifetime = 1;
            afterImage.shadowLifetime = 0.3f;
            e1 = 0;

            var obj = UnityEngine.Object.Instantiate(VFXStorage.MachoBraceDustupVFX, muncherController.sprite.WorldBottomCenter, Quaternion.identity);
            Destroy(obj, 3);
            float d = ((Distance) / 64) + 1;
            while (e1< d)
            {
                e1 += BraveTime.DeltaTime;
                Vector3 position1 = Vector2.Lerp(currentPosition, newPosition, e1 / d);
                position1 += new Vector3(0, (Distance / 3) * Toolbox.SinLerpTValue(e1 / d));
                muncherController.transform.position = position1;
                yield return null;
            }
            Destroy(this.gameObject);


            yield break;
        }

        public IEnumerator DoTalkRed()
        {
            TalkDoerLite talker = muncherController.GetComponent<TalkDoerLite>();

            var room = muncherController.transform.position.GetAbsoluteRoom();
            if (room.IsRegistered(muncherController.GetComponent<IPlayerInteractable>()))
            {
                room.DeregisterInteractable(muncherController.GetComponent<IPlayerInteractable>());
            }
            float e1 = 0;

            TextBoxManager.ShowTextBox(talker.speakPoint.position + new Vector3(0f, 0f, -4f), talker.speakPoint, 3, ". . .", talker.audioCharacterSpeechTag, talker, TextBoxManager.BoxSlideOrientation.FORCE_LEFT, false, false);
            e1 = 0;
            while (e1 < 3)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }

            muncherController.aiAnimator.PlayUntilFinished("activate", false, null, -1f, false);
            e1 = 0;
            while (e1 < 6f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }

            GameObject itemForPlayer = GlobalModuleStorage.ReturnRandomModule(false).gameObject;// PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject;
            tk2dBaseSprite component = itemForPlayer.GetComponent<tk2dBaseSprite>();
            Vector2 b = Vector2.zero;
            if (component != null)
            {
                b = -1f * component.GetBounds().center.XY();
            }
            DebrisObject debrisObject = LootEngine.SpawnItem(itemForPlayer, muncherController.sprite.WorldCenter + b, Vector2.down, 4, true, false, false);
            debrisObject.bounceCount = 0;
            DebrisObject debrisObject2 = debrisObject;
            debrisObject2.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject2.OnGrounded, new Action<DebrisObject>(muncherController.DoSteamOnGrounded));
            isMonologued = true;
            e1 = 0;
            while (e1 < 1f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            TextBoxManager.ShowTextBox(talker.speakPoint.position + new Vector3(0f, 0f, -4f), talker.speakPoint, 2, "Asshole.", talker.audioCharacterSpeechTag, talker, TextBoxManager.BoxSlideOrientation.FORCE_LEFT, false, false);
            muncherController.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
            e1 = 0;
            while (e1 < 2.5f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }
            var currentPosition = this.transform.position;
            var newPosition = this.transform.position + Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 30);

            var Distance = Vector2.Distance(currentPosition, newPosition);
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_jump_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_blobulord_leap_01", base.gameObject);
            Exploder.DoDistortionWave(muncherController.sprite.WorldCenter, 0.2f, 0.25f, 5f, 0.5f);
            var afterImage = muncherController.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage.dashColor = new Color(0.3f, 0.3f, 0.3f);
            afterImage.shadowLifetime = 1;
            afterImage.shadowLifetime = 0.3f;
            e1 = 0;

            var obj = UnityEngine.Object.Instantiate(VFXStorage.MachoBraceDustupVFX, muncherController.sprite.WorldBottomCenter, Quaternion.identity);
            Destroy(obj, 3);
            float d = ((Distance) / 64) + 1;
            while (e1 < d)
            {
                e1 += BraveTime.DeltaTime;
                Vector3 position1 = Vector2.Lerp(currentPosition, newPosition, e1 / d);
                position1 += new Vector3(0, (Distance / 3) * Toolbox.SinLerpTValue(e1 / d));
                muncherController.transform.position = position1;
                yield return null;
            }
            Destroy(this.gameObject);


            yield break;
        }


        public void RemoveIcon()
        {
            var room = muncherController.transform.position.GetAbsoluteRoom();
            List<GameObject> ICONS = new List<GameObject>();
            var rTI = Minimap.Instance.roomToIconsMap;
            if (rTI.ContainsKey(room))
            {
                rTI.TryGetValue(room, out ICONS);
                if (ICONS != null && ICONS.Count > 0)
                {
                    for (int i = 0; i < ICONS.Count; i++)
                    {
                        if (ICONS[i].name.ToLower().Contains("muncher"))
                        {
                            Minimap.Instance.DeregisterRoomIcon(room, ICONS[i]);
                            return;
                        }
                    }
                }
            }
        }

        public IEnumerator DoHop()
        {
            TalkDoerLite talker = muncherController.GetComponent<TalkDoerLite>();
            TextBoxManager.ShowTextBox(talker.speakPoint.position + new Vector3(0f, 0f, -4f), talker.speakPoint, 2, "...", talker.audioCharacterSpeechTag, talker, TextBoxManager.BoxSlideOrientation.FORCE_LEFT, false, false);

            float e1 = 0;
            while (e1 < 2.25f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }

            CanJump = true;
            scrapper.gunberMunchers.Add(muncherController);
            yield break;
            /*
            muncherController.specRigidbody.enabled = false;
            var currentPosition = this.transform.position;
            RoomHandler room;
            var rooms = this.transform.position.GetAbsoluteRoom().connectedRooms;
            var r = rooms.Where(self => self.IsVisible);
            if (r == null || r.Count () == 0 || r.ToList().Count() == 0)
            {
                room = currentPosition.GetAbsoluteRoom();
            }
            else
            {
                room = r.ToList()[UnityEngine.Random.Range(0, r.Count())];
            }
            var c = room.GetRandomAvailableCell(new IntVector2(2,2));
            float e = 0;
            var Distance = Vector2.Distance(currentPosition, c.Value.ToCenterVector2());
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_jump_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_blobulord_leap_01", base.gameObject);
            Exploder.DoDistortionWave(muncherController.sprite.WorldCenter, 0.2f, 0.25f, 5f, 0.5f);
            var afterImage = muncherController.gameObject.AddComponent<ImprovedAfterImage>();
            afterImage.dashColor = new Color(0.3f, 0.3f, 0.3f);
            afterImage.shadowLifetime = 1;
            afterImage.shadowLifetime = 0.3f;

            var obj = UnityEngine.Object.Instantiate(VFXStorage.MachoBraceDustupVFX, muncherController.sprite.WorldBottomCenter, Quaternion.identity);
            Destroy(obj, 3);
            float d = ((Distance) / 64) + 1;
            while (e < d)
            {
                e += BraveTime.DeltaTime;
                Vector3 position1 = Vector2.Lerp(currentPosition, c.Value.ToCenterVector2(), e / d);
                position1 += new Vector3(0, (Distance / 3) * Toolbox.SinLerpTValueFull(e / d));
                muncherController.transform.position = position1;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_land_01", base.gameObject);
            var obj2 = UnityEngine.Object.Instantiate(VFXStorage.DragunBoulderLandVFX, muncherController.sprite.WorldBottomCenter, Quaternion.identity);
            Destroy(obj2, 3);
            Exploder.DoDistortionWave(muncherController.sprite.WorldCenter, 0.2f, 0.25f, 5f, 0.5f);
            e1 = 0;
            while (e1 < 0.25f)
            {
                e1 += BraveTime.DeltaTime;
                yield return null;
            }

            AkSoundEngine.PostEvent("Play_OBJ_boulder_break_01", base.gameObject);
            Destroy(afterImage, 4);
            if (!room.IsRegistered(muncherController.GetComponent<IPlayerInteractable>()))
            {
                room.RegisterInteractable(muncherController.GetComponent<IPlayerInteractable>());
            }
            muncherController.sprite.renderer.enabled = true;
            muncherController.specRigidbody.enabled = true;
            muncherController.specRigidbody.Reinitialize();
            Minimap.Instance.RegisterRoomIcon(base.transform.position.GetAbsoluteRoom(), (GameObject)ResourceCache.Acquire("Global Prefabs/Minimap_Muncher_Icon"), false);
            CanJump = true;
            scrapper.gunberMunchers.Add(muncherController);

            yield break;
            */
        }

        public void OnDestroy()
        {
            UnityEngine.Object.Instantiate(VFXStorage.TelefragVFX, muncherController.sprite.WorldCenter, Quaternion.identity);
            if (isMonologued == false)
            {


                if (muncherController.RequiredNumberOfGuns == 2)
                {
                    GameObject itemForPlayer = PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject;
                    tk2dBaseSprite component = itemForPlayer.GetComponent<tk2dBaseSprite>();
                    Vector2 b = Vector2.zero;
                    if (component != null)
                    {
                        b = -1f * component.GetBounds().center.XY();
                    }
                    DebrisObject debrisObject = LootEngine.SpawnItem(itemForPlayer, muncherController.sprite.WorldCenter + b, Vector2.down, 4, true, false, false);
                    debrisObject.bounceCount = 0;
                    DebrisObject debrisObject2 = debrisObject;
                    debrisObject2.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject2.OnGrounded, new Action<DebrisObject>(muncherController.DoSteamOnGrounded));
                }
                else
                {
                    GameObject itemForPlayer = GlobalModuleStorage.ReturnRandomModule(false).gameObject;
                    tk2dBaseSprite component = itemForPlayer.GetComponent<tk2dBaseSprite>();
                    Vector2 b = Vector2.zero;
                    if (component != null)
                    {
                        b = -1f * component.GetBounds().center.XY();
                    }
                    DebrisObject debrisObject = LootEngine.SpawnItem(itemForPlayer, muncherController.sprite.WorldCenter + b, Vector2.down, 4, true, false, false);
                    debrisObject.bounceCount = 0;
                    DebrisObject debrisObject2 = debrisObject;
                    debrisObject2.OnGrounded = (Action<DebrisObject>)Delegate.Combine(debrisObject2.OnGrounded, new Action<DebrisObject>(muncherController.DoSteamOnGrounded));
                }
            }
        }
    }
}
