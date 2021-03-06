#!/bin/bash

if [ ! -f .paket/paket.exe ]; then
    echo "run bootstrapper"
    mono .paket/paket.bootstrapper.exe
fi

if [ ! -f .paket/nuget.exe ]; then
	echo "download nuget"
	wget -P .paket https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
fi

if [ ! -f paket.lock ]; then
	mono .paket/paket.exe install
fi

if [ -f boot.fsx ]; then
	mono .paket/paket.exe restore
        mono .paket/nuget.exe install FAKE -ExcludeVersion -OutputDirectory .paket -NonInteractive -Verbosity quiet
        mono .paket/FAKE/tools/FAKE.exe --removeLegacyFakeWarning run boot.fsx
	rm boot.fsx
	mono .paket/paket.exe install
fi


mono packages/build/FAKE/tools/FAKE.exe "build.fsx" Dummy --fsiargs build.fsx $@
