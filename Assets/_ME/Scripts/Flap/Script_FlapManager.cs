using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Script_FlapManager : MonoBehaviour
{
    public static Script_FlapManager instance;

    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;

    [SerializeField] GameObject obj_playerClone;
    [SerializeField] GameObject obj_iaClone;

    public bool movel;
    public int pontuacaoParaVencer;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        obj_playerClone.transform.SetPositionAndRotation(obj_player.transform.position, obj_player.transform.rotation);

        obj_iaClone.transform.SetPositionAndRotation(obj_ia.transform.position, obj_ia.transform.rotation);
    }

    public void Call_FbSet(int _valor)
    {
        Script_GeralManager.instance.FbSet(_valor);
    }
}