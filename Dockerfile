# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=8.0

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /src

COPY teamseven.EzExam.sln .
COPY teamseven.EzExam.API/teamseven.EzExam.API.csproj teamseven.EzExam.API/
COPY teamseven.EzExam.Repository/teamseven.EzExam.Repository.csproj teamseven.EzExam.Repository/
COPY teamseven.EzExam.Services/teamseven.EzExam.Services.csproj teamseven.EzExam.Services/

# Restore 
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore "teamseven.EzExam.API/teamseven.EzExam.API.csproj"

# Copy
COPY . .

# Build and publish
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache \
    dotnet publish "teamseven.EzExam.API/teamseven.EzExam.API.csproj" -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Security 
RUN addgroup --system ezexam && adduser --system --ingroup ezexam ezexamuser
USER ezexamuser

# Copy published output
COPY --from=builder /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "teamseven.EzExam.API.dll"]
