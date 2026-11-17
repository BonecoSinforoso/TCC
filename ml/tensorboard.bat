@echo off
rem Graficos dos treinos (TensorBoard). Abra http://localhost:6006 no navegador.
cd /d "%~dp0.."
"ml\venv\Scripts\tensorboard.exe" --logdir ml\results
pause
