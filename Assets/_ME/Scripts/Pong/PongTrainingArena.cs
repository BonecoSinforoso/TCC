using UnityEngine;

// Mesa de Pong usada SO no treino do ML-Agents.
// Copia as regras do Script_PongManager / Script_BolaPong, mas sem singleton, sem UI e sem Time.timeScale = 0,
// para que varias mesas possam rodar ao mesmo tempo na mesma cena.
// Tudo eh calculado em posicao local da mesa (o objeto que tem este script eh o centro dela).
public class PongTrainingArena : MonoBehaviour
{
    [SerializeField] Rigidbody rb_bola;
    [SerializeField] AgentPong agente;
    [SerializeField] PongTrainingOpponent oponente;
    [SerializeField] float golX = 11f; //mesmo limite do Script_BolaPong
    [SerializeField] float bolaAcelerarTempo = 10f; //mesmo intervalo do BolaMoveSpeedUp
    [SerializeField] float bolaSairTempo = 0.5f; //espera antes da bola sair (no jogo eh a barra de espaco)

    float agenteLado; //-1 = agente na esquerda, 1 = agente na direita
    float tempoSair;
    float tempoAcelerar;
    bool bolaMovendo;

    //placar desde o Play (somado em todas as mesas), para medir o agente contra o adversario reativo
    public static int GolsFeitos;
    public static int GolsSofridos;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Placar_Reset()
    {
        GolsFeitos = 0;
        GolsSofridos = 0;
    }

    public void Configurar(Rigidbody _rb_bola, AgentPong _agente, PongTrainingOpponent _oponente)
    {
        rb_bola = _rb_bola;
        agente = _agente;
        oponente = _oponente;
    }

    void Awake()
    {
        agenteLado = transform.InverseTransformPoint(agente.transform.position).x < 0 ? -1 : 1;
    }

    //chamado pelo agente no inicio de cada episodio
    public void Reiniciar()
    {
        bolaMovendo = false;
        tempoSair = bolaSairTempo;

        rb_bola.transform.position = transform.position;
        rb_bola.linearVelocity = Vector3.zero;

        if (oponente != null) oponente.EnergiaReset();
    }

    void FixedUpdate()
    {
        if (!bolaMovendo)
        {
            tempoSair -= Time.fixedDeltaTime;
            if (tempoSair <= 0) BolaMover();
            return;
        }

        tempoAcelerar -= Time.fixedDeltaTime;
        if (tempoAcelerar <= 0) BolaAcelerar();

        float _bolaX = transform.InverseTransformPoint(rb_bola.position).x;

        if (_bolaX <= -golX || _bolaX >= golX)
        {
            bolaMovendo = false; //o agente chama Reiniciar() ao comecar o proximo episodio

            if (_bolaX * agenteLado > 0)
            {
                GolsSofridos++;
                agente.GolSofrido();
            }
            else
            {
                GolsFeitos++;
                agente.GolFeito();
            }
        }
    }

    void BolaMover() //mesma conta do Script_PongManager.BolaMover
    {
        float _bolaMoveSpeedX = Random.Range(5, 8);
        float _bolaMoveSpeedY = 5 + (5 - _bolaMoveSpeedX);

        Vector3 _sentido = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right;

        rb_bola.linearVelocity = _sentido * _bolaMoveSpeedX + ((Random.Range(0, 2) == 0 ? -1 : 1) * _bolaMoveSpeedY) * Vector3.forward;

        bolaMovendo = true;
        tempoAcelerar = bolaAcelerarTempo;
    }

    void BolaAcelerar() //mesma conta do Script_PongManager.BolaMoveSpeedUp
    {
        if (Random.Range(0, 2) == 0)
        {
            if (rb_bola.linearVelocity.x > 0) rb_bola.linearVelocity += Vector3.right;
            else rb_bola.linearVelocity += Vector3.left;
        }
        else
        {
            if (rb_bola.linearVelocity.z > 0) rb_bola.linearVelocity += Vector3.forward;
            else rb_bola.linearVelocity += Vector3.back;
        }

        tempoAcelerar = bolaAcelerarTempo;
    }
}
