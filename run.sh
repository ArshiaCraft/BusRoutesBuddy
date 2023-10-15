#!/bin/bash

function Build() {
  echo ""
  echo "  This is your first run of the project"
  echo "  Compiling..."
  echo ""
  dotnet build --configuration Release --nologo --verbosity q
  chmod +x ./TicketOffice/bin/Release/net6.0/TicketOffice
  cp -r  ./TicketOffice/wwwroot/ ./TicketOffice/bin/Release/net6.0/
  echo ""
  echo "  Project compiled."
  echo ""
}

function RenewDatabase() {
  echo ""
  echo "  Renewing the database..."
  echo ""
  dotnet-ef database drop -f --project ./TicketOffice/
  dotnet-ef database update --project ./TicketOffice/
  yes | cp ./TicketOffice/wwwroot/db/TicketOffice-SQLite.db ./TicketOffice/bin/Release/net6.0/wwwroot/db/
  echo ""
  echo "  Database renewed."
  echo ""
}

function Start() {
  echo ""
  echo "  Starting..."
  echo ""
  cd ./TicketOffice/bin/Release/net6.0/
  ./TicketOffice
}

function RunScript() {
  str=$(ls ./TicketOffice/bin | grep Release)

  if [ -z "$str" ]
  then
    Build
    echo ""
    echo "  Copying essential files..."
    echo ""
    cp ./TicketOffice/wwwroot/ ./TicketOffice/bin/Release/net6.0/ -r
    echo ""
    echo "  Files copied."
    echo ""
    RenewDatabase
    Start
  else
    Start
  fi
}

if [ $# = 0 ]
then
  RunScript
elif [ $1 = "--renew" ]
then
  RenewDatabase
  Start
  exit 0
elif [ $1 = "--rebuild" ]
then
  echo ""
  echo "  Deleting compiled project..."
  echo ""
  rm -Rf ./TicketOffice/bin/Release
  echo ""
  echo "  Deletion complete."
  echo ""
  Build
  Start
  exit 0
elif [ $1 = "--help" ]
then
  echo ""
  echo "  Available options:"
  echo "    --renew     renew the database"
  echo "    --rebuild   rebuild the project"
  echo ""
  exit 0
elif [ -n "$1" ]
then
  echo "$0: unrecognized option '$1'"
  echo "Try '$0 --help' for more information."
  exit 0
fi
