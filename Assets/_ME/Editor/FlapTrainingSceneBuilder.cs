using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCC
{
    // Monta a cena de treino do Flap (varias arenas) pelo menu TCC > ML-Agents.
    // Os canos e o ponto sao os mesmos prefabs do jogo (mesmos colisores); o passaro tem as medidas do prefab_obj_iaFlap.
    public static class FlapTrainingSceneBuilder
    {
        #region membros

        private const string _scenePath = "Assets/_ME/Scenes/Training/FlapTraining.unity";
        private const string _pipePrefabPath = "Assets/_ME/Prefabs/Flap/prefab_obj_cano.prefab";
        private const string _pointPrefabPath = "Assets/_ME/Prefabs/Flap/prefab_obj_ponto.prefab";
        private const string behaviorName = "Flap";

        private const int _columns = 4;
        private const int _rows = 4;
        private const float _spacingX = 40f;
        private const float _spacingY = 30f;

        #endregion

        #region metodos

        [MenuItem("TCC/ML-Agents/Criar cena de treino - Flap")]
        private static void CreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            GameObject pipePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_pipePrefabPath);
            GameObject pointPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_pointPrefabPath);

            if (pipePrefab == null || pointPrefab == null)
            {
                Debug.LogError("Prefabs do Flap nn encontrados: " + _pipePrefabPath + " / " + _pointPrefabPath);
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            float width = (_columns - 1) * _spacingX;
            float height = (_rows - 1) * _spacingY;

            GameObject camera = new GameObject("Main Camera") { tag = "MainCamera" };
            Camera cam = camera.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = (height + _spacingY) / 2;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
            camera.AddComponent<AudioListener>();
            camera.transform.position = new Vector3(width / 2 + 6, height / 2, -20);

            GameObject light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(30, 20, 0);

            for (int l = 0; l < _rows; l++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    CreateArena("arena_" + (l * _columns + c).ToString("00"), new Vector3(c * _spacingX, l * _spacingY, 0), pipePrefab, pointPrefab);
                }
            }

            EditorSceneManager.SaveScene(scene, _scenePath);
            Debug.Log("Cena de treino do Flap criada em " + _scenePath);
        }

        private static void CreateArena(string name, Vector3 pos, GameObject pipePrefab, GameObject pointPrefab)
        {
            GameObject arena = new GameObject(name);
            arena.transform.position = pos;

            //chao (mesma altura do obj_chao do jogo)
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "obj_chao";
            ground.tag = "Chao";
            ground.transform.SetParent(arena.transform, false);
            ground.transform.localPosition = new Vector3(6, -5.5f, 0);
            ground.transform.localScale = new Vector3(30, 1, 10);

            //canos
            FlapTrainingPipes[] pipes = new FlapTrainingPipes[3];
            Transform[] pipeTransforms = new Transform[3];

            for (int i = 0; i < 3; i++)
            {
                GameObject pipesObj = new GameObject("obj_canos_" + i) { tag = "Cano" };
                pipesObj.transform.SetParent(arena.transform, false);
                pipesObj.transform.localPosition = new Vector3(6 * (i + 1), 0, 0);
                pipesObj.AddComponent<Rigidbody>().isKinematic = true;

                CreatePipe(pipePrefab, pipesObj.transform, 1.5f, 180);
                CreatePipe(pipePrefab, pipesObj.transform, -1.5f, 0);

                GameObject point = (GameObject)PrefabUtility.InstantiatePrefab(pointPrefab, pipesObj.transform);
                point.transform.localPosition = Vector3.zero;

                pipes[i] = pipesObj.AddComponent<FlapTrainingPipes>();
                pipeTransforms[i] = pipesObj.transform;
            }

            for (int i = 0; i < 3; i++) pipes[i].Configure(pipes[(i + 2) % 3]); //o "outro" eh o cano que vem logo atras

            FlapTrainingArena arenaScript = arena.AddComponent<FlapTrainingArena>();
            arenaScript.Configure(pipes);

            //passaro
            GameObject bird = new GameObject("obj_agentFlap");
            bird.transform.SetParent(arena.transform, false);
            bird.AddComponent<SphereCollider>().radius = 0.5f;
            Rigidbody rb = bird.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mesh.name = "mesh";
            Object.DestroyImmediate(mesh.GetComponent<Collider>());
            mesh.transform.SetParent(bird.transform, false);

            AgentFlap agent = bird.AddComponent<AgentFlap>(); //ja adiciona o Behavior Parameters
            agent.MaxStep = 15000; //5 minutos de voo (100 canos), mais que a pontuacao para vencer
            agent.Configure(arenaScript, pipeTransforms);

            BehaviorParameters behavior = bird.GetComponent<BehaviorParameters>();
            behavior.BehaviorName = behaviorName;
            behavior.BrainParameters.VectorObservationSize = AgentFlap.ObservationCount;
            behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentFlap.ActionCount);

            DecisionRequester decision = bird.AddComponent<DecisionRequester>();
            decision.DecisionPeriod = 3;
            decision.TakeActionsBetweenDecisions = false;
        }

        private static void CreatePipe(GameObject pipePrefab, Transform parent, float y, float rotZ)
        {
            GameObject pipe = (GameObject)PrefabUtility.InstantiatePrefab(pipePrefab, parent);
            pipe.transform.localPosition = new Vector3(0, y, 0);
            pipe.transform.localRotation = Quaternion.Euler(0, 0, rotZ);

            //no prefab o mesh fica desligado (o visual do jogo vem de outro objeto); aqui liga para enxergar o treino
            Transform mesh = pipe.transform.Find("mesh");
            if (mesh != null) mesh.gameObject.SetActive(true);
        }

        #endregion
    }
}
