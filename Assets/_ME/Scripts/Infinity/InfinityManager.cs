using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TCC
{
    public class InfinityManager : MonoBehaviour
    {
        #region membros

        public static InfinityManager Instance { get; private set; }

        [FormerlySerializedAs("txt_distancia")]
        [SerializeField] private Text[] _distanceTexts;
        [FormerlySerializedAs("distanciaGanhar")]
        [SerializeField] private float _distanceToWin;

        [FormerlySerializedAs("obj_jogador")]
        [SerializeField] private GameObject[] _playerObjects;

        [FormerlySerializedAs("obj_chao_01")]
        [SerializeField] private GameObject[] _ground01Objects;
        [FormerlySerializedAs("obj_chao_02")]
        [SerializeField] private GameObject[] _ground02Objects;

        [FormerlySerializedAs("t_camera")]
        [SerializeField] private Transform[] _cameraTransforms;
        [FormerlySerializedAs("distanciaZ")]
        [SerializeField] private float _distanceZ;

        [FormerlySerializedAs("emp_pai")]
        [SerializeField] private GameObject[] _parentObjects;
        [FormerlySerializedAs("prefab_obj_objeto")]
        [SerializeField] private GameObject[] _objectPrefabs;
        [FormerlySerializedAs("posUltima")]
        [SerializeField] private float _lastSpawnZ;

        [Header("Outros")]

        [Header("Testes")]
        [FormerlySerializedAs("spawnQuant_edit")]
        [SerializeField] private int _spawnCountEdit;
        [Space]
        [FormerlySerializedAs("spawnPos_edit")]
        [SerializeField] private bool _spawnPosEdit;
        [SerializeField] private int _spawnPos;
        [FormerlySerializedAs("objTipo_edit")]
        [SerializeField] private bool _objectTypeEdit;
        [FormerlySerializedAs("objTipo")]
        [SerializeField] private int _objectType;

        private bool[] _lostFlags = new bool[3];

        #endregion

        #region metodos

        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
            for (int i = 0; i < 3; i++)
            {
                _cameraTransforms[i].position = Vector3.Lerp(_cameraTransforms[i].position, new Vector3(_cameraTransforms[i].position.x, 3, _playerObjects[i].transform.position.z - _distanceZ), 1f);
                _distanceTexts[i].text = _playerObjects[i].transform.position.z.ToString("f0") + "m";
            }

            if (_playerObjects[0].transform.position.z >= _distanceToWin) CallShowResult(1);

            ChangeGroundPos();
        }

        private void ChangeGroundPos() //isso vai dar merda se a diferença de posicao for grande -> usar eskema do mario kart... deixo para vc icaro do futuro
        {
            bool spawn = false;

            for (int i = 0; i < 3; i++)
            {
                if (_playerObjects[i].transform.position.z - _ground01Objects[i].transform.position.z > 120)
                {
                    _ground01Objects[i].transform.position += Vector3.forward * 200;

                    if (_ground01Objects[i].transform.position.z > _lastSpawnZ)
                    {
                        spawn = true;
                        _lastSpawnZ = _ground01Objects[i].transform.position.z;
                    }
                }
                if (_playerObjects[i].transform.position.z - _ground02Objects[i].transform.position.z > 120)
                {
                    _ground02Objects[i].transform.position += Vector3.forward * 200;

                    if (_ground02Objects[i].transform.position.z > _lastSpawnZ)
                    {
                        spawn = true;
                        _lastSpawnZ = _ground02Objects[i].transform.position.z;
                    }
                }
            }

            if (spawn) SpawnObjects(_lastSpawnZ, _spawnCountEdit <= 0 ? 1 : _spawnCountEdit);
        }

        private void SpawnObjects(float pos, int count)
        {
            float startOffset = 60;
            float posCoef = 10;

            for (int j = 0; j < count; j++)
            {
                int randomPos = Random.Range(0, 3);
                int randomObject = Random.Range(0, _objectPrefabs.Length);

                if (_spawnPosEdit) randomPos = _spawnPos;
                if (_objectTypeEdit) randomObject = _objectType;

                for (int i = 0; i < 2; i++)
                {
                    if (_lostFlags[i]) continue;

                    for (int k = 0; k < 3; k++)
                    {
                        if (k != randomPos)
                        {
                            //GameObject obj = Instantiate(prefab_obj_objeto[_obj_rand], new Vector3(20 * i + ((2.5f * _pos_rand) - 2.5f), 0.5f, pos - _posIni + (posCoef * j)), Quaternion.identity);
                            GameObject obj = Instantiate(_objectPrefabs[randomObject], new Vector3(20 * i + ((2.5f * k) - 2.5f), 0.5f, pos - startOffset + (posCoef * j)), Quaternion.identity);
                            obj.transform.parent = _parentObjects[randomObject].transform;

                            if (randomObject == 1 || randomObject == 2) obj.GetComponent<Carro>().SetObject(_playerObjects[i]);
                        }
                    }
                }
            }

            //Instantiate(prefab_obj_oculos, new Vector3(0f, 1f, pos - _posIni + (posCoef * 9)), Quaternion.identity);
            //Instantiate(prefab_obj_oculos, new Vector3(20f, 1f, pos - _posIni + (posCoef * 9)), Quaternion.identity);
        }

        public void SetLost(int index)
        {
            if (index == 0) CallShowResult(0);

            _lostFlags[index] = true;
        }

        public void CallShowResult(int value)
        {
            GeralManager.Instance.ShowResult(value);
        }

        public void BlindPlayer()
        {
            _cameraTransforms[0].GetComponent<Camera>().farClipPlane = 12;

            Invoke(nameof(RestorePlayerSight), 5f);
        }

        private void RestorePlayerSight()
        {
            _cameraTransforms[0].GetComponent<Camera>().farClipPlane = 50;
        }

        public void BlindIa()
        {
            _cameraTransforms[1].GetComponent<Camera>().farClipPlane = 12;

            _playerObjects[1].GetComponent<IaInfinity>().SetForwardRayLength(4f);

            Invoke(nameof(RestoreIaSight), 5f);
        }

        private void RestoreIaSight()
        {
            _cameraTransforms[1].GetComponent<Camera>().farClipPlane = 50;

            _playerObjects[1].GetComponent<IaInfinity>().SetForwardRayLength(5f); //sete para o tamanho inicial, deixe de preguica
        }

        #endregion
    }
}
