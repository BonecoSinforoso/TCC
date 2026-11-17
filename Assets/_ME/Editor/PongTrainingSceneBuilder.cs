using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Monta a cena de treino do Pong (varias mesas lado a lado) pelo menu TCC > ML-Agents.
// As medidas sao as mesmas da Scene_gamePong: raquetes em x = +-9, paredes em z = +-6, bola com raio 0.25.
public static class PongTrainingSceneBuilder
{
    private const string cenaPath = "Assets/_ME/Scenes/Training/PongTraining.unity";
    private const string behaviorName = "Pong";

    private const int colunas = 4;
    private const int linhas = 3;
    private const float espacoX = 32f;
    private const float espacoZ = 14f;

    [MenuItem("TCC/ML-Agents/Criar cena de treino - Pong")]
    private static void Cena_Criar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene _cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        float _largura = (colunas - 1) * espacoX;
        float _altura = (linhas - 1) * espacoZ;

        GameObject _camera = new GameObject("Main Camera") { tag = "MainCamera" };
        Camera _cam = _camera.AddComponent<Camera>();
        _cam.orthographic = true;
        _cam.orthographicSize = (_altura + espacoZ) / 2;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
        _camera.AddComponent<AudioListener>();
        _camera.transform.SetPositionAndRotation(new Vector3(_largura / 2, 20, _altura / 2), Quaternion.Euler(90, 0, 0));

        GameObject _luz = new GameObject("Directional Light");
        _luz.AddComponent<Light>().type = LightType.Directional;
        _luz.transform.rotation = Quaternion.Euler(60, 30, 0);

        for (int l = 0; l < linhas; l++)
        {
            for (int c = 0; c < colunas; c++)
            {
                Arena_Criar("arena_" + (l * colunas + c).ToString("00"), new Vector3(c * espacoX, 0, l * espacoZ));
            }
        }

        EditorSceneManager.SaveScene(_cena, cenaPath);
        Debug.Log("Cena de treino do Pong criada em " + cenaPath);
    }

    private static void Arena_Criar(string _nome, Vector3 _pos)
    {
        GameObject _arena = new GameObject(_nome);
        _arena.transform.position = _pos;

        Parede_Criar(_arena.transform, 6);
        Parede_Criar(_arena.transform, -6);

        //bola
        GameObject _bola = new GameObject("obj_bola");
        _bola.transform.SetParent(_arena.transform, false);
        SphereCollider _bolaCol = _bola.AddComponent<SphereCollider>();
        _bolaCol.isTrigger = true;
        _bolaCol.radius = 0.25f;
        Rigidbody _rb_bola = _bola.AddComponent<Rigidbody>();
        _rb_bola.useGravity = false;
        _rb_bola.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        Mesh_Criar(_bola.transform, PrimitiveType.Sphere, Vector3.one * 0.5f);
        _bola.AddComponent<PongTrainingBall>().Configurar(_arena.transform);

        //agente (esquerda, no lugar do jogador) e adversario reativo (direita, igual ao jogo)
        GameObject _agenteObj = Raquete_Criar(_arena.transform, "obj_agentPong", "Player", -9);
        GameObject _oponenteObj = Raquete_Criar(_arena.transform, "obj_oponentePong", "IA", 9);

        PongTrainingOpponent _oponente = _oponenteObj.AddComponent<PongTrainingOpponent>();
        _oponente.Configurar(_arena.transform, _rb_bola);

        PongTrainingArena _arenaScript = _arena.AddComponent<PongTrainingArena>();

        AgentPong _agente = _agenteObj.AddComponent<AgentPong>(); //ja adiciona o Behavior Parameters
        _agente.MaxStep = 3000; //60s de partida (0.02s por step), igual ao fbGanharTempo
        _agente.Configurar(_arenaScript, _bola.transform, _rb_bola, null);

        BehaviorParameters _behavior = _agenteObj.GetComponent<BehaviorParameters>();
        _behavior.BehaviorName = behaviorName;
        _behavior.BrainParameters.VectorObservationSize = AgentPong.observacoesQuant;
        _behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentPong.acoesQuant);

        DecisionRequester _decisao = _agenteObj.AddComponent<DecisionRequester>();
        _decisao.DecisionPeriod = 2; //o reativo decide todo frame; decidir mais vezes gasta menos energia a toa
        _decisao.TakeActionsBetweenDecisions = true;

        _arenaScript.Configurar(_rb_bola, _agente, _oponente);
    }

    private static void Parede_Criar(Transform _pai, float _z)
    {
        GameObject _parede = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _parede.name = "col_parede";
        _parede.tag = "Parede";
        _parede.transform.SetParent(_pai, false);
        _parede.transform.localPosition = new Vector3(0, 0, _z);
        _parede.transform.localScale = new Vector3(30, 1, 1);
    }

    private static GameObject Raquete_Criar(Transform _pai, string _nome, string _tag, float _x)
    {
        GameObject _raquete = new GameObject(_nome) { tag = _tag };
        _raquete.transform.SetParent(_pai, false);
        _raquete.transform.localPosition = new Vector3(_x, 0, 0);

        _raquete.AddComponent<BoxCollider>().size = new Vector3(1, 1, 3);

        Rigidbody _rb = _raquete.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        Mesh_Criar(_raquete.transform, PrimitiveType.Cube, new Vector3(1, 0.5f, 3));

        return _raquete;
    }

    private static void Mesh_Criar(Transform _pai, PrimitiveType _tipo, Vector3 _escala)
    {
        GameObject _mesh = GameObject.CreatePrimitive(_tipo);
        _mesh.name = "mesh";
        Object.DestroyImmediate(_mesh.GetComponent<Collider>());
        _mesh.transform.SetParent(_pai, false);
        _mesh.transform.localScale = _escala;
    }
}
