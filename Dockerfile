ARG PARENT_VERSION=1.5.0-dotnet6.0

# Development
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

ARG PARENT_VERSION

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p /home/dotnet/EST.MIT.Web/ /home/dotnet/EST.MIT.Web.Test/ 

COPY --chown=dotnet:dotnet ./EST.MIT.Web/*.csproj ./EST.MIT.Web/
RUN dotnet restore ./EST.MIT.Web/EST.MIT.Web.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Web.Test/*.csproj ./EST.MIT.Web.Test/
RUN dotnet restore ./EST.MIT.Web.Test/EST.MIT.Web.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Web/ ./EST.MIT.Web/
COPY --chown=dotnet:dotnet ./EST.MIT.Web.Test/ ./EST.MIT.Web.Test/

RUN dotnet publish ./EST.MIT.Web/ -c Release -o /home/dotnet/out

ARG PORT=3000
ENV PORT ${PORT}
EXPOSE ${PORT}

CMD dotnet watch --project /home/dotnet/EST.MIT.Web/ run --urls "http://*:${PORT}"

# Production
FROM defradigital/dotnetcore:$PARENT_VERSION AS production

ARG PARENT_VERSION
ARG PARENT_REGISTRY

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

ARG PORT=3000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

COPY --from=development /home/dotnet/out/ ./

ENTRYPOINT ["dotnet", "EST.MIT.Web.dll"]
