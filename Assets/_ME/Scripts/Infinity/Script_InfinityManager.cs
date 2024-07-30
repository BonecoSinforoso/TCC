using UnityEngine;
using UnityEngine.UI;

public class Script_InfinityManager : MonoBehaviour
{
    public static Script_InfinityManager instance;

    [SerializeField] Text[] txt_distancia;
    [SerializeField] float distanciaGanhar;

    [SerializeField] GameObject[] obj_jogador;

    [SerializeField] GameObject[] obj_chao_01;
    [SerializeField] GameObject[] obj_chao_02;

    [SerializeField] Transform[] t_camera;
    [SerializeField] float distanciaZ;

    [SerializeField] GameObject[] emp_pai;
    [SerializeField] GameObject[] prefab_obj_objeto;
    [SerializeField] float posUltima;

    [Header("Outros")]
    [SerializeField] GameObject prefab_obj_oculos;

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
        instance = this;
    }

    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            t_camera[i].position = Vector3.Lerp(t_camera[i].position, new Vector3(t_camera[i].position.x, 3, obj_jogador[i].transform.position.z - distanciaZ), 1f);
            txt_distancia[i].text = obj_jogador[i].transform.position.z.ToString("f0") + "m";
        }

        if (obj_jogador[0].transform.position.z >= distanciaGanhar) Call_FbSet(1);

        ChaoPos_Change();
    }

    void ChaoPos_Change() //isso vai dar merda se a diferença de posicao for grande -> usar eskema do mario kart... deixo para vc icaro do futuro
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
        float _posIni = 60;
        float _posCoef = 10;

        for (int j = 0; j < _quant; j++)
        {
            int _pos_rand = Random.Range(0, 3);
            int _obj_rand = Random.Range(0, prefab_obj_objeto.Length);

            if (spawnPos_edit) _pos_rand = spawnPos;
            if (objTipo_edit) _obj_rand = objTipo;

            for (int i = 0; i < 2; i++)
            {
                if (perdeu[i]) continue;

                for (int k = 0; k < 3; k++)
                {
                    if (k != _pos_rand)
                    {
                        //GameObject _obj = Instantiate(prefab_obj_objeto[_obj_rand], new Vector3(20 * i + ((2.5f * _pos_rand) - 2.5f), 0.5f, _pos - _posIni + (_posCoef * j)), Quaternion.identity);
                        GameObject _obj = Instantiate(prefab_obj_objeto[_obj_rand], new Vector3(20 * i + ((2.5f * k) - 2.5f), 0.5f, _pos - _posIni + (_posCoef * j)), Quaternion.identity);
                        _obj.transform.parent = emp_pai[_obj_rand].transform;

                        if (_obj_rand == 1 || _obj_rand == 2) _obj.GetComponent<Script_carroTudo>().ObjSet(obj_jogador[i]);
                    }
                }
            }
        }

        //Instantiate(prefab_obj_oculos, new Vector3(0f, 1f, _pos - _posIni + (_posCoef * 9)), Quaternion.identity);
        //Instantiate(prefab_obj_oculos, new Vector3(20f, 1f, _pos - _posIni + (_posCoef * 9)), Quaternion.identity);
    }

    public void Perdeu_Set(int _index)
    {
        if (_index == 0) Call_FbSet(0);

        perdeu[_index] = true;
    }

    public void Call_FbSet(int _valor)
    {
        Script_GeralManager.instance.FbSet(_valor);
    }

    public void PlayerCegar()
    {
        t_camera[0].GetComponent<Camera>().farClipPlane = 12;        

        Invoke(nameof(PlayerEnxegar), 5f);
    }

    void PlayerEnxegar()
    {
        t_camera[0].GetComponent<Camera>().farClipPlane = 50;
    }

    public void IaCegar()
    {
        t_camera[1].GetComponent<Camera>().farClipPlane = 12;

        obj_jogador[1].GetComponent<Script_IaInfinity>().RaioFrenteTamanho_Set(4f);

        Invoke(nameof(IaEnxergar), 5f);
    }

    void IaEnxergar()
    {
        t_camera[1].GetComponent<Camera>().farClipPlane = 50;

        obj_jogador[1].GetComponent<Script_IaInfinity>().RaioFrenteTamanho_Set(5f); //sete para o tamanho inicial, deixe de preguica
    }
}