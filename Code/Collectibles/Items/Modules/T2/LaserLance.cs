using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class LaserLance : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LaserLance))
        {
            Name = "Laser Lance",
            Description = "Pointy",
            LongDescription = "Reduces projectile speed by 50%. Projectiles will now fire out short-range, damaging beams that point in the direction the projectile is moving.. (+Beam Damage And Range per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("laserlance_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("laserlance_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Laser Lance " + h.ReturnTierLabel();
            h.LabelDescription = "Reduces projectile speed by 50%.\nProjectiles will now fire out short-range damaging beams\nthat point in the direction the projectile is moving.\n(" + StaticColorHexes.AddColorToLabelString("+Beam Damage And Range", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.9f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
            BasicBeamController beamComp = projectile.GenerateBeamPrefabBundle(
                "lancebeam_impact_001",
                StaticCollections.Beam_Collection,
                StaticCollections.Beam_Animation,
                "lancebeam_idle",
                new Vector2(6, 6),
                new Vector2(0, 0),
                "lancebeam_impact",
                new Vector2(6, 6),
                new Vector2(0, 0),
                "lancebeam_end",
                new Vector2(6, 6),
                new Vector2(0, 0),
                "lancebeam_idle", new Vector2(6, 6), new Vector2(0, 0), false);
            EmmisiveBeams emission = projectile.gameObject.GetOrAddComponent<EmmisiveBeams>();
            emission.EmissivePower = 20;
            emission.EmissiveColorPower = 20;
            projectile.baseData.damage = 26;
            projectile.baseData.range = 2f;
            projectile.baseData.speed = 10;
            projectile.baseData.force = 0;

            BounceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces = 1;
            PierceProjModifier pierceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceProjModifier.penetration = 3;
            pierceProjModifier.preventPenetrationOfActors = false;


            FakePrefab.MakeFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile);

            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.interpolateStretchedBones = false;
            beamComp.ContinueBeamArtToWall = true;
            beamComp.penetration = 3;

            LanceBeam = beamComp.projectile;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static Projectile LanceBeam;


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.01f) { return; }
            int stack = 1;
            p.baseData.speed *= 0.5f;
            p.UpdateSpeed();

            BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(LanceBeam, player, p.gameObject, p.gameObject.transform.PositionVector2(), false, p.angularVelocity, 100);
            Projectile component3 = beamController3.GetComponent<Projectile>();
            float Dmg = p.baseData.damage * player.stats.GetStatValue(PlayerStats.StatType.Damage);
            component3.baseData.damage = ((5f + (stack*5))+(p.baseData.damage / 5)) + (p.baseData.damage * (Dmg));//(p.baseData.damage * (Dmg) * 1 + (0.5f * stack)) / 10f;
            component3.AdditionalScaleMultiplier *= 0.5f;
            component3.baseData.range *= stack;

            var point = p.gameObject.AddComponent<BeamPointer>();
            point.self = p;
            point.beam = beamController3;

            var t = p.gameObject.GetComponent<BounceProjModifier>();
            if (t == null)
            {
                BounceProjModifier bounceProjModifier = p.gameObject.AddComponent<BounceProjModifier>();
                bounceProjModifier.numberOfBounces = 1;
            }
            else
            {
                t.numberOfBounces += 1;
            }

            var mod = p.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = false, CanStickEnemies = false });
            mod.OnStick += OPSA;
            mod.OnStickToWall += OPSAT;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnProjectileStickAction += OPSA;
            modulePrinter.OnProjectileStickToWallAction += OPSAT;

        }

        public void OPSA(GameObject s, StickyProjectileModifier mod, tk2dBaseSprite sprite, PlayerController p)
        {
            if (s.GetComponent<BeamPointer>())
            {
                s.GetComponent<BeamPointer>().OnStuck(s);
            }
        }
        public void OPSAT(GameObject s, StickyProjectileModifier mod, tk2dBaseSprite sprite, PlayerController p, PhysicsEngine.Tile t)
        {
            if (s.GetComponent<BeamPointer>())
            {
                s.GetComponent<BeamPointer>().OnStuck(s, t);
            }
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnProjectileStickAction -= OPSA;
            modulePrinter.OnProjectileStickToWallAction -= OPSAT;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.speed *= 0.5f;
            p.UpdateSpeed();

            BeamController beamController3 = BeamToolbox.FreeFireBeamFromAnywhere(LanceBeam, player, p.gameObject, p.gameObject.transform.PositionVector2(), false, p.angularVelocity, 100);
            Projectile component3 = beamController3.GetComponent<Projectile>();
            float Dmg = p.baseData.damage * player.stats.GetStatValue(PlayerStats.StatType.Damage);
            component3.baseData.damage = ((5f + (stack * 5)) + (p.baseData.damage / 5)) + (p.baseData.damage * (Dmg));//(p.baseData.damage * (Dmg * 4f) * 1 + (0.5f * stack)) / 10f;
            component3.AdditionalScaleMultiplier *= 0.5f;
            component3.baseData.range *= stack;

            var point = p.gameObject.AddComponent<BeamPointer>();
            point.self = p;
            point.Object = p.gameObject;
            point.beam = beamController3;
            var t = p.gameObject.GetComponent<BounceProjModifier>();
            if (t == null)
            {
                BounceProjModifier bounceProjModifier = p.gameObject.AddComponent<BounceProjModifier>();
                bounceProjModifier.numberOfBounces = 1;
            }
            else
            {
                t.numberOfBounces += 1;
            }
        }
    }

    public class BeamPointer : MonoBehaviour
    {
        public GameObject Object;
        public Projectile self;
        public BeamController beam;

        public void Start(){}


        public void OnStuck(GameObject p, PhysicsEngine.Tile t = null)
        {
            FUCK = true;
            if (beam)
            {
                Vector2 vector2 = t != null ? GameManager.Instance.Dungeon.data.tilemap.GetTilePosition(t.X, t.Y) : new Vector3(-60, 60);
                beam.Direction = Toolbox.GetUnitOnCircle(t != null ? BraveMathCollege.GetNearestAngle((p.transform.PositionVector2() - vector2).ToAngle(), new float[] {0,90,180,270}) : beam.Direction.ToAngle() + 180, 1);
            }
        }
        public bool FUCK = false;
        public void Update()
        {
            if (FUCK == false && beam != null && Object != null)
            {
                beam.Direction = Toolbox.GetUnitOnCircle(self.LastVelocity.ToAngle(), 1);

            }
            else if (Object == null && beam != null)
            {
                beam.CeaseAttack();
            }
        }
    }
}

