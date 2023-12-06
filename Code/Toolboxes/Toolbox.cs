using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Alexandria;
using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using FullInspector;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using Planetside;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = System.Random;

namespace ModularMod
{
    public static class Toolbox
    {

        public static float NearestCardinalWallAngle(this Vector2 pos, float minDistance, float maxDistance = 200)
        {
            Dictionary<float, float> f1 = new Dictionary<float, float>();
            List<float> f = new List<float>();

            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.PlayerHitBox, CollisionLayer.BulletBreakable);
            for (int i = 0; i < 4; i++)
            {
                RaycastResult hit;
                if (PhysicsEngine.Instance.Raycast(
                pos + BraveMathCollege.DegreesToVector(i * 90, minDistance), BraveMathCollege.DegreesToVector(i * 90), maxDistance, out hit,
                    false, true, rayMask2, null, false, rigidbodyExcluder))
                {
                    f1.Add(hit.Distance, i * 90);
                    f.Add(hit.Distance);
                }
                else
                {

                }
                RaycastResult.Pool.Free(ref hit);
                
            }
            f.Sort();
            float g;
            f1.TryGetValue(f[0], out g);
            return g;//f[0];
        }

        public static Vector3 AxisRound(this Vector3 vector, Transform relativeTo = null)
        {
            if (relativeTo)
            {
                vector = relativeTo.InverseTransformDirection(vector);
            }
            int largestIndex = 0;
            for (int i = 1; i < 3; i++)
            {
                largestIndex = Mathf.Abs(i == 1 ? vector.x : vector.y) > Mathf.Abs(vector[largestIndex]) ? i : largestIndex;
            }
            float newLargest = vector[largestIndex] > 0 ? 1 : -1;
            vector = Vector3.zero;
            vector[largestIndex] = newLargest;
            if (relativeTo)
            {
                vector = relativeTo.TransformDirection(vector);
            }
            return vector;
        }

        //Thank you Pretzel, very cool!
        //Like actually :), this is cool
        public static Vector2 ToNearestWall(this Vector2 pos, out Vector2 normal, float angle, float minDistance = 1)
        {
            RaycastResult hit;
            Vector2 contact;
            if (PhysicsEngine.Instance.Raycast(
              pos + BraveMathCollege.DegreesToVector(angle, minDistance), BraveMathCollege.DegreesToVector(angle), 200, out hit,
              collideWithRigidbodies: false))
            {
                contact = hit.Contact;
                normal = hit.Normal;
            }
            else
            {
                contact = pos + BraveMathCollege.DegreesToVector(angle, minDistance);
                normal = Vector2.zero;
            }
            RaycastResult.Pool.Free(ref hit);
            return contact;
        }

