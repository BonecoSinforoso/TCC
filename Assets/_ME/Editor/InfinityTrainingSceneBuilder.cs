using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCC
{
    // Monta a cena de treino do Infinity (varias pistas lado a lado) pelo menu TCC > ML-Agents.
    public static class InfinityTrainingSceneBuilder
    {
        #region membros

        private const string _scenePath = "Assets/_ME/Scenes/Training/InfinityTraining.unity";
        private const string behaviorName = "Infinity";

        private const int _trackCount = 8;
        private const float _spacingX = 20f; //mesma distancia entre as pistas do jogo
        private const float _trackLength = 1300f;
        private const float _laneDistance = 2.5f;
        private const float _rayLength = 20f;

        #endregion

        #region metodos

        [MenuItem("TCC/ML-Agents/Criar cena de treino - Infinity")]
        private static void CreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);

            Transform firstAgent = null;

            for (int i = 0; i < _trackCount; i++)
            {
                Transform agent = CreateTrack("pista_" + i.ToString("00"), new Vector3(i * _spacingX, 0, 0));
                if (i == 0) firstAgent = agent;
            }

            //camera presa no primeiro corredor, so para acompanhar o treino
            GameObject camera = new GameObject("Main Camera") { tag = "MainCamera" };
            camera.AddComponent<Camera>().farClipPlane = 150;
            camera.AddComponent<AudioListener>();
            camera.transform.SetParent(firstAgent, false);
            camera.transform.localPosition = new Vector3(0, 6, -8);
            camera.transform.localRotation = Quaternion.Euler(25, 0, 0);

            EditorSceneManager.SaveScene(scene, _scenePath);
            Debug.Log("Cena de treino do Infinity criada em " + _scenePath);
        }

        private static Transform CreateTrack(string name, Vector3 pos)
        {
            GameObject track = new GameObject(name);
            track.transform.position = pos;

            InfinityTrainingArena arena = track.AddComponent<InfinityTrainingArena>();

            //chao: uma pista comprida so, com a face de cima em y = 0 (no jogo sao dois planos que vao se revezando)
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "obj_chao";
            ground.tag = "Chao";
            ground.transform.SetParent(track.transform, false);
            ground.transform.localPosition = new Vector3(0, -0.5f, _trackLength / 2 - 50);
            ground.transform.localScale = new Vector3(10, 1, _trackLength);

            //corredor
            GameObject agentObj = new GameObject("obj_agentInfinity");
            agentObj.transform.SetParent(track.transform, false);
            agentObj.transform.localPosition = new Vector3(0, 0.1f, 0);

            BoxCollider col = agentObj.AddComponent<BoxCollider>();
            col.size = new Vector3(0.5f, 1, 0.5f);
            col.center = new Vector3(0, 0.5f, 0);

            agentObj.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.name = "mesh";
            Object.DestroyImmediate(mesh.GetComponent<Collider>());
            mesh.transform.SetParent(agentObj.transform, false);
            mesh.transform.localPosition = new Vector3(0, 0.5f, 0);
            mesh.transform.localScale = new Vector3(0.5f, 1, 0.5f);

            //sensores: um raio para frente em cada faixa
            GameObject sensors = new GameObject("emp_sensores");
            sensors.transform.SetParent(agentObj.transform, false);

            CreateSensor(sensors.transform, "RaioFaixa0Esq", -_laneDistance);
            CreateSensor(sensors.transform, "RaioFaixa1Cen", 0);
            CreateSensor(sensors.transform, "RaioFaixa2Dir", _laneDistance);

            AgentInfinity agent = agentObj.AddComponent<AgentInfinity>(); //ja adiciona o Behavior Parameters
            agent.MaxStep = 0; //o episodio acaba ao bater ou ao chegar nos 1000m
            agent.Configure(arena, sensors.transform);

            BehaviorParameters behavior = agentObj.GetComponent<BehaviorParameters>();
            behavior.BehaviorName = behaviorName;
            behavior.BrainParameters.VectorObservationSize = AgentInfinity.ObservationCount;
            behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentInfinity.LaneCount, 2);
            behavior.UseChildSensors = true;

            DecisionRequester decision = agentObj.AddComponent<DecisionRequester>();
            decision.DecisionPeriod = 5;
            decision.TakeActionsBetweenDecisions = false; //cada decisao vale um toque de tecla

            return agentObj.transform;
        }

        private static void CreateSensor(Transform parent, string name, float x)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = new Vector3(x, 0, 0.5f);

            RayPerceptionSensorComponent3D sensor = obj.AddComponent<RayPerceptionSensorComponent3D>();
            sensor.SensorName = name;
            sensor.DetectableTags = new List<string> { "Carro", "Onibus" };
            sensor.RaysPerDirection = 0; //so o raio do meio (para frente)
            sensor.RayLength = _rayLength;
            sensor.SphereCastRadius = 0.2f;
            sensor.StartVerticalOffset = 0.3f;
            sensor.EndVerticalOffset = 0.3f;
            sensor.RayLayerMask = (1 << 0) | (1 << 3); //Default (carro) e Obstaculo (onibus)
        }

        #endregion
    }
}
