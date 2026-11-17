# Resultados dos treinos (ML-Agents)

Ambiente: Unity 6000.3.8f1, `com.unity.ml-agents` 4.2.0, `mlagents` 1.1.0, PPO, rede 2 x 64, `time_scale` 20.
"Placar" = gols do agente x gols do adversario reativo de treino (`PongTrainingOpponent`, copia do `Script_IaPong`),
somados nas 12 mesas da `PongTraining` com o modelo em inferencia. Ainda nao foi medido contra o `Script_IaPong`
original na cena do jogo.

## Pong

| Run | Data | Passos | Tempo | Observacoes / decisao | Recompensas | Recompensa media final | Placar (agente x reativo) |
|---|---|---|---|---|---|---|---|
| Pong01 | 2026-11-16 | 800 mil (interrompido; previsto 1 M) | ~7 min | 8 obs, decisao a cada 5 steps | rebater +1, gol sofrido -1, gol feito +1, -0.0005/step | ~4 a 5 (estavel desde ~300 mil) | 88 x 273 (24% dos gols) |
| Pong02 | 2026-11-16 | 1,5 M | ~10 min | 9 obs (+ ponto previsto de chegada da bola), decisao a cada 2 steps | rebater +1, gol sofrido -1, gol feito +1, sem penalidade por step | ~8 a 12 (estavel desde ~900 mil) | 146 x 307 (32% dos gols) |

Modelo atual em `Assets/_ME/Models/Pong.onnx`: Pong02 (final).

Leitura: nos dois treinos a recompensa para de subir cedo, mas o agente estabiliza num nivel em que ainda perde.
Ele aprende a rebater (a recompensa vem quase toda das rebatidas), nao a fazer gol.

## Flap

Medicao: 16 arenas da `FlapTraining` com o modelo em inferencia, metade das tentativas com canos moveis
(`movelChance` 0.5). So entram na conta as tentativas que terminaram em batida.

| Run | Data | Passos | Tempo | Observacoes / decisao | Recompensas | Recompensa media final | Canos por tentativa (inferencia) |
|---|---|---|---|---|---|---|---|
| Flap01 | 2026-11-16 | 1 M | ~7 min | 8 obs, decisao a cada 3 steps | cano +1, bater -1, +0.002 por decisao | ~30 a 50 (pico ~53 em 720 mil, oscilando) | media 19,7 em 133 tentativas, maximo 99 |

Modelo atual em `Assets/_ME/Models/Flap.onnx`: Flap01 (final). A meta do jogo eh 50 canos (`pontuacaoParaVencer`).

## Infinity

Ainda nao treinado.
