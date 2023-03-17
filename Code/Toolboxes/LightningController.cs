using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class LightningController : MonoBehaviour
    {

        public static void Init()
        {
            AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
            var lightningReticlelocal = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

            LightningReticle = UnityEngine.Object.Instantiate<GameObject>(lightningReticlelocal);
            LightningReticle.gameObject.SetActive(false);

            FakePrefab.MarkAsFakePrefab(LightningReticle);
            DontDestroyOnLoad(LightningReticle);

            var component2 = LightningReticle.GetComponent<tk2dTiledSprite>();
            component2.renderer.gameObject.layer = 23;

            component2.usesOverrideMaterial = true;
            component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
            component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);

            component2.sprite.renderer.material.SetColor("_OverrideColor", new Color(1.4f, 1.7f, 1.7f));
            component2.sprite.renderer.material.SetColor("_EmissiveColor", new Color(1.4f, 1.7f, 1.7f));

            ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = 1;
            yes1.shadowTimeDelay = 0.1f;
            yes1.dashColor = new Color(0.9f, 1, 1).WithAlpha(3.33f);
            yes1.overrideHeight = 23;

            bundle = null;
        }

        public static GameObject LightningReticle;

        public void GenerateLightning(Vector3 startPosition, Vector3 impactPosition)
        {
            StartPosition = startPosition;
            ImpactPosition = impactPosition;
            Nodes = GenerateNodes();
            GameManager.Instance.StartCoroutine(StartLightning());
        }

        public IEnumerator StartLightning()
        {
            if (DoesDelay() == true)
            {
                if (OnPreDelay != null) { OnPreDelay(ImpactPosition); }
                float elapsed = 0;
                while (elapsed < LightningPreDelay)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is LightningMajorNode major)
                {
                    float elapsed = 0;
                    float elapsedTwo = 0;

                    while (elapsed < LightningMajorNodeDelay)
                    {
                        elapsed += BraveTime.DeltaTime;
                        yield return null;
                    }

                    for (int e = 0; e < major.minorNodes.Count - 1; e++)
                    {
                        elapsedTwo = 0;
                        while (elapsed < LightningMinorNodeDelay)
                        {
                            elapsedTwo += BraveTime.DeltaTime;
                            yield return null;
                        }
                        GameObject vfx = GenerateLine(major.minorNodes[e].position, major.minorNodes[e + 1].position);
                        UnityEngine.Object.Destroy(vfx, 0.25f);
                    }
                    if (UnityEngine.Random.value < MajorNodeSplitoffChance)
                    {
                        for (int e = 0; e < major.branchNodes.Count - 1; e++)
                        {
                            GameObject vfx = GenerateLine(major.branchNodes[e].position, major.branchNodes[e + 1].position);
                            UnityEngine.Object.Destroy(vfx, LinePieceLifetime);
                        }
                    }
                }
            }

            if (OnPostStrike != null) { OnPostStrike(ImpactPosition); }

            Destroy(this, 1);
            yield break;
        }

        public GameObject GenerateLine(Vector2 startPosition, Vector2 endPosition)
        {
            GameObject gameObject = SpawnManager.SpawnVFX(LightningReticle, false);
            gameObject.name = "Lightning_Piece";
            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            component2.gameObject.transform.position = startPosition;
            component2.renderer.gameObject.layer = 23;

            component2.transform.localRotation = Quaternion.Euler(0f, 0f, (endPosition - startPosition).ToAngle());
            component2.dimensions = new Vector2((Vector2.Distance(startPosition, endPosition) * 16), Thickness);

            ImprovedAfterImageForTiled yes1 = component2.gameObject.GetOrAddComponent<ImprovedAfterImageForTiled>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = AfterimageLifetime;

            return gameObject;
        }



        public List<LightningNode> GenerateNodes()
        {
            List<LightningNode> List = new List<LightningNode>();
            List.Add(new LightningMajorNode() { position = StartPosition });
            float Dist = (Vector3.Distance(ImpactPosition, StartPosition)) / MajorNodesCount;
            Dist *= PreFinalLengthMultiplier;
            for (int i = 1; i < MajorNodesCount + 1; i++)
            {

                Vector2 createdPosition = List[i - 1].position;

                float Offset = BraveUtility.RandomBool() == true ? UnityEngine.Random.Range(-MajorNodeMaxAngleSpacing, -MajorNodeMinAngleSpacing) : UnityEngine.Random.Range(MajorNodeMinAngleSpacing, MajorNodeMaxAngleSpacing);
                float Angle = (ImpactPosition - createdPosition).ToAngle();
                Angle += Offset;

                List.Add(new LightningMajorNode()
                {
                    position = (createdPosition + Toolbox.GetUnitOnCircle(Angle, Dist)),
                    minorNodes = GenerateMinorNodes(createdPosition, createdPosition + Toolbox.GetUnitOnCircle(Angle, Dist), UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax + 1), MinorNodeMaxAngleSpacing, MinorNodeMaxAngleSpacing),
                    branchNodes = GenerateMinorNodes(createdPosition + Toolbox.GetUnitOnCircle(Angle, Dist), createdPosition + Toolbox.GetUnitOnCircle(Angle, Dist) + Toolbox.GetUnitOnCircle(Angle, UnityEngine.Random.Range(RadiusBranchMin, RadiusBranchMax)), UnityEngine.Random.Range(MinorBranchNodesMin, MinorBranchNodesMin + 1), MinorBranchNodeMinAngleSpacing, MinorBranchNodeMaxAngleSpacing),//, UnityEngine.Random.Range(RadiusBranchMin, RadiusBranchMax), Angle)
                });
            }
            List.Add(new LightningMajorNode()
            {
                position = ImpactPosition,
                minorNodes = GenerateMinorNodes(List[List.Count - 1].position, ImpactPosition, UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax + 1), MinorNodeMaxAngleSpacing, MinorNodeMaxAngleSpacing)
            });
            return List;
        }


        public List<LightningNode> GenerateMinorNodes(Vector2 startPos, Vector2 endPos, float NodesToGenerate, float offsetLower, float offsetHigher)
        {
            float minorNodesTogenerate = NodesToGenerate;//UnityEngine.Random.Range(MinorNodesMin, MinorNodesMax+1);
            List<LightningNode> List = new List<LightningNode>();
            List.Add(new LightningNode() { position = startPos });
            float Dist = (Vector3.Distance(endPos, startPos)) / minorNodesTogenerate;
            for (int i = 1; i < minorNodesTogenerate + 1; i++)
            {
                Vector2 createdPosition = List[i - 1].position;

                float Offset = BraveUtility.RandomBool() == true ? UnityEngine.Random.Range(-offsetHigher, -offsetLower) : UnityEngine.Random.Range(offsetLower, offsetHigher);
                float Angle = (endPos - createdPosition).ToAngle();
                Angle += Offset;
                List.Add(new LightningNode()
                {
                    position = (createdPosition + Toolbox.GetUnitOnCircle(Angle, Dist)),
                });

            }
            List.Add(new LightningNode() { position = endPos });
            return List;
        }

        private bool DoesDelay()
        { return LightningPreDelay > 0; }


        public float LightningPreDelay = 0f;


        //Major Node Data
        public int MajorNodesCount = 2;
        public float MajorNodeMinAngleSpacing = 20f;
        public float MajorNodeMaxAngleSpacing = 45f;

        //Minor Node Data
        public int MinorNodesMin = 1;
        public int MinorNodesMax = 2;
        public float MinorNodeMinAngleSpacing = 7f;
        public float MinorNodeMaxAngleSpacing = 15f;

        //Minor Branch Node Data
        public int MinorBranchNodesMin = 3;
        public int MinorBranchNodesMax = 4;
        public float MinorBranchNodeMinAngleSpacing = 6f;
        public float MinorBranchNodeMaxAngleSpacing = 14f;
        public float RadiusBranchMin = 2f;
        public float RadiusBranchMax = 4f;
        public float MajorNodeSplitoffChance = 0.5f;

        public float LightningMajorNodeDelay = 0.01f;
        public float LightningMinorNodeDelay = 0.01f;

        public float LinePieceLifetime = 0.20f;
        public float AfterimageLifetime = 0.75f;


        public float PreFinalLengthMultiplier = 0.85f;

        public float Thickness = 1;
        //public float MinimumThickness = 1;

        public Vector2 ImpactPosition;
        public Vector2 StartPosition;

        public Action<Vector2> OnPostStrike;
        public Action<Vector2> OnPreDelay;


        public List<LightningNode> Nodes = new List<LightningNode>();

        public class LightningNode
        {
            public Vector2 position;
        }
        public class LightningMajorNode : LightningNode
        {
            public List<LightningNode> minorNodes = new List<LightningNode>();
            public List<LightningNode> branchNodes = new List<LightningNode>();
        }
    }
    public class ImprovedAfterImageForTiled : BraveBehaviour
    {

        public ImprovedAfterImageForTiled()
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
            this.lastSpawnAngle = base.transform.eulerAngles.z;
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
            LinkedListNode<ImprovedAfterImageForTiled.Shadow> next;
            for (LinkedListNode<ImprovedAfterImageForTiled.Shadow> linkedListNode = this.m_activeShadows.First; linkedListNode != null; linkedListNode = next)
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
            if (this.spawnShadows)// && CanTrigger)
            {
                if (base.GetComponent<tk2dTiledSprite>() == null) { ETGModConsole.Log("fucc"); }
                if (this.m_spawnTimer > 0f)
                {
                    this.m_spawnTimer -= BraveTime.DeltaTime;
                }
                if (this.m_spawnTimer <= 0f)
                {
                    this.SpawnNewShadow();
                    this.m_spawnTimer += this.shadowTimeDelay;
                    this.lastSpawnAngle = base.transform.eulerAngles.z;
                }
            }
        }

        private IEnumerator HandleDeathShadowCleanup()
        {
            while (this.m_activeShadows.Count > 0)
            {
                LinkedListNode<ImprovedAfterImageForTiled.Shadow> next;
                for (LinkedListNode<ImprovedAfterImageForTiled.Shadow> node = this.m_activeShadows.First; node != null; node = next)
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

            if (base.GetComponentInChildren<tk2dTiledSprite>() == null) { ETGModConsole.Log("tk2dTiledSprite is NULL"); return; }
            if (base.GetComponent<tk2dTiledSprite>() == null) { ETGModConsole.Log("tk2dTiledSprite is NULL"); return; }
            if (this.m_inactiveShadows.Count == 0)
            {
                this.CreateInactiveShadow();
            }


            LinkedListNode<ImprovedAfterImageForTiled.Shadow> first = this.m_inactiveShadows.First;
            tk2dTiledSprite sprite = first.Value.sprite;
            this.m_inactiveShadows.RemoveFirst();
            if (!sprite || !sprite.renderer)
            {
                return;
            }

            first.Value.timer = this.shadowLifetime;


            sprite.SetSprite(base.GetComponent<tk2dTiledSprite>().sprite.Collection, base.GetComponent<tk2dTiledSprite>().sprite.spriteId);

            sprite.transform.position = base.GetComponent<tk2dTiledSprite>().sprite.transform.position;

            sprite.transform.rotation = base.GetComponent<tk2dTiledSprite>().sprite.transform.rotation;

            if (base.transform.parent != null)
            {
                if (base.transform.parent.GetComponentInChildren<BasicBeamController>() != null)
                {

                    float angle = base.transform.parent.GetComponentInChildren<BasicBeamController>().Direction.ToAngle();

                    sprite.transform.rotation = Quaternion.Euler(0, 0, angle);

                }
                if (base.transform.parent.GetComponentInChildren<BeamController>() != null)
                {
                    float angle = base.transform.parent.GetComponentInChildren<BeamController>().Direction.ToAngle();
                    sprite.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }


            sprite.scale = base.GetComponent<tk2dTiledSprite>().sprite.scale;
            sprite.dimensions = base.GetComponent<tk2dTiledSprite>().dimensions;
            sprite.usesOverrideMaterial = true;
            sprite.IsPerpendicular = true;
            sprite.renderer.enabled = true;
            if (overrideHeight != -1)
            {
                sprite.renderer.gameObject.layer = overrideHeight;

            }

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
                sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 1f);
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
            //gameObject.AddComponent<tk2dBaseSprite>();
            tk2dTiledSprite sprite = gameObject.AddComponent<tk2dTiledSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            this.m_inactiveShadows.AddLast(new ImprovedAfterImageForTiled.Shadow
            {
                timer = this.shadowLifetime,
                sprite = sprite
            });
        }

        public int overrideHeight = -1;

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

        private readonly LinkedList<ImprovedAfterImageForTiled.Shadow> m_activeShadows;

        private readonly LinkedList<ImprovedAfterImageForTiled.Shadow> m_inactiveShadows;

        private readonly List<Shader> shaders;

        private float m_spawnTimer;

        private float lastSpawnAngle;

        private bool m_previousFrameSpawnShadows;

        private class Shadow
        {
            public float timer;
            public tk2dTiledSprite sprite;
        }
    }

}
