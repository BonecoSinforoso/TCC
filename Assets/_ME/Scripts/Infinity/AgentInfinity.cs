using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Agente treinado (ML-Agents) do corredor do Infinity.
    // Na cena de treino quem reinicia os obstaculos eh o InfinityTrainingArena; na cena do jogo o campo "arena" fica vazio.
    //
    // Observacoes:
    //  - 3 RayPerceptionSensorComponent3D (um raio para frente em cada faixa, tags Carro e Onibus), no filho "emp_sensores".
    //    Eles substituem os raycasts manuais do IaInfinity. O "emp_sensores" fica sempre no meio da pista,
    //    na altura do chao, acompanhando so o Z do corredor.
    //  - vetor (5): faixa atual (-1 a 1), velocidade, altura, velocidade vertical, se pode pular.
    // Acoes (2 ramos discretos): faixa (0 fica, 1 vai uma para a esquerda, 2 uma para a direita) e pulo (0 nada, 1 pular).
    public class AgentInfinity : Agent
    {
        #region membros

        public const int ObservationCount = 5;
        public const int LaneCount = 3;

        [FormerlySerializedAs("puloForca")]
        [SerializeField] private float _jumpForce = 6;
        [FormerlySerializedAs("faixaDist")]
        [SerializeField] private float _laneDistance = 2.5f;
        [FormerlySerializedAs("faixaTrocaSpeed")]
        [SerializeField] private float _laneSwitchSpeed = 1000f; //troca na hora, igual ao jogador
        [FormerlySerializedAs("distanciaGanhar")]
        [SerializeField] private float _distanceToWin = 1000f;
        [FormerlySerializedAs("t_sensores")]
        [SerializeField] private Transform _sensorsTransform;

        [Header("Treino")]
        [FormerlySerializedAs("arena")]
        [SerializeField] private InfinityTrainingArena _arena;
        [FormerlySerializedAs("recompensaPorMetro")]
        [SerializeField] private float _rewardPerMeter = 0.1f;
        [FormerlySerializedAs("recompensaBater")]
        [SerializeField] private float _crashReward = -1f;

        //mesma conta de velocidade do IaInfinity: 5 + 1 a cada 100m, ate 8
        private const float moveSpeedMin = 5f;
        private const float moveSpeedMax = 8f;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private Transform _centerTransform; //a arena no treino, o mundo no jogo
        private float _laneCenterX; //x (local) do meio da pista
        private float _startZ;
        private float _previousZ;
        private float _moveSpeed = moveSpeedMin;
        private int _targetLane = 1;
        private bool _canJump;

        public bool Lost { get; private set; }
        public float Distance => ToLocalPos(transform.position).z - _startZ;

        #endregion

        #region metodos

        public override void Initialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            _centerTransform = _arena != null ? _arena.transform : null;

            Vector3 pos = ToLocalPos(transform.position);
            _laneCenterX = pos.x;
            _startZ = pos.z;
            _previousZ = pos.z;
        }

        public void Configure(InfinityTrainingArena arena, Transform newSensors)
        {
            _arena = arena;
            _sensorsTransform = newSensors;
        }

        public override void OnEpisodeBegin()
        {
            if (_arena == null) return; //no jogo a corrida nn reinicia sozinha

            Lost = false;
            _canJump = false; //volta a poder pular quando encostar no chao
            _moveSpeed = moveSpeedMin;

            _targetLane = Random.Range(0, LaneCount);
            Vector3 pos = new Vector3(_laneCenterX + (_targetLane - 1) * _laneDistance, 0.1f, _startZ);

            _rigidbody.linearVelocity = Vector3.zero;
            transform.position = ToWorldPos(pos);
            _previousZ = _startZ;

            PositionSensors();
            _arena.Restart();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Vector3 pos = ToLocalPos(transform.position);

            sensor.AddObservation(Mathf.Clamp((pos.x - _laneCenterX) / _laneDistance, -1f, 1f));
            sensor.AddObservation((_moveSpeed - moveSpeedMin) / (moveSpeedMax - moveSpeedMin));
            sensor.AddObservation(Mathf.Clamp(pos.y / 2f, 0f, 1.5f));
            sensor.AddObservation(Mathf.Clamp(_rigidbody.linearVelocity.y / _jumpForce, -1.5f, 1.5f));
            sensor.AddObservation(_canJump ? 1f : 0f);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (Lost) return;

            int move = actions.DiscreteActions[0];
            if (move == 1) _targetLane = Mathf.Max(_targetLane - 1, 0);
            else if (move == 2) _targetLane = Mathf.Min(_targetLane + 1, LaneCount - 1);

            if (actions.DiscreteActions[1] == 1) Jump();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
            float h = Input.GetAxisRaw("Horizontal");

            actionSegment[0] = h < 0 ? 1 : h > 0 ? 2 : 0;
            actionSegment[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        }

        private void FixedUpdate()
        {
            if (Lost) return;

            if (_animator != null) _animator.SetBool("_jump", !_canJump);

            Vector3 pos = ToLocalPos(transform.position);

            _moveSpeed = Mathf.Min(moveSpeedMin + ((int)(pos.z - _startZ) / 100), moveSpeedMax);
            _rigidbody.linearVelocity = Vector3.forward * _moveSpeed + Vector3.up * _rigidbody.linearVelocity.y;

            //troca de faixa
            float targetX = _laneCenterX + (_targetLane - 1) * _laneDistance;
            if (pos.x != targetX)
            {
                pos.x = Mathf.MoveTowards(pos.x, targetX, _laneSwitchSpeed * Time.fixedDeltaTime);
                transform.position = ToWorldPos(pos);
            }

            //recompensa pela distancia percorrida
            AddReward((pos.z - _previousZ) * _rewardPerMeter);
            _previousZ = pos.z;

            PositionSensors();

            if (_arena != null && pos.z - _startZ >= _distanceToWin) //chegou ao fim da pista
            {
                Wins++;
                CountEpisode();
                EndEpisode();
            }
        }

        //placar desde o Play (somado em todas as pistas), para medir o modelo
        public static int Episodes;
        public static int Wins;
        public static float TotalDistance;
        public static float MaxDistance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetScore()
        {
            Episodes = 0;
            Wins = 0;
            TotalDistance = 0;
            MaxDistance = 0;
        }

        private void CountEpisode()
        {
            Episodes++;
            TotalDistance += Distance;
            if (Distance > MaxDistance) MaxDistance = Distance;
        }

        private void PositionSensors()
        {
            if (_sensorsTransform == null) return;

            float z = ToLocalPos(transform.position).z;
            _sensorsTransform.SetPositionAndRotation(ToWorldPos(new Vector3(_laneCenterX, 0, z)), Quaternion.identity);
        }

        private void Jump()
        {
            if (!_canJump) return;
            _canJump = false;
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private void Crash()
        {
            CountEpisode();
            AddReward(_crashReward);

            if (_arena != null)
            {
                EndEpisode();
            }
            else //no jogo o corredor para, igual aos outros personagens
            {
                Lost = true;
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Lost) return;

            if (collision.gameObject.CompareTag("Carro") || collision.gameObject.CompareTag("Onibus"))
            {
                if (_arena == null) Destroy(collision.gameObject);
                Crash();
            }
            else if (collision.gameObject.CompareTag("Chao"))
            {
                _canJump = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Chao")) _canJump = true;
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
