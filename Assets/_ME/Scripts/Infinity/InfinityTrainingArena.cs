using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Pista do Infinity usada so no treino. Copia o spawn do InfinityManager:
    // uma fileira a cada 10m, com 2 das 3 faixas bloqueadas, e os objetos andando para tras.
    public class InfinityTrainingArena : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("fileiraPrimeiraZ")]
        [SerializeField] private float _firstRowZ = 15f;
        [FormerlySerializedAs("fileiraDist")]
        [SerializeField] private float _rowDistance = 10f; //_posCoef do InfinityManager
        [FormerlySerializedAs("fileirasQuant")]
        [SerializeField] private int _rowCount = 110; //cobre os 1000m mesmo com os objetos andando para tras
        [FormerlySerializedAs("faixaDist")]
        [SerializeField] private float _laneDistance = 2.5f;
        [FormerlySerializedAs("objetoSpeed")]
        [SerializeField] private float _objectSpeed = 1f; //Vector3.back do Carro
        [FormerlySerializedAs("onibusChance")]
        [Range(0, 1)][SerializeField] private float _busChance = 0.5f;

        private const int _obstacleLayer = 3;

        //medidas dos colisores dos prefabs prefab_obj_carro e prefab_obj_onibus
        private static readonly Vector3 _carSize = new Vector3(1, 0.7f, 1);
        private static readonly Vector3 _busSize = new Vector3(1, 1.1f, 4);

        private Transform _objectsTransform;
        private BoxCollider[] _objectColliders; //2 por fileira
        private Transform[] _objectMeshTransforms;

        #endregion

        #region metodos

        private void Awake()
        {
            _objectsTransform = new GameObject("emp_objetos").transform;
            _objectsTransform.SetParent(transform, false);

            _objectColliders = new BoxCollider[_rowCount * 2];
            _objectMeshTransforms = new Transform[_rowCount * 2];

            for (int i = 0; i < _objectColliders.Length; i++)
            {
                GameObject obj = new GameObject("obj_objeto");
                obj.transform.SetParent(_objectsTransform, false);
                obj.AddComponent<Rigidbody>().isKinematic = true;
                _objectColliders[i] = obj.AddComponent<BoxCollider>();

                GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(mesh.GetComponent<Collider>());
                mesh.transform.SetParent(obj.transform, false);
                _objectMeshTransforms[i] = mesh.transform;
            }
        }

        //chamado pelo agente no inicio de cada episodio
        public void Restart()
        {
            _objectsTransform.localPosition = Vector3.zero;

            for (int f = 0; f < _rowCount; f++)
            {
                int freeLane = Random.Range(0, 3);
                bool isBus = Random.value < _busChance;
                int index = f * 2;

                for (int k = 0; k < 3; k++)
                {
                    if (k == freeLane) continue;

                    SetObject(index, isBus, new Vector3((k - 1) * _laneDistance, 0, _firstRowZ + _rowDistance * f));
                    index++;
                }
            }
        }

        private void SetObject(int index, bool isBus, Vector3 pos)
        {
            Vector3 size = isBus ? _busSize : _carSize;
            GameObject obj = _objectColliders[index].gameObject;

            obj.tag = isBus ? "Onibus" : "Carro";
            obj.layer = isBus ? _obstacleLayer : 0;
            obj.transform.localPosition = pos;

            _objectColliders[index].size = size;
            _objectColliders[index].center = Vector3.up * (size.y / 2);

            _objectMeshTransforms[index].localScale = size;
            _objectMeshTransforms[index].localPosition = Vector3.up * (size.y / 2);
        }

        private void FixedUpdate()
        {
            _objectsTransform.localPosition += _objectSpeed * Time.fixedDeltaTime * Vector3.back;
        }

        #endregion
    }
}
