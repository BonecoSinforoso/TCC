using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Agente treinado (ML-Agents) do passaro do Flap.
    // Na cena de treino quem reinicia os canos eh o FlapTrainingArena; na cena do jogo o campo "arena" fica vazio.
    //
    // Observacoes (8): altura, velocidade vertical, distancia X e diferenca de altura do proximo cano,
    //                  diferenca de altura do cano seguinte, distancia ao chao, se pode pular,
    //                  velocidade vertical do proximo cano (canos moveis).
    // Acao (1 ramo discreto de 2): 0 nada, 1 pular.
    public class AgentFlap : Agent
    {
        #region membros

        public const int ObservationCount = 8;
        public const int ActionCount = 2;

        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce = 5;
        [FormerlySerializedAs("puloCd")]
        [SerializeField] private float _jumpCooldown = 0.2f;
        [FormerlySerializedAs("t_cano")]
        [SerializeField] private Transform[] _pipeTransforms;
        [FormerlySerializedAs("chaoY")]
        [SerializeField] private float _groundY = -5f;
        [FormerlySerializedAs("tetoY")]
        [SerializeField] private float _ceilingY = 7f; //acima disso conta como batida (evita passar por cima dos canos)

        [Header("Treino")]
        [FormerlySerializedAs("arena")]
        [SerializeField] private FlapTrainingArena _arena;
        [FormerlySerializedAs("recompensaPonto")]
        [SerializeField] private float _pointReward = 1f;
        [FormerlySerializedAs("recompensaBater")]
        [SerializeField] private float _crashReward = -1f;
        [FormerlySerializedAs("recompensaPorDecisao")]
        [SerializeField] private float _decisionReward = 0.002f;

        //normalizacao das observacoes
        private const float _normHeight = 5f;
        private const float _normSpeed = 10f;
        private const float _normPipeDistance = 6f; //distancia entre dois canos
        private const float _pipePassed = 1f; //meia largura do cano + raio do passaro

        private Rigidbody _rigidbody;
        private Transform _centerTransform; //a arena no treino, o mundo no jogo
        private float _jumpTimer;
        private float[] _pipePrevY;
        private float[] _pipeVelocityY;

        public int Score { get; private set; }
        public bool Dead { get; private set; }

        #endregion

        #region metodos

        public override void Initialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _centerTransform = _arena != null ? _arena.transform : null;

            _pipePrevY = new float[_pipeTransforms.Length];
            _pipeVelocityY = new float[_pipeTransforms.Length];
            for (int i = 0; i < _pipeTransforms.Length; i++) _pipePrevY[i] = ToLocalPos(_pipeTransforms[i].position).y;
        }

        public void Configure(FlapTrainingArena arena, Transform[] newPipes)
        {
            _arena = arena;
            _pipeTransforms = newPipes;
        }

        public override void OnEpisodeBegin()
        {
            if (_arena == null) return; //no jogo a partida nn reinicia sozinha

            Score = 0;
            Dead = false;
            _jumpTimer = 0;

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(ToWorldPos(new Vector3(0, Random.Range(-1f, 1f), 0)), Quaternion.identity);

            _arena.Restart();

            for (int i = 0; i < _pipeTransforms.Length; i++)
            {
                _pipePrevY[i] = ToLocalPos(_pipeTransforms[i].position).y;
                _pipeVelocityY[i] = 0;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Vector3 pos = ToLocalPos(transform.position);

            GetNearestPipes(pos.x, out int pipe1, out int pipe2);

            sensor.AddObservation(Mathf.Clamp(pos.y / _normHeight, -1.5f, 1.5f));
            sensor.AddObservation(Mathf.Clamp(_rigidbody.linearVelocity.y / _normSpeed, -1.5f, 1.5f));

            if (pipe1 >= 0)
            {
                Vector3 pipe = ToLocalPos(_pipeTransforms[pipe1].position);
                sensor.AddObservation(Mathf.Clamp((pipe.x - pos.x) / _normPipeDistance, -1f, 2f));
                sensor.AddObservation(Mathf.Clamp((pipe.y - pos.y) / _normHeight, -2f, 2f));
            }
            else
            {
                sensor.AddObservation(2f);
                sensor.AddObservation(0f);
            }

            sensor.AddObservation(pipe2 >= 0 ? Mathf.Clamp((ToLocalPos(_pipeTransforms[pipe2].position).y - pos.y) / _normHeight, -2f, 2f) : 0f);
            sensor.AddObservation(Mathf.Clamp((pos.y - _groundY) / (_normHeight * 2), 0f, 1.5f));
            sensor.AddObservation(_jumpTimer <= 0 ? 1f : 0f);
            sensor.AddObservation(pipe1 >= 0 ? Mathf.Clamp(_pipeVelocityY[pipe1], -1f, 1f) : 0f);
        }

        // Proximo cano = o mais perto que ainda nn ficou para tras; seguinte = o que vem depois dele.
        private void GetNearestPipes(float x, out int pipe1, out int pipe2)
        {
            pipe1 = -1;
            pipe2 = -1;
            float dist1 = float.MaxValue;
            float dist2 = float.MaxValue;

            for (int i = 0; i < _pipeTransforms.Length; i++)
            {
                float dist = ToLocalPos(_pipeTransforms[i].position).x - x;
                if (dist < -_pipePassed) continue;

                if (dist < dist1)
                {
                    pipe2 = pipe1;
                    dist2 = dist1;
                    pipe1 = i;
                    dist1 = dist;
                }
                else if (dist < dist2)
                {
                    pipe2 = i;
                    dist2 = dist;
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (Dead) return;

            if (actions.DiscreteActions[0] == 1) Jump();

            AddReward(_decisionReward);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> actionSegment = actionsOut.DiscreteActions;

            actionSegment[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        }

        private void FixedUpdate()
        {
            if (_jumpTimer > 0) _jumpTimer -= Time.fixedDeltaTime;

            for (int i = 0; i < _pipeTransforms.Length; i++)
            {
                float height = ToLocalPos(_pipeTransforms[i].position).y;
                _pipeVelocityY[i] = (height - _pipePrevY[i]) / Time.fixedDeltaTime;
                _pipePrevY[i] = height;
            }

            if (!Dead && ToLocalPos(transform.position).y > _ceilingY) Crash();
        }

        private void Jump()
        {
            if (_jumpTimer > 0) return;
            _jumpTimer = _jumpCooldown;

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        //placar desde o Play (somado em todas as arenas), para medir o modelo
        public static int Episodes;
        public static int TotalScore;
        public static int MaxScore;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetScore()
        {
            Episodes = 0;
            TotalScore = 0;
            MaxScore = 0;
        }

        private void Crash()
        {
            Episodes++;
            TotalScore += Score;
            if (Score > MaxScore) MaxScore = Score;

            AddReward(_crashReward);

            if (_arena != null) EndEpisode();
            else Dead = true; //no jogo o passaro so para de agir; quem decide o fim eh o FlapManager
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Dead) return;

            if (collision.gameObject.CompareTag("Chao")) Crash();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Dead) return;

            if (other.CompareTag("Cano")) Crash();

            if (other.CompareTag("Ponto"))
            {
                Score++;
                AddReward(_pointReward);
            }
        }

        private Vector3 ToLocalPos(Vector3 pos)
        {
            return _centerTransform != null ? _centerTransform.InverseTransformPoint(pos) : pos;
        }

        private Vector3 ToWorldPos(Vector3 pos)
        {
            return _centerTransform != null ? _centerTransform.TransformPoint(pos) : pos;
        }

        #endregion
    }
}
