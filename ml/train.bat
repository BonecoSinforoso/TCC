@echo off
rem Treino dos agentes do TCC (ML-Agents).
rem Uso: train.bat JOGO NOME_DO_TREINO      ex.: train.bat pong Pong01
rem   JOGO = pong | flap | infinity  (usa ml\config_JOGO.yaml)
rem Se o treino ja existir, continua de onde parou (--resume) automaticamente.
rem Depois de "Listening on port 5004", aperte Play na cena de treino do jogo.
rem Para parar sem perder progresso: Ctrl+C aqui, espere "Exported", e so depois pare o Play.
cd /d "%~dp0.."
set JOGO=%1
if "%JOGO%"=="" set JOGO=pong
set RUN=%2
if "%RUN%"=="" set RUN=%JOGO%01
set RESUME=
if exist "ml\results\%RUN%\run_logs\training_status.json" set RESUME=--resume
echo Treino: %JOGO% / %RUN% %RESUME%
"ml\venv\Scripts\mlagents-learn.exe" "ml\config_%JOGO%.yaml" --run-id=%RUN% --results-dir=ml\results %RESUME% %3 %4
pause
