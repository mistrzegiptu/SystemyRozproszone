@echo off
echo Starting Space System Message Broker Architecture...

start "Administrator" cmd /k "title Administrator && cd SpaceSystem.Admin && dotnet run"

start "Carrier 1" cmd /k "title Carrier 1 && cd SpaceSystem.Carrier && dotnet run"
start "Carrier 2" cmd /k "title Carrier 2 && cd SpaceSystem.Carrier && dotnet run"

start "Agency 1" cmd /k "title Agency 1 && cd SpaceSystem.Agency && dotnet run"
start "Agency 2" cmd /k "title Agency 2 && cd SpaceSystem.Agency && dotnet run"

pause