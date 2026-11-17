# Plano: ML-Agents no TCC (treino no Editor, inferência na build WebGL)

> Documento pensado para ser entregue a outra sessão/modelo. É autocontido.

## 1. Contexto

- Projeto Unity **6000.3.8f1** (era 2020.3.37f1) em `F:\Projetos\Unity\TCC`. Três minigames: **Flap**, **Infinity** (runner) e **Pong**.
- Scripts em `Assets/_ME/Scripts/<Jogo>/`. Cenas em `Assets/_ME/Scenes/`.
- Já existem IAs **reativas escritas à mão**: `Script_IaFlap`, `Script_IaPong`, `Script_IaInfinity`. Elas **devem ser mantidas** como baseline de comparação no TCC.
- Física **3D** (`Rigidbody`, `Collision`/`Trigger`) nos três jogos. Hoje a morte/fim faz `Time.timeScale = 0` (Flap e Pong).
- Nomes de código e comentários em português, sem acentos nos identificadores.
- Meta: **treinar no Editor** e **rodar só a inferência (`.onnx`) na build WebGL**.

## 2. Restrições técnicas (não violar)

> **Atualizado em 2026-11-16:** o projeto foi migrado para a **Unity 6000.3.8f1** (commit `5366f7b`), a mesma do Kinho Kart.
> Por isso as versões abaixo substituem as do plano original (Unity 2020.3 + ML-Agents 2.x + Barracuda).

- Pacote **`com.unity.ml-agents` 4.2.0** (traz o `com.unity.ai.inference` 2.6.1, ex-Sentis, que roda a inferência).
- Python **3.10** no venv `ml/venv`, versões fixadas em `ml/requirements.txt` (`mlagents==1.1.0`, `torch==2.2.2`, `setuptools<81`). Treino fora do navegador.
  - Recriar o venv: `uv venv ml/venv --python 3.10` e `uv pip install --system-certs --python ml/venv/Scripts/python.exe -r ml/requirements.txt`.
- WebGL: manter a inferência leve.
  - Rede pequena (2 camadas x 64).
  - **Sem observações visuais.** Só vetor + `RayPerceptionSensor`.
  - **1 agente ativo** na cena final.
- O pacote ML-Agents **fica na build** (é o runtime de inferência).
- Avast intercepta HTTPS: no `uv`, usar `--system-certs`; se o Package Manager falhar com erro de certificado, desligar o Web Shield temporariamente.
- Cenas de treino: menu **TCC > ML-Agents > Criar cena de treino** (scripts em `Assets/_ME/Editor/`). Treinar com `ml\treinar.bat <jogo> <run-id>`.

## 3. Fases

### Fase 0 — Preparação
1. Criar branch `feature/mlagents`.
2. Instalar `com.unity.ml-agents` no `Packages/manifest.json` (versão da seção 2).
3. Criar venv Python e instalar `mlagents==0.30.0`. Validar com `mlagents-learn --help`.
4. Criar `Assets/_ME/Models/` (onde ficam os `.onnx`) e `ml/` na raiz do repo (onde ficam os `config_*.yaml`).
5. Conferir `.gitignore`: ignorar `results/`, `venv/`; **versionar** os `.onnx` copiados para `Assets/_ME/Models/`.
- **Pronto quando:** projeto compila sem erros e `mlagents-learn --help` roda.

