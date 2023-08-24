using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_admGameTudo : MonoBehaviour
{
    [SerializeField] Text txt_playerDist;
    [SerializeField] Text txt_iaDist;
    [SerializeField] Text txt_mlDist;

    [SerializeField] Text[] txt_pontos;

    [SerializeField] GameObject[] obj_jogador;

    [SerializeField] GameObject[] obj_chao_01;
    [SerializeField] GameObject[] obj_chao_02;

    [SerializeField] Transform[] t_camera;
    [SerializeField] float distanciaZ;

    [SerializeField] float posUltima;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        for (int i = 0; i < 3; i++)
        {
            t_camera[i].position = Vector3.Lerp(t_camera[i].position, new Vector3(t_camera[0].position.x, 3, obj_jogador[i].transform.position.z - distanciaZ), 1f);
        }

        txt_playerDist.text = obj_jogador[0].transform.position.z.ToString("f2") + "m";
        txt_iaDist.text = obj_jogador[1].transform.position.z.ToString("f2") + "m";
        txt_mlDist.text = obj_jogador[2].transform.position.z.ToString("f2") + "m";

        ChaoPosChange();
    }

    void ChaoPosChange()
    {
        bool _spawnar = false;

        for (int i = 0; i < 3; i++)
        {
            if (obj_jogador[i].transform.position.z - obj_chao_01[i].transform.position.z > 120)
            {
                obj_chao_01[i].transform.position += Vector3.forward * 200;
            }
            if (obj_jogador[i].transform.position.z - obj_chao_02[i].transform.position.z > 120)
            {
                obj_chao_02[i].transform.position += Vector3.forward * 200;
            }
        }
    }

    void ObjetosSpawnar(float _pos)
    {
        for (int i = 0; i < 3; i++)
        {

        }
    }

    public void TextoPontosChange(int _index, int _pontos)
    {
        txt_pontos[_index].text = _pontos.ToString();
    }
}