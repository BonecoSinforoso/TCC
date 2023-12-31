using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script_admFlapGameTudo : MonoBehaviour
{
    [SerializeField] GameObject obj_player;
    [SerializeField] GameObject obj_ia;

    [SerializeField] GameObject obj_playerClone;
    [SerializeField] GameObject obj_iaClone;

    public bool movel;

    [Header("FB")]
    [SerializeField] GameObject obj_fb;
    [SerializeField] TextMeshProUGUI txt_fb;
    [SerializeField] Color[] color_fb;

    void Start()
    {
        Application.targetFrameRate = 60;

        Time.timeScale = 1;
    }

    void Update()
    {
        obj_playerClone.transform.SetPositionAndRotation(obj_player.transform.position, obj_player.transform.rotation);

        obj_iaClone.transform.SetPositionAndRotation(obj_ia.transform.position, obj_ia.transform.rotation);
    }

    public void FbSet(int _valor)
    {
        obj_fb.SetActive(true);
        obj_fb.GetComponent<Image>().color = color_fb[_valor];

        Time.timeScale = 0;

        txt_fb.text = _valor == 0 ? "PERDEU!" : "GANHOU!";
    }
}