param(
	[Parameter(Mandatory, Position = 0)]
	[string]$ProjectName,
	[Parameter(Mandatory, Position = 1)]
	[ValidateSet("Major", "Minor", "Patch", "Overwrite", IgnoreCase)]
	[string]$VersionUpdate
)

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
	throw New-Object -TypeName System.IO.FileNotFoundException -ArgumentList "dotnet executable not found.";
}

$location = Get-Location;
Set-Location $PSScriptRoot;

$VersionUpdate = $VersionUpdate.ToLower();
$projects = [string[]](Get-ChildItem "*/*.csproj" -File -Name);
$project = "$ProjectName.csproj";
if (!$projects.Contains($project)) {
	throw New-Object -TypeName System.IO.FileNotFoundException -ArgumentList "$project is not a valid project name.";
}
if ($VersionUpdate -ne 'overwrite') {
	$projectFile = [string]((Get-ChildItem "*/$project")[0]);
	$project = New-Object xml;
	$project.PreserveWhitespace = $true;
	$project.Load($projectFile);
	$versionNode = $project.SelectSingleNode("/Project/PropertyGroup/Version");
	$curVersion = [string]$versionNode.InnerText;
	$components = [int[]]($curVersion.Split('.'));
	switch ($VersionUpdate) {
		'major' {
			++$components[0];
			$components[1] = $components[2] = 0;
		}
		'minor' {
			++$components[1];
			$components[2] = 0;
		}
		'patch' {
			++$components[2];
		}
	}
	$versionNode.InnerText = $components -join '.';
	$project.Save($projectFile);
}

dotnet pack $ProjectName -c Release;

Set-Location $location;