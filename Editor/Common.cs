
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEditor;

#if NET_4_6
using Microsoft.Win32;
#endif

namespace Knit
{
	[InitializeOnLoad]
	public static partial class External
	{
		static External()
		{
			InitializeGit();
		#if UNITY_EDITOR_WIN
			InitializeTortoiseGit();
			InitializeWinMerge();
		#endif
		}
		static void InitializeGit()
		{
			var diff = Process.Start( new ProcessStartInfo
			{
				FileName = "git",
				Arguments = "version",
				CreateNoWindow = true,
				UseShellExecute = false,
			});
			diff.WaitForExit();
			
			if( diff.ExitCode == 0)
			{
				s_GitPath = "git";
				EditorApplication.update += UpdateBranch;
			}
		}
	#if UNITY_EDITOR_WIN
		static void InitializeTortoiseGit()
		{
			if( string.IsNullOrEmpty( s_GitPath) == false)
			{
				const string kTortoiseGitPath = @"TortoiseGit\bin\TortoiseGitProc.exe";
			#if NET_4_6
				if( string.IsNullOrEmpty( s_TortoiseGitPath) != false)
				{
					using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\TortoiseGit"))
					{
						string value = (string)key.GetValue( "ProcPath");
						
						if( File.Exists( value) != false)
						{
							s_TortoiseGitPath = value;
						}
					}
				}
			#endif
				if( string.IsNullOrEmpty( s_TortoiseGitPath) != false)
				{
					string value = null;
					try
					{
						value = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles), kTortoiseGitPath);
					}
					catch( Exception e)
					{
						UnityEngine.Debug.LogError( e);
					}
					if( File.Exists( value) != false)
					{
						s_TortoiseGitPath = value;
					}
				}
				if( string.IsNullOrEmpty( s_TortoiseGitPath) != false)
				{
					string value = null;
					try
					{
						value = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86), kTortoiseGitPath);
					}
					catch( Exception e)
					{
						UnityEngine.Debug.LogError( e);
					}
					if( File.Exists( value) != false)
					{
						s_TortoiseGitPath = value;
					}
				}
			}
		}
		static void InitializeWinMerge()
		{
			const string kWinMergePath = @"WinMerge\WinMergeU.exe";
		#if NET_4_6
			if( string.IsNullOrEmpty( s_TortoiseGitPath) != false)
			{
				using( RegistryKey key = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\Thingamahoochie\WinMerge"))
				{
					string value = (string)key.GetValue( "Executable");
					
					if( File.Exists( value) != false)
					{
						s_WinMergePath = value;
					}
				}
			}
		#endif
			if( string.IsNullOrEmpty( s_WinMergePath) != false)
			{
				string value = null;
				try
				{
					value = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles), kWinMergePath);
				}
				catch( Exception e)
				{
					UnityEngine.Debug.LogError( e);
				}
				if( File.Exists( value) != false)
				{
					s_WinMergePath = value;
				}
			}
			if( string.IsNullOrEmpty( s_WinMergePath) != false)
			{
				string value = null;
				try
				{
					value = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86), kWinMergePath);
				}
				catch( Exception e)
				{
					UnityEngine.Debug.LogError( e);
				}
				if( File.Exists( value) != false)
				{
					s_WinMergePath = value;
				}
			}
		}
	#endif
		static string Git( string arguments)
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = s_GitPath,
				Arguments = arguments,
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				StandardErrorEncoding = Encoding.UTF8,
				StandardOutputEncoding = Encoding.UTF8,
				UseShellExecute = false,
			};
			using( var process = Process.Start( startInfo))
			{
				process.WaitForExit();
				return process.StandardOutput.ReadToEnd();
			}
		}
		static string GetRepositoryDirectory()
		{
			return Git( "rev-parse --show-toplevel").Trim();
		}
		static string GetProjectDirectory()
		{
			const BindingFlags kFlags = 
				BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Static | BindingFlags.Instance;
			
			Type typeProjectBrowser = 
				Assembly.Load( "UnityEditor.dll")
				.GetType( "UnityEditor.ProjectBrowser");
			EditorWindow projectBrowserWindow = 
				EditorWindow.GetWindow( typeProjectBrowser);
			var directory = typeProjectBrowser
				.GetMethod( "GetActiveFolderPath", kFlags)
				.Invoke( projectBrowserWindow, null) as string;
			
			if( string.IsNullOrEmpty( directory) != false)
			{
				directory = Directory.GetCurrentDirectory();
			}
			return directory; 
		}
		public static bool AvailableWinMerge()
		{
			return string.IsNullOrEmpty( s_WinMergePath) == false;
		}
		public static void WinMerge( params string[] paths)
		{
			WinMerge( paths as IEnumerable<string>);
		}
		public static void WinMerge( IEnumerable<string> paths)
		{
			if( string.IsNullOrEmpty( s_WinMergePath) == false)
			{
				Process.Start( new ProcessStartInfo
				{
					FileName = s_WinMergePath,
					Arguments = string.Join( ' ', paths)
				});
			}
		}
		static string s_GitPath;
		static string s_TortoiseGitPath;
		static string s_WinMergePath;
		static bool s_IsFocused;
	}
}
