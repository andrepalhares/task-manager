FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskManager.slnx ./
COPY src/TaskManager.WebApi/TaskManager.WebApi.csproj src/TaskManager.WebApi/
COPY src/TaskManager.Application/TaskManager.Application.csproj src/TaskManager.Application/
COPY src/TaskManager.Domain/TaskManager.Domain.csproj src/TaskManager.Domain/
COPY src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj src/TaskManager.Infrastructure/
COPY tests/TaskManager.Application.Tests/TaskManager.Application.Tests.csproj tests/TaskManager.Application.Tests/
COPY tests/TaskManager.Domain.Tests/TaskManager.Domain.Tests.csproj tests/TaskManager.Domain.Tests/
COPY tests/TaskManager.Infrastructure.Tests/TaskManager.Infrastructure.Tests.csproj tests/TaskManager.Infrastructure.Tests/
COPY tests/TaskManager.WebApi.Tests/TaskManager.WebApi.Tests.csproj tests/TaskManager.WebApi.Tests/

RUN dotnet restore TaskManager.slnx

COPY . .
RUN dotnet publish src/TaskManager.WebApi/TaskManager.WebApi.csproj \
    --configuration Release \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

USER app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManager.WebApi.dll"]