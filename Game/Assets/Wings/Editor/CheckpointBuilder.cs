using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Wings.Editor
{
    public static class CheckpointBuilder
    {
        const string Root = "Assets/Wings/Generated";
        const string ScenePath = Root + "/FlightStudy.unity";
        static string Repository => Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        static Material grass, rock, darkRock, water, trunk, leaves, gold, birdMaterial, cream;

        [MenuItem("Wings/Rebuild checkpoint scene and player")]
        public static void Build()
        {
            CreateScene();
            Validate();
            BirdRigValidation.Validate();
            string output = Path.Combine(Repository, "Builds/Checkpoint1D/Wings.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath }, locationPathName = output,
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
            });
            File.WriteAllText(Path.Combine(Repository, "Artifacts/build-summary.json"), JsonUtility.ToJson(new BuildEvidence
            {
                result = report.summary.result.ToString(), errors = report.summary.totalErrors,
                warnings = report.summary.totalWarnings, bytes = report.summary.totalSize,
                unity = Application.unityVersion, backend = "Mono", graphics = "Direct3D11", scene = ScenePath
            }, true));
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Windows player build failed.");
            Debug.Log("WINGS_BUILD_PASS: " + output);
        }

        [Serializable] class BuildEvidence
        {
            public string result, unity, backend, graphics, scene;
            public int errors, warnings;
            public ulong bytes;
        }

        // Offscreen scene-only preview. This does not validate the runtime HUD or window presentation.
        public static void CaptureScene()
        {
            EditorSceneManager.OpenScene(ScenePath);
            ShaderUtil.allowAsyncCompilation = false;
            var camera = Camera.main;
            var target = new RenderTexture(1440, 900, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(1440, 900, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                target.Create();
                var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
                Debug.Log("WINGS_PREVIEW_MATERIALS: meadow=" + AssetDatabase.LoadAssetAtPath<Material>(Root + "/Meadow.mat").GetColor("_BaseColor") + " bird=" + AssetDatabase.LoadAssetAtPath<Material>(Root + "/Bird slate.mat").GetColor("_BaseColor"));
                RenderPipeline.SubmitRenderRequest(camera, request);
                RenderPipeline.SubmitRenderRequest(camera, request);
                RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, 1440, 900), 0, 0); image.Apply();
                File.WriteAllBytes(Path.Combine(Repository, "Artifacts/checkpoint1a-scene-preview.png"), image.EncodeToPNG());
                Debug.Log("WINGS_SCENE_PREVIEW_WRITTEN");
            }
            finally
            {
                RenderTexture.active = previous;
                target.Release();
                UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(image);
            }
        }

        public static void CreateScene()
        {
            Directory.CreateDirectory(Root);
            AssetDatabase.Refresh();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "Wings Study";
            PlayerSettings.productName = "Wings - Flight Study";
            PlayerSettings.bundleVersion = "0.4.0-checkpoint1d";
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });
            PlayerSettings.defaultScreenWidth = 1440;
            PlayerSettings.defaultScreenHeight = 900;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            QualitySettings.vSyncCount = 0;
            QualitySettings.shadows = UnityEngine.ShadowQuality.All;
            QualitySettings.shadowDistance = 150;
            Time.fixedDeltaTime = 0.02f;

            grass = Mat("Meadow", new Color(0.34f, 0.47f, 0.29f));
            rock = Mat("Ridge", new Color(0.43f, 0.50f, 0.49f));
            darkRock = Mat("Stone", new Color(0.27f, 0.35f, 0.35f));
            water = Mat("Lake", new Color(0.20f, 0.49f, 0.57f), 0.55f);
            trunk = Mat("Bark", new Color(0.30f, 0.24f, 0.17f));
            leaves = Mat("Canopy", new Color(0.20f, 0.35f, 0.28f));
            gold = Mat("Guide", new Color(0.90f, 0.67f, 0.33f));
            birdMaterial = Mat("Bird slate", new Color(0.10f, 0.22f, 0.29f));
            cream = Mat("Bird warm", new Color(0.93f, 0.82f, 0.60f));
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.66f, 0.79f, 0.80f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0032f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.64f, 0.76f, 0.83f);
            RenderSettings.ambientEquatorColor = new Color(0.49f, 0.56f, 0.49f);
            RenderSettings.ambientGroundColor = new Color(0.26f, 0.29f, 0.24f);
            var sun = new GameObject("Afternoon light").AddComponent<Light>();
            sun.type = LightType.Directional; sun.intensity = 1.5f;
            sun.color = new Color(1, 0.91f, 0.76f); sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(42, -38, 0);
            RenderSettings.sun = sun;
            var sky = new Material(Shader.Find("Skybox/Procedural"));
            sky.SetFloat("_SunSize", 0.025f);
            sky.SetColor("_SkyTint", new Color(0.51f, 0.60f, 0.65f));
            sky.SetColor("_GroundColor", new Color(0.37f, 0.48f, 0.43f));
            sky.SetFloat("_Exposure", 1.15f);
            SaveAsset(sky, "Sky.mat"); RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Sky.mat");

            Primitive("Meadow floor", PrimitiveType.Cube, new Vector3(0, -2, 0), new Vector3(720, 4, 720), grass);
            var lake = Primitive("Lake", PrimitiveType.Cylinder, new Vector3(115, 0.10f, 40), new Vector3(115, 0.1f, 160), water);
            // A scaled capsule would expand vertically with the lake's width.
            UnityEngine.Object.DestroyImmediate(lake.GetComponent<Collider>());
            lake.AddComponent<MeshCollider>().sharedMesh = lake.GetComponent<MeshFilter>().sharedMesh;
            lake.AddComponent<WaterSurface>();
            var random = new System.Random(1249);
            for (int i = 0; i < 24; i++)
            {
                float angle = i * Mathf.PI * 2 / 24;
                var location = new Vector3(Mathf.Cos(angle) * 290, 0, Mathf.Sin(angle) * 290);
                float height = 45 + (float)random.NextDouble() * 65;
                MeshObject("Distant ridge " + i, Cone(45 + (float)random.NextDouble() * 35, height, 7), location, rock, true);
            }
            for (int i = 0; i < 65; i++)
            {
                float x = -230 + (float)random.NextDouble() * 440;
                float z = -210 + (float)random.NextDouble() * 450;
                if (Mathf.Abs(x) < 32 || (x > 48 && x < 185 && z > -50 && z < 130)) continue;
                if (Vector2.Distance(new Vector2(x, z), new Vector2(-55, 65)) < 15 || Vector2.Distance(new Vector2(x, z), new Vector2(100, 145)) < 15) continue;
                float height = 6 + (float)random.NextDouble() * 8;
                Primitive("Tree trunk", PrimitiveType.Cylinder, new Vector3(x, height * 0.3f, z), new Vector3(0.7f, height * 0.3f, 0.7f), trunk);
                MeshObject("Tree crown", Cone(height * 0.30f, height, 7), new Vector3(x, height * 0.25f, z), leaves, true);
            }
            var assisted = Profile("Assisted", FlightExperiment.Assisted, 5, 7, 32);
            var momentum = Profile("Momentum", FlightExperiment.Momentum, 1.5f, 1.8f, 52);
            var bird = new GameObject("Player bird").AddComponent<BirdMotor>();
            bird.profile = assisted;
            bird.perches = new[] { CreatePerch("Overlook branch", new Vector3(-20,22,-55)), CreatePerch("Meadow bough",new Vector3(0,22,-10)), CreatePerch("Woodland roost",new Vector3(-55,18,65)), CreatePerch("Lakeside branch",new Vector3(100,10,145)) };
            bird.startingPerch = bird.perches[0];
            bird.startingPerch.transform.rotation = Quaternion.Euler(0,24,0);
            bird.origin = new Vector3(0,25,-45);
            bird.transform.position = bird.startingPerch.Point;
            var motionPath=Root+"/PerchMotion.asset";
            bird.perchMotion=AssetDatabase.LoadAssetAtPath<PerchMotionProfile>(motionPath);
            if(bird.perchMotion==null) { bird.perchMotion=ScriptableObject.CreateInstance<PerchMotionProfile>(); AssetDatabase.CreateAsset(bird.perchMotion,motionPath); }
            var visual = new GameObject("MotionRoot").transform; visual.SetParent(bird.transform,false);
            var presentation = bird.gameObject.AddComponent<BirdPresentation>(); presentation.motor=bird; presentation.visual=visual;
            presentation.rig=CreateBirdRig(visual);
            PrefabUtility.SaveAsPrefabAsset(visual.gameObject,Root+"/BirdPrototype.prefab");
            ExportRigTemplate(visual);
            MeshObject("Overlook rocks",Cone(11,17,7),new Vector3(-29,0,-58),rock,true);
            var input = bird.gameObject.AddComponent<FlightInput>(); bird.input = input;
            var camera = new GameObject("Flight camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.fieldOfView = 64; camera.farClipPlane = 900;
            camera.gameObject.AddComponent<AudioListener>();
            camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            var follow = camera.gameObject.AddComponent<FlightCamera>(); follow.bird = bird; follow.input = input;
            camera.transform.position = bird.startingPerch.Point + new Vector3(-3, 1, -4);
            camera.transform.LookAt(bird.startingPerch.Point);
            var sandbox = new GameObject("Flight experiment").AddComponent<FlightSandbox>();
            sandbox.bird = bird; sandbox.input = input; sandbox.flightCamera = follow;
            sandbox.assistedAsset = assisted; sandbox.momentumAsset = momentum; input.sandbox = sandbox;
            sandbox.gameObject.AddComponent<RuntimeSmoke>();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("WINGS_SCENE_CREATED");
        }

        public static void Validate()
        {
            if (GraphicsSettings.defaultRenderPipeline is not UniversalRenderPipelineAsset) throw new Exception("URP not configured.");
            if (PlayerSettings.GetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone) != ScriptingImplementation.Mono2x) throw new Exception("Expected Mono.");
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64)) throw new Exception("Windows x64 target missing.");
            const string fbx = "Assets/Wings/Art/Smoke/OneMetreReference.fbx";
            var importer = AssetImporter.GetAtPath(fbx) as ModelImporter;
            if (importer == null) throw new Exception("Blender FBX not found.");
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.importAnimation = true;
            importer.SaveAndReimport();
            var instance = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(fbx));
            try
            {
                var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer == null) throw new Exception("Reference skin missing.");
                Vector3 size = renderer.bounds.size;
                if ((size - Vector3.one).magnitude > 0.06f) throw new Exception("FBX scale mismatch: " + size);
                var marker = instance.GetComponentsInChildren<Transform>().Single(t => t.name == "ForwardMarker");
                if (Vector3.Distance(marker.position, new Vector3(0, 0.5f, 1.5f)) > 0.02f) throw new Exception("FBX orientation mismatch: " + marker.position);
                var clips = AssetDatabase.LoadAllAssetsAtPath(fbx).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__")).ToArray();
                if (clips.Length == 0) throw new Exception("No animation imported.");
                var mesh0 = new Mesh(); var mesh1 = new Mesh();
                clips[0].SampleAnimation(instance, 0); renderer.BakeMesh(mesh0);
                clips[0].SampleAnimation(instance, clips[0].length * 0.5f); renderer.BakeMesh(mesh1);
                float movement = mesh0.vertices.Zip(mesh1.vertices, (a, b) => Vector3.Distance(a, b)).Max();
                UnityEngine.Object.DestroyImmediate(mesh0); UnityEngine.Object.DestroyImmediate(mesh1);
                if (movement < 0.05f) throw new Exception("Imported animation did not deform the mesh.");
                File.WriteAllText(Path.Combine(Repository, "Artifacts/unity-validation.json"), JsonUtility.ToJson(new ValidationEvidence
                { result = "passed", referenceSize = size, forwardMarker = marker.position, importedClips = clips.Length, animationVertexMovement = movement, urp = GraphicsSettings.defaultRenderPipeline.name }, true));
                Debug.Log("WINGS_VALIDATION_PASS: scale=" + size + " animation=" + movement);
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }

        [Serializable] class ValidationEvidence { public string result, urp; public Vector3 referenceSize, forwardMarker; public int importedClips; public float animationVertexMovement; }
        static Perch CreatePerch(string name, Vector3 point)
        {
            var branch=Primitive(name+" bough",PrimitiveType.Cylinder,point+Vector3.down*0.76f,new Vector3(0.28f,3.2f,0.28f),trunk);
            branch.transform.rotation=Quaternion.Euler(0,0,90);
            Primitive(name+" trunk",PrimitiveType.Cylinder,new Vector3(point.x-2.5f,(point.y-0.76f)*0.5f,point.z),new Vector3(0.8f,(point.y-0.76f)*0.5f,0.8f),trunk);
            var crown=Primitive(name+" foliage",PrimitiveType.Sphere,point+new Vector3(-4,-3,1),new Vector3(5,3,4),leaves);
            var node=new GameObject(name); node.transform.position=point;
            var perch=node.AddComponent<Perch>(); perch.displayName=name;
            return perch;
        }
        static Transform Bone(string name,Transform parent,Vector3 point)
        {
            var bone=new GameObject(name).transform; bone.SetParent(parent,false); bone.localPosition=point; return bone;
        }
        static BirdRig CreateBirdRig(Transform root)
        {
            var rig=root.gameObject.AddComponent<BirdRig>();
            rig.body=Bone("Body",root,Vector3.zero);
            BirdPart("Body mesh",PrimitiveType.Sphere,Vector3.zero,new Vector3(0.62f,0.55f,1.15f),birdMaterial,rig.body);
            BirdPart("Breast",PrimitiveType.Sphere,new Vector3(0,-0.1f,0.2f),new Vector3(0.5f,0.44f,0.85f),cream,rig.body);
            var neck=Bone("Neck",rig.body,new Vector3(0,0.18f,0.38f));
            rig.head=Bone("Head",neck,new Vector3(0,0.13f,0.17f));
            BirdPart("Head mesh",PrimitiveType.Sphere,Vector3.zero,new Vector3(0.42f,0.4f,0.45f),birdMaterial,rig.head);
            BirdPart("Beak",PrimitiveType.Sphere,new Vector3(0,-0.035f,0.28f),new Vector3(0.13f,0.1f,0.28f),gold,rig.head);
            for(int side=-1;side<=1;side+=2)
            {
                BirdPart("Eye rim",PrimitiveType.Sphere,new Vector3(side*0.188f,0.045f,0.10f),new Vector3(0.075f,0.11f,0.11f),cream,rig.head);
                BirdPart("Eye",PrimitiveType.Sphere,new Vector3(side*0.217f,0.045f,0.12f),new Vector3(0.055f,0.067f,0.067f),darkRock,rig.head);
            }
            rig.tail=Bone("Tail",rig.body,new Vector3(0,-0.02f,-0.52f));
            rig.tailFeathers=new Transform[7];
            for(int i=0;i<7;i++)
            {
                rig.tailFeathers[i]=Bone("TailFeather_"+i,rig.tail,new Vector3((i-3)*0.06f,0,0));
                BirdPart("Feather",PrimitiveType.Sphere,new Vector3(0,0,-0.35f),new Vector3(0.14f,0.045f,0.8f),i%2==0?birdMaterial:cream,rig.tailFeathers[i]);
            }
            rig.leftWing=CreateWing(rig.body,-1); rig.rightWing=CreateWing(rig.body,1);
            rig.leftLeg=CreateLeg(root,-1); rig.rightLeg=CreateLeg(root,1);
            return rig;
        }
        [Serializable] class RigExport { public RigBone[] bones; public RigMesh[] meshes; }
        [Serializable] class RigBone { public string name,parent; public Vector3 head,tail; }
        [Serializable] class RigMesh { public string name,bone; public Vector3[] vertices; public int[] triangles; public Color color; }
        static void ExportRigTemplate(Transform root)
        {
            var bones=root.GetComponentsInChildren<Transform>().Where(t=>t.GetComponent<MeshFilter>()==null).ToArray();
            var exported=bones.Select(b=>new RigBone {
                name=b.name,parent=b==root?"":b.parent.name,head=root.InverseTransformPoint(b.position),
                tail=root.InverseTransformPoint(b.position)+Vector3.up*0.15f
            }).ToArray();
            var meshes=root.GetComponentsInChildren<MeshFilter>().Select(m=>new RigMesh {
                name=m.name,bone=m.transform.parent.name,
                vertices=m.sharedMesh.vertices.Select(v=>root.InverseTransformPoint(m.transform.TransformPoint(v))).ToArray(),
                triangles=m.sharedMesh.triangles,color=m.GetComponent<Renderer>().sharedMaterial.GetColor("_BaseColor")
            }).ToArray();
            string folder=Path.Combine(Repository,"SourceArt/Bird"); Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder,"PrototypeRig.json"),JsonUtility.ToJson(new RigExport {bones=exported,meshes=meshes}));
        }
        static BirdRig.Wing CreateWing(Transform body,int side)
        {
            var wing=new BirdRig.Wing(); string name=side<0?"L":"R";
            wing.shoulder=Bone(name+"_Shoulder",body,new Vector3(side*0.24f,0.1f,0.13f));
            BirdPart("Upper wing",PrimitiveType.Sphere,new Vector3(side*0.24f,0,-0.1f),new Vector3(0.62f,0.09f,0.5f),birdMaterial,wing.shoulder);
            wing.elbow=Bone(name+"_Elbow",wing.shoulder,new Vector3(side*0.5f,0,-0.07f));
            BirdPart("Forewing",PrimitiveType.Sphere,new Vector3(side*0.23f,0,-0.1f),new Vector3(0.55f,0.075f,0.47f),birdMaterial,wing.elbow);
            wing.wrist=Bone(name+"_Wrist",wing.elbow,new Vector3(side*0.46f,0,-0.08f));
            for(int i=0;i<6;i++) BirdPart("Primary_"+i,PrimitiveType.Sphere,new Vector3(side*(0.12f+i*0.07f),0,-0.05f-i*0.065f),new Vector3(0.5f-i*0.035f,0.04f,0.14f),i%2==0?cream:birdMaterial,wing.wrist);
            return wing;
        }
        static BirdRig.Leg CreateLeg(Transform root,int side)
        {
            string name=side<0?"L":"R"; var leg=new BirdRig.Leg();
            leg.hip=Bone(name+"_Hip",root,new Vector3(side*0.17f,-0.12f,-0.08f));
            BirdPart("Thigh",PrimitiveType.Cube,Vector3.down*0.175f,new Vector3(0.065f,0.35f,0.065f),gold,leg.hip);
            leg.knee=Bone(name+"_Knee",leg.hip,Vector3.down*0.35f);
            BirdPart("Shin",PrimitiveType.Cube,Vector3.down*0.175f,new Vector3(0.055f,0.35f,0.055f),gold,leg.knee);
            leg.ankle=Bone(name+"_Ankle",leg.knee,Vector3.down*0.35f);
            leg.foot=Bone(name+"_Foot",leg.ankle,Vector3.zero);
            leg.toes=new Transform[4];
            for(int i=0;i<4;i++)
            {
                var toe=Bone(name+"_Toe_"+i,leg.foot,new Vector3((i-1)*0.045f,0,0)); leg.toes[i]=toe;
                BirdPart("Toe",PrimitiveType.Cube,new Vector3(0,0,i==3?-0.10f:0.13f),new Vector3(0.033f,0.03f,i==3?0.2f:0.26f),gold,toe);
            }
            return leg;
        }
        static void SaveAsset(UnityEngine.Object asset, string name)
        {
            string path = Root + "/" + name;
            var existing = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (existing != null) { EditorUtility.CopySerialized(asset, existing); UnityEngine.Object.DestroyImmediate(asset); }
            else AssetDatabase.CreateAsset(asset, path);
        }
        static Material Mat(string name, Color color, float smoothness = 0.05f)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = name;
            mat.SetColor("_BaseColor", color); mat.SetFloat("_Smoothness", smoothness);
            SaveAsset(mat, name + ".mat");
            return AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + name + ".mat");
        }
        static FlightProfile Profile(string name, FlightExperiment experiment, float response, float velocityResponse, float bank)
        {
            string path = Root + "/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<FlightProfile>(path);
            if (existing != null)
            {
                EditorUtility.SetDirty(existing); // Serialize newly added defaults, preserving existing tuning.
                return existing;
            }
            var profile = ScriptableObject.CreateInstance<FlightProfile>();
            profile.experiment = experiment; profile.response = response; profile.velocityResponse = velocityResponse; profile.maxBank = bank;
            AssetDatabase.CreateAsset(profile, path); return profile;
        }
        static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
        {
            var obj = GameObject.CreatePrimitive(type); obj.name = name;
            obj.transform.position = position; obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material; return obj;
        }
        static void BirdPart(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            var part = Primitive(name, type, Vector3.zero, scale, material);
            UnityEngine.Object.DestroyImmediate(part.GetComponent<Collider>());
            part.transform.SetParent(parent, false); part.transform.localPosition = position;
        }
        static Transform Wing(Transform parent, int side)
        {
            var pivot = new GameObject(side < 0 ? "Left wing" : "Right wing").transform;
            pivot.SetParent(parent, false); pivot.localPosition = new Vector3(side * 0.24f, 0.07f, 0.12f);
            BirdPart("Wing", PrimitiveType.Cube, new Vector3(side * 0.67f, 0, -0.08f), new Vector3(1.4f, 0.09f, 0.58f), birdMaterial, pivot);
            BirdPart("Wing tip", PrimitiveType.Cube, new Vector3(side * 1.37f, 0, -0.22f), new Vector3(0.4f, 0.07f, 0.35f), cream, pivot);
            return pivot;
        }
        static void MeshObject(string name, Mesh mesh, Vector3 position, Material material, bool collider)
        {
            string assetName = name.Replace(" ", "_") + ".asset";
            // Mesh dimensions differ between trees; use current root count for stable generation order.
            assetName = "Mesh_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().rootCount + "_" + assetName;
            SaveAsset(mesh, assetName);
            mesh = AssetDatabase.LoadAssetAtPath<Mesh>(Root + "/" + assetName);
            var obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            obj.transform.position = position; obj.GetComponent<MeshFilter>().sharedMesh = mesh;
            obj.GetComponent<MeshRenderer>().sharedMaterial = material;
            if (collider) obj.AddComponent<MeshCollider>().sharedMesh = mesh;
        }
        static Mesh Cone(float radius, float height, int sides)
        {
            var vertices = new Vector3[sides * 3]; var triangles = new int[vertices.Length];
            for (int i = 0; i < sides; i++)
            {
                float a = i * Mathf.PI * 2 / sides, b = (i + 1) * Mathf.PI * 2 / sides;
                vertices[i * 3] = new Vector3(Mathf.Cos(a) * radius, 0, Mathf.Sin(a) * radius);
                vertices[i * 3 + 1] = Vector3.up * height;
                vertices[i * 3 + 2] = new Vector3(Mathf.Cos(b) * radius, 0, Mathf.Sin(b) * radius);
            }
            for (int i = 0; i < triangles.Length; i++) triangles[i] = i;
            var mesh = new Mesh { vertices = vertices, triangles = triangles }; mesh.RecalculateNormals(); return mesh;
        }
        static Mesh Ring(float radius, float thickness)
        {
            const int around = 64, tube = 6;
            var v = new Vector3[around * tube]; var t = new int[around * tube * 6];
            for (int i = 0; i < around; i++) for (int j = 0; j < tube; j++)
            {
                float a = i * Mathf.PI * 2 / around, b = j * Mathf.PI * 2 / tube;
                v[i * tube + j] = new Vector3(Mathf.Cos(a) * (radius + thickness * Mathf.Cos(b)), Mathf.Sin(a) * (radius + thickness * Mathf.Cos(b)), thickness * Mathf.Sin(b));
                int n = (i * tube + j) * 6, p = i * tube + j, q = ((i + 1) % around) * tube + j, r = i * tube + (j + 1) % tube, s = ((i + 1) % around) * tube + (j + 1) % tube;
                t[n] = p; t[n + 1] = r; t[n + 2] = q; t[n + 3] = r; t[n + 4] = s; t[n + 5] = q;
            }
            var mesh = new Mesh { vertices = v, triangles = t }; mesh.RecalculateNormals(); return mesh;
        }
    }
}
