using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class JetPropellant : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(JetPropellant))
        {
            Name = "Jet Propellant",
            Description = "Speed Is War",
            LongDescription = "Projectiles start slow, but accelarate to high speeds after 1 second. Projectiles now deal 20% (+20% per stack) of their speed as damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("jetpropellant_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("jetpropellant_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Jet Propellant " + h.ReturnTierLabel();
            h.LabelDescription = "Projectiles start slow, but accelerate to\nhigh speeds after 1 second.\nProjectiles now deal 12.5% (" + StaticColorHexes.AddColorToLabelString("+12.5%", StaticColorHexes.Light_Orange_Hex) +") of their speed as damage.";
            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 0.9f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            var RPGParticlesvar = (PickupObjectDatabase.GetById(39) as Gun).DefaultModule.projectiles[0].transform.Find("VFX_Rocket_Exhaust_Fire").gameObject;

            GameObject particleObject = UnityEngine.Object.Instantiate<GameObject>(RPGParticlesvar);
            particleObject.transform.parent = null;
            particleObject.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(particleObject);
            UnityEngine.Object.DontDestroyOnLoad(particleObject);

            RPGParticles = particleObject;
            curve = new AnimationCurve()
            {
                postWrapMode = WrapMode.ClampForever,
                keys = new Keyframe[] {
                new Keyframe(){time = 0, value = 0.05f, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.5f, value = 0.1f, inTangent = 0.75f, outTangent = 0.25f},
                new Keyframe(){time = 0.95f, value = 1.33f, inTangent = 0.75f, outTangent = 0.25f},
                }
            };
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;
        public static AnimationCurve curve;
        public static GameObject RPGParticles;
        public int stack = 0;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            stack = this.ReturnStack(modulePrinter);
        }


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.1f) { return; }
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
            p.baseData.UsesCustomAccelerationCurve = true;
            p.baseData.AccelerationCurve = curve;
            var trail = p.gameObject.AddComponent<TrailRocketController>();
            trail.self = p;
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
            p.baseData.UsesCustomAccelerationCurve = true;
            p.baseData.AccelerationCurve = curve;
            var trail = p.gameObject.AddComponent<TrailRocketController>();
            trail.self = p;
        }
        public void OPC(SpeculativeRigidbody mR, PixelCollider mP, SpeculativeRigidbody oR, PixelCollider oP)
        {
            if (oR.aiActor != null && oR.healthHaver != null && mR.projectile != null)
            {
                float damage = mR.projectile.baseData.damage;
                float damageMult = (mR.projectile.baseData.speed / 1000) * (1.25f * stack);
                mR.projectile.baseData.damage *= 1 + damageMult;
                mR.projectile.StartCoroutine(FrameDelay(mR.projectile, damage));
            }
        }
        public IEnumerator FrameDelay(Projectile p, float DmG)
        {
            yield return null;
            p.baseData.damage = DmG;
        }
        public class TrailRocketController : MonoBehaviour
        {
            public Projectile self;
            GameObject particleObject;

            public void Start()
            {
                particleObject = UnityEngine.Object.Instantiate<GameObject>(JetPropellant.RPGParticles);
                particleObject.transform.parent = self.transform;
                particleObject.transform.localPosition = new Vector3(0, 0);
            }
            public void Update()
            {
                if (particleObject == null || self == null) { return; }
                particleObject.transform.localRotation = self.transform.localRotation;
            }
        }
    }
}

