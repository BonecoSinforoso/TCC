using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Mesa de Pong usada SO no treino do ML-Agents.
    // Copia as regras do PongManager / BolaPong, mas sem singleton, sem UI e sem Time.timeScale = 0,
    // para que varias mesas possam rodar ao mesmo tempo na mesma cena.
    // Tudo eh calculado em posicao local da mesa (o objeto que tem este script eh o centro dela).
    public class PongTrainingArena : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("rb_bola")]
        [SerializeField] private Rigidbody _ballRigidbody;
        [FormerlySerializedAs("agente")]
        [SerializeField] private AgentPong _agent;
        [FormerlySerializedAs("oponente")]
        [SerializeField] private PongTrainingOpponent _opponent;
        [FormerlySerializedAs("golX")]
        [SerializeField] private float _goalX = 11f; //mesmo limite do BolaPong
        [FormerlySerializedAs("bolaAcelerarTempo")]
        [SerializeField] private float _ballSpeedUpTime = 10f; //mesmo intervalo do SpeedUpBall
        [FormerlySerializedAs("bolaSairTempo")]
        [SerializeField] private float _ballLaunchDelay = 0.5f; //espera antes da bola sair (no jogo eh a barra de espaco)

        private float _agentSide; //-1 = agente na esquerda, 1 = agente na direita
        private float _launchTimer;
        private float _speedUpTimer;
        private bool _ballMoving;

        //placar desde o Play (somado em todas as mesas), para medir o agente contra o adversario reativo
        public static int GoalsScored;
        public static int GoalsConceded;

        #endregion

        #region metodos

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetScore()
        {
            GoalsScored = 0;
            GoalsConceded = 0;
        }

        public void Configure(Rigidbody newBallBody, AgentPong newAgent, PongTrainingOpponent newOpponent)
        {
            _ballRigidbody = newBallBody;
            _agent = newAgent;
            _opponent = newOpponent;
        }

        private void Awake()
        {
            _agentSide = transform.InverseTransformPoint(_agent.transform.position).x < 0 ? -1 : 1;
        }

        //chamado pelo agente no inicio de cada episodio
        public void Restart()
        {
            _ballMoving = false;
            _launchTimer = _ballLaunchDelay;

            _ballRigidbody.transform.position = transform.position;
            _ballRigidbody.linearVelocity = Vector3.zero;

            if (_opponent != null) _opponent.ResetEnergy();
        }

        private void FixedUpdate()
        {
            if (!_ballMoving)
            {
                _launchTimer -= Time.fixedDeltaTime;
                if (_launchTimer <= 0) MoveBall();
                return;
            }

            _speedUpTimer -= Time.fixedDeltaTime;
            if (_speedUpTimer <= 0) SpeedUpBall();

            float ballX = transform.InverseTransformPoint(_ballRigidbody.position).x;

            if (ballX <= -_goalX || ballX >= _goalX)
            {
                _ballMoving = false; //o agente chama Restart() ao comecar o proximo episodio

                if (ballX * _agentSide > 0)
                {
                    GoalsConceded++;
                    _agent.ConcedeGoal();
                }
                else
                {
                    GoalsScored++;
                    _agent.ScoreGoal();
                }
            }
        }

        private void MoveBall() //mesma conta do PongManager.MoveBall
        {
            float ballSpeedX = Random.Range(5, 8);
            float ballSpeedY = 5 + (5 - ballSpeedX);

            Vector3 direction = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;

            _ballRigidbody.linearVelocity = direction * ballSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * ballSpeedY) * Vector3.forward;

            _ballMoving = true;
            _speedUpTimer = _ballSpeedUpTime;
        }

        private void SpeedUpBall() //mesma conta do PongManager.SpeedUpBall
        {
            if (Random.Range(0, 2) == 0)
            {
                if (_ballRigidbody.linearVelocity.x > 0) _ballRigidbody.linearVelocity += Vector3.right;
                else _ballRigidbody.linearVelocity += Vector3.left;
            }
            else
            {
                if (_ballRigidbody.linearVelocity.z > 0) _ballRigidbody.linearVelocity += Vector3.forward;
                else _ballRigidbody.linearVelocity += Vector3.back;
            }

            _speedUpTimer = _ballSpeedUpTime;
        }

        #endregion
    }
}
