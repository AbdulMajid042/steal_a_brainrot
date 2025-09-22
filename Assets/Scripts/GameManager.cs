using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material daySky, nightSky;
    public Color dayColor, nightColor;
    public GameObject boyPlayer, girlPlayer;
    public Animator targetAnimator;   // Drag your Animator here
    public Avatar boyAvatar, girlAvatar;

    public string[] girlPropsNames;
    public string[] boysPropsNames;
    public SkinnedMeshRenderer girlLegRight, girlLegLeft, girlHead, girlBody, girlArmRight, girlArmLeft;
    public SkinnedMeshRenderer boyLegRight, boyLegLeft, boyHead, boyBody, boyArmRight, boyArmLeft;
    [System.Serializable]
    public class GirlsMaterials
    {
        public Material leg_right, leg_left, head, body, arm_right, arm_left;
    }
    public GirlsMaterials[] girlsMaterials;
    [System.Serializable]
    public class BoysMaterials
    {
        public Material leg_right, leg_left, head, body, arm_right, arm_left;
    }
    public BoysMaterials[] boysMaterials;
   
    public int selectedAvatarIndex = 0;
    private void Awake()
    {
        if (PlayerPrefs.GetString("Avatar") == "Boy")
        {
            boyPlayer.SetActive(true);
            girlPlayer.SetActive(false);
            targetAnimator.avatar = boyAvatar;
        }
        else
        {
            boyPlayer.SetActive(false);
            girlPlayer.SetActive(true);
            targetAnimator.avatar = girlAvatar;
        }
        Invoke("ChangeCostumeWithDelay", 0.1f);
    }
    void ChangeCostumeWithDelay()
    {
        ChangeCostume(selectedAvatarIndex);
    }
    public void ChangeCostumeWithVideo(int index)
    {
        selectedAvatarIndex = index;

        if (Ads_Manager.instance)
        {
            Ads_Manager.instance.ShowRewardedVideo(ChangeCostumeWithDelay);
        }
    }
    public void ChangeCostume(int index)
    {
        if (PlayerPrefs.GetString("Avatar") == "Boy")
        {
            boyLegRight.material = boysMaterials[index].leg_right;
            boyLegLeft.material = boysMaterials[index].leg_left;
            boyHead.material = boysMaterials[index].head;
            boyBody.material = boysMaterials[index].body;
            boyArmRight.material = boysMaterials[index].arm_right;
            boyArmLeft.material = boysMaterials[index].arm_left;
            EnableChildByName(boyPlayer.transform, boysPropsNames[index]);
        }
        else
        {
            girlLegRight.material = girlsMaterials[index].leg_right;
            girlLegLeft.material = girlsMaterials[index].leg_left;
            girlHead.material = girlsMaterials[index].head;
            girlBody.material = girlsMaterials[index].body;
            girlArmRight.material = girlsMaterials[index].arm_right;
            girlArmLeft.material = girlsMaterials[index].arm_left;
            EnableChildByName(girlPlayer.transform, girlPropsNames[index]);
        }

        if(UIManager.instance)
        {
            UIManager.instance.boyCollection.SetActive(false);
            UIManager.instance.girlCollection.SetActive(false);
            UIManager.instance.SetTime(1);

            UIManager.instance.boyCollection.transform.parent.gameObject.SetActive(false);
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            selectedAvatarIndex++;
            ChangeCostume(selectedAvatarIndex);
        }
    }
    private List<GameObject> propsObjects = new List<GameObject>();
    void EnableChildByName(Transform parent, string childName)
    {
        if (propsObjects.Count > 0)
        {
            foreach (GameObject go in propsObjects)
            {
                if (go != null)
                    go.SetActive(false);
            }
            propsObjects.Clear();
        }
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
            {
                child.gameObject.SetActive(true);
                propsObjects.Add(child.gameObject);

                if (child.GetComponent<SkinnedMeshRenderer>())
                {
                    var smr = child.GetComponent<SkinnedMeshRenderer>();
                    var mr = child.gameObject.AddComponent<MeshRenderer>();
                    var mf = child.gameObject.AddComponent<MeshFilter>();

                    Mesh bakedMesh = new Mesh();
                    smr.BakeMesh(bakedMesh);
                    mf.mesh = bakedMesh;

                    mr.material = smr.material;
                    Destroy(smr);
                }
            }
        }
    }


    private void Start()
    {
        if (PlayerPrefs.GetString("Time") == "Day")
        {
            RenderSettings.skybox = daySky;
            RenderSettings.ambientLight = dayColor;
        }
        else
        {
            RenderSettings.skybox = nightSky;
            RenderSettings.ambientLight = nightColor;
        }
    }
}
