FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FireHurdaTakip.csproj", "./"]
RUN dotnet restore "FireHurdaTakip.csproj"

COPY . .
RUN dotnet publish "FireHurdaTakip.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FireHurdaTakip.dll"]
