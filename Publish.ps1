function Publish-Package([string] $project, [string] $version) {
	if (!(Test-Path "$project/*.csproj")) {
		return "Project $project not found";
	}
	$csprojPath = Get-ChildItem "$project/*.csproj" -File -Name;
	$structure = [xml](Get-Content "$project/$csprojPath");
	$packageId = $structure.Project.PropertyGroup.PackageId;
	Set-Location "$project/bin/Release";
	if (!$version) {
		$pkgs = [string[]](Get-ChildItem "$packageId.*.nupkg" -File -Name);
		foreach ($pkg in $pkgs) {
			Invoke-Expression "dotnet nuget push `"$pkg`" --source github-personal"
		}
	}
	else {
		if ($version -eq 'latest') {
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
			Invoke-Expression "dotnet nuget push `"$packageId.$major.$minor.$patch.nupkg`" --source github-personal";
		}
		else {
			if (!($version -match '\d+\.\d+\.\d+')) {
				return "Wrong version syntax";
			}
			$package = "$packageId.$version.nupkg";
			if (!(Test-Path $package)) {
				return "$package not found";
			}
			Invoke-Expression "dotnet nuget push `"$package`" --source github-personal";
		}
	}
	Set-Location "../../../";
	return $true;
}
$location = Get-Location;
Set-Location $PSScriptRoot;
if ($args.Count -eq 0) {
	$projects = [string[]](Get-ChildItem -Directory -Name);
	foreach ($project in $projects) {
		Publish-Package $project;
	}
}
else {
	foreach ($project in $args) {
		$separator = $project.IndexOf('@');
		if ($separator -eq -1) {
			Publish-Package $project;
		}
		else {
			$version = $project.Substring($separator + 1);
			$project = $project.Substring(0, $separator);
			Publish-Package $project $version;
		}
	}
}
Set-Location $location;