using UnityEngine;
using UnityEngine.Serialization;

namespace TCC
{
    // Arena do Flap usada so no treino: reinicia os canos e sorteia se eles se movem.
    public class FlapTrainingArena : MonoBehaviour
    {
        #region membros

        [FormerlySerializedAs("canos")]
        [SerializeField] private FlapTrainingPipes[] _pipes;
        [FormerlySerializedAs("canosDist")]
        [SerializeField] private float _pipeDistance = 6; //no jogo os canos comecam em x = 6, 12 e 18
        [FormerlySerializedAs("movelChance")]
        [Range(0, 1)][SerializeField] private float _movableChance = 0.5f;

        #endregion

        #region metodos

        public void Configure(FlapTrainingPipes[] newPipes)
        {
            _pipes = newPipes;
        }

        //chamado pelo agente no inicio de cada episodio
        public void Restart()
        {
            bool movable = Random.value < _movableChance;

            for (int i = 0; i < _pipes.Length; i++)
            {
                _pipes[i].Restart(_pipeDistance * (i + 1), movable);
            }
        }

        #endregion
    }
}
