using UnityEngine;

// Arena do Flap usada SO no treino do ML-Agents: reinicia os canos a cada episodio.
// "movelChance" sorteia, por episodio, se os canos sobem e descem (a variacao "movel" do Script_FlapManager),
// para o mesmo modelo aprender as duas dificuldades.
public class FlapTrainingArena : MonoBehaviour
{
    [SerializeField] FlapTrainingPipes[] canos;
    [SerializeField] float canosDist = 6; //no jogo os canos comecam em x = 6, 12 e 18
    [Range(0, 1)] [SerializeField] float movelChance = 0.5f;

    public void Configurar(FlapTrainingPipes[] _canos)
    {
        canos = _canos;
    }

    //chamado pelo agente no inicio de cada episodio
    public void Reiniciar()
    {
        bool _movel = Random.value < movelChance;

        for (int i = 0; i < canos.Length; i++)
        {
            canos[i].Reiniciar(canosDist * (i + 1), _movel);
        }
    }
}
