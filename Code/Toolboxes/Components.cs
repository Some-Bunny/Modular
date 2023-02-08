using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using UnityEngine;

public class CustomProximityMine : BraveBehaviour
{
    private void TransitionToIdle(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
        if (this.idleAnimName != null && !animator.IsPlaying(this.explodeAnimName))
        {
            animator.Play(this.idleAnimName);
        }
        animator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(animator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToIdle));
    }

    private void Update()
    {
        if (!this.MovesTowardEnemies && this.HomingTriggeredOnSynergy && GameManager.Instance.PrimaryPlayer.HasActiveBonusSynergy(this.TriggerSynergy, false))
        {
            this.MovesTowardEnemies = true;
        }
        if (this.MovesTowardEnemies)
        {
            RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
            float maxValue = float.MaxValue;
            AIActor nearestEnemy = absoluteRoom.GetNearestEnemy(base.sprite.WorldCenter, out maxValue, true, false);
            if (nearestEnemy && maxValue < this.HomingRadius)
            {
                Vector2 centerPosition = nearestEnemy.CenterPosition;
                Vector2 normalized = (centerPosition - base.sprite.WorldCenter).normalized;
                if (base.debris)
                {
                    base.debris.ApplyFrameVelocity(normalized * this.HomingSpeed);
                }
                else
                {
                    base.transform.position = base.transform.position + normalized.ToVector3ZisY(0f) * this.HomingSpeed * BraveTime.DeltaTime;
                }
            }
        }
    }

    private IEnumerator Start()
    {
        if (!string.IsNullOrEmpty(this.deployAnimName))
        {
            base.spriteAnimator.Play(this.deployAnimName);
            tk2dSpriteAnimator spriteAnimator = base.spriteAnimator;
            spriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(spriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToIdle));
        }
        else if (!string.IsNullOrEmpty(this.idleAnimName))
        {
            base.spriteAnimator.Play(this.idleAnimName);
        }
        if (this.explosionStyle == CustomProximityMine.ExplosiveStyle.PROXIMITY)
        {
            Vector2 position = base.transform.position.XY();
            List<AIActor> allActors = StaticReferenceManager.AllEnemies;
            AkSoundEngine.PostEvent("Play_OBJ_mine_set_01", base.gameObject);
            while (!this.m_triggered)
            {

                if (this.MovesTowardEnemies)
                {
                    position = base.sprite.WorldCenter;
                }
                if (!GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor)).HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
                {
                    this.m_triggered = true;
                    this.m_disarmed = true;
                    break;
                }
                bool shouldContinue = false;
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    if (GameManager.Instance.AllPlayers[i] && !GameManager.Instance.AllPlayers[i].IsGhost)
                    {
                        float num = Vector2.SqrMagnitude(position - GameManager.Instance.AllPlayers[i].specRigidbody.UnitCenter);
                        if (num < this.detectRadius * this.detectRadius)
                        {
                            shouldContinue = true;
                            break;
                        }
                    }
                }
                if (shouldContinue)
                {
                    yield return null;
                }
                else
                {
                    for (int j = 0; j < allActors.Count; j++)
                    {
                        AIActor aiactor = allActors[j];
                        if (aiactor.IsNormalEnemy)
                        {
                            if (aiactor.gameObject.activeSelf)
                            {
                                if (aiactor.HasBeenEngaged)
                                {
                                    if (!aiactor.healthHaver.IsDead)
                                    {
                                        float num2 = Vector2.SqrMagnitude(position - aiactor.specRigidbody.UnitCenter);
                                        if (num2 < this.detectRadius * this.detectRadius)
                                        {
                                            if (Force_Disarm == false)
                                            {
                                                this.m_triggered = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    yield return null;
                }
            }
        }
        else if (this.explosionStyle == CustomProximityMine.ExplosiveStyle.TIMED)
        {
            yield return new WaitForSeconds(this.explosionDelay);
            if (this.MovesTowardEnemies && this.HomingDelay > this.explosionDelay)
            {
                yield return new WaitForSeconds(this.HomingDelay - this.explosionDelay);
            }
        }
        if (!this.m_disarmed)
        {
            if (!string.IsNullOrEmpty(this.explodeAnimName))
            {
                base.spriteAnimator.Play(this.explodeAnimName);
                if (this.usesCustomExplosionDelay)
                {
                    yield return new WaitForSeconds(this.customExplosionDelay);
                }
                else
                {
                    tk2dSpriteAnimationClip clip = base.spriteAnimator.GetClipByName(this.explodeAnimName);
                    yield return new WaitForSeconds((float)clip.frames.Length / clip.fps);
                }
            }
            Exploder.Explode(base.sprite.WorldCenter.ToVector3ZUp(0f), this.explosionData, Vector2.zero, null, false, CoreDamageTypes.None, false);
            UnityEngine.Object.Destroy(base.gameObject);
        }
        else
        {
            base.spriteAnimator.StopAndResetFrame();
        }
        yield break;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public ExplosionData explosionData;

    public CustomProximityMine.ExplosiveStyle explosionStyle;

    [ShowInInspectorIf("explosionStyle", 0, false)]
    public float detectRadius = 2.5f;

    public float explosionDelay = 0.3f;

    public bool usesCustomExplosionDelay;

    [ShowInInspectorIf("usesCustomExplosionDelay", false)]
    public float customExplosionDelay = 0.1f;

    [CheckAnimation(null)]
    public string deployAnimName;

    [CheckAnimation(null)]
    public string idleAnimName;

    [CheckAnimation(null)]
    public string explodeAnimName;

    [Header("Homing")]
    public bool MovesTowardEnemies;

    public bool HomingTriggeredOnSynergy;

    [LongNumericEnum]
    public CustomSynergyType TriggerSynergy;

    public float HomingRadius = 5f;

    public float HomingSpeed = 3f;

    public float HomingDelay = 5f;

    protected bool m_triggered;

    protected bool m_disarmed;

    public bool Force_Disarm = false;


    public enum ExplosiveStyle
    {
        PROXIMITY,
        TIMED
    }
}
public class ImprovedAfterImage : BraveBehaviour
{

    public ImprovedAfterImage()
    {
        shaders = new List<Shader>
            {
                ShaderCache.Acquire("Brave/Internal/RainbowChestShader"),
                ShaderCache.Acquire("Brave/Internal/GlitterPassAdditive"),
                ShaderCache.Acquire("Brave/Internal/HologramShader"),
                ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage")
            };
        //shaders.Add(ShaderCache.Acquire("Brave/ItemSpecific/MetalSkinShader"));
        this.IsRandomShader = false;
        this.spawnShadows = true;
        this.shadowTimeDelay = 0.1f;
        this.shadowLifetime = 0.6f;
        this.minTranslation = 0.2f;
        this.maxEmission = 800f;
        this.minEmission = 100f;
        this.targetHeight = -2f;
        this.dashColor = new Color(1f, 0f, 1f, 1f);
        this.m_activeShadows = new LinkedList<Shadow>();
        this.m_inactiveShadows = new LinkedList<Shadow>();
        this.OverrideImageShader = null;
    }

    public void Start()
    {
        if (this.OptionalImageShader != null)
        {
            this.OverrideImageShader = this.OptionalImageShader;
        }
        if (base.transform.parent != null && base.transform.parent.GetComponent<Projectile>() != null)
        {
            base.transform.parent.GetComponent<Projectile>().OnDestruction += this.ProjectileDestruction;
        }
        this.m_lastSpawnPosition = base.transform.position;
    }

    private void ProjectileDestruction(Projectile source)
    {
        if (this.m_activeShadows.Count > 0)
        {
            GameManager.Instance.StartCoroutine(this.HandleDeathShadowCleanup());
        }
    }

    public void LateUpdate()
    {
        if (this.spawnShadows && !this.m_previousFrameSpawnShadows)
        {
            this.m_spawnTimer = this.shadowTimeDelay;
        }
        this.m_previousFrameSpawnShadows = this.spawnShadows;
        LinkedListNode<ImprovedAfterImage.Shadow> next;
        for (LinkedListNode<ImprovedAfterImage.Shadow> linkedListNode = this.m_activeShadows.First; linkedListNode != null; linkedListNode = next)
        {
            next = linkedListNode.Next;
            linkedListNode.Value.timer -= BraveTime.DeltaTime;
            if (linkedListNode.Value.timer <= 0f)
            {
                this.m_activeShadows.Remove(linkedListNode);
                this.m_inactiveShadows.AddLast(linkedListNode);
                if (linkedListNode.Value.sprite)
                {
                    linkedListNode.Value.sprite.renderer.enabled = false;
                }
            }
            else if (linkedListNode.Value.sprite)
            {
                float num = linkedListNode.Value.timer / this.shadowLifetime;
                Material sharedMaterial = linkedListNode.Value.sprite.renderer.sharedMaterial;
                sharedMaterial.SetFloat("_EmissivePower", Mathf.Lerp(this.maxEmission, this.minEmission, num));
                sharedMaterial.SetFloat("_Opacity", num);
            }
        }
        if (this.spawnShadows)
        {
            if (this.m_spawnTimer > 0f)
            {
                this.m_spawnTimer -= BraveTime.DeltaTime;
            }
            if (this.m_spawnTimer <= 0f && Vector2.Distance(this.m_lastSpawnPosition, base.transform.position) > this.minTranslation)
            {
                this.SpawnNewShadow();
                this.m_spawnTimer += this.shadowTimeDelay;
                this.m_lastSpawnPosition = base.transform.position;
            }
        }
    }

    private IEnumerator HandleDeathShadowCleanup()
    {
        while (this.m_activeShadows.Count > 0)
        {
            LinkedListNode<ImprovedAfterImage.Shadow> next;
            for (LinkedListNode<ImprovedAfterImage.Shadow> node = this.m_activeShadows.First; node != null; node = next)
            {
                next = node.Next;
                node.Value.timer -= BraveTime.DeltaTime;
                if (node.Value.timer <= 0f)
                {
                    this.m_activeShadows.Remove(node);
                    this.m_inactiveShadows.AddLast(node);
                    if (node.Value.sprite)
                    {
                        node.Value.sprite.renderer.enabled = false;
                    }
                }
                else if (node.Value.sprite)
                {
                    float num = node.Value.timer / this.shadowLifetime;
                    Material sharedMaterial = node.Value.sprite.renderer.sharedMaterial;
                    sharedMaterial.SetFloat("_EmissivePower", Mathf.Lerp(this.maxEmission, this.minEmission, num));
                    sharedMaterial.SetFloat("_Opacity", num);
                }
            }
            yield return null;
        }
        yield break;
    }

    public override void OnDestroy()
    {
        GameManager.Instance.StartCoroutine(this.HandleDeathShadowCleanup());
        base.OnDestroy();
    }


    private void SpawnNewShadow()
    {
        if (this.m_inactiveShadows == null)
        {
            return;
        }

        if (this.m_inactiveShadows.Count == 0)
        {
            this.CreateInactiveShadow();
        }

        LinkedListNode<ImprovedAfterImage.Shadow> first = this.m_inactiveShadows.First;
        tk2dSprite sprite = first.Value.sprite;
        this.m_inactiveShadows.RemoveFirst();
        if (!sprite || !sprite.renderer)
        {
            return;
        }


        first.Value.timer = this.shadowLifetime;
        sprite.SetSprite(base.sprite.Collection, base.sprite.spriteId);
        sprite.transform.position = base.sprite.transform.position;
        sprite.transform.rotation = base.sprite.transform.rotation;
        sprite.scale = base.sprite.scale;
        sprite.usesOverrideMaterial = true;
        sprite.IsPerpendicular = true;


        if (sprite.renderer && IsRandomShader)
        {
            sprite.renderer.enabled = true;
            sprite.renderer.material.shader = shaders[(int)UnityEngine.Random.Range(0, shaders.Count)];

            if (sprite.renderer.material.shader == shaders[3])
            {
                sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", this.minEmission);
                sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
                sprite.renderer.sharedMaterial.SetColor("_DashColor", Color.HSVToRGB(UnityEngine.Random.value, 1.0f, 1.0f));
            }
            if (sprite.renderer.material.shader == shaders[0])
            {
                sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 1f);
            }
        }
        else if (sprite.renderer)
        {

            sprite.renderer.enabled = true;
            sprite.renderer.material.shader = (this.OverrideImageShader ?? ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage"));
            sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", this.minEmission);
            sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
            sprite.renderer.sharedMaterial.SetColor("_DashColor", this.dashColor);
            sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 0f);
        }

        sprite.HeightOffGround = this.targetHeight;
        sprite.UpdateZDepth();
        this.m_activeShadows.AddLast(first);

    }

    public bool IsRandomShader;

    private void CreateInactiveShadow()
    {
        GameObject gameObject = new GameObject("after image");
        if (this.UseTargetLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(this.TargetLayer);
        }
        tk2dSprite sprite = gameObject.AddComponent<tk2dSprite>();
        gameObject.transform.parent = SpawnManager.Instance.VFX;
        this.m_inactiveShadows.AddLast(new ImprovedAfterImage.Shadow
        {
            timer = this.shadowLifetime,
            sprite = sprite
        });
    }


    public bool spawnShadows;

    public float shadowTimeDelay;

    public float shadowLifetime;

    public float minTranslation;

    public float maxEmission;

    public float minEmission;

    public float targetHeight;

    public Color dashColor;

    public Shader OptionalImageShader;

    public bool UseTargetLayer;

    public string TargetLayer;

    [NonSerialized]
    public Shader OverrideImageShader;

    private readonly LinkedList<ImprovedAfterImage.Shadow> m_activeShadows;

    private readonly LinkedList<ImprovedAfterImage.Shadow> m_inactiveShadows;

    private readonly List<Shader> shaders;

    private float m_spawnTimer;

    private Vector2 m_lastSpawnPosition;

    private bool m_previousFrameSpawnShadows;

    private class Shadow
    {
        public float timer;
        public tk2dSprite sprite;
    }
}