        public static void ApplyStat(PlayerController player, PlayerStats.StatType statType, float amountToApply, StatModifier.ModifyMethod modifyMethod)
        {
            player.stats.RecalculateStats(player, false, false);
            StatModifier statModifier = new StatModifier()
            {
                statToBoost = statType,
                amount = amountToApply,
                modifyType = modifyMethod
            };
            player.ownerlessStatModifiers.Add(statModifier);
            player.stats.RecalculateStats(player, false, false);
        }
        public static DebrisObject GenerateDebrisObject(string sprite, tk2dSpriteCollectionData data, bool debrisObjectsCanRotate = true, float LifeSpanMin = 0.33f, float LifeSpanMax = 2f, float AngularVelocity = 540, float AngularVelocityVariance = 180f, tk2dSprite shadowSprite = null, float Mass = 1, string AudioEventName = null, GameObject BounceVFX = null, int DebrisBounceCount = 0, bool DoesGoopOnRest = false, GoopDefinition GoopType = null, float GoopRadius = 1f, bool usesWorldShader = true)
        {
            GameObject debrisObject = new GameObject(sprite);
            FakePrefab.MarkAsFakePrefab(debrisObject);
            UnityEngine.Object.DontDestroyOnLoad(debrisObject);
            tk2dSprite tk2dsprite = debrisObject.GetOrAddComponent<tk2dSprite>();
            tk2dsprite.collection = data;
            tk2dsprite.SetSprite(data.GetSpriteIdByName(sprite));

            DebrisObject DebrisObj = debrisObject.AddComponent<DebrisObject>();

            DebrisObj.canRotate = debrisObjectsCanRotate;
            DebrisObj.lifespanMin = LifeSpanMin;
            DebrisObj.lifespanMax = LifeSpanMax;
            DebrisObj.bounceCount = DebrisBounceCount;
            DebrisObj.angularVelocity = AngularVelocity;
            DebrisObj.angularVelocityVariance = AngularVelocityVariance;
            if (AudioEventName != null) { DebrisObj.audioEventName = AudioEventName; }
            if (BounceVFX != null) { DebrisObj.optionalBounceVFX = BounceVFX; }
            DebrisObj.sprite = tk2dsprite;
            DebrisObj.DoesGoopOnRest = DoesGoopOnRest;
            if (GoopType != null) { DebrisObj.AssignedGoop = GoopType; } else if (GoopType == null && DebrisObj.DoesGoopOnRest == true) { DebrisObj.DoesGoopOnRest = false; }
            DebrisObj.GoopRadius = GoopRadius;
            if (shadowSprite != null) { DebrisObj.shadowSprite = shadowSprite; }
            DebrisObj.inertialMass = Mass;
            if (usesWorldShader == true)
            {
                //DebrisObj.sprite.renderer.material.shader = worldShader;
                //DebrisObj.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            }
            return DebrisObj;
        }

        public static AdvancedSynergyEntry CopySynergy(string NameKey)
        {
            AdvancedSynergyEntry copyEntry = null;
            foreach (AdvancedSynergyEntry entry in GameManager.Instance.SynergyManager.synergies)
            {
                if (entry.NameKey == NameKey)
                {
                    copyEntry = new AdvancedSynergyEntry();
                    copyEntry.ActivationStatus = entry.ActivationStatus;
                    copyEntry.ActiveWhenGunUnequipped = entry.ActiveWhenGunUnequipped;
                    copyEntry.RequiresAtLeastOneGunAndOneItem = entry.RequiresAtLeastOneGunAndOneItem;
                    copyEntry.bonusSynergies = entry.bonusSynergies;
                    copyEntry.IgnoreLichEyeBullets = entry.IgnoreLichEyeBullets;
                    copyEntry.MandatoryGunIDs = entry.MandatoryGunIDs;
                    copyEntry.MandatoryItemIDs = entry.MandatoryItemIDs;
                    copyEntry.NameKey = entry.NameKey;
                    copyEntry.NumberObjectsRequired = entry.NumberObjectsRequired;
                    copyEntry.OptionalGunIDs = entry.OptionalGunIDs;
                    copyEntry.OptionalItemIDs = entry.OptionalItemIDs;
                    copyEntry.statModifiers = entry.statModifiers;
                    copyEntry.SuppressVFX = entry.SuppressVFX;
                }
                /*
                Debug.Log("======================");

                Debug.Log(entry.NameKey +" : ");
                Debug.Log(entry.ActivationStatus + " : ");
                Debug.Log(entry.NumberObjectsRequired + " : ");

                Debug.Log("\nMandatory Guns:\n=====");

                foreach (var e in entry.MandatoryGunIDs)
                {
                    Debug.Log("    "+e);
                }
                Debug.Log("\nMandatory Items:\n=====");

                foreach (var e in entry.MandatoryItemIDs)
                {
                    Debug.Log("    " + e);
                }
                Debug.Log("\nOptional Guns:\n=====");

                foreach (var e in entry.OptionalGunIDs)
                {
                    Debug.Log("    " + e);
                }
                Debug.Log("\nOptional Items:\n=====");
                foreach (var e in entry.OptionalItemIDs)
                {
                    Debug.Log("    " + e);
                }
                */
            }
            return copyEntry;
        }


        public static void DoMagicSparkles(int amount, Vector2 position, float direction, float MinMult = 1, float MaxMult = 3, float DevMinus = 0, float DevPlus = 0, GlobalSparksDoer.SparksType sparks = GlobalSparksDoer.SparksType.FLOATY_CHAFF)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 push = Toolbox.GetUnitOnCircle(direction + UnityEngine.Random.Range(DevMinus, DevPlus), 1f);
                GlobalSparksDoer.DoSingleParticle(position, push * UnityEngine.Random.Range(MinMult, MaxMult), null, 2, null, sparks);
            }
        }
        public static void DoMagicSparkles(int amount, float lifetime, Vector2 position, Vector2 direction, float MinMult = 1, float MaxMult = 3, float DevMinus = 0, float DevPlus = 0, GlobalSparksDoer.SparksType sparks = GlobalSparksDoer.SparksType.FLOATY_CHAFF)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 push = direction + Toolbox.GetUnitOnCircle(direction.ToAngle()+ UnityEngine.Random.Range(DevMinus, DevPlus), 1f);
                GlobalSparksDoer.DoSingleParticle(position, push * UnityEngine.Random.Range(MinMult, MaxMult), null, lifetime, null, sparks);
            }
        }
        public static void ModifyVolley(ProjectileVolleyData volleyToModify, PlayerController player, int DuplicatesOfEachModule = 1, float DuplicateAngleOffset = 3, float DuplicateAngleBaseOffset = 0, float EachModuleOffsetAngle = 3, int DuplicatesOfBaseModule = 0, ProjectileModule moduleToAdd = null, int frameDelay = 0)
        {

            GameManager.Instance.StartCoroutine(WhateverThisIs(volleyToModify, player, DuplicatesOfEachModule, DuplicateAngleOffset, DuplicateAngleBaseOffset, EachModuleOffsetAngle, DuplicatesOfBaseModule, moduleToAdd, frameDelay));
        }

        public static IEnumerator WhateverThisIs(ProjectileVolleyData volleyToModify, PlayerController player, int DuplicatesOfEachModule = 1, float DuplicateAngleOffset = 3, float DuplicateAngleBaseOffset = 0, float EachModuleOffsetAngle = 3, int DuplicatesOfBaseModule = 0, ProjectileModule moduleToAdd = null, int frameDelay = 0)
        {
            if (frameDelay > 0)
            {
                int de = 0;
                while (de < frameDelay)
                {
                    de++;
                    yield return null;
                }
            }
            if (moduleToAdd != null)
            {
                bool flag = true;
                if (volleyToModify != null && volleyToModify.projectiles.Count > 0 && volleyToModify.projectiles[0].projectiles != null && volleyToModify.projectiles[0].projectiles.Count > 0)
                {
                    Projectile projectile = volleyToModify.projectiles[0].projectiles[0];
                    if (projectile && projectile.GetComponent<ArtfulDodgerProjectileController>())
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    moduleToAdd.isExternalAddedModule = true;
                    volleyToModify.projectiles.Add(moduleToAdd);
                }
            }
            if (DuplicatesOfEachModule > 0)
            {
                int num = 1;
                int count = volleyToModify.projectiles.Count;
                for (int i = 0; i < count; i++)
                {



                    float num2 = (float)(DuplicatesOfEachModule * DuplicateAngleOffset) * -1f / 2f;


                    //num2 += EachModuleOffsetAngle;

                    ProjectileModule projectileModule = volleyToModify.projectiles[i];
                    for (int j = 0; j < DuplicatesOfEachModule; j++)
                    {

                        projectileModule.angleFromAim = num2;

                        int sourceIndex = num;
                        if (projectileModule.CloneSourceIndex >= 0)
                        {
                            sourceIndex = projectileModule.CloneSourceIndex;
                        }
                        ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                        float angleFromAim = num2 + DuplicateAngleOffset * (float)(j + 1);

                        projectileModule2.angleFromAim = angleFromAim;
                        projectileModule2.ignoredForReloadPurposes = true;
                        projectileModule2.ammoCost = 0;
                        volleyToModify.projectiles.Add(projectileModule2);

                    }
                    num++;
                }
                if (!volleyToModify.UsesShotgunStyleVelocityRandomizer)
                {
                    volleyToModify.UsesShotgunStyleVelocityRandomizer = true;
                    volleyToModify.DecreaseFinalSpeedPercentMin = -10f;
                    volleyToModify.IncreaseFinalSpeedPercentMax = 10f;
                }
            }
            if (DuplicatesOfBaseModule > 0)
            {
                GunVolleyModificationItem.AddDuplicateOfBaseModule(volleyToModify, player, DuplicatesOfBaseModule, DuplicateAngleOffset, DuplicateAngleBaseOffset);
            }
            yield break;
        }


        public static Random RNG = new Random();

        public static T RandomEnum<T>()
        {
            Type type = typeof(T);
            Array values = Enum.GetValues(type);
            lock (RNG)
            {
                object value = values.GetValue(RNG.Next(values.Length));
                return (T)Convert.ChangeType(value, type);
            }
        }
        public static void ShowHitBox(this SpeculativeRigidbody body)
        {
            PixelCollider hitboxPixelCollider = body.HitboxPixelCollider;
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = "HitboxDisplay";
            gameObject.transform.SetParent(body.transform);
            gameObject.layer = 28;
            gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));

            //Tools.Print<string>(body.name ?? "", "FFFFFF", false);
            //Tools.Print<string>(string.Format("    Offset: {0}, Dimesions: {1}", hitboxPixelCollider.Offset, hitboxPixelCollider.Dimensions), "FFFFFF", false);
            gameObject.transform.localScale = new Vector3((float)hitboxPixelCollider.Dimensions.x / 16f, (float)hitboxPixelCollider.Dimensions.y / 16f, 1f);
            Vector3 localPosition = new Vector3((float)hitboxPixelCollider.Offset.x + (float)hitboxPixelCollider.Dimensions.x * 0.5f, (float)hitboxPixelCollider.Offset.y + (float)hitboxPixelCollider.Dimensions.y * 0.5f, -16f) / 16f;
            gameObject.transform.localPosition = localPosition;
        }
        public static AIBulletBank.Entry CopyBulletBankEntry(AIBulletBank.Entry entryToCopy, string Name, string AudioEvent = null, VFXPool muzzleflashVFX = null, bool ChangeMuzzleFlashToEmpty = true)
        {
            AIBulletBank.Entry entry = CopyFields<AIBulletBank.Entry>(entryToCopy);
            entry.Name = Name;
            Projectile projectile = UnityEngine.Object.Instantiate<GameObject>(entry.BulletObject).GetComponent<Projectile>();
            projectile.gameObject.SetLayerRecursively(18);
            projectile.transform.position = projectile.transform.position.WithZ(210.5125f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            entry.BulletObject = projectile.gameObject;
            if (AudioEvent != "DNC") { entry.AudioEvent = AudioEvent; }
            if (ChangeMuzzleFlashToEmpty == true) { entry.MuzzleFlashEffects = muzzleflashVFX == null ? new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] } : muzzleflashVFX; }
            return entry;
        }
        public static AIBulletBank.Entry CopyFields<T>(AIBulletBank.Entry sample2) where T : AIBulletBank.Entry
        {
            AIBulletBank.Entry sample = new AIBulletBank.Entry();
            sample.AudioEvent = sample2.AudioEvent;
            sample.AudioLimitOncePerAttack = sample2.AudioLimitOncePerAttack;
            sample.AudioLimitOncePerFrame = sample2.AudioLimitOncePerFrame;
            sample.AudioSwitch = sample2.AudioSwitch;
            sample.PlayAudio = sample2.PlayAudio;
            sample.BulletObject = sample2.BulletObject;
            sample.conditionalMinDegFromNorth = sample2.conditionalMinDegFromNorth;

            sample.DontRotateShell = sample2.DontRotateShell;
            sample.forceCanHitEnemies = sample2.forceCanHitEnemies;
            sample.MuzzleFlashEffects = sample2.MuzzleFlashEffects;
            sample.MuzzleInheritsTransformDirection = sample2.MuzzleInheritsTransformDirection;
            sample.MuzzleLimitOncePerFrame = sample2.MuzzleLimitOncePerFrame;
            sample.Name = sample2.Name;
            sample.OverrideProjectile = sample2.OverrideProjectile;
            sample.preloadCount = sample2.preloadCount;
            sample.ProjectileData = sample2.ProjectileData;
            sample.rampBullets = sample2.rampBullets;

            sample.rampStartHeight = sample2.rampStartHeight;
            sample.rampTime = sample2.rampTime;
            sample.ShellForce = sample2.ShellForce;
            sample.ShellForceVariance = sample2.ShellForceVariance;
            sample.ShellGroundOffset = sample2.ShellGroundOffset;
            sample.ShellPrefab = sample2.ShellPrefab;
            sample.ShellsLimitOncePerFrame = sample2.ShellsLimitOncePerFrame;
            sample.ShellTransform = sample2.ShellTransform;
            sample.SpawnShells = sample2.SpawnShells;
            sample.suppressHitEffectsIfOffscreen = sample2.suppressHitEffectsIfOffscreen;

            return sample;
        }

        public static RaycastResult ReturnRaycast(Vector2 startPosition, Vector2 angle, int rayCastMask, float overrideDistance = 1000, SpeculativeRigidbody bodyToIgnore = null)
        {
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
            RaycastResult raycastResult2;
            PhysicsEngine.Instance.Raycast(startPosition, angle, overrideDistance, out raycastResult2, true, true, rayCastMask, null, false, rigidbodyExcluder, bodyToIgnore);
            return raycastResult2;
        }
        public static void ProcessAnimations(this CustomCharacterData self, Dictionary<string, int> dict)
        {
            var lib = self.animator.Library;
            foreach (var entry in dict)
            { lib.GetClipByName(entry.Key).fps = entry.Value;}
        }
        public static void AddGlowShaderToGun(this Gun self, Color32 glowColor, int glowstrength, int colorGlowStrength = 0)
        {
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", glowColor);
            mat.SetFloat("_EmissiveColorPower", colorGlowStrength);
            mat.SetFloat("_EmissivePower", glowstrength);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            MeshRenderer component = self.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
        }

        public static VFXPool MakeObjectIntoVFX(GameObject obj)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            return pool;
        }

        public static GameObject GenerateTransformPoint(GameObject attacher, Vector2 attachpoint, string name = "shootPoint")
        {
            GameObject shootpoint = new GameObject(name);
            shootpoint.transform.parent = attacher.transform;
            shootpoint.transform.position = attachpoint;
            return attacher.transform.Find(name).gameObject;
        }


        public static float SinLerpTValue(float t)
        {
            return Mathf.Sin(t * (Mathf.PI / 2));
        }

        public static float CosLerpTValue(float t)
        {
            return Mathf.Cos(t * (Mathf.PI));
        }


        public static float SinLerpTValueFull(float t)
        {
            return Mathf.Sin(t * (Mathf.PI));
        }

        public static float SubdivideArc(float startAngle, float sweepAngle, int numBullets, int i, bool offset = false)
        {
            return startAngle + Mathf.Lerp(0f, sweepAngle, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)(numBullets - 1));
        }

        public static float SubdivideCircle(float startAngle, int numBullets, int i, float direction = 1f, bool offset = false)
        {
            return startAngle + direction * Mathf.Lerp(0f, 360f, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)numBullets);
        }
        public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector2 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);

            // return the vector info
            return _returnVector;
        }
        public static Vector3 GetUnitOnCircleVec3(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector3 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector3(_x, _y);

            // return the vector info
            return _returnVector;
        }


        public const int ResolutionIBuiltOffOf_X = 1600;
        public const int ResolutionIBuiltOffOf_Y = 1024;

        public const int Resolution_Ratio_X = 25;
        public const int Resolution_Ratio_Y = 16;

        public static Vector2 CalculateScale_X_Y_Based_On_Resolution()
        {
            Vector2 vector2 = new Vector2();
            vector2.x = Screen.width; //GameManager.Options.GetRecommendedResolution().width;
            vector2.y = Screen.height; //GameManager.Options.GetRecommendedResolution().height;
            //Debug.Log("sc: "+ vector2);
            return new Vector2(((float)vector2.x / (float)ResolutionIBuiltOffOf_X), ((float)vector2.y / (float)ResolutionIBuiltOffOf_Y));
        }

        //FUCK THIS
        public static ModifiedDefaultLabelManager GenerateText(Transform trans, Vector2 offset, float time, string Text, Color32 color, bool Autotrigger = true, float size = 5, bool UsesScaling = true)
        {
            var labelToSet = UnityEngine.Object.Instantiate(DefaultModule.LabelController).gameObject.GetComponent<ModifiedDefaultLabelManager>();

            labelToSet.label.textScale = ((size / (GameUIUtility.GetCurrentTK2D_DFScale(labelToSet.panel.GetManager()) * 20) * (UsesScaling ? (Smaller() ? ScaleMult().x : 1) : 1)));
            labelToSet.label.Text = Text;



            if (Autotrigger == true)
            {
                labelToSet.Trigger_CustomTime(trans,  offset *= UsesScaling ? Smaller() ? ScaleMult_Inv().x : 1 : 1, time);
            }
            labelToSet.label.backgroundColor = color;
            labelToSet.scaleMultiplier = Mathf.Max(1, UsesScaling ? ScaleMult().x : 1);
            GameUIRoot.Instance.m_manager.AddControl(labelToSet.panel);
            dfLabel componentInChildren = labelToSet.gameObject.GetComponentInChildren<dfLabel>();

            //componentInChildren.OnResolutionChanged
            componentInChildren.ColorizeSymbols = false;
            componentInChildren.ProcessMarkup = true;
            componentInChildren.autoHeight = false;// *= GameManager.Options.SmallUIEnabled == true ? 1 : 2;
            componentInChildren.updateCollider();            
            componentInChildren.Invalidate();
            if (UsesScaling)
            {
                GameManager.Instance.StartCoroutine(FuckYou(componentInChildren.gameObject));
            }

            return labelToSet;
        }
        public static IEnumerator FuckYou(GameObject sadcat)
        {
            yield return null;
            sadcat.transform.localScale *= (Smaller() ? 1 : ScaleMult_Inv().x);// ScaleMult_Inv().x;//ScaleMult().x > Vector2.one.x ? ScaleMult() : Vector2.one;


            yield break;
        }


        public static bool Smaller()
        {
            Vector2 val = Toolbox.CalculateScale_X_Y_Based_On_Resolution();
            return val.x < 1;
        }

        private static Vector2 ScaleMult()
        {
            Vector2 val = Toolbox.CalculateScale_X_Y_Based_On_Resolution();
            return new Vector2(val.x * val.x, val.x * val.x);
        }

        private static Vector2 ScaleMult_Inv()
        {
            Vector2 val = Toolbox.CalculateScale_X_Y_Based_On_Resolution();
            return new Vector2(val.x / 1, val.x / 1);
        }

        public static void AddColorLight(this DefaultModule self, Color color)
        {
            AdditionalBraveLight braveLight = self.gameObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = self.sprite.WorldCenter;
            braveLight.LightColor = color;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;
            self.BraveLight = braveLight;         
        }

        public static void NotifyCustom(string header, string text, int spriteID, tk2dSpriteCollectionData CollectionData, UINotificationController.NotificationColor color = UINotificationController.NotificationColor.GOLD, bool forceSingleLine = false)
        {
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, CollectionData, spriteID, color, false, forceSingleLine);
        }

        public static Vector3 RandomPositionOnSprite(tk2dBaseSprite targetSprite, float zOffset = 0f)
        {
            if (targetSprite == null) { return Vector3.one; }
            Vector3 vector = targetSprite.WorldBottomLeft.ToVector3ZisY(zOffset);
            Vector3 vector2 = targetSprite.WorldTopRight.ToVector3ZisY(zOffset);
            float num = (vector2.y - vector.y) * (vector2.x - vector.x);
            float num2 = 25f * num;
            int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
            int num4 = num3;
            Vector3 minPosition = vector;
            Vector3 maxPosition = vector2;
            return new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), UnityEngine.Random.Range(minPosition.y, maxPosition.y));
        }


        public static float PercentageOfClipLeft(this Gun g, int modify = 0)
        {
            //:sadcat:
            float q = g.ClipShotsRemaining;
            float r = g.ClipCapacity;
            r += modify;
            float t = q / r;
            return t;
        }


        public static DungeonPlaceable GenerateDungeonPlaceable(Dictionary<GameObject, float> gameObjects, int placeableWidth = 1, int placeableLength = 1, DungeonPrerequisite[] dungeonPrerequisites = null)
        {
            if (dungeonPrerequisites == null) { dungeonPrerequisites = new DungeonPrerequisite[0]; }
            DungeonPlaceable placeableContents = ScriptableObject.CreateInstance<DungeonPlaceable>();
            {
                placeableContents.width = placeableWidth;
                placeableContents.height = placeableLength;
                placeableContents.respectsEncounterableDifferentiator = true;
                placeableContents.variantTiers = new List<DungeonPlaceableVariant>();
            }
            foreach (var Entry in gameObjects)
            {
                DungeonPlaceableVariant variant = new DungeonPlaceableVariant();
                variant.percentChance = Entry.Value;
                variant.prerequisites = dungeonPrerequisites;
                variant.nonDatabasePlaceable = Entry.Key;
                placeableContents.variantTiers.Add(variant);
            }
            return placeableContents;
        }


        public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
        {
            T t = default(T);
            foreach (string path2 in Toolbox.BundlePrereqs)
            {
                try
                {
                    t = ResourceManager.LoadAssetBundle(path2).LoadAsset<T>(path);
                }
                catch
                {
                }
                bool flag2 = t != null;
                if (flag2)
                {
                    break;
                }
            }
            return t;
        }
        private static string[] BundlePrereqs = new string[] {"brave_resources_001",
                "dungeon_scene_001",
                "encounters_base_001",
                "enemies_base_001",
                "flows_base_001",
                "foyer_001",
                "foyer_002",
                "foyer_003",
                "shared_auto_001",
                "shared_auto_002",
                "shared_base_001",
                "dungeons/base_bullethell",
                "dungeons/base_castle",
                "dungeons/base_catacombs",
                "dungeons/base_cathedral",
                "dungeons/base_forge",
                "dungeons/base_foyer",
                "dungeons/base_gungeon",
                "dungeons/base_mines",
                "dungeons/base_nakatomi",
                "dungeons/base_resourcefulrat",
                "dungeons/base_sewer",
                "dungeons/base_tutorial",
                "dungeons/finalscenario_bullet",
                "dungeons/finalscenario_convict",
                "dungeons/finalscenario_coop",
                "dungeons/finalscenario_guide",
                "dungeons/finalscenario_pilot",
                "dungeons/finalscenario_robot",
                "dungeons/finalscenario_soldier"
        };

        public static void CreateFastBody(this GameObject gameObject, IntVector2 colliderX_Y, IntVector2 OffsetX_Y)
        {
            SpeculativeRigidbody specBody = gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            specBody.CollideWithTileMap = false;
            if (specBody.PixelColliders == null) { specBody.PixelColliders = new List<PixelCollider>(); }
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.HighObstacle,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = OffsetX_Y.x,
                ManualOffsetY = OffsetX_Y.y,
                ManualWidth = colliderX_Y.x,
                ManualHeight = colliderX_Y.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BeamBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = OffsetX_Y.x,
                ManualOffsetY = OffsetX_Y.y,
                ManualWidth = colliderX_Y.x,
                ManualHeight = colliderX_Y.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BulletBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = OffsetX_Y.x,
                ManualOffsetY = OffsetX_Y.y,
                ManualWidth = colliderX_Y.x,
                ManualHeight = colliderX_Y.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
        }

        public static SpeculativeRigidbody CreateFastBody(this GameObject gameObject, CollisionLayer layer, IntVector2 colliderX_Y, IntVector2 OffsetX_Y)
        {
            SpeculativeRigidbody specBody = gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            specBody.CollideWithTileMap = false;
            if (specBody.PixelColliders == null) { specBody.PixelColliders = new List<PixelCollider>(); }
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = layer,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = OffsetX_Y.x,
                ManualOffsetY = OffsetX_Y.y,
                ManualWidth = colliderX_Y.x,
                ManualHeight = colliderX_Y.y,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            return specBody;
        }

        public static void AddShadowToObject(this GameObject obj, tk2dSpriteCollectionData Data, string spriteName, Vector2 OffsetX_Y)
        {
            GameObject shadow = PrefabBuilder.BuildObject(obj.name + "_Shadow");
            var tk2d = shadow.AddComponent<tk2dSprite>();
            tk2d.Collection = Data;
            tk2d.SetSprite(Data.GetSpriteIdByName(spriteName));
            shadow.transform.parent = obj.transform;
            shadow.transform.localPosition = OffsetX_Y;
            var zPos = shadow.transform.localPosition;
            zPos.z -= 1;
            tk2d.HeightOffGround = obj.transform.GetComponent<tk2dSprite>() != null ? obj.transform.GetComponent<tk2dSprite>().HeightOffGround - 0.1f : 1;

        }

        public static AIBeamShooter2 AddAIBeamShooter2(AIActor enemy, Transform transform, string name, Projectile beamProjectile, ProjectileModule beamModule = null, float angle = 0)
        {
            AIBeamShooter2 bholsterbeam1 = enemy.gameObject.AddComponent<AIBeamShooter2>();
            bholsterbeam1.beamTransform = transform;
            bholsterbeam1.beamModule = beamModule;
            bholsterbeam1.beamProjectile = beamProjectile;
            bholsterbeam1.firingEllipseCenter = transform.position;
            bholsterbeam1.northAngleTolerance = angle;
            return bholsterbeam1;
        }

        public static void BuildSpriteObject(string spriteName, string ObjectName, bool shitfuck = true)
        {
            GameObject obj = PrefabBuilder.BuildObject(ObjectName);
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            string naame = shitfuck == true ? ObjectName + "_MDLR" : ObjectName;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(naame, obj);
        }


        public static GameObject BuildSpriteObject_FuckingSHit(string spriteName, string ObjectName)
        {
            GameObject obj = PrefabBuilder.BuildObject(ObjectName);
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            return obj;
        }

        public static Delegate GetEventDelegate(this object self, string eventName)
        {
            Delegate result = null;
            if (self != null)
            {
                FieldInfo t = self.GetType().GetField(eventName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (t != null)
                {
                    object val = t.GetValue(self);
                    if (val != null && val is Delegate)
                    {
                        result = val as Delegate;
                    }
                }
            }
            return result;
        }

        public static T GetEventDelegate<T>(this object self, string eventName) where T : Delegate
        {
            return self.GetEventDelegate(eventName) as T;
        }

        public static void RaiseEvent(this object self, string eventName, params object[] args)
        {
            self.GetEventDelegate<Delegate>(eventName)?.DynamicInvoke(args);
        }

        public static object RaiseEventWithReturn(this object self, string eventName, params object[] args)
        {
            return self.GetEventDelegate<Delegate>(eventName)?.DynamicInvoke(args);
        }


        public static object InvokeNotOverride(this MethodInfo methodInfo,
    object targetObject, params object[] arguments)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                if (arguments != null && arguments.Length != 0)
                    throw new Exception("Arguments cont doesn't match");
            }
            else
            {
                if (parameters.Length != arguments.Length)
                    throw new Exception("Arguments cont doesn't match");
            }

            Type returnType = null;
            if (methodInfo.ReturnType != typeof(void))
            {
                returnType = methodInfo.ReturnType;
            }

            var type = targetObject.GetType();
            var dynamicMethod = new DynamicMethod("", returnType,
                    new Type[] { type, typeof(object) }, type);

            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0); // this

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                iLGenerator.Emit(OpCodes.Ldarg_1); // load array argument

                // get element at index
                iLGenerator.Emit(OpCodes.Ldc_I4_S, i); // specify index
                iLGenerator.Emit(OpCodes.Ldelem_Ref); // get element

                var parameterType = parameter.ParameterType;
                if (parameterType.IsPrimitive)
                {
                    iLGenerator.Emit(OpCodes.Unbox_Any, parameterType);
                }
                else if (parameterType == typeof(object))
                {
                    // do nothing
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterType);
                }
            }

            iLGenerator.Emit(OpCodes.Call, methodInfo);
            iLGenerator.Emit(OpCodes.Ret);

            return dynamicMethod.Invoke(null, new object[] { targetObject, arguments });
        }
    }
}