### Fase 1 — Pong (primeiro, é o mais simples)
1. Criar `AgentPong : Agent` (novo arquivo; não editar `Script_IaPong`).
2. Observações (vetor): posição Z da raquete, posição e velocidade da bola, energia normalizada, flag de bola em direção ao gol. Normalizar tudo.
3. Ação: 1 discreta com 3 ramos (parado, cima, baixo). Respeitar limites de Z (±3.5) e o consumo/recarga de energia como no script atual.
4. Recompensas: `+1` ao rebater a bola, `-1` ao tomar gol, pequena penalidade por existir (`-0.0005/step`) opcional.
5. Fim de episódio: gol sofrido ou limite de steps. `EndEpisode()` + reset da bola e da raquete. **Sem `Time.timeScale = 0` durante o treino.**
6. Implementar `Heuristic()` com o teclado (para testar o agente à mão).
7. Criar cena/prefab de treino com **várias arenas duplicadas** para acelerar.
8. `ml/config_pong.yaml` (PPO, `hidden_units: 64`, `num_layers: 2`, `max_steps` ~ 500k a 2M, ajustar).
9. Treinar: `mlagents-learn ml/config_pong.yaml --run-id=Pong01` e dar Play no Editor.
10. Copiar `results/Pong01/<Behavior>.onnx` para `Assets/_ME/Models/` e atribuir em `Behavior Parameters` (Inference Only).
- **Pronto quando:** o agente treinado rebate a bola de forma consistente e vence ou empata com a IA reativa em N partidas medidas.

### Fase 2 — Flap
1. `AgentFlap : Agent`, mesmo padrão. Observações: altura Y, velocidade vertical, distância X e altura Y do próximo cano, distância ao chão.
2. Ação: discreta (nada / pular), respeitando o cooldown de pulo.
3. Recompensas: `+1` ao passar pelo cano (trigger `Ponto`), `-1` ao bater (`Cano`/`Chao`), pequena recompensa por sobreviver.
4. Reset: reposicionar o pássaro e reiniciar os canos (ver `Script_FlapManager`, `Script_canosTudo`). Tratar canos móveis (`movel`) como variação de dificuldade.
5. Config, treino, exportação e validação como na Fase 1.

### Fase 3 — Infinity
1. `AgentInfinity : Agent`. Trocar os raycasts manuais por **`RayPerceptionSensorComponent3D`** (layer 3 = obstáculos, tag `Carro`), mais velocidade e estado de pulo.
2. Ação: ramo de faixa (esquerda/centro/direita) + ramo de pulo.
3. Recompensas: `+` por distância percorrida em Z, `-1` ao colidir.
4. Reset: limpar objetos gerados (ver `Script_InfinityManager`, `Script_objDestruir*`).
5. Este é o mais difícil. Fazer por último.

### Fase 4 — Integração no jogo e UI
1. Opção de escolher quem joga: **Humano / IA reativa / IA treinada (ML-Agents)**.
2. Trocar `Behavior Type` por script (Heuristic para humano, Inference para IA treinada). Garantir que a IA treinada só roda em modo inferência na build.
3. Um agente ativo por cena nas cenas de jogo finais.

### Fase 5 — Build WebGL
1. Build WebGL com o pacote e os `.onnx` incluídos.
2. Testar no navegador: FPS estável, agente se comporta como no Editor.
3. Se o FPS cair: reduzir a rede, reduzir a frequência de decisão (`Decision Requester` com `DecisionPeriod` maior) ou diminuir os raycasts.
- **Pronto quando:** a build web roda o agente treinado sem Python e sem travar.

## 4. Regras de trabalho para quem executar

- Não apagar nem alterar o comportamento das IAs reativas existentes.
- Um agente/jogo por vez, com commit ao fim de cada fase.
- Não commitar `results/`, `venv/` nem `Library/`.
- Mudou observações ou ações depois de treinar? **Retreinar** (o `.onnx` antigo fica inválido).
- Registrar em `ml/RESULTADOS.md`: run-id, config, passos treinados e métrica final (útil para o texto do TCC).

## 5. Riscos

| Risco | Mitigação |
|---|---|
| Incompatibilidade de versões Unity / ML-Agents / Python | Fixar versões da seção 2 |
| Treino não converge | Simplificar observações, ajustar recompensas, curriculum de dificuldade |
| FPS baixo no WebGL | Rede pequena, `DecisionPeriod` maior, 1 agente |
| `Time.timeScale = 0` quebra o treino | Usar `EndEpisode()` e reset |
| Certificado do Avast no download de pacotes | Desligar Web Shield temporariamente |
