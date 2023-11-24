using UnityEngine;

public class Script_admFlapGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;

    [SerializeField] GameObject obj_playerClone;
    [SerializeField] GameObject obj_iaClone;

    public bool movel;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        obj_playerClone.transform.SetPositionAndRotation(obj_player.transform.position, obj_player.transform.rotation);

        obj_iaClone.transform.SetPositionAndRotation(obj_ia.transform.position, obj_ia.transform.rotation);
    }
}