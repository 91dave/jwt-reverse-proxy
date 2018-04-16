FROM microsoft/dotnet:2-sdk AS builder
COPY Jwt.Proxy.sln /code/
COPY Jwt.Proxy /code/Jwt.Proxy
COPY Jwt.Proxy.Website/ /code/Jwt.Proxy.Website
WORKDIR /code/Jwt.Proxy.Website
RUN dotnet restore
RUN dotnet publish -o /app


FROM microsoft/aspnetcore:2
COPY --from=builder /app /app
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5001
CMD [ "dotnet", "Jwt.Proxy.Website.dll" ]
EXPOSE 5001
