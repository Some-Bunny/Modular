using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class FocusingLens : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(FocusingLens))
        {
            Name = "Focusing Lens",
            Description = "Focus",
            LongDescription = "Deal 50% (+50% per stack) more damage to nearby enemies." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("focusingkens_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("focusingkens_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.66f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Focusing Lens " + h.ReturnTierLabel();
            h.LabelDescription = "Deal 50% (" + StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + ") more damage\nto nearby enemies.";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            pinkImpact = (PickupObjectDatabase.GetById(58) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static VFXPool pinkImpact;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.OnPostProcessProjectile -= PPP;
            ring.StartCoroutine(DoRingLerp(ring, 4.5f, 0f, true));
        }

        public float Mult;
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
            Mult = 1 + (stack * 0.5f);
        }


        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);

            if (ring == null)
            {
                ring = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), player.transform.position, Quaternion.identity, player.transform)).GetComponent<HeatIndicatorController>();
                ring.CurrentRadius = 4.5f;
                ring.CurrentColor = new Color(255, 56, 152).WithAlpha(0.02f);
                ring.IsFire = false;
                ring.GetComponent<MeshRenderer>().material.SetFloat("_PxWidth", 0.002f);
                ring.transform.position = player.sprite.WorldCenter.ToVector3ZUp(100);          
                ring.StartCoroutine(DoRingLerp(ring, 0, 4.5f));
                //ring.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            }
            if (player.CurrentRoom != null)
            {nearbyEnemies = ApplyActionToNearbyEnemies(player.sprite.WorldCenter, 4.5f, player.CurrentRoom);}
        }

        public IEnumerator DoRingLerp(HeatIndicatorController a, float from, float to, bool Destroys = false)
        {
            float e = 0;
            while (e < 1)
            {
                if (a == null) { yield break; }
                e += BraveTime.DeltaTime;
                a.CurrentRadius = Mathf.Lerp(from, to, Toolbox.SinLerpTValue(e));
                yield return null;
            }
            if (Destroys == true) { Destroy(a.gameObject); }
            yield break;
        }


        public List<AIActor> ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room)
        {
            List<AIActor> a = new List<AIActor>();
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            a.Add(room.activeEnemies[i]);
                        }
                    }
                }
            }
            return a;
        }


        private List<AIActor> nearbyEnemies = new List<AIActor>();
        private HeatIndicatorController ring;

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
        }

        public void OPC(SpeculativeRigidbody mR, PixelCollider mP, SpeculativeRigidbody oR, PixelCollider oP)
        {
            if (oR.aiActor != null && oR.healthHaver != null && mR.projectile != null)
            {
                if (nearbyEnemies.Contains(oR.aiActor))
                {
                    float damage = mR.projectile.baseData.damage;
                    mR.projectile.baseData.damage *= Mult;
                    mR.projectile.StartCoroutine(FrameDelay(mR.projectile, damage));
                }
            }
        }

        public IEnumerator FrameDelay(Projectile p, float DmG)
        {
            VFXPool Ef = p.hitEffects.enemy;
            VFXPool deathEf =p.hitEffects.deathEnemy;
            p.hitEffects.enemy = pinkImpact;
            p.hitEffects.deathEnemy = pinkImpact;
            yield return null;
            p.baseData.damage = DmG;
            p.hitEffects.enemy = Ef;
            p.hitEffects.deathEnemy = deathEf;
            yield break;
        }

    } 
}

