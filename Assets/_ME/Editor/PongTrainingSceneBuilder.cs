using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCC
{
    // Monta a cena de treino do Pong (varias mesas lado a lado) pelo menu TCC > ML-Agents.
    // As medidas sao as mesmas da Scene_PongGame: raquetes em x = +-9, paredes em z = +-6, bola com raio 0.25.
    public static class PongTrainingSceneBuilder
    {
        #region membros

        private const string _scenePath = "Assets/_ME/Scenes/Training/PongTraining.unity";
        private const string behaviorName = "Pong";

        private const int _columns = 4;
        private const int _rows = 3;
        private const float _spacingX = 32f;
        private const float _spacingZ = 14f;

        #endregion

        #region metodos

        [MenuItem("TCC/ML-Agents/Criar cena de treino - Pong")]
        private static void CreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            float width = (_columns - 1) * _spacingX;
            float height = (_rows - 1) * _spacingZ;

            GameObject camera = new GameObject("Main Camera") { tag = "MainCamera" };
            Camera cam = camera.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = (height + _spacingZ) / 2;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f);
            camera.AddComponent<AudioListener>();
            camera.transform.SetPositionAndRotation(new Vector3(width / 2, 20, height / 2), Quaternion.Euler(90, 0, 0));

            GameObject light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(60, 30, 0);

            for (int l = 0; l < _rows; l++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    CreateArena("arena_" + (l * _columns + c).ToString("00"), new Vector3(c * _spacingX, 0, l * _spacingZ));
                }
            }

            EditorSceneManager.SaveScene(scene, _scenePath);
            Debug.Log("Cena de treino do Pong criada em " + _scenePath);
        }

        private static void CreateArena(string name, Vector3 pos)
        {
            GameObject arena = new GameObject(name);
            arena.transform.position = pos;

            CreateWall(arena.transform, 6);
            CreateWall(arena.transform, -6);

            //bola
            GameObject ball = new GameObject("obj_bola");
            ball.transform.SetParent(arena.transform, false);
            SphereCollider ballCollider = ball.AddComponent<SphereCollider>();
            ballCollider.isTrigger = true;
            ballCollider.radius = 0.25f;
            Rigidbody ballRigidbody = ball.AddComponent<Rigidbody>();
            ballRigidbody.useGravity = false;
            ballRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            CreateMesh(ball.transform, PrimitiveType.Sphere, Vector3.one * 0.5f);
            ball.AddComponent<PongTrainingBall>().Configure(arena.transform);

            //agente (esquerda, no lugar do jogador) e adversario reativo (direita, igual ao jogo)
            GameObject agentObj = CreatePaddle(arena.transform, "obj_agentPong", "Player", -9);
            GameObject opponentObj = CreatePaddle(arena.transform, "obj_oponentePong", "IA", 9);

            PongTrainingOpponent opponent = opponentObj.AddComponent<PongTrainingOpponent>();
            opponent.Configure(arena.transform, ballRigidbody);

            PongTrainingArena arenaScript = arena.AddComponent<PongTrainingArena>();

            AgentPong agent = agentObj.AddComponent<AgentPong>(); //ja adiciona o Behavior Parameters
            agent.MaxStep = 3000; //60s de partida (0.02s por step), igual ao _matchDuration
            agent.Configure(arenaScript, ball.transform, ballRigidbody, null);

            BehaviorParameters behavior = agentObj.GetComponent<BehaviorParameters>();
            behavior.BehaviorName = behaviorName;
            behavior.BrainParameters.VectorObservationSize = AgentPong.ObservationCount;
            behavior.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(AgentPong.ActionCount);

            DecisionRequester decision = agentObj.AddComponent<DecisionRequester>();
            decision.DecisionPeriod = 2; //o reativo decide todo frame; decidir mais vezes gasta menos energia a toa
            decision.TakeActionsBetweenDecisions = true;

            arenaScript.Configure(ballRigidbody, agent, opponent);
        }

        private static void CreateWall(Transform parent, float z)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "col_parede";
            wall.tag = "Parede";
            wall.transform.SetParent(parent, false);
            wall.transform.localPosition = new Vector3(0, 0, z);
            wall.transform.localScale = new Vector3(30, 1, 1);
        }

        private static GameObject CreatePaddle(Transform parent, string name, string tag, float x)
        {
            GameObject paddle = new GameObject(name) { tag = tag };
            paddle.transform.SetParent(parent, false);
            paddle.transform.localPosition = new Vector3(x, 0, 0);

            paddle.AddComponent<BoxCollider>().size = new Vector3(1, 1, 3);

            Rigidbody rb = paddle.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            CreateMesh(paddle.transform, PrimitiveType.Cube, new Vector3(1, 0.5f, 3));

            return paddle;
        }

        private static void CreateMesh(Transform parent, PrimitiveType type, Vector3 scale)
        {
            GameObject mesh = GameObject.CreatePrimitive(type);
            mesh.name = "mesh";
            Object.DestroyImmediate(mesh.GetComponent<Collider>());
            mesh.transform.SetParent(parent, false);
            mesh.transform.localScale = scale;
        }

        #endregion
    }
}
