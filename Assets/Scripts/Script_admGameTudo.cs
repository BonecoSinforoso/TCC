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
    [SerializeField] int spawnQuant_edit;
    [Space]
    [SerializeField] bool spawnPos_edit;
    [SerializeField] int spawnPos;
    [SerializeField] bool objTipo_edit;
    [SerializeField] int objTipo;

    bool[] perdeu = new bool[3];

    void Start()
    {
        Application.targetFrameRate = 60;

        TextoPontosChange(0, 0);
        TextoPontosChange(1, 0);
        TextoPontosChange(2, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        for (int i = 0; i < 3; i++)
        {
            t_camera[i].position = Vector3.Lerp(t_camera[i].position, new Vector3(t_camera[i].position.x, 3, obj_jogador[i].transform.position.z - distanciaZ), 1f);
            txt_distancia[i].text = obj_jogador[i].transform.position.z.ToString("f2") + "m";
        }

        ChaoPosChange();
    }

    void ChaoPosChange() //isso vai dar merda se a diferença de posicao for grande -> usar eskema do mario kart... deixo para vc icaro do futuro
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

        if (_spawnar) ObjetosSpawnar(posUltima, spawnQuant_edit <= 0 ? 1 : spawnQuant_edit);
    }

    void ObjetosSpawnar(float _pos, int _quant)
    {
        float _posIni = 50;
        float _posCoef = 10;        

        for (int j = 0; j < _quant; j++)
        {
            int _pos_rand = Random.Range(0, 3);
            int _obj_rand = Random.Range(0, prefab_obj_objeto.Length);

            if (spawnPos_edit) _pos_rand = spawnPos;
            if (objTipo_edit) _obj_rand = objTipo;

            for (int i = 0; i < 3; i++)
            {
                if (perdeu[i]) continue;

                GameObject _obj = Instantiate(prefab_obj_objeto[_obj_rand], new Vector3(20 * i + ((2.5f * _pos_rand) - 2.5f), 0.5f, _pos - _posIni + (_posCoef * j)), Quaternion.identity);

                if (_obj_rand == 1 || _obj_rand == 2) _obj.GetComponent<Script_carroTudo>().ObjSet(obj_jogador[i]);
            }
        }        
    }

    public void TextoPontosChange(int _index, int _pontos)
    {
        txt_pontos[_index].text = _pontos.ToString();
    }

    public void PerdeuSet(int _index)
    {
        perdeu[_index] = true;
    }
}