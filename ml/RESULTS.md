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

| Run | Data | Passos | Tempo | Configuracao | Recompensas | Resultado |
|---|---|---|---|---|---|---|
| Infinity01 | 2026-11-16 | 400 mil (interrompido; previsto 1 M) | ~11 min | 3 raios (um por faixa) + 5 obs, decisao a cada 5 steps, 1a fileira a 30m | +0.02 por metro, bater -1 | nao aprendeu: recompensa de -0.40 a -0.30, morre na 1a ou 2a fileira (~32m) |
| Infinity02 | 2026-11-16 | ~100 mil (interrompido; previsto 1 M) | - | igual, com a 1a fileira a 15m | +0.1 por metro, bater -1 | nao aprendeu, igual ao Infinity01 |
| Infinity03 | 2026-11-17 | 1 M | ~26 min | acao de faixa relativa (fica / uma para a esquerda / uma para a direita), troca de faixa na hora, uma acao por decisao | +0.1 por metro, bater -1 | aprendeu: recompensa de 0,85 para 11,5, ainda subindo no fim |
| Infinity04 | 2026-11-17 | 1 M (partindo dos pesos do Infinity03) | ~23 min | igual ao Infinity03 | +0.1 por metro, bater -1 | recompensa de ~12 para ~50 a 57 (pico 66), oscilando no fim |

Medicao do Infinity03 (8 pistas da `InfinityTraining`, modelo em inferencia): media de 116 m por tentativa em
582 tentativas, maximo 807 m, nenhuma chegada aos 1000 m (`_distanceToWin`).

Medicao do Infinity04 (mesma cena): media de 573 m por tentativa em 225 tentativas, 65 chegadas aos 1000 m (29%).
Ele pula muito: 16.413 pulos em 128.847 m, cerca de um pulo a cada 8 m (quase um por fileira).

Modelo atual em `Assets/_ME/Models/Infinity.onnx`: Infinity04 (final).

Diferenca para o jogo: na `Scene_InfinityGame` a 1a fileira aparece a 40 m da largada; no treino ela fica a 15 m.

Leitura: o que destravou foi a acao de faixa. Com faixa absoluta (0/1/2) a politica aleatoria do inicio ficava
oscilando em volta do meio e morria sempre na 1a fileira. A curva nao estabilizou em 1 M, entao mais passos
devem melhorar.
