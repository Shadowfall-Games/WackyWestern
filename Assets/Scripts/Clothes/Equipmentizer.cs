using System.Collections.Generic;
using UnityEngine;
public class ClothingBoneMapper : MonoBehaviour
{
    public SkinnedMeshRenderer TargetMeshRenderer;
    void Start()
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in TargetMeshRenderer.bones)
            boneMap[bone.gameObject.name] = bone;
        SkinnedMeshRenderer myRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        int bonenamelength;         //Added in case bonename needs to be modified in forloop below
        string bonename;            //Added in case bonename needs to be modified in forloop below
        string newname;             //Added in case bonename needs to be modified in forloop below
        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            bonename = bone.name;                   //Added in case bonename needs to be modified in forloop below
            bonenamelength = bonename.Length;       //Added in case bonename needs to be modified in forloop below
            Debug.Log(bone.name);
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {
                newname = bonename.Substring(0, bonenamelength - 2);        //Added to modify the name of the bone to drop the last 2 digits.
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton. Checking for name " + newname);    //Modified message to indicate bonename was modified
                if (!boneMap.TryGetValue(newname, out newBones[i]))                                                         //Added in case bonename needs to be modified in forloop below.  Repeats the mapping with last 2 characters removed from name
                {
                    Debug.Log("Unable to map bone \"" + bone.name + "\" or \"" + newname + "\" to target skeleton.");       //Added to repeat the bonemap with newname.  Will provide new debug and break if this also fails.
                    break;
                }
            }
        }
        myRenderer.bones = newBones;
    }
}