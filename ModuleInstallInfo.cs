using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Erwine.Leonard.T.Setup
{
	public class ModuleInstallInfo
	{
		#region Fields
		
		private PSModuleInfo _moduleInfo;
		private bool _isCurrentlyInstalled;
		private Dictionary<string, object> _contents = new Dictionary<string, object>();
		private Dictionary<string, object> _override = new Dictionary<string, object>();
		private Dictionary<string, PropertyDescriptor> _properties = new Dictionary<string, PropertyDescriptor>();
		private FileInfo _sourceLocation = null;
		private FileInfo _targetLocation = null;
		public PSModuleInfo ModuleInfo { get { return _moduleInfo; } }
		public bool IsCurrentlyInstalled { get { return _isCurrentlyInstalled; } }
		
		public const string PropertyName_DisplayName = "DisplayName";
		public const string PropertyName_RootModule = "RootModule";
		public const string PropertyName_ModuleToProcess = "ModuleToProcess";
		public const string PropertyName_Author = "Author";
		public const string PropertyName_ClrVersion = "ClrVersion";
		public const string PropertyName_CompanyName = "CompanyName";
		public const string PropertyName_Copyright = "Copyright";
		public const string PropertyName_DotNetFrameworkVersion = "DotNetFrameworkVersion";
		public const string PropertyName_FileList = "FileList";
		public const string PropertyName_HelpInfoUri = "HelpInfoUri";
		public const string PropertyName_ModuleList = "ModuleList";
		public const string PropertyName_PowerShellHostName = "PowerShellHostName";
		public const string PropertyName_PowerShellHostVersion = "PowerShellHostVersion";
		public const string PropertyName_PowerShellVersion = "PowerShellVersion";
		public const string PropertyName_ProcessorArchitecture = "ProcessorArchitecture";
		public const string PropertyName_ProjectUri = "ProjectUri";
		public const string PropertyName_ReleaseNotes = "ReleaseNotes";
		public const string PropertyName_RepositorySourceLocation = "RepositorySourceLocation";
		public const string PropertyName_RequiredAssemblies = "RequiredAssemblies";
		public const string PropertyName_Tags = "Tags";
		
		#endregion
		
		#region Properties
		
		public FileInfo SourceLocation
		{
			get { return _sourceLocation; }
			set
			{
				if (_sourceLocation != null && !_isCurrentlyInstalled)
					throw new NotSupportedException("Cannot change the source location for a module that is not installed.");
				
				_sourceLocation = value;
			}
		}
		public FileInfo TargetLocation
		{
			get { return _targetLocation; }
			set
			{
				if (_targetLocation != null && _isCurrentlyInstalled)
					throw new NotSupportedException("Cannot change the target location for a module that is installed.");
				
				_targetLocation = value;
			}
		}
		public string RootModule
		{
			get
			{
				if (_override.ContainsKey(PropertyName_RootModule))
					return _override[PropertyName_RootModule] as string;
				if (_isCurrentlyInstalled)
				{
					if (_targetLocation != null)
						return _targetLocation.Name;
					if (_sourceLocation != null)
						return _sourceLocation.Name;
				} else if (_sourceLocation != null)
					return _sourceLocation.Name;
				else if (_targetLocation != null)
					return _targetLocation.Name;
				string name;
				if ((_contents.ContainsKey(PropertyName_RootModule) && (name = GetContentAsSingleString(PropertyName_RootModule, true)).Length > 0) ||
						(_contents.ContainsKey(PropertyName_ModuleToProcess) && (name = GetContentAsSingleString(PropertyName_ModuleToProcess, true)).Length > 0))
					return name;
				return null;
			}
			set { SetPropertyString(PropertyName_RootModule, value); }
		}
		
		public string DisplayName
		{
			get
			{
				string name = _moduleInfo.Name;
				if (name != null && (name = _moduleInfo.Name.Trim()).Length > 0)
					return name;
				name = RootModule;
				if (String.IsNullOrEmpty(name))
					return _moduleInfo.Guid.ToString("n");
				try { return Path.GetFileNameWithoutExtension(name); } catch { return name; }
			}
		}
		
		public string Author
		{
			get { return GetPropertyString(PropertyName_Author); }
			set { SetPropertyString(PropertyName_Author, value); }
		}
		public Version ClrVersion
		{
			get { return GetPropertyVersion(PropertyName_ClrVersion); }
			set { SetPropertyObject(PropertyName_ClrVersion, value); }
		}
		public string CompanyName
		{
			get { return GetPropertyString(PropertyName_CompanyName); }
			set { SetPropertyString(PropertyName_CompanyName, value); }
		}
		public string Copyright
		{
			get { return GetPropertyString(PropertyName_Copyright); }
			set { SetPropertyString(PropertyName_Copyright, value); }
		}
		public Version DotNetFrameworkVersion
		{
			get { return GetPropertyVersion(PropertyName_DotNetFrameworkVersion); }
			set { SetPropertyObject(PropertyName_DotNetFrameworkVersion, value); }
		}
		public IEnumerable<string> FileList
		{
			get { return GetPropertyEnumerableString(PropertyName_FileList); }
			set { SetPropertyEnumerableString(PropertyName_FileList, value); }
		}
		public string HelpInfoUri
		{
			get { return GetPropertyString(PropertyName_HelpInfoUri); }
			set { SetPropertyString(PropertyName_HelpInfoUri, value); }
		}
		public IEnumerable<object> ModuleList
		{
			get
			{
				if (_override.ContainsKey(PropertyName_ModuleList))
					return _override[PropertyName_ModuleList] as IEnumerable<object>;
				if (_properties.ContainsKey(PropertyName_ModuleList))
				{
					IEnumerable<object> obj = _properties[PropertyName_ModuleList].GetValue(_moduleInfo) as IEnumerable<object>;
					if (obj != null)
						return obj;
				}
				return GetContentAsEnumerable(PropertyName_ModuleList);
			}
			set
			{
				if (value != null)
				{
					bool hasNonEmpty = false;
					using (IEnumerator<object> enumerator = value.GetEnumerator())
					{
						while (!hasNonEmpty && enumerator.MoveNext())
						{
							if (enumerator.Current == null)
								continue;
							
							if (enumerator.Current is PSModuleInfo || enumerator.Current is Hashtable && (enumerator.Current as Hashtable).ContainsKey("Name") ||
								(enumerator.Current is string && (enumerator.Current as string).Trim().Length > 0))
									hasNonEmpty = true;
						}
					}
					if (hasNonEmpty)
					{
						if (_override.ContainsKey(PropertyName_ModuleList))
							_override[PropertyName_ModuleList] = value;
						else
							_override.Add(PropertyName_ModuleList, value);
						return;
					}
				}
				
				if (_override.ContainsKey(PropertyName_ModuleList))
					_override.Remove(PropertyName_ModuleList);
			}
		}
		public string PowerShellHostName
		{
			get { return GetPropertyString(PropertyName_PowerShellHostName); }
			set { SetPropertyString(PropertyName_PowerShellHostName, value); }
		}
		public Version PowerShellHostVersion
		{
			get { return GetPropertyVersion(PropertyName_PowerShellHostVersion); }
			set { SetPropertyObject(PropertyName_PowerShellHostVersion, value); }
		}
		public Version PowerShellVersion
		{
			get { return GetPropertyVersion(PropertyName_PowerShellVersion); }
			set { SetPropertyObject(PropertyName_PowerShellVersion, value); }
		}
		public ProcessorArchitecture? ProcessorArchitecture
		{
			get
			{
				if (_override.ContainsKey(PropertyName_ProcessorArchitecture))
					return _override[PropertyName_ProcessorArchitecture] as ProcessorArchitecture?;
				if (_properties.ContainsKey(PropertyName_ProcessorArchitecture))
				{
					ProcessorArchitecture? value = _properties[PropertyName_ProcessorArchitecture].GetValue(_moduleInfo) as ProcessorArchitecture?;
					if (value.HasValue)
						return value.Value;
				}
				if (!_contents.ContainsKey(PropertyName_ProcessorArchitecture))
					return null;
				using (IEnumerator<ProcessorArchitecture> enumerator = GetContentOfType<ProcessorArchitecture>(PropertyName_ProcessorArchitecture).GetEnumerator())
				{
					if (enumerator.MoveNext())
						return enumerator.Current;
				}
				
				using (IEnumerator<string> enumerator = GetContentAsEnumString(PropertyName_ProcessorArchitecture).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string s = enumerator.Current;
						if (s != null && (s = s.Trim()).Length > 0)
							try { return (ProcessorArchitecture)(Enum.Parse(typeof(ProcessorArchitecture), s, true)); } catch { }
					}
				}
				
				return null;
			}
			set
			{
				if (value.HasValue)
				{
					if (_override.ContainsKey(PropertyName_ProcessorArchitecture))
						_override[PropertyName_ProcessorArchitecture] = value;
					else
						_override.Add(PropertyName_ProcessorArchitecture, value);
				}
				else if (_override.ContainsKey(PropertyName_ProcessorArchitecture))
					_override.Remove(PropertyName_ProcessorArchitecture);
			}
		}
		public Uri ProjectUri
		{
			get { return GetPropertyUri(PropertyName_ProjectUri); }
			set { SetPropertyObject(PropertyName_ProjectUri, value); }
		}
		public string ReleaseNotes
		{
			get { return GetPropertyString(PropertyName_ReleaseNotes); }
			set { SetPropertyString(PropertyName_ReleaseNotes, value); }
		}
		public Uri RepositorySourceLocation
		{
			get { return GetPropertyUri(PropertyName_RepositorySourceLocation); }
			set { SetPropertyObject(PropertyName_RepositorySourceLocation, value); }
		}
		public IEnumerable<string> RequiredAssemblies
		{
			get { return GetPropertyEnumerableString(PropertyName_RequiredAssemblies); }
			set { SetPropertyEnumerableString(PropertyName_RequiredAssemblies, value); }
		}
		public IEnumerable<string> Tags
		{
			get { return GetPropertyEnumerableString(PropertyName_Tags); }
			set { SetPropertyEnumerableString(PropertyName_Tags, value); }
		}
		
		#endregion
		
		#region Private methods
		
		private string GetPropertyString(string key)
		{
			if (_override.ContainsKey(key))
				return _override[key] as string;
			if (_properties.ContainsKey(key))
			{
				string s = _properties[key].GetValue(_moduleInfo) as string;
				if (s != null && s.Trim().Length > 0)
					return s;
			}
			return GetContentAsSingleString(key);
		}
		private IEnumerable<string> GetPropertyEnumerableString(string key)
		{
			if (_override.ContainsKey(key))
				return _override[key] as IEnumerable<string>;
			if (_properties.ContainsKey(key))
			{
				IEnumerable<string> s = _properties[key].GetValue(_moduleInfo) as IEnumerable<string>;
				if (s != null)
					return s;
			}
			return GetContentAsEnumString(key);
		}
		private Version GetPropertyVersion(string key)
		{
			if (_override.ContainsKey(key))
				return _override[key] as Version;
			if (_properties.ContainsKey(key))
			{
				Version version = _properties[key].GetValue(_moduleInfo) as Version;
				if (version != null)
					return version;
			}
			if (!_contents.ContainsKey(key))
				return null;
			using (IEnumerator<Version> enumerator = GetContentOfType<Version>(key).GetEnumerator())
			{
				if (enumerator.MoveNext())
					return enumerator.Current;
			}
			using (IEnumerator<string> enumerator = GetContentAsEnumString(key).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string s = enumerator.Current;
					if (s != null && (s = s.Trim()).Length > 0)
						try { return new Version(s); } catch { }
				}
			}
			return null;
		}
		private Uri GetPropertyUri(string key)
		{
			if (_override.ContainsKey(key))
				return _override[key] as Uri;
			if (_properties.ContainsKey(key))
			{
				Uri uri = _properties[key].GetValue(_moduleInfo) as Uri;
				if (uri != null)
					return uri;
			}
			if (!_contents.ContainsKey(key))
				return null;
			using (IEnumerator<Uri> enumerator = GetContentOfType<Uri>(key).GetEnumerator())
			{
				if (enumerator.MoveNext())
					return enumerator.Current;
			}
			
			Uri relativeUri = null;
			using (IEnumerator<string> enumerator = GetContentAsEnumString(key).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string s = enumerator.Current;
					Uri uri;
					if (s != null && (s = s.Trim()).Length > 0)
					{
						if (Uri.TryCreate(s, UriKind.Absolute, out uri))
							return uri;
						if (relativeUri == null && Uri.TryCreate(s, UriKind.Relative, out uri))
							relativeUri = uri;
					}
				}
			}
			return relativeUri;
		}
		private void SetPropertyString(string key, string value)
		{
			if (value == null || value.Trim().Length == 0)
			{
				if (_override.ContainsKey(key))
					_override.Remove(key);
				return;
			}
			if (_override.ContainsKey(key))
				_override[key] = value;
			else
				_override.Add(key, value);
		}
		private void SetPropertyEnumerableString(string key, IEnumerable<string> value)
		{
			if (value != null)
			{
				bool hasNonEmpty = false;
				using (IEnumerator<string> enumerator = value.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null && enumerator.Current.Trim().Length > 0)
						{
							hasNonEmpty = true;
							break;
						}
					}
				}
				if (hasNonEmpty)
				{
					if (_override.ContainsKey(key))
						_override[key] = value;
					else
						_override.Add(key, value);
					return;
				}
			}
			
			if (_override.ContainsKey(key))
				_override.Remove(key);
		}
		private void SetPropertyObject(string key, object value)
		{
			if (value == null)
			{
				if (_override.ContainsKey(key))
					_override.Remove(key);
			}
			else if (_override.ContainsKey(key))
				_override[key] = value;
			else
				_override.Add(key, value);
		}
		private string GetContentAsSingleString(string key, bool trim)
		{
			using (IEnumerator<string> enumerator = GetContentOfType<string>(key).GetEnumerator())
			{
				if (enumerator.MoveNext())
					return (trim) ? enumerator.Current.Trim() : enumerator.Current;
			}
			
			using (IEnumerator<string> enumerator = GetContentAsEnumString(key).GetEnumerator())
			{
				if (enumerator.MoveNext() && enumerator.Current != null)
					return (trim) ? enumerator.Current.Trim() : enumerator.Current;
			}
			
			return (trim) ? "" : null;
		}
		private string GetContentAsSingleString(string key) { return GetContentAsSingleString(key, false); }
		private object GetContentAsSingleValue(string key)
		{
			using (IEnumerator<object> enumerator = GetContentAsEnumerable(key).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
						return enumerator.Current;
				}
			}
			
			return null;
		}
		
		private IEnumerable<T> GetContentOfType<T>(string key)
		{
			foreach (object obj in GetContentAsEnumerable(key))
			{
				if (obj != null && obj is T)
					yield return (T)obj;
			}
		}
		
		private IEnumerable<object> GetContentOfType(string key, Type t)
		{
			if (t == null)
			{
				foreach (object obj in GetContentAsEnumerable(key))
				{
					if (obj != null)
						yield return obj;
				}
			}
			else
			{
				foreach (object obj in GetContentAsEnumerable(key))
				{
					if (obj != null && t.IsInstanceOfType(obj))
						yield return obj;
				}
			}
		}
		
		private IEnumerable<string> GetContentAsEnumString(string key)
		{
			foreach (object obj in GetContentAsEnumerable(key))
			{
				if (obj == null || obj is string)
					yield return obj as string;
				else
					yield return obj.ToString();
			}
		}
		private IEnumerable<object> GetContentAsEnumerable(string key)
		{
			if (_contents.ContainsKey(key))
			{
				object obj = _contents[key];
				if (obj != null && obj is PSObject)
					obj = (obj as PSObject).BaseObject;
				if (obj != null)
				{
					if (obj is string || !(obj is IEnumerable))
						yield return obj;
					else
					{
						foreach (object o in (obj as IEnumerable))
							yield return o;
					}
				}
			}
		}
		
		#endregion
		
		public ModuleInstallInfo(PSModuleInfo moduleInfo, bool isCurrentlyInstalled)
		{
			if (moduleInfo == null)
				throw new ArgumentNullException();
			_moduleInfo = moduleInfo;
			_isCurrentlyInstalled = isCurrentlyInstalled;
			if (isCurrentlyInstalled)
				try { _targetLocation = new FileInfo(moduleInfo.Path); } catch { }
			else
				try { _sourceLocation = new FileInfo(moduleInfo.Path); } catch { }
		}
		
		public void Refresh()
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_moduleInfo);
			_properties.Clear();
			for (int i = 0; i < properties.Count; i++)
				_properties.Add(properties[i].Name, properties[i]);
		}
	}
}