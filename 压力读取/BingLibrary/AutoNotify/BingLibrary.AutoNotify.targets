<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <UsingTask TaskName="AutoTask" AssemblyFile="BingLibrary.AutoNotify.exe"/>
  <Target Name="AfterCompile" DependsOnTargets="CoreCompile">
    <AutoTask Assembly="@(IntermediateAssembly)" References="@(ReferencePath)" KeyFile="$(KeyOriginatorFile)"/>
  </Target>
</Project>