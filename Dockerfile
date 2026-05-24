# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 80
EXPOSE 445


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./ZucoHR/ZucoHR.csproj", "ZucoHR/"]
COPY ["./ZucoHR.Application/ZucoHR.Application.csproj", "ZucoHR.Application/"]
COPY ["./ZucoHR.Infrastructure/ZucoHR.Infrastructure.csproj", "ZucoHR.Infrastructure/"]
COPY ["./ZucoHR.Domain/ZucoHR.Domain.csproj", "ZucoHR.Domain/"]
COPY ["./ZucoHR.Shared/ZucoHR.Shared.csproj", "ZucoHR.Shared/"]
RUN dotnet restore "./ZucoHR/ZucoHR.csproj"
COPY . .
WORKDIR "/src/ZucoHR"
RUN dotnet build "./ZucoHR.csproj" -c %BUILD_CONFIGURATION% -o /app/build 

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ZucoHR.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZucoHR.dll"]