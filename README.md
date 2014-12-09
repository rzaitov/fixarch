Csproj util
===========

This util will set correct arch for each configuration:
* (Debug|Release) iPhoneSimulator will use `i386` arch only
* (Release|AddHoc|AppStore) iPhone will use fat arch `armv7+arm64`
* Debug iPhone will use `armv7` only

Instructions
------------

Build the solution first. You will get `Csproj.exe` inside your output dir. Run util as follow:  
`$ path/to/util/Csproj.exe path/to/MyCoolApp.csproj`

You can run it on multiple projects via `find` and `xargs` as follow:
```bash
cd path/to/repository
find . -name '*.csproj' -print0 | xargs -0 -L 1 mono path/to/util/Csproj.exe
```
