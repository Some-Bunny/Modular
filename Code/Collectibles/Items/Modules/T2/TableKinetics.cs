using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class TableKinetics : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TableKinetics))
        {
            Name = "Table Kinetics",
            Description = "Table-tic Railgun",
            LongDescription = "Enemies spawn tables when killed,\\nand chance to spawn a table upon reloading. Projectiles will now pierce tables, and get a 25% (+25% per stack) damage and speed boost. Piercing a table makes all other unflipped tables in the room do a shockwave (+Shockwave Damage).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("tablekinetics_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("tablekinetics_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Table Kinetics " + h.ReturnTierLabel();
            h.LabelDescription = "Enemies spawn tables when killed,\nand chance to spawn a table upon reloading.\nProjectiles will now pierce tables,\nand get a 25% ("+ StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ") damage and speed boost.\nPiercing a table makes all other unflipped tables\nin the room do a shockwave ("+ StaticColorHexes.AddColorToLabelString("+Shockwave Damage", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AdditionalWeightMultiplier = 0.75f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;

            (v as TableKinetics).tableToInstantiate = (PickupObjectDatabase.GetById(644) as FoldingTableItem).TableToSpawn.gameObject;
            (v as TableKinetics).boostVFX = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
        }
        public static int ID;
        public GameObject tableToInstantiate;
        public GameObject boostVFX;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy += OKE;
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnGunReloaded += OGR;
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            if (UnityEngine.Random.value > g.PercentageOfClipLeft())
            {
                AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", player.gameObject);
                Vector2 nearbyPoint = player.CenterPosition + (player.unadjustedAimPoint.XY() - player.CenterPosition).normalized;
                IntVector2? nearestAvailableCell = player.CurrentRoom.GetNearestAvailableCell(nearbyPoint, new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), false, null);
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(tableToInstantiate, nearestAvailableCell.Value.ToVector2(), Quaternion.identity);
                SpeculativeRigidbody componentInChildren = gameObject.GetComponentInChildren<SpeculativeRigidbody>();
                FlippableCover component = gameObject.GetComponent<FlippableCover>();
                component.transform.position.XY().GetAbsoluteRoom().RegisterInteractable(component);
                component.ConfigureOnPlacement(component.transform.position.XY().GetAbsoluteRoom());
                componentInChildren.Initialize();
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(componentInChildren, null, false);

            }
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.specRigidbody.OnPreRigidbodyCollision += HandlePreCollision;
        }
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.gameObject.name != null)
            {
                if (otherRigidbody.gameObject.name == "Table_Vertical" || otherRigidbody.gameObject.name == "Table_Horizontal")
                {
                    if (myRigidbody.projectile)
                    {
                        myRigidbody.projectile.baseData.damage *= 1 + (0.25f * this.ReturnStack(Stored_Core));
                        myRigidbody.projectile.baseData.speed *= 1 + (0.25f * this.ReturnStack(Stored_Core));
                        myRigidbody.projectile.baseData.force *= 1 + (0.5f * this.ReturnStack(Stored_Core));
                        myRigidbody.projectile.UpdateSpeed();
                        myRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 0.4f);
                        AllTables(otherRigidbody.transform.position.GetAbsoluteRoom());

                        if (ConfigManager.DoVisualEffect == true)
                        {
                            GameObject vfx = SpawnManager.SpawnVFX(boostVFX, true);
                            vfx.transform.position = myRigidbody.projectile.transform.position;
                            vfx.transform.localRotation = Quaternion.Euler(0f, 0f, myRigidbody.projectile.LastVelocity.ToAngle());
                            vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;

                            Destroy(vfx, 2);
                        }
                    }
                    PhysicsEngine.SkipCollision = true;
                }
            }

           
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnKilledEnemy -= OKE;
            modulePrinter.OnGunReloaded -= OGR;

        }

        public void OKE(ModulePrinterCore modulePrinter, PlayerController player, AIActor aIActor)
        {
            AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", player.gameObject);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(tableToInstantiate, aIActor.transform.PositionVector2(), Quaternion.identity);
            SpeculativeRigidbody componentInChildren = gameObject.GetComponentInChildren<SpeculativeRigidbody>();
            FlippableCover component = gameObject.GetComponent<FlippableCover>();
            component.transform.position.XY().GetAbsoluteRoom().RegisterInteractable(component);
            component.ConfigureOnPlacement(component.transform.position.XY().GetAbsoluteRoom());
            componentInChildren.Initialize();
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(componentInChildren, null, false);
        }




        public void AllTables(RoomHandler r)
        {
            ReadOnlyCollection<IPlayerInteractable> roomInteractables = r.GetRoomInteractables();
            for (int i = 0; i < roomInteractables.Count; i++)
            {
                if (r.IsRegistered(roomInteractables[i]))
                {
                    FlippableCover flippableCover = roomInteractables[i] as FlippableCover;
                    if (flippableCover != null)
                    {
                        int power = this.ReturnStack(Stored_Core);
                        if (ConfigManager.DoVisualEffect == true)
                        {
                            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, flippableCover.sprite.WorldCenter, Quaternion.identity);
                            blankObj.transform.localScale = Vector3.one * 0.5f;
                            Destroy(blankObj, 2f);
                        }

                        Exploder.DoRadialPush(flippableCover.sprite.WorldCenter, 40 * power, 3);
                        Exploder.DoRadialKnockback(flippableCover.sprite.WorldCenter, 40 * power, 3);
                        Exploder.DoRadialMinorBreakableBreak(flippableCover.sprite.WorldCenter, 3);
                        ApplyActionToNearbyEnemies(flippableCover.sprite.WorldCenter, 3, r, power);
                    }             
                }
            }
        }

        public void ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room, float m)
        {
            List<AIActor> a = new List<AIActor>();
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        AIActor aI = room.activeEnemies[i];
                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            aI.healthHaver.ApplyDamage(8 * m, aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }
    }
}

