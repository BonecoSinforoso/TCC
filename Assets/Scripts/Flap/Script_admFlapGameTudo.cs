using UnityEngine;

public class Script_admFlapGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;

    [SerializeField] GameObject obj_playerClone;
    [SerializeField] GameObject obj_iaClone;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        obj_playerClone.transform.position = obj_player.transform.position;
        obj_playerClone.transform.rotation = obj_player.transform.rotation;

        obj_iaClone.transform.position = obj_ia.transform.position;
        obj_iaClone.transform.rotation = obj_ia.transform.rotation;
    }
}