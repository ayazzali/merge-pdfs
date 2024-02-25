FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MergePdf/MergePdf.csproj", "MergePdf/"]
RUN dotnet restore "MergePdf/MergePdf.csproj"
COPY . .
WORKDIR "/src/MergePdf"
RUN dotnet build "MergePdf.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MergePdf.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MergePdf.dll"]
