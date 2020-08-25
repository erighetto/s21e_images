#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY S21eImages/S21eImages.csproj S21eImages/
RUN dotnet restore "S21eImages/S21eImages.csproj"
COPY . .
WORKDIR "/src/S21eImages"
RUN dotnet build "S21eImages.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "S21eImages.csproj" -c Release -o /app/publish

FROM base AS final
# Firefox browser to run the tests
RUN apt-get update && apt-get install -y firefox-esr wget
 
# Gecko Driver
ENV GECKODRIVER_VERSION 0.27.0
RUN wget --no-verbose -O /tmp/geckodriver.tar.gz https://github.com/mozilla/geckodriver/releases/download/v$GECKODRIVER_VERSION/geckodriver-v$GECKODRIVER_VERSION-linux64.tar.gz \
  && rm -rf /opt/geckodriver \
  && tar -C /opt -zxf /tmp/geckodriver.tar.gz \
  && rm /tmp/geckodriver.tar.gz \
  && mv /opt/geckodriver /opt/geckodriver-$GECKODRIVER_VERSION \
  && chmod 755 /opt/geckodriver-$GECKODRIVER_VERSION \
  && ln -fs /opt/geckodriver-$GECKODRIVER_VERSION /usr/bin/geckodriver \
  && ln -fs /opt/geckodriver-$GECKODRIVER_VERSION /usr/bin/wires \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/* \
  && rm -rf /var/cache/apt
  
WORKDIR /app
COPY --from=publish /app/publish .

COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

#ENTRYPOINT ["/docker-entrypoint.sh"]
ENTRYPOINT [ "dotnet", "S21eImages.dll", "scrape" ]
