$location = Get-Location;
Set-Location $PSScriptRoot;
if ($args.Count -lt 2) {
	Write-Output "Too few arguments";
}
elseif ($args.Count -gt 2) {
	Write-Output "Too many arguments";
}
else {
	$args[1] = $args[1].ToLower();
	$projects = [string[]](Get-ChildItem "*/*.csproj" -File -Name);
	$project = "$($args[0]).csproj";
	if (!$projects.Contains($project)) {
		Write-Output "$project is not a valid project name";
	}
	elseif (!('major', 'minor', 'patch', 'overwrite').Contains($args[1])) {
		Write-Output "${args[1]} is not a valid version component";
	}
	else {
		if ($args[1] -ne 'overwrite') {
			$projectFile = [string]((Get-ChildItem "*/$project")[0]);
			$project = New-Object xml;
			$project.PreserveWhitespace = $true;
			$project.Load($projectFile);
			$versionNode = $project.SelectSingleNode("/Project/PropertyGroup/Version");
			$curVersion = [string]$versionNode.InnerText;
			$components = [int[]]($curVersion.Split('.'));
			switch ($args[1]) {
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
		Invoke-Expression "dotnet pack $($args[0]) -c Release";
	}
}
Set-Location $location;