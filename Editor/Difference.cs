#if UNITY_EDITOR_WIN

using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEditor;

namespace Knit
{
	static partial class External
	{
		enum MenuPriority
		{
			Default = 9,
			LogRoot,
			LogCurrent,
			LogSelection,
			CommitCompareRoot,
			CommitCompareCurrent,
			RepoStatusRoot,
			RepoStatusCurrent,
			AssetCompare,
		}
		[MenuItem( "Assets/Difference/Commit Log/Root", true, (int)MenuPriority.LogRoot)]
		static bool IsValidateLogRoot()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Commit Log/Root", false, (int)MenuPriority.LogRoot)]
		static void ExecuteLogRoot()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = "/command:log",
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = GetRepositoryDirectory()
			});
		}
		[MenuItem( "Assets/Difference/Commit Log/Current", true, (int)MenuPriority.LogCurrent)]
		static bool IsValidateLogCurrent()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Commit Log/Current", false, (int)MenuPriority.LogCurrent)]
		static void ExecuteLogCurrent()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = $"/command:log /path:\"{GetProjectDirectory()}\"",
				CreateNoWindow = true,
				UseShellExecute = false,
			});
		}
		[MenuItem( "Assets/Difference/Commit Log/Selection", true, (int)MenuPriority.LogSelection)]
		static bool IsValidateLogSelection()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false && Selection.assetGUIDs.Length == 1;
		}
		[MenuItem( "Assets/Difference/Commit Log/Selection", false, (int)MenuPriority.LogSelection)]
		static void ExecuteLogSelection()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = "/command:log /path:\"" + 
					Path.GetFullPath( AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ 0])).Replace( @"\", "/") + "\"",
				CreateNoWindow = true,
				UseShellExecute = false,
			});
		}
		[MenuItem( "Assets/Difference/Commit Compare/Root", true, (int)MenuPriority.CommitCompareRoot)]
		static bool IsValidateCommitCompareRoot()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Commit Compare/Root", false, (int)MenuPriority.CommitCompareRoot)]
		static void ExecuteCommitCompareRoot()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = "/command:showcompare /revision1:HEAD /revision2:HEAD~ /alternative",
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = GetRepositoryDirectory()
			});
		}
		[MenuItem( "Assets/Difference/Commit Compare/Current", true, (int)MenuPriority.CommitCompareCurrent)]
		static bool IsValidateCommitCompareCurrent()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Commit Compare/Current", false, (int)MenuPriority.CommitCompareCurrent)]
		static void ExecuteCommitCompareCurrent()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = $"/command:showcompare /path:\"{GetProjectDirectory()}\" /revision1:HEAD /revision2:HEAD~ /alternative",
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = GetRepositoryDirectory()
			});
		}
		[MenuItem( "Assets/Difference/Check For Modifications/Root", true, (int)MenuPriority.RepoStatusRoot)]
		static bool IsValidateRepoStatusRoot()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Check For Modifications/Root", false, (int)MenuPriority.RepoStatusRoot)]
		static void ExecuteRepoStatusRoot()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = "/command:repostatus",
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = GetRepositoryDirectory()
			});
		}
		[MenuItem( "Assets/Difference/Check For Modifications/Current", true, (int)MenuPriority.RepoStatusCurrent)]
		static bool IsValidateRepoStatusCurrent()
		{
			return string.IsNullOrEmpty( s_TortoiseGitPath) == false;
		}
		[MenuItem( "Assets/Difference/Check For Modifications/Current", false, (int)MenuPriority.RepoStatusCurrent)]
		static void RepoStatusCurrent()
		{
			Process.Start( new ProcessStartInfo
			{
				FileName = s_TortoiseGitPath,
				Arguments = $"/command:repostatus /path:\"{GetProjectDirectory()}\"",
				CreateNoWindow = true,
				UseShellExecute = false,
			});
		}
		[MenuItem( "Assets/Difference/Asset Compare", true, (int)MenuPriority.AssetCompare)]
		static bool IsValidateAssetCompare()
		{
			return AvailableWinMerge() != false && (Selection.assetGUIDs.Length & 0x7ffffffe) == 2;
		}
		[MenuItem( "Assets/Difference/Asset Compare", false, (int)MenuPriority.AssetCompare)]
		static void ExecuteAssetCompare()
		{
			WinMerge( Selection.assetGUIDs.Select( x => $"\"{AssetDatabase.GUIDToAssetPath( x)}\""));
		}
	}
}
#endif