
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace Knit
{
	static partial class External
	{
		static void UpdateBranch()
		{
			var isApplicationActive = InternalEditorUtility.isApplicationActive;
			
			if( s_IsFocused == false && isApplicationActive != false)
			{
				s_IsFocused = true;
				
				if( s_UpdateMainWindowTitleEvent == null
				||	s_UpdateMainWindowTitleMethod == null
				||	s_UpdateMainWindowTitleDelegate == null)
				{
					Type tEditorApplication = typeof( EditorApplication);
					Type tApplicationTitleDescriptor = tEditorApplication.Assembly.GetTypes()
						.First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");
					
					s_UpdateMainWindowTitleEvent = tEditorApplication.GetEvent( "updateMainWindowTitle", kUpdateMainWindowFlags);
					s_UpdateMainWindowTitleMethod = tEditorApplication.GetMethod( "UpdateMainWindowTitle", kUpdateMainWindowFlags);
					
					Type delegateType = typeof(Action<>).MakeGenericType( tApplicationTitleDescriptor);
					MethodInfo methodInfo = ((Action<object>)UpdateMainWindowTitle).Method;
					s_UpdateMainWindowTitleDelegate = Delegate.CreateDelegate( delegateType, null, methodInfo);
				}
				if( s_UpdateMainWindowTitleEvent != null && s_UpdateMainWindowTitleMethod != null)
				{
					s_UpdateMainWindowTitleEvent.GetAddMethod( true).Invoke( 
						null, new object[]{ s_UpdateMainWindowTitleDelegate });
					s_UpdateMainWindowTitleMethod.Invoke( null, new object[ 0]);
					s_UpdateMainWindowTitleEvent.GetRemoveMethod( true).Invoke( 
						null, new object[]{ s_UpdateMainWindowTitleDelegate });
				}
			}
			else if( s_IsFocused != false && isApplicationActive == false)
			{
				s_IsFocused = false;
			}
		}
		static void UpdateMainWindowTitle( object descriptor)
		{
			if( s_TitleField == null)
			{
				s_TitleField = typeof(EditorApplication).Assembly.GetTypes()
					.First( x => x.FullName == "UnityEditor.ApplicationTitleDescriptor")
					.GetField( "title", BindingFlags.Instance | BindingFlags.Public);
			}
			var title = s_TitleField.GetValue( descriptor) as string;
			s_TitleField.SetValue( descriptor, 
				string.Format( "{1}* [{2}] {0}", title, 
					Git( "symbolic-ref --short HEAD").Trim(),
					Git( "rev-parse --short HEAD").Trim()));
		}
		// ApplicationTitleDescriptor が internal のバージョンがある
		const BindingFlags kUpdateMainWindowFlags = 
		#if UNITY_2023_2_OR_NEWER
			BindingFlags.Static | BindingFlags.Public;
		#else
			BindingFlags.Static | BindingFlags.NonPublic;
		#endif
		static EventInfo s_UpdateMainWindowTitleEvent;
		static MethodInfo s_UpdateMainWindowTitleMethod;
		static Delegate s_UpdateMainWindowTitleDelegate;
		static FieldInfo s_TitleField;
	}
}
