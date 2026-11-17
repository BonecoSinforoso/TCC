using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Agente treinado (ML-Agents) da raquete do Pong.
    // Funciona dos dois lados da mesa: as observacoes sao espelhadas pelo lado em que a raquete esta.
    // Na cena de treino quem reinicia a partida eh o PongTrainingArena; na cena do jogo o campo "arena" fica vazio.
    //
    // Observacoes (9): z da raquete, x e z da bola, velocidade x e z da bola, diferenca de z bola-raquete,
    //                  energia, bola vindo na minha direcao, ponto previsto de chegada da bola.
    // Acao (1 ramo discreto de 3): 0 parado, 1 sobe (+z), 2 desce (-z).
    public class AgentPong : Agent
    {
        #region membros

        public const int ObservationCount = 9;
        public const int ActionCount = 3;

        [FormerlySerializedAs("moveSpeed")]
        [SerializeField] private float _moveSpeed = 7;
        [FormerlySerializedAs("posLimiteZ")]
        [SerializeField] private float _limitZ = 3.5f;
        [FormerlySerializedAs("t_bola")]
        [SerializeField] private Transform _ballTransform;
        [FormerlySerializedAs("rb_bola")]
        [SerializeField] private Rigidbody _ballRigidbody;

        [Header("Energia (mesmas regras do IaPong)")]
        [FormerlySerializedAs("energiaMax")]
        [SerializeField] private float _maxEnergy = 100;
        [FormerlySerializedAs("energiaAtual")]
        [SerializeField] private float _currentEnergy = 100;
        [FormerlySerializedAs("energiaConsumo")]
        [SerializeField] private float _energyConsumption = 100;
        [FormerlySerializedAs("energiaRecarga")]
        [SerializeField] private float _energyRecharge = 25;
        [FormerlySerializedAs("energiaMinMover")]
        [SerializeField] private float _minEnergyToMove = 5;
        [FormerlySerializedAs("t_energiaBarra")]
        [SerializeField] private Transform _energyBarTransform;

        [Header("Treino")]
        [FormerlySerializedAs("arena")]
        [SerializeField] private PongTrainingArena _arena;
        [FormerlySerializedAs("recompensaRebater")]
        [SerializeField] private float _hitReward = 1f;
        [FormerlySerializedAs("recompensaGolSofrido")]
        [SerializeField] private float _goalConcededReward = -1f;
        [FormerlySerializedAs("recompensaGolFeito")]
        [SerializeField] private float _goalScoredReward = 1f;
        [FormerlySerializedAs("recompensaPorStep")]
        [SerializeField] private float _rewardPerStep = 0f; //penalidade por existir (opcional); ligada ela premia tomar gol logo

        //normalizacao das observacoes
        private const float _fieldX = 11f;
        private const float _fieldZ = 5.5f;
        private const float _maxBallSpeed = 12f;
        private const float _ballLimitZ = 5.25f; //parede (5.5) menos o raio da bola (0.25)

        private Rigidbody _rigidbody;
        private Transform _centerTransform; //referencia do meio da mesa (a arena no treino, o mundo no jogo)
        private float _side; //-1 = raquete da esquerda, 1 = raquete da direita
        private int _currentAction;

        #endregion

        #region metodos

        public override void Initialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _centerTransform = _arena != null ? _arena.transform : null;
            _side = ToLocalPos(transform.position).x < 0 ? -1 : 1;
        }

        public void Configure(PongTrainingArena arena, Transform newBall, Rigidbody newBallBody, Transform newEnergyBar)
        {
            _arena = arena;
            _ballTransform = newBall;
            _ballRigidbody = newBallBody;
            _energyBarTransform = newEnergyBar;
        }

        public override void OnEpisodeBegin()
        {
            _currentAction = 0;

            if (_arena == null) return; //no jogo quem controla a partida eh o PongManager

            _currentEnergy = _maxEnergy;
            _rigidbody.linearVelocity = Vector3.zero;

            Vector3 pos = ToLocalPos(transform.position);
            pos.z = Random.Range(-_limitZ, _limitZ);
            transform.position = ToWorldPos(pos);

            _arena.Restart();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Vector3 paddle = ToLocalPos(transform.position);
            Vector3 ball = ToLocalPos(_ballTransform.position);
            Vector3 ballVel = _ballRigidbody.linearVelocity;

            sensor.AddObservation(paddle.z / _limitZ);
            sensor.AddObservation(Mathf.Clamp(ball.x * _side / _fieldX, -1f, 1f)); //1 = bola no meu gol
            sensor.AddObservation(Mathf.Clamp(ball.z / _fieldZ, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(ballVel.x * _side / _maxBallSpeed, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(ballVel.z / _maxBallSpeed, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp((ball.z - paddle.z) / (_fieldZ * 2), -1f, 1f));
            sensor.AddObservation(_currentEnergy / _maxEnergy);
            sensor.AddObservation(ballVel.x * _side > 0 ? 1f : 0f); //bola vindo em direcao ao meu gol
            sensor.AddObservation(Mathf.Clamp(CalculateBallArrival(paddle, ball, ballVel) / (_fieldZ * 2), -1f, 1f));
        }

        // Onde a bola vai cruzar a linha da raquete (ja contando as rebatidas nas paredes), relativo ao z da raquete.
        // Eh a mesma informacao que o agente reativo tira dos raycasts de trajetoria do BolaPong.
        // Com a bola indo para o outro lado devolve 0.
        private float CalculateBallArrival(Vector3 paddle, Vector3 ball, Vector3 ballVel)
        {
            if (ballVel.x * _side <= 0) return 0;

            float time = (paddle.x - ball.x) / ballVel.x;
            if (time < 0) return 0; //bola ja passou da raquete

            //"desdobra" o caminho da bola como se as paredes fossem espelhos
            float z = Mathf.Repeat(ball.z + ballVel.z * time + _ballLimitZ, _ballLimitZ * 4);
            z = z < _ballLimitZ * 2 ? z - _ballLimitZ : _ballLimitZ * 3 - z;

            return z - paddle.z;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            _currentAction = actions.DiscreteActions[0];

            AddReward(_rewardPerStep);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
            float v = Input.GetAxisRaw("Vertical");

            actionSegment[0] = v > 0 ? 1 : v < 0 ? 2 : 0;
        }

        private void FixedUpdate()
        {
            float z = ToLocalPos(transform.position).z;
            bool moved = false;

            if (_currentEnergy > _minEnergyToMove)
            {
                if (_currentAction == 1 && z < _limitZ)
                {
                    _rigidbody.linearVelocity = Vector3.forward * _moveSpeed;
                    moved = true;
                }
                else if (_currentAction == 2 && z > -_limitZ)
                {
                    _rigidbody.linearVelocity = Vector3.back * _moveSpeed;
                    moved = true;
                }
            }

            if (moved) //se andou diminui a energia
            {
                _currentEnergy -= _energyConsumption * Time.fixedDeltaTime;
                if (_currentEnergy < 0) _currentEnergy = 0;
            }
            else //se nn andou, aumenta a energia e para a raquete
            {
                _currentEnergy += _energyRecharge * Time.fixedDeltaTime;
                if (_currentEnergy > _maxEnergy) _currentEnergy = _maxEnergy;

                _rigidbody.linearVelocity = Vector3.zero;
            }

            if (_energyBarTransform != null) _energyBarTransform.localScale = new Vector3(1, 1, _currentEnergy / _maxEnergy);
        }

        //chamados pela arena de treino
        public void HitBall()
        {
            AddReward(_hitReward);
        }

        public void ConcedeGoal()
        {
            AddReward(_goalConcededReward);
            EndEpisode();
        }

        public void ScoreGoal()
        {
            AddReward(_goalScoredReward);
            EndEpisode();
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
