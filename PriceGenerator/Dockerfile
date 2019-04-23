FROM mcr.microsoft.com/dotnet/core/sdk:2.2.203 as build
WORKDIR /code

COPY . .
RUN dotnet restore
RUN dotnet publish -o output -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2 as runtime
WORKDIR /app
COPY --from=build /code/output ./

ENTRYPOINT [ "dotnet", "PriceGenerator.dll" ]