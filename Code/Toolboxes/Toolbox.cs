using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria;
using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using Planetside;
using UnityEngine;

namespace ModularMod
{
    public static class Toolbox
    {
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

        public const int ResolutionIBuiltOffOf_X = 1600;
        public const int ResolutionIBuiltOffOf_Y = 1024;

        public const int Resolution_Ratio_X = 25;
        public const int Resolution_Ratio_Y = 16;

        public static Vector2 CalculateScale_X_Y_Based_On_Resolution()
        {
            Vector2 vector2 = new Vector2();
            vector2.x = GameManager.Options.preferredResolutionX;
            vector2.y = GameManager.Options.preferredResolutionY;//Screen.currentResolution;
            //Debug.Log("sc: "+ vector2);
            return new Vector2(((float)vector2.x / (float)ResolutionIBuiltOffOf_X), ((float)vector2.y / (float)ResolutionIBuiltOffOf_Y));
        }


        public static ModifiedDefaultLabelManager GenerateText(Transform trans, Vector2 offset, float time, string Text, Color32 color, bool Autotrigger = true, float size = 5)
        {
            var labelToSet = UnityEngine.Object.Instantiate(DefaultModule.LabelController).gameObject.GetComponent<ModifiedDefaultLabelManager>();
            Vector2 scaler = CalculateScale_X_Y_Based_On_Resolution();

            labelToSet.label.textScale = (size / (GameUIUtility.GetCurrentTK2D_DFScale(labelToSet.panel.GetManager()) * 20))* scaler.x;
            labelToSet.label.Text = Text;
            if (Autotrigger == true)
            {
                labelToSet.Trigger_CustomTime(trans, offset, time);
            }
            labelToSet.label.backgroundColor = color;

            GameUIRoot.Instance.m_manager.AddControl(labelToSet.panel);
            dfLabel componentInChildren = labelToSet.gameObject.GetComponentInChildren<dfLabel>();
            componentInChildren.ColorizeSymbols = false;
            componentInChildren.ProcessMarkup = true;
            componentInChildren.autoHeight = false;// *= GameManager.Options.SmallUIEnabled == true ? 1 : 2;
            componentInChildren.updateCollider();            
            componentInChildren.Invalidate();
            return labelToSet;
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

        public static void NotifyCustom(string header, string text, int spriteID, tk2dSpriteCollectionData CollectionData, UINotificationController.NotificationColor color = UINotificationController.NotificationColor.SILVER, bool forceSingleLine = false)
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


        public static float PercentageOfClipLeft(this Gun g)
        {
            //:sadcat:
            float q = g.ClipShotsRemaining;
            float r = g.ClipCapacity;
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

        public static void CreateFastBody(this GameObject gameObject, CollisionLayer layer, IntVector2 colliderX_Y, IntVector2 OffsetX_Y)
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

        public static void BuildSpriteObject(string spriteName, string ObjectName)
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
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(ObjectName + "_MDLR", obj);
        }
    }
}
