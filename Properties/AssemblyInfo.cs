using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(NEP.Hitmarkers.BuildInfo.Description)]
[assembly: AssemblyDescription(NEP.Hitmarkers.BuildInfo.Description)]
[assembly: AssemblyCompany(NEP.Hitmarkers.BuildInfo.Company)]
[assembly: AssemblyProduct(NEP.Hitmarkers.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NEP.Hitmarkers.BuildInfo.Author)]
[assembly: AssemblyTrademark(NEP.Hitmarkers.BuildInfo.Company)]
[assembly: AssemblyVersion(NEP.Hitmarkers.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NEP.Hitmarkers.BuildInfo.Version)]
[assembly: MelonInfo(typeof(NEP.Hitmarkers.Main), NEP.Hitmarkers.BuildInfo.Name, NEP.Hitmarkers.BuildInfo.Version, NEP.Hitmarkers.BuildInfo.Author, NEP.Hitmarkers.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]