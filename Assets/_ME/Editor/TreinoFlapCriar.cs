using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Monta a cena de treino do Flap (varias arenas) pelo menu TCC > ML-Agents.
// Os canos e o ponto sao os mesmos prefabs do jogo (mesmos colisores); o passaro tem as medidas do prefab_obj_iaFlap.
public static class TreinoFlapCriar
{
    const string cenaPath = "Assets/_ME/Scenes/Treino/Scene_treinoFlap.unity";
    const string canoPrefabPath = "Assets/_ME/Prefabs/Flap/prefab_obj_cano.prefab";
    const string pontoPrefabPath = "Assets/_ME/Prefabs/Flap/prefab_obj_ponto.prefab";
    const string behaviorName = "Flap";

    const int colunas = 4;
    const int linhas = 4;
    const float espacoX = 40f;
    const float espacoY = 30f;

    [MenuItem("TCC/ML-Agents/Criar cena de treino - Flap")]
    static void Cena_Criar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        GameObject _canoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(canoPrefabPath);
        GameObject _pontoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pontoPrefabPath);

        if (_canoPrefab == null || _pontoPrefab == null)
        {
            Debug.LogError("Prefabs do Flap nn encontrados: " + canoPrefabPath + " / " + pontoPrefabPath);
            return;
        }

        Scene _cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        float _largura = (colunas - 1) * espacoX;
        float _altura = (linhas - 1) * espacoY;

        GameObject _camera = new GameObject("Main Camera") { tag = "MainCamera" };
        Camera _cam = _camera.AddComponent<Camera>();
        _cam.orthographic = true;
        _cam.orthographicSize = (_altura + espacoY) / 2;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
        _camera.AddComponent<AudioListener>();
        _camera.transform.position = new Vector3(_largura / 2 + 6, _altura / 2, -20);

        GameObject _luz = new GameObject("Directional Light");
        _luz.AddComponent<Light>().type = LightType.Directional;
        _luz.transform.rotation = Quaternion.Euler(30, 20, 0);

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                Arena_Criar("arena_" + (l * colunas + c).ToString("00"), new Vector3(c * espacoX, l * espacoY, 0), _canoPrefab, _pontoPrefab);
            }
        }

        EditorSceneManager.SaveScene(_cena, cenaPath);
        Debug.Log("Cena de treino do Flap criada em " + cenaPath);
    }

    static void Arena_Criar(string _nome, Vector3 _pos, GameObject _canoPrefab, GameObject _pontoPrefab)
    {
        GameObject _arena = new GameObject(_nome);
        _arena.transform.position = _pos;

        //chao (mesma altura do obj_chao do jogo)
        GameObject _chao = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _chao.name = "obj_chao";
        _chao.tag = "Chao";
        _chao.transform.SetParent(_arena.transform, false);
        _chao.transform.localPosition = new Vector3(6, -5.5f, 0);
        _chao.transform.localScale = new Vector3(30, 1, 10);

        //canos
        CanosFlapTreino[] _canos = new CanosFlapTreino[3];
        Transform[] _t_cano = new Transform[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject _canosObj = new GameObject("obj_canos_" + i) { tag = "Cano" };
            _canosObj.transform.SetParent(_arena.transform, false);
            _canosObj.transform.localPosition = new Vector3(6 * (i + 1), 0, 0);
            _canosObj.AddComponent<Rigidbody>().isKinematic = true;

            Cano_Criar(_canoPrefab, _canosObj.transform, 1.5f, 180);
            Cano_Criar(_canoPrefab, _canosObj.transform, -1.5f, 0);

            GameObject _ponto = (GameObject)PrefabUtility.InstantiatePrefab(_pontoPrefab, _canosObj.transform);
            _ponto.transform.localPosition = Vector3.zero;

            _canos[i] = _canosObj.AddComponent<CanosFlapTreino>();
            _t_cano[i] = _canosObj.transform;
        }

        for (int i = 0; i < 3; i++) _canos[i].Configurar(_canos[(i + 2) % 3]); //o "outro" eh o cano que vem logo atras

        ArenaFlapTreino _arenaScript = _arena.AddComponent<ArenaFlapTreino>();
        _arenaScript.Configurar(_canos);

        //passaro
        GameObject _passaro = new GameObject("obj_agentFlap");
        _passaro.transform.SetParent(_arena.transform, false);
        _passaro.AddComponent<SphereCollider>().radius = 0.5f;
        Rigidbody _rb = _passaro.AddComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        GameObject _mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _mesh.name = "mesh";
        Object.DestroyImmediate(_mesh.GetComponent<Collider>());
        _mesh.transform.SetParent(_passaro.transform, false);

        AgentFlap _agente = _passaro.AddComponent<AgentFlap>(); //ja adiciona o Behavior Parameters
        _agente.MaxStep = 15000; //5 minutos de voo (100 canos), mais que a pontuacao para vencer
        _agente.Configurar(_arenaScript, _t_cano);

        BehaviorParameters _behavior = _passaro.GetComponent<BehaviorParameters>();
        _behavior.BehaviorName = behaviorName;
        _behavior.BrainParameters.VectorObservationSize = AgentFlap.observacoesQuant;
        _behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentFlap.acoesQuant);

        DecisionRequester _decisao = _passaro.AddComponent<DecisionRequester>();
        _decisao.DecisionPeriod = 3;
        _decisao.TakeActionsBetweenDecisions = false;
    }

    static void Cano_Criar(GameObject _canoPrefab, Transform _pai, float _y, float _rotZ)
    {
        GameObject _cano = (GameObject)PrefabUtility.InstantiatePrefab(_canoPrefab, _pai);
        _cano.transform.localPosition = new Vector3(0, _y, 0);
        _cano.transform.localRotation = Quaternion.Euler(0, 0, _rotZ);

        //no prefab o mesh fica desligado (o visual do jogo vem de outro objeto); aqui liga para enxergar o treino
        Transform _mesh = _cano.transform.Find("mesh");
        if (_mesh != null) _mesh.gameObject.SetActive(true);
    }
}
