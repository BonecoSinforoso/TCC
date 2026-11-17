using UnityEngine;

// Pista do Infinity usada SO no treino do ML-Agents.
// Copia a regra de spawn do Script_InfinityManager.ObjetosSpawnar: uma fileira a cada 10m, com 2 das 3 faixas
// bloqueadas pelo mesmo tipo de objeto (carro ou onibus), e os objetos andando para tras a 1 m/s (Script_Carro).
// No lugar de Instantiate/Destroy, os objetos sao criados uma vez e so reposicionados a cada episodio.
public class InfinityTrainingArena : MonoBehaviour
{
    [SerializeField] private float fileiraPrimeiraZ = 15f;
    [SerializeField] private float fileiraDist = 10f; //_posCoef do Script_InfinityManager
    [SerializeField] private int fileirasQuant = 110; //cobre os 1000m mesmo com os objetos andando para tras
    [SerializeField] private float faixaDist = 2.5f;
    [SerializeField] private float objetoSpeed = 1f; //Vector3.back do Script_Carro
    [Range(0, 1)][SerializeField] private float onibusChance = 0.5f;

    private const int layerObstaculo = 3;

    //medidas dos colisores dos prefabs prefab_obj_carro e prefab_obj_onibus
    private static readonly Vector3 carroTamanho = new Vector3(1, 0.7f, 1);
    private static readonly Vector3 onibusTamanho = new Vector3(1, 1.1f, 4);

    private Transform t_objetos;
    private BoxCollider[] objeto; //2 por fileira
    private Transform[] objetoMesh;

    private void Awake()
    {
        t_objetos = new GameObject("emp_objetos").transform;
        t_objetos.SetParent(transform, false);

        objeto = new BoxCollider[fileirasQuant * 2];
        objetoMesh = new Transform[fileirasQuant * 2];

        for (int i = 0; i < objeto.Length; i++)
        {
            GameObject _obj = new GameObject("obj_objeto");
            _obj.transform.SetParent(t_objetos, false);
            _obj.AddComponent<Rigidbody>().isKinematic = true;
            objeto[i] = _obj.AddComponent<BoxCollider>();

            GameObject _mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(_mesh.GetComponent<Collider>());
            _mesh.transform.SetParent(_obj.transform, false);
            objetoMesh[i] = _mesh.transform;
        }
    }

    //chamado pelo agente no inicio de cada episodio
    public void Reiniciar()
    {
        t_objetos.localPosition = Vector3.zero;

        for (int f = 0; f < fileirasQuant; f++)
        {
            int _faixaLivre = Random.Range(0, 3);
            bool _onibus = Random.value < onibusChance;
            int _indice = f * 2;

            for (int k = 0; k < 3; k++)
            {
                if (k == _faixaLivre) continue;

                Objeto_Set(_indice, _onibus, new Vector3((k - 1) * faixaDist, 0, fileiraPrimeiraZ + fileiraDist * f));
                _indice++;
            }
        }
    }

    private void Objeto_Set(int _indice, bool _onibus, Vector3 _pos)
    {
        Vector3 _tamanho = _onibus ? onibusTamanho : carroTamanho;
        GameObject _obj = objeto[_indice].gameObject;

        _obj.tag = _onibus ? "Onibus" : "Carro";
        _obj.layer = _onibus ? layerObstaculo : 0;
        _obj.transform.localPosition = _pos;

        objeto[_indice].size = _tamanho;
        objeto[_indice].center = Vector3.up * (_tamanho.y / 2);

        objetoMesh[_indice].localScale = _tamanho;
        objetoMesh[_indice].localPosition = Vector3.up * (_tamanho.y / 2);
    }

    private void FixedUpdate()
    {
        t_objetos.localPosition += objetoSpeed * Time.fixedDeltaTime * Vector3.back;
    }
}
