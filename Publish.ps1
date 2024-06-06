$NUGET_SOURCE = 'GitHub-truemogician'

function Publish-Package {
	param(
		[Parameter(Mandatory, Position = 0)]
		[string]$Project,
		[Parameter(Position = 1)]
		[ValidatePattern('^(?:latest|\d+\.\d+\.\d+)$', Options = 'IgnoreCase')]
		[string]$Version
	)

	if (-not (Test-Path "$Project/*.csproj" -ErrorAction SilentlyContinue)) {
		throw New-Object -TypeName System.IO.FileNotFoundException -ArgumentList "Project $project not found.";
	}
	$csprojPath = Get-ChildItem "$Project/*.csproj" -File -Name;
	$structure = [xml](Get-Content "$Project/$csprojPath");
	$packageId = $structure.SelectSingleNode("/Project/PropertyGroup/PackageId").InnerText;
	Set-Location "$Project/bin/Release";
	if (-not $Version) {
		$pkgs = [string[]](Get-ChildItem "$packageId.*.nupkg" -File -Name);
		foreach ($pkg in $pkgs) {
			dotnet nuget push $pkg --source $NUGET_SOURCE;
		}
	}
	elseif ($Version.ToLower() -eq 'latest') {
		$pkgs = [string[]](Get-ChildItem "$packageId.*.nupkg" -File -Name);
		$major = 0;
		$minor = 0;
		$patch = 0;
		foreach ($pkg in $pkgs) {
			$match = [Regex]::Match($pkg, "$packageId\.(?<major>\d+)\.(?<minor>\d+)\.(?<fix>\d+)\.nupkg");
			if ($match.Success -eq $false) {
				continue;
			}
			$curMajor = [int]($match.Groups['major'].Value);
			$curMinor = [int]($match.Groups['minor'].Value);
			$curPatch = [int]($match.Groups['fix'].Value);
			if ($curMajor -gt $major -or ($curMajor -eq $major -and ($curMinor -gt $minor -or ($curMinor -eq $minor -and $curPatch -gt $patch)))) {
				$major = $curMajor;
				$minor = $curMinor;
				$patch = $curPatch;
			}
		}
		dotnet nuget push "`"$packageId.$major.$minor.$patch.nupkg`"" --source $NUGET_SOURCE;
	}
	else {
		$package = "$packageId.$Version.nupkg";
		if (-not (Test-Path $package -ErrorAction SilentlyContinue)) {
			throw New-Object -TypeName System.IO.FileNotFoundException -ArgumentList "Package not found: $package";
		}
		dotnet nuget push $package --source $NUGET_SOURCE;
	}
	Set-Location "../../../";
}

$location = Get-Location;
Set-Location $PSScriptRoot;
if ($args.Count -eq 0) {
	$projects = [string[]](Get-ChildItem -Directory -Name);
	foreach ($project in $projects) {
		Publish-Package -Project $project;
	}
}
else {
	foreach ($project in $args) {
		$separator = $project.IndexOf('@');
		if ($separator -eq -1) {
			Publish-Package -Project $project;
		}
		else {
			$version = $project.Substring($separator + 1);
			$project = $project.Substring(0, $separator);
			Publish-Package -Project $project -Version $version;
		}
	}
}
Set-Location $location;