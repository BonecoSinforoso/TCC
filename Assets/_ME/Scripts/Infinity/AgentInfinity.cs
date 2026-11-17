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
// Acoes (2 ramos discretos): faixa alvo (0 esquerda, 1 centro, 2 direita) e pulo (0 nada, 1 pular).
public class AgentInfinity : Agent
{
    public const int observacoesQuant = 5;
    public const int faixasQuant = 3;

    [SerializeField] float puloForca = 6;
    [SerializeField] float faixaDist = 2.5f;
    [SerializeField] float faixaTrocaSpeed = 10f;
    [SerializeField] float distanciaGanhar = 1000f;
    [SerializeField] Transform t_sensores;

    [Header("Treino")]
    [SerializeField] InfinityTrainingArena arena;
    [SerializeField] float recompensaPorMetro = 0.02f;
    [SerializeField] float recompensaBater = -1f;

    //mesma conta de velocidade do Script_IaInfinity: 5 + 1 a cada 100m, ate 8
    const float moveSpeedMin = 5f;
    const float moveSpeedMax = 8f;

    Rigidbody rb;
    Animator animator;
    Transform t_centro; //a arena no treino, o mundo no jogo
    float faixaCentroX; //x (local) do meio da pista
    float zInicial;
    float zAnterior;
    float moveSpeed = moveSpeedMin;
    int faixaAlvo = 1;
    bool puloPedido;
    bool puloPode;

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
        faixaAlvo = 1;
        puloPedido = false;

        if (arena == null) return; //no jogo a corrida nn reinicia sozinha

        Perdeu = false;
        puloPode = false; //volta a poder pular quando encostar no chao
        moveSpeed = moveSpeedMin;

        int _faixa = Random.Range(0, faixasQuant);
        Vector3 _pos = new Vector3(faixaCentroX + (_faixa - 1) * faixaDist, 0.1f, zInicial);

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
        faixaAlvo = actions.DiscreteActions[0];
        puloPedido = actions.DiscreteActions[1] == 1;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> _acoes = actionsOut.DiscreteActions;
        float _h = Input.GetAxisRaw("Horizontal");

        _acoes[0] = _h < 0 ? 0 : _h > 0 ? 2 : 1;
        _acoes[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    void FixedUpdate()
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

        if (puloPedido) Pulo();

        //recompensa pela distancia percorrida
        AddReward((_pos.z - zAnterior) * recompensaPorMetro);
        zAnterior = _pos.z;

        Sensores_Posicionar();

        if (arena != null && _pos.z - zInicial >= distanciaGanhar) EndEpisode(); //chegou ao fim da pista
    }

    void Sensores_Posicionar()
    {
        if (t_sensores == null) return;

        float _z = PosLocal(transform.position).z;
        t_sensores.SetPositionAndRotation(PosMundo(new Vector3(faixaCentroX, 0, _z)), Quaternion.identity);
    }

    void Pulo()
    {
        if (!puloPode) return;
        puloPode = false;
        rb.AddForce(Vector3.up * puloForca, ForceMode.Impulse);
    }

    void Bateu()
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

    Vector3 PosLocal(Vector3 _pos)
    {
        return t_centro != null ? t_centro.InverseTransformPoint(_pos) : _pos;
    }

    Vector3 PosMundo(Vector3 _pos)
    {
        return t_centro != null ? t_centro.TransformPoint(_pos) : _pos;
    }
}
