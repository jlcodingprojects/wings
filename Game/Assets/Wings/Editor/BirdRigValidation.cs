using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Wings.Editor
{
    public static class BirdRigValidation
    {
        [Serializable] class Evidence { public string result; public int transformNodes,skinnedMeshes,clips; public float minimumChannelRotation; }
        public static void Validate()
        {
            const string path="Assets/Wings/Art/Bird/BirdRigTemplate.fbx";
            var importer=AssetImporter.GetAtPath(path) as ModelImporter;
            if(importer==null) throw new Exception("Run Tools/create_bird_rig.py to export the bird rig template.");
            importer.animationType=ModelImporterAnimationType.Generic; importer.importAnimation=true; importer.isReadable=true; importer.SaveAndReimport();
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var instance=UnityEngine.Object.Instantiate(asset);
            try
            {
                var transforms=instance.GetComponentsInChildren<Transform>();
                string[] required={"L_Shoulder","R_Shoulder","L_Elbow","R_Elbow","L_Wrist","R_Wrist","L_Hip","L_Knee","L_Foot","R_Hip","R_Knee","R_Foot","Head","Neck","Tail"};
                foreach(var name in required) if(!transforms.Any(t=>t.name==name)) throw new Exception("Missing rig bone: "+name);
                var skins=instance.GetComponentsInChildren<SkinnedMeshRenderer>();
                if(skins.Length==0 || skins.Any(s=>s.sharedMesh.boneWeights.Length==0)) throw new Exception("Bird template must have weighted meshes.");
                var clips=AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
                if(clips.Length==0) throw new Exception("Rig channel-check clip missing.");
                string[] animated={"L_Shoulder","R_Shoulder","L_Knee","R_Knee","Head","Tail"};
                var bones=animated.Select(n=>transforms.Single(t=>t.name==n)).ToArray();
                clips[0].SampleAnimation(instance,0); var before=bones.Select(b=>b.localRotation).ToArray();
                clips[0].SampleAnimation(instance,clips[0].length*0.5f);
                float movement=bones.Select((b,i)=>Quaternion.Angle(before[i],b.localRotation)).Min();
                if(movement<5) throw new Exception("One or more wing/leg/head/tail animation channels did not survive import.");
                string output=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Artifacts/bird-rig-validation.json"));
                File.WriteAllText(output,JsonUtility.ToJson(new Evidence {result="passed",transformNodes=transforms.Length,skinnedMeshes=skins.Length,clips=clips.Length,minimumChannelRotation=movement},true));
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
    }
}
