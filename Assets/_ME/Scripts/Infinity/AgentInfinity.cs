using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

// Agente treinado (ML-Agents) do corredor do Infinity.
// Na cena de treino quem reinicia os obstaculos eh o InfinityTrainingArena; na cena do jogo o campo "arena" fica vazio.
//
// Observacoes:
//  - 3 RayPerceptionSensorComponent3D (um raio para frente em cada faixa, tags Carro e Onibus), no filho "emp_sensores".
//    Eles substituem os raycasts manuais do Script_IaInfinity. O "emp_sensores" fica sempre no meio da pista,
//    na altura do chao, acompanhando so o Z do corredor.
//  - vetor (5): faixa atual (-1 a 1), velocidade, altura, velocidade vertical, se pode pular.
// Acoes (2 ramos discretos): faixa (0 fica, 1 vai uma para a esquerda, 2 uma para a direita) e pulo (0 nada, 1 pular).
public class AgentInfinity : Agent
{
    public const int observacoesQuant = 5;
    public const int faixasQuant = 3;

    [SerializeField] private float puloForca = 6;
    [SerializeField] private float faixaDist = 2.5f;
    [SerializeField] private float faixaTrocaSpeed = 1000f; //troca na hora, igual ao jogador
    [SerializeField] private float distanciaGanhar = 1000f;
    [SerializeField] private Transform t_sensores;

    [Header("Treino")]
    [SerializeField] private InfinityTrainingArena arena;
    [SerializeField] private float recompensaPorMetro = 0.1f;
    [SerializeField] private float recompensaBater = -1f;

    //mesma conta de velocidade do Script_IaInfinity: 5 + 1 a cada 100m, ate 8
    private const float moveSpeedMin = 5f;
    private const float moveSpeedMax = 8f;

    private Rigidbody rb;
    private Animator animator;
    private Transform t_centro; //a arena no treino, o mundo no jogo
    private float faixaCentroX; //x (local) do meio da pista
    private float zInicial;
    private float zAnterior;
    private float moveSpeed = moveSpeedMin;
    private int faixaAlvo = 1;
    private bool puloPode;

    public bool Perdeu { get; private set; }
    public float Distancia => PosLocal(transform.position).z - zInicial;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        t_centro = arena != null ? arena.transform : null;

        Vector3 _pos = PosLocal(transform.position);
        faixaCentroX = _pos.x;
        zInicial = _pos.z;
        zAnterior = _pos.z;
    }

    public void Configurar(InfinityTrainingArena _arena, Transform _t_sensores)
    {
        arena = _arena;
        t_sensores = _t_sensores;
    }

    public override void OnEpisodeBegin()
    {
        if (arena == null) return; //no jogo a corrida nn reinicia sozinha

        Perdeu = false;
        puloPode = false; //volta a poder pular quando encostar no chao
        moveSpeed = moveSpeedMin;

        faixaAlvo = Random.Range(0, faixasQuant);
        Vector3 _pos = new Vector3(faixaCentroX + (faixaAlvo - 1) * faixaDist, 0.1f, zInicial);

        rb.linearVelocity = Vector3.zero;
        transform.position = PosMundo(_pos);
        zAnterior = zInicial;

        Sensores_Posicionar();
        arena.Reiniciar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 _pos = PosLocal(transform.position);

        sensor.AddObservation(Mathf.Clamp((_pos.x - faixaCentroX) / faixaDist, -1f, 1f));
        sensor.AddObservation((moveSpeed - moveSpeedMin) / (moveSpeedMax - moveSpeedMin));
        sensor.AddObservation(Mathf.Clamp(_pos.y / 2f, 0f, 1.5f));
        sensor.AddObservation(Mathf.Clamp(rb.linearVelocity.y / puloForca, -1.5f, 1.5f));
        sensor.AddObservation(puloPode ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (Perdeu) return;

        int _mover = actions.DiscreteActions[0];
        if (_mover == 1) faixaAlvo = Mathf.Max(faixaAlvo - 1, 0);
        else if (_mover == 2) faixaAlvo = Mathf.Min(faixaAlvo + 1, faixasQuant - 1);

        if (actions.DiscreteActions[1] == 1) Pulo();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> _acoes = actionsOut.DiscreteActions;
        float _h = Input.GetAxisRaw("Horizontal");

        _acoes[0] = _h < 0 ? 1 : _h > 0 ? 2 : 0;
        _acoes[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if (Perdeu) return;

        if (animator != null) animator.SetBool("_jump", !puloPode);

        Vector3 _pos = PosLocal(transform.position);

        moveSpeed = Mathf.Min(moveSpeedMin + ((int)(_pos.z - zInicial) / 100), moveSpeedMax);
        rb.linearVelocity = Vector3.forward * moveSpeed + Vector3.up * rb.linearVelocity.y;

        //troca de faixa
        float _alvoX = faixaCentroX + (faixaAlvo - 1) * faixaDist;
        if (_pos.x != _alvoX)
        {
            _pos.x = Mathf.MoveTowards(_pos.x, _alvoX, faixaTrocaSpeed * Time.fixedDeltaTime);
            transform.position = PosMundo(_pos);
        }

        //recompensa pela distancia percorrida
        AddReward((_pos.z - zAnterior) * recompensaPorMetro);
        zAnterior = _pos.z;

        Sensores_Posicionar();

        if (arena != null && _pos.z - zInicial >= distanciaGanhar) EndEpisode(); //chegou ao fim da pista
    }

    private void Sensores_Posicionar()
    {
        if (t_sensores == null) return;

        float _z = PosLocal(transform.position).z;
        t_sensores.SetPositionAndRotation(PosMundo(new Vector3(faixaCentroX, 0, _z)), Quaternion.identity);
    }

    private void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
    }

    private void Bateu()
    {
        AddReward(recompensaBater);

        if (arena != null)
        {
            EndEpisode();
        }
        else //no jogo o corredor para, igual aos outros personagens
        {
            Perdeu = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Perdeu) return;

        if (collision.gameObject.CompareTag("Carro") || collision.gameObject.CompareTag("Onibus"))
        {
            if (arena == null) Destroy(collision.gameObject);
            Bateu();
        }
        else if (collision.gameObject.CompareTag("Chao"))
        {
            puloPode = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Chao")) puloPode = true;
    }

    private Vector3 PosLocal(Vector3 _pos)
    {
        return t_centro != null ? t_centro.InverseTransformPoint(_pos) : _pos;
    }

    private Vector3 PosMundo(Vector3 _pos)
    {
        return t_centro != null ? t_centro.TransformPoint(_pos) : _pos;
    }
}
