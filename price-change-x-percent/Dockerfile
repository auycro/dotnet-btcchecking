FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
EXPOSE 8080

# Copy Everything
FROM base AS build
#WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish --no-restore -c Release -o out

#FROM build as test
#WORKDIR /{unittest project}

# Build runtime image
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out .
COPY .env .
ENTRYPOINT ["dotnet", "price-change-x-percent.dll"]