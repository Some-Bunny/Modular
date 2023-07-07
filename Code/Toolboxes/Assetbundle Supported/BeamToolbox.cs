using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public static class BeamBuilder
    {
        public static BasicBeamController GenerateBeamPrefabBundle(this Projectile projectile, string defaultSpriteName, tk2dSpriteCollectionData data, tk2dSpriteAnimation animation, string IdleAnimationName, Vector2 colliderDimensions, Vector2 colliderOffsets, string impactVFXAnimationName = null, Vector2? impactVFXColliderDimensions = null, Vector2? impactVFXColliderOffsets = null, string endAnimation = null, Vector2? endColliderDimensions = null, Vector2? endColliderOffsets = null, string muzzleAnimationName = null, Vector2? muzzleColliderDimensions = null, Vector2? muzzleColliderOffsets = null, bool glows = false,
        bool canTelegraph = false, string beamTelegraphIdleAnimationName = null, string beamStartTelegraphAnimationName = null, string beamEndTelegraphAnimationName = null, float telegraphTime = 1,
        bool canDissipate = false, string beamDissipateAnimationName = null, string beamStartDissipateAnimationName = null, string beamEndDissipateAnimationName = null, float dissipateTime = 1)

        {
            try
            {
                projectile.specRigidbody.CollideWithOthers = false;


                float convertedColliderX = colliderDimensions.x / 16f;
                float convertedColliderY = colliderDimensions.y / 16f;
                float convertedOffsetX = colliderOffsets.x / 16f;
                float convertedOffsetY = colliderOffsets.y / 16f;


                tk2dTiledSprite tiledSprite = projectile.gameObject.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.Collection = data;
                tiledSprite.SetSprite(data, data.GetSpriteIdByName(defaultSpriteName));
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.colliderVertices = new Vector3[]{
                    new Vector3(convertedOffsetX, convertedOffsetY, 0f),
                    new Vector3(convertedColliderX, convertedColliderY, 0f)
                };

                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);

                //tiledSprite.anchor = tk2dBaseSprite.Anchor.MiddleCenter;
                tk2dSpriteAnimator animator = projectile.gameObject.GetOrAddComponent<tk2dSpriteAnimator>();
                animator._startingSpriteCollection = data;
                animator.Library = animation;
                animator.library = animation;
                animator.playAutomatically = true;
                animator.defaultClipId = animation.GetClipIdByName(IdleAnimationName);

                UnityEngine.Object.Destroy(projectile.GetComponentInChildren<tk2dSprite>());
                projectile.sprite = tiledSprite;
                projectile.sprite.Collection = data;

                BasicBeamController beamController = projectile.gameObject.GetOrAddComponent<BasicBeamController>();
                beamController.sprite = tiledSprite;
                beamController.spriteAnimator = animator;
                beamController.spriteAnimator._startingSpriteCollection = data;
                beamController.m_beamSprite = tiledSprite;


                //---------------- Sets up the animation for the main part of the beam
                beamController.beamAnimation = IdleAnimationName;

                //------------- Sets up the animation for the part of the beam that touches the wall
                
                if (endAnimation != null && endColliderDimensions != null && endColliderOffsets != null)
                {
                    SetupBeamPart(animation, data, endAnimation, (Vector2)endColliderDimensions, (Vector2)endColliderOffsets);
                    beamController.beamEndAnimation = endAnimation;
                }
                else
                {
                    SetupBeamPart(animation, data, IdleAnimationName, null, null, def.colliderVertices);
                    beamController.beamEndAnimation = IdleAnimationName;
                }

                //---------------Sets up the animaton for the VFX that plays over top of the end of the beam where it hits stuff
                if (impactVFXAnimationName != null && impactVFXColliderDimensions != null && impactVFXColliderOffsets != null)
                {
                    SetupBeamPart(animation, data, impactVFXAnimationName, (Vector2)impactVFXColliderDimensions, (Vector2)impactVFXColliderOffsets);
                    beamController.impactAnimation = impactVFXAnimationName;
                }

                //--------------Sets up the animation for the very start of the beam
                if (muzzleAnimationName != null && muzzleColliderDimensions != null && muzzleColliderOffsets != null)
                {
                    SetupBeamPart(animation, data, muzzleAnimationName, (Vector2)muzzleColliderDimensions, (Vector2)muzzleColliderOffsets);
                    beamController.beamStartAnimation = muzzleAnimationName;
                }
                else
                {
                    SetupBeamPart(animation, data, IdleAnimationName, null, null, def.colliderVertices);
                    beamController.beamStartAnimation = IdleAnimationName;
                }
               


                if (canTelegraph == true)
                {
                    beamController.usesTelegraph = true;
                    beamController.telegraphAnimations = new BasicBeamController.TelegraphAnims();
                    if (beamStartTelegraphAnimationName != null)
                    {
                        SetupBeamPart(animation, data , beamStartTelegraphAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.telegraphAnimations.beamStartAnimation = beamStartTelegraphAnimationName;
                    }
                    if (beamTelegraphIdleAnimationName != null)
                    {
                        SetupBeamPart(animation, data, beamTelegraphIdleAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.telegraphAnimations.beamAnimation = beamTelegraphIdleAnimationName;
                    }
                    if (beamEndTelegraphAnimationName != null)
                    {
                        SetupBeamPart(animation, data, beamEndTelegraphAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.telegraphAnimations.beamEndAnimation = beamEndTelegraphAnimationName;
                    }
                    beamController.telegraphTime = telegraphTime;
                }
                
                
                if (canDissipate == true)
                {
                    beamController.endType = BasicBeamController.BeamEndType.Dissipate;
                    beamController.dissipateAnimations = new BasicBeamController.TelegraphAnims();
                    if (beamStartDissipateAnimationName != null)
                    {
                        SetupBeamPart(animation, data, beamStartDissipateAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.dissipateAnimations.beamStartAnimation = beamStartDissipateAnimationName;
                    }
                    if (beamDissipateAnimationName != null)
                    {
                        SetupBeamPart(animation, data, beamDissipateAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.dissipateAnimations.beamAnimation = beamDissipateAnimationName;
                    }
                    if (beamEndDissipateAnimationName != null)
                    {
                        SetupBeamPart(animation, data, beamEndDissipateAnimationName, new Vector2(0, 0), new Vector2(0, 0));
                        beamController.dissipateAnimations.beamEndAnimation = beamEndDissipateAnimationName;
                    }
                    beamController.dissipateTime = dissipateTime;
                }
                

                if (glows)
                {
                    EmmisiveBeams emission = projectile.gameObject.GetOrAddComponent<EmmisiveBeams>();
                    //emission

                }
                return beamController;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }

        public static void ConstructOffsetsFromAnchor(this tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
        {
            if (!scale.HasValue)
            {
                scale = new Vector2?(def.position3);
            }
            if (fixesScale)
            {
                Vector2 fixedScale = scale.Value - def.position0.XY();
                scale = new Vector2?(fixedScale);
            }
            float xOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
            {
                xOffset = -(scale.Value.x / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                xOffset = -scale.Value.x;
            }
            float yOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
            {
                yOffset = -(scale.Value.y / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                yOffset = -scale.Value.y;
            }
            def.MakeOffset(new Vector2(xOffset, yOffset), false);
            if (changesCollider && def.colliderVertices != null && def.colliderVertices.Length > 0)
            {
                float colliderXOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.UpperLeft)
                {
                    colliderXOffset = (scale.Value.x / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderXOffset = -(scale.Value.x / 2f);
                }
                float colliderYOffset = 0;
                if (anchor == tk2dBaseSprite.Anchor.LowerLeft || anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.LowerRight)
                {
                    colliderYOffset = (scale.Value.y / 2f);
                }
                else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
                {
                    colliderYOffset = -(scale.Value.y / 2f);
                }
                def.colliderVertices[0] += new Vector3(colliderXOffset, colliderYOffset, 0);
            }
        }
        public static void MakeOffset(this tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
            if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
            {
                def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
            }
        }

        private static void SetupBeamPart(tk2dSpriteAnimation beamAnimation, tk2dSpriteCollectionData  data, string animationName, Vector2? colliderDimensions = null, Vector2? colliderOffsets = null, Vector3[] overrideVertices = null, tk2dSpriteAnimationClip.WrapMode wrapMode = tk2dSpriteAnimationClip.WrapMode.Once)
        {

            foreach (var path in beamAnimation.GetClipByName(animationName).frames)
            {
                tk2dSpriteDefinition frameDef = data.spriteDefinitions[path.spriteId];
                frameDef.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                if (overrideVertices != null)
                {
                    frameDef.colliderVertices = overrideVertices;
                }
                else
                {
                    if (colliderDimensions == null || colliderOffsets == null)
                    {
                        ETGModConsole.Log("<size=100><color=#ff0000ff>BEAM ERROR: colliderDimensions or colliderOffsets was null with no override vertices!</color></size>", false);
                    }
                    else
                    {
                        Vector2 actualDimensions = (Vector2)colliderDimensions;
                        Vector2 actualOffsets = (Vector2)colliderDimensions;
                        frameDef.colliderVertices = new Vector3[]{
                            new Vector3(actualOffsets.x / 16, actualOffsets.y / 16, 0f),
                            new Vector3(actualDimensions.x / 16, actualDimensions.y / 16, 0f)
                        };
                    }
                }
            }
        }


    }
    internal class EmmisiveBeams : MonoBehaviour
    {
        public EmmisiveBeams()
        {
            this.EmissivePower = 100;
            this.EmissiveColorPower = 1.55f;
        }
        public void Start()
        {
            Shader glowshader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");

            foreach (Transform transform in base.transform)
            {
                if (TransformList.Contains(transform.name))
                {
                    tk2dSprite sproot = transform.GetComponent<tk2dSprite>();
                    if (sproot != null)
                    {
                        sproot.usesOverrideMaterial = true;
                        sproot.renderer.material.shader = glowshader;
                        sproot.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                        sproot.renderer.material.SetFloat("_EmissivePower", EmissivePower);
                        sproot.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);
                    }
                }
            }
            this.beamcont = base.GetComponent<BasicBeamController>();
            BasicBeamController beam = this.beamcont;
            beam.sprite.usesOverrideMaterial = true;
            BasicBeamController component = beam.gameObject.GetComponent<BasicBeamController>();
            bool flag = component != null;
            bool flag2 = flag;
            if (flag2)
            {
                component.sprite.renderer.material.shader = glowshader;
                component.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component.sprite.renderer.material.SetFloat("_EmissivePower", EmissivePower);
                component.sprite.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);
            }
        }


        private List<string> TransformList = new List<string>()
        {
            "Sprite",
            "beam impact vfx 2",
            "beam impact vfx",
        };


        public void Update()
        {
            Shader glowshader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            Transform trna = base.transform.Find("beam pierce impact vfx");
            if (trna != null)
            {
                tk2dSprite sproot = trna.GetComponent<tk2dSprite>();
                if (sproot != null)
                {
                    sproot.renderer.material.shader = glowshader;
                    sproot.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    sproot.renderer.material.SetFloat("_EmissivePower", EmissivePower);
                    sproot.renderer.material.SetFloat("_EmissiveColorPower", EmissiveColorPower);
                }
            }
        }
        private BasicBeamController beamcont;
        public float EmissivePower;
        public float EmissiveColorPower;
    }

    public static class BeamToolbox
    {
        public static BeamController FreeFireBeamFromAnywhere(Projectile projectileToSpawn, PlayerController owner, GameObject otherShooter, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool skipChargeTime = false, bool CanRotate = false, float RotationSpeedperSecond = 60, bool DoPostProcess = false)
        {
            Vector2 sourcePos = Vector2.zero;
            SpeculativeRigidbody rigidBod = null;
            bool HasSpecrigidBody = false;
            if (usesFixedPosition == true) { sourcePos = fixedPosition; }
            else
            {
                if (otherShooter.GetComponent<SpeculativeRigidbody>()) { rigidBod = otherShooter.GetComponent<SpeculativeRigidbody>(); HasSpecrigidBody = true; }
                else if (otherShooter.GetComponentInChildren<SpeculativeRigidbody>()) { rigidBod = otherShooter.GetComponentInChildren<SpeculativeRigidbody>(); HasSpecrigidBody = true; }

                if (rigidBod) { sourcePos = rigidBod.UnitCenter; }


                if (otherShooter == null) { ETGModConsole.Log("projectileToSpawn.gameObject is NULL"); }
                if (rigidBod == null) { ETGModConsole.Log("rigidBod is NULL"); }

            }

            if (otherShooter != null && sourcePos == Vector2.zero)
            {
                sourcePos = otherShooter.transform.PositionVector2();
            }
            if (sourcePos != Vector2.zero)
            {
                BasicBeamController basicBeam = projectileToSpawn.gameObject.GetComponent<BasicBeamController>();
                if (basicBeam) { basicBeam.SkipPostProcessing = DoPostProcess; }

                GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, sourcePos, Quaternion.identity, true);
                Projectile component = gameObject.GetComponent<Projectile>();

                component.Owner = owner;
                BeamController component2 = gameObject.GetComponent<BeamController>();

                if (skipChargeTime)
                {
                    component2.chargeDelay = 0f;
                    component2.usesChargeDelay = false;
                }

                component2.Owner = owner;
                component2.HitsPlayers = false;
                component2.HitsEnemies = true;
                Vector3 vector = BraveMathCollege.DegreesToVector(targetAngle, 1f);
                component2.Direction = vector;
                component2.Origin = sourcePos;
                component.StartCoroutine(BeamToolbox.HandleFreeFiringBeam(component2, otherShooter, rigidBod, fixedPosition, usesFixedPosition, targetAngle, duration, HasSpecrigidBody, CanRotate, RotationSpeedperSecond));




                return component2;
            }
            else
            {

                ETGModConsole.Log("ERROR IN BEAM FREEFIRE CODE. SOURCEPOS WAS NULL, EITHER DUE TO INVALID FIXEDPOS OR SOURCE GAMEOBJECT.");
                return null;
            }
        }
        private static IEnumerator HandleFreeFiringBeam(BeamController beam, GameObject otherShooter, SpeculativeRigidbody body, Vector2 fixedPosition, bool usesFixedPosition, float targetAngle, float duration, bool evenHasBody, bool CanRotate = false, float RotationSpeedPerSecond = 60)
        {
            float elapsed = 0f;
            yield return null;
            while (elapsed < duration)
            {
                Vector2 sourcePos;
                if (otherShooter == null && evenHasBody == true) { break; }
                if (beam == null) { break; }
                if (usesFixedPosition) sourcePos = fixedPosition;

                else sourcePos = body.projectile != null ? body.UnitCenter : otherShooter.transform.PositionVector2();


                elapsed += BraveTime.DeltaTime;
                if (sourcePos != null)
                {
                    if (CanRotate == true)
                    {
                        Vector3 vector = BraveMathCollege.DegreesToVector(beam.Direction.ToAngle() + RotationSpeedPerSecond * BraveTime.DeltaTime, 1f);
                        beam.Direction = vector;
                    }
                    beam.Origin = sourcePos;
                    beam.LateUpdatePosition(sourcePos);


                }
                else { ETGModConsole.Log("SOURCEPOS WAS NULL IN BEAM FIRING HANDLER"); }
                yield return null;
            }
            beam.CeaseAttack();

            yield break;
        }



        public static BeamController FreeFireBeamFromPosition(Projectile projectileToSpawn, PlayerController owner, Vector2 fixedPosition, float targetAngle, float duration, bool skipChargeTime = false, bool CanRotate = false, float RotationSpeedperSecond = 60, bool DoPostProcess = false)
        {
            if (fixedPosition != Vector2.zero)
            {
                BasicBeamController basicBeam = projectileToSpawn.gameObject.GetComponent<BasicBeamController>();
                if (basicBeam) { basicBeam.SkipPostProcessing = DoPostProcess; }


                GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, fixedPosition, Quaternion.identity, true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.Owner = owner;
                BeamController component2 = gameObject.GetComponent<BeamController>();
                if (skipChargeTime)
                {
                    component2.chargeDelay = 0f;
                    component2.usesChargeDelay = false;
                }
                component2.Owner = owner;
                component2.HitsPlayers = false;
                component2.HitsEnemies = true;
                Vector3 vector = BraveMathCollege.DegreesToVector(targetAngle, 1f);
                component2.Direction = vector;
                component2.Origin = fixedPosition;
                GameManager.Instance.Dungeon.StartCoroutine(BeamToolbox.HandleFreeFiringBeamFromPosition(component2, fixedPosition, targetAngle, duration, CanRotate, RotationSpeedperSecond));
                return component2;
            }
            else
            {

                ETGModConsole.Log("ERROR IN BEAM FREEFIRE CODE. SOURCEPOS WAS NULL, EITHER DUE TO INVALID FIXEDPOS OR SOURCE GAMEOBJECT.");
                return null;
            }
        }

        private static IEnumerator HandleFreeFiringBeamFromPosition(BeamController beam, Vector2 fixedPosition, float targetAngle, float duration, bool CanRotate = false, float RotationSpeedPerSecond = 60)
        {
            float elapsed = 0f;
            yield return null;
            while (elapsed < duration)
            {
                Vector2 sourcePos;
                if (beam == null) { break; }
                sourcePos = fixedPosition;
                elapsed += BraveTime.DeltaTime;
                if (sourcePos != null)
                {
                    if (CanRotate == true)
                    {
                        Vector3 vector = BraveMathCollege.DegreesToVector(beam.Direction.ToAngle() + RotationSpeedPerSecond * BraveTime.DeltaTime, 1f);
                        beam.Direction = vector;
                    }
                    beam.Origin = sourcePos;
                    beam.LateUpdatePosition(sourcePos);


                }
                else { ETGModConsole.Log("SOURCEPOS WAS NULL IN BEAM FIRING HANDLER"); }
                yield return null;
            }
            if (beam != null)
            {
                beam.CeaseAttack();
            }
            yield break;
        }

        public static bool PosIsNearAnyBoneOnBeam(this BasicBeamController beam, Vector2 positionToCheck, float distance)
        {
            foreach (BasicBeamController.BeamBone bone in beam.m_bones)
            {
                Vector2 bonepos = beam.GetBonePosition(bone);
                if (Vector2.Distance(positionToCheck, bonepos) < distance) return true;
            }
            return false;
        }

        public static int GetBoneCount(this BasicBeamController beam)
        {
            if (!beam.UsesBones)
            {
                return 1;
            }
            else
            {
                return beam.m_bones.Count();
            }
        }
        public static float GetFinalBoneDirection(this BasicBeamController beam)
        {
            if (!beam.UsesBones)
            {
                return beam.Direction.ToAngle();
            }
            else
            {
                LinkedListNode<BasicBeamController.BeamBone> linkedListNode = beam.m_bones.Last;
                return linkedListNode.Value.RotationAngle;
            }
        }

        public static BasicBeamController.BeamBone GetIndexedBone(this BasicBeamController beam, int boneIndex)
        {
            var bones = beam.m_bones;
            if (bones == null) return null;
            if (bones.ElementAt(boneIndex) == null) { Debug.LogError("Attempted to fetch a beam bone at an invalid index"); return null; }
            return bones.ElementAt(boneIndex);
        }
        public static Vector2 GetIndexedBonePosition(this BasicBeamController beam, int boneIndex)
        {
            LinkedList<BasicBeamController.BeamBone> bones;
            bones = beam.m_bones;
                
            if (bones.ElementAt(boneIndex) == null) { Debug.LogError("Attempted to fetch the position of a beam bone at an invalid index"); return Vector2.zero; }
            if (!beam.UsesBones)
            {
                return beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), bones.ElementAt(boneIndex).PosX);
            }
            if (beam.ProjectileAndBeamMotionModule != null)
            {
                return bones.ElementAt(boneIndex).Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(bones.ElementAt(boneIndex), beam, beam.projectile.Inverted);
            }
            return bones.ElementAt(boneIndex).Position;
        }
        public static Vector2 GetBonePosition(this BasicBeamController beam, BasicBeamController.BeamBone bone)
        {
            if (!beam.UsesBones)
            {
                return beam.Origin + BraveMathCollege.DegreesToVector(beam.Direction.ToAngle(), bone.PosX);
            }
            if (beam.ProjectileAndBeamMotionModule != null)
            {
                return bone.Position + beam.ProjectileAndBeamMotionModule.GetBoneOffset(bone, beam, beam.projectile.Inverted);
            }
            return bone.Position;
        }
    }

}
