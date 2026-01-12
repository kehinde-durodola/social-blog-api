FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SocialBlogApi.csproj", "./"]
RUN dotnet restore "SocialBlogApi.csproj"
COPY . .
RUN dotnet publish "SocialBlogApi.csproj" -c Release -o /src/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /src/out .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "SocialBlogApi.dll"]
