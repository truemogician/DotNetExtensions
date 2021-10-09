function Publish-Package([string] $project, [string] $version) {
	if (!(Test-Path "$project/*.csproj")) {
		return "Project $project not found";
	}
	if (!$version) {
		Set-Location "$project/bin/Release";
		Invoke-Expression "dotnet nuget push `"*.nupkg`" --source github-personal --skip-duplicate";
		Set-Location "../../../";
	}
	else {
		if (!($version -match '\d+\.\d+\.\d+')) {
			return "Wrong version syntax";
		}
		$csprojPath = Get-ChildItem "$project/*.csproj" -File -Name;
		$structure = [xml](Get-Content "$project/$csprojPath");
		$packageId = $structure.Project.PropertyGroup.PackageId;
		$packagePath = "$project/bin/Release/$packageId.$version.nupkg";
		if (!(Test-Path $packagePath)) {
			return "$packagePath not found";
		}
		Invoke-Expression "dotnet nuget push `"$packagePath`" --source github-personal";
	}
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