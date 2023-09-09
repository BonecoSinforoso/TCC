using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_admGameTudo : MonoBehaviour
{
    [SerializeField] Text[] txt_distancia;
    [SerializeField] Text[] txt_pontos;

    [SerializeField] GameObject[] obj_jogador;

    [SerializeField] GameObject[] obj_chao_01;
    [SerializeField] GameObject[] obj_chao_02;

    [SerializeField] Transform[] t_camera;
    [SerializeField] float distanciaZ;

    [SerializeField] GameObject[] prefab_obj_objeto;
    [SerializeField] float posUltima;

    //[Space(100f)]
    [Header("Testes")]
    [SerializeField] bool spawnPos_edit;
    [SerializeField] int spawnPos;
    [SerializeField] bool objTipo_edit;
    [SerializeField] int objTipo;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        for (int i = 0; i < 3; i++)
        {
            t_camera[i].position = Vector3.Lerp(t_camera[i].position, new Vector3(t_camera[i].position.x, 3, obj_jogador[i].transform.position.z - distanciaZ), 1f);
            txt_distancia[i].text = obj_jogador[0].transform.position.z.ToString("f2") + "m";
        }

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

                if (obj_chao_01[i].transform.position.z > posUltima)
                {
                    _spawnar = true;
                    posUltima = obj_chao_01[i].transform.position.z;
                }
            }
            if (obj_jogador[i].transform.position.z - obj_chao_02[i].transform.position.z > 120)
            {
                obj_chao_02[i].transform.position += Vector3.forward * 200;

                if (obj_chao_02[i].transform.position.z > posUltima)
                {
                    _spawnar = true;
                    posUltima = obj_chao_02[i].transform.position.z;
                }
            }
        }

        if (_spawnar) ObjetosSpawnar(posUltima);
    }

    void ObjetosSpawnar(float _pos)
    {
        int _pos_rand = Random.Range(0, 3);
        int _obj_rand = Random.Range(0, prefab_obj_objeto.Length);

        if (spawnPos_edit) _pos_rand = spawnPos;
        if (objTipo_edit) _obj_rand = objTipo;

        for (int i = 0; i < 3; i++)
        {
            Instantiate(prefab_obj_objeto[_obj_rand], new Vector3(20 * i + ((2.5f * _pos_rand) - 2.5f), 0.5f, _pos), Quaternion.identity);
        }
    }

    public void TextoPontosChange(int _index, int _pontos)
    {
        txt_pontos[_index].text = _pontos.ToString();
    }
}