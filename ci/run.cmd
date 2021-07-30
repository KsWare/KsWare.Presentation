cd ..
:: add msbuild path
path=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\bin;%path%

:: add nunit path
path=C:\Program Files (x86)\NUnit.org\nunit-console;%path%

nuget restore src

::msbuild "src\KsWare.Presentation.sln" /verbosity:minimal /p:GeneratePackageOnBuild=false
msbuild "src\KsWare.Presentation.sln" /verbosity:minimal /p:GeneratePackageOnBuild=false /property:Configuration=Release

nunit3-console "src\KsWare.Presentation.BusinessFramework.Tests\bin\Release\net45\KsWare.Presentation.BusinessFramework.Tests.dll" "src\KsWare.Presentation.Core.Tests\bin\Release\net45\KsWare.Presentation.Core.Tests.dll" "src\KsWare.Presentation.Labs.Tests\bin\Release\net45\KsWare.Presentation.Labs.Tests.dll" "src\KsWare.Presentation.Testing.Tests\bin\Release\net45\KsWare.Presentation.Testing.Tests.dll" "src\KsWare.Presentation.Testing.Tests\bin\Release\net45\KsWare.Presentation.Tests.dll" "src\KsWare.Presentation.Tests\bin\Release\net45\KsWare.Presentation.Tests.dll" "src\KsWare.Presentation.ViewFramework.Tests\bin\Release\net45\KsWare.Presentation.ViewFramework.Tests.dll" "src\KsWare.Presentation.ViewModelFramework.Tests\bin\Release\net45\KsWare.Presentation.ViewModelFramework.Tests.dll" --where "cat != LocalOnly"

echo Running .NET Core tests...
dotnet test "src\KsWare.Presentation.Tests\KsWare.Presentation.Tests.csproj" --configuration Release --no-build --filter "TestCategory!=LocalOnly" /property:Platform=AnyCPU

::dotnet pack "C:\projects\ksware-presentation\src\KsWare.Presentation\KsWare.Presentation.csproj" --configuration Release --include-symbols --output "C:\Users\appveyor\AppData\Local\Temp\1\dto7ifl5jd" --no-build

dotnet pack "src\KsWare.Presentation\KsWare.Presentation.csproj" --configuration Release --include-symbols --output "ci\temp" --no-build
 Die Eigenschaft "NoBuild" wurde auf TRUE festgelegt, aber das Ziel "Build" wurde aufgerufen.
pause