using UnityEngine;

public class Script_FlapManager : MonoBehaviour
{
    public static Script_FlapManager instance;

    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;

    [SerializeField] GameObject obj_playerClone;
    [SerializeField] GameObject obj_iaClone;

    public bool movel;
    public int pontuacaoParaVencer;
    [SerializeField] AudioClip[] ac;

    AudioSource audioSource;

    void Start()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
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

    public void AS_Play(int _index)
    {
        audioSource.PlayOneShot(ac[_index]);
    }
}