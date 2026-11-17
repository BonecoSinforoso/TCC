using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Monta a cena de treino do Infinity (varias pistas lado a lado) pelo menu TCC > ML-Agents.
// O corredor tem as medidas do prefab_obj_ia (colisor 0.5 x 1 x 0.5, pulo 6); os obstaculos sao criados
// pelo InfinityTrainingArena ao dar Play.
public static class InfinityTrainingSceneBuilder
{
    private const string cenaPath = "Assets/_ME/Scenes/Training/InfinityTraining.unity";
    private const string behaviorName = "Infinity";

    private const int pistasQuant = 8;
    private const float espacoX = 20f; //mesma distancia entre as pistas do jogo
    private const float pistaTamanho = 1300f;
    private const float faixaDist = 2.5f;
    private const float raioTamanho = 20f;

    [MenuItem("TCC/ML-Agents/Criar cena de treino - Infinity")]
    private static void Cena_Criar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Scene _cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject _luz = new GameObject("Directional Light");
        _luz.AddComponent<Light>().type = LightType.Directional;
        _luz.transform.rotation = Quaternion.Euler(50, -30, 0);

        Transform _primeiroAgente = null;

        for (int i = 0; i < pistasQuant; i++)
        {
            Transform _agente = Pista_Criar("pista_" + i.ToString("00"), new Vector3(i * espacoX, 0, 0));
            if (i == 0) _primeiroAgente = _agente;
        }

        //camera presa no primeiro corredor, so para acompanhar o treino
        GameObject _camera = new GameObject("Main Camera") { tag = "MainCamera" };
        _camera.AddComponent<Camera>().farClipPlane = 150;
        _camera.AddComponent<AudioListener>();
        _camera.transform.SetParent(_primeiroAgente, false);
        _camera.transform.localPosition = new Vector3(0, 6, -8);
        _camera.transform.localRotation = Quaternion.Euler(25, 0, 0);

        EditorSceneManager.SaveScene(_cena, cenaPath);
        Debug.Log("Cena de treino do Infinity criada em " + cenaPath);
    }

    private static Transform Pista_Criar(string _nome, Vector3 _pos)
    {
        GameObject _pista = new GameObject(_nome);
        _pista.transform.position = _pos;

        InfinityTrainingArena _arena = _pista.AddComponent<InfinityTrainingArena>();

        //chao: uma pista comprida so, com a face de cima em y = 0 (no jogo sao dois planos que vao se revezando)
        GameObject _chao = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _chao.name = "obj_chao";
        _chao.tag = "Chao";
        _chao.transform.SetParent(_pista.transform, false);
        _chao.transform.localPosition = new Vector3(0, -0.5f, pistaTamanho / 2 - 50);
        _chao.transform.localScale = new Vector3(10, 1, pistaTamanho);

        //corredor
        GameObject _agenteObj = new GameObject("obj_agentInfinity");
        _agenteObj.transform.SetParent(_pista.transform, false);
        _agenteObj.transform.localPosition = new Vector3(0, 0.1f, 0);

        BoxCollider _col = _agenteObj.AddComponent<BoxCollider>();
        _col.size = new Vector3(0.5f, 1, 0.5f);
        _col.center = new Vector3(0, 0.5f, 0);

        _agenteObj.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        GameObject _mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _mesh.name = "mesh";
        Object.DestroyImmediate(_mesh.GetComponent<Collider>());
        _mesh.transform.SetParent(_agenteObj.transform, false);
        _mesh.transform.localPosition = new Vector3(0, 0.5f, 0);
        _mesh.transform.localScale = new Vector3(0.5f, 1, 0.5f);

        //sensores: um raio para frente em cada faixa
        GameObject _sensores = new GameObject("emp_sensores");
        _sensores.transform.SetParent(_agenteObj.transform, false);

        Sensor_Criar(_sensores.transform, "RaioFaixa0Esq", -faixaDist);
        Sensor_Criar(_sensores.transform, "RaioFaixa1Cen", 0);
        Sensor_Criar(_sensores.transform, "RaioFaixa2Dir", faixaDist);

        AgentInfinity _agente = _agenteObj.AddComponent<AgentInfinity>(); //ja adiciona o Behavior Parameters
        _agente.MaxStep = 0; //o episodio acaba ao bater ou ao chegar nos 1000m
        _agente.Configurar(_arena, _sensores.transform);

        BehaviorParameters _behavior = _agenteObj.GetComponent<BehaviorParameters>();
        _behavior.BehaviorName = behaviorName;
        _behavior.BrainParameters.VectorObservationSize = AgentInfinity.observacoesQuant;
        _behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentInfinity.faixasQuant, 2);
        _behavior.UseChildSensors = true;

        DecisionRequester _decisao = _agenteObj.AddComponent<DecisionRequester>();
        _decisao.DecisionPeriod = 5;
        _decisao.TakeActionsBetweenDecisions = false; //cada decisao vale um toque de tecla

        return _agenteObj.transform;
    }

    private static void Sensor_Criar(Transform _pai, string _nome, float _x)
    {
        GameObject _obj = new GameObject(_nome);
        _obj.transform.SetParent(_pai, false);
        _obj.transform.localPosition = new Vector3(_x, 0, 0.5f);

        RayPerceptionSensorComponent3D _sensor = _obj.AddComponent<RayPerceptionSensorComponent3D>();
        _sensor.SensorName = _nome;
        _sensor.DetectableTags = new List<string> { "Carro", "Onibus" };
        _sensor.RaysPerDirection = 0; //so o raio do meio (para frente)
        _sensor.RayLength = raioTamanho;
        _sensor.SphereCastRadius = 0.2f;
        _sensor.StartVerticalOffset = 0.3f;
        _sensor.EndVerticalOffset = 0.3f;
        _sensor.RayLayerMask = (1 << 0) | (1 << 3); //Default (carro) e Obstaculo (onibus)
    }
}
