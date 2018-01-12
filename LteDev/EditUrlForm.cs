using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LteDev
{
    public partial class EditUrlForm : Form
    {
		private QueryItemList _queryItemList = new UriSegmentList();
		private UriSegmentList _segmentsList = new QueryItemList();
		private DataGridViewTextBoxColumn _orderBuildDataGridViewTextBoxColumn;
		private DataGridViewTextBoxColumn _keyBuildDataGridViewTextBoxColumn;
		private DataGridViewTextBoxColumn _valueBuildDataGridViewTextBoxColumn;
		private DataGridViewCheckBoxColumn _hasValueBuildDataGridViewCheckBoxColumn;
		private DataGridViewTextBoxColumn _orderComponentDataGridViewTextBoxColumn;
		private DataGridViewTextBoxColumn _keyComponentDataGridViewTextBoxColumn;
		private DataGridViewTextBoxColumn _valueComponentDataGridViewTextBoxColumn;
		private DataGridViewCheckBoxColumn _hasValueComponentDataGridViewCheckBoxColumn;
		
        public EditUrlForm()
        {
            InitializeComponent();
			
			_orderBuildDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_keyBuildDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_valueBuildDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_hasValueBuildDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
			_orderSegmentsDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_nameSegmentsDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_orderComponentDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_keyComponentDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_valueComponentDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			_hasValueComponentDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
			
			_orderBuildDataGridViewTextBoxColumn.Name = "orderBuildDataGridViewTextBoxColumn";
			_orderBuildDataGridViewTextBoxColumn.HeaderText = "Order";
			_orderBuildDataGridViewTextBoxColumn.DataPropertyName = "Order";
			_orderBuildDataGridViewTextBoxColumn.ReadOnly = true;
			_orderBuildDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			_orderBuildDataGridViewTextBoxColumn.Name = "orderBuildDataGridViewTextBoxColumn";
			_keyBuildDataGridViewTextBoxColumn.HeaderText = "Key";
			_keyBuildDataGridViewTextBoxColumn.DataPropertyName = "Key";
			_keyBuildDataGridViewTextBoxColumn.ReadOnly = true;
			_keyBuildDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			_valueBuildDataGridViewTextBoxColumn.Name = "valueBuildDataGridViewTextBoxColumn";
			_valueBuildDataGridViewTextBoxColumn.HeaderText = "Value";
			_valueBuildDataGridViewTextBoxColumn.DataPropertyName = "Value";
			_valueBuildDataGridViewTextBoxColumn.ReadOnly = true;
			_valueBuildDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			_valueBuildDataGridViewTextBoxColumn.FillWeight = 100F;
			
			_hasValueBuildDataGridViewCheckBoxColumn.Name = "hasValueBuildDataGridViewCheckBoxColumn";
			_hasValueBuildDataGridViewCheckBoxColumn.HeaderText = "HasValue";
			_hasValueBuildDataGridViewCheckBoxColumn.DataPropertyName = "HasValue";
			_hasValueBuildDataGridViewCheckBoxColumn.ReadOnly = true;
			_hasValueBuildDataGridViewCheckBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			queryBuildDataGridView.Columns.Add(_orderBuildDataGridViewTextBoxColumn);
			queryBuildDataGridView.Columns.Add(_keyBuildDataGridViewTextBoxColumn);
			queryBuildDataGridView.Columns.Add(_valueBuildDataGridViewTextBoxColumn);
			queryBuildDataGridView.Columns.Add(_hasValueBuildDataGridViewCheckBoxColumn);
			
			queryBuildDataGridView.DataSource = _queryItemList;
			
			_orderSegmentsDataGridViewTextBoxColumn.Name = "orderSegmentsDataGridViewTextBoxColumn";
			_orderSegmentsDataGridViewTextBoxColumn.HeaderText = "Order";
			_orderSegmentsDataGridViewTextBoxColumn.DataPropertyName = "Order";
			_orderSegmentsDataGridViewTextBoxColumn.ReadOnly = true;
			_orderSegmentsDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			_nameSegmentsDataGridViewTextBoxColumn.Name = "nameSegmentsDataGridViewTextBoxColumn";
			_nameSegmentsDataGridViewTextBoxColumn.HeaderText = "Name";
			_nameSegmentsDataGridViewTextBoxColumn.DataPropertyName = "Name";
			_nameSegmentsDataGridViewTextBoxColumn.ReadOnly = true;
			_nameSegmentsDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			segmentsComponentDataGridView.DataSource = _segmentsList;
			
			_orderComponentDataGridViewTextBoxColumn.Name = "orderComponentDataGridViewTextBoxColumn";
			_orderComponentDataGridViewTextBoxColumn.HeaderText = "Order";
			_orderComponentDataGridViewTextBoxColumn.DataPropertyName = "Order";
			_orderComponentDataGridViewTextBoxColumn.ReadOnly = true;
			_orderComponentDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			_keyComponentDataGridViewTextBoxColumn.Name = "keyComponentDataGridViewTextBoxColumn";
			_keyComponentDataGridViewTextBoxColumn.HeaderText = "Key";
			_keyComponentDataGridViewTextBoxColumn.DataPropertyName = "Key";
			_keyComponentDataGridViewTextBoxColumn.ReadOnly = true;
			_keyComponentDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			_valueComponentDataGridViewTextBoxColumn.Name = "valueComponentDataGridViewTextBoxColumn";
			_valueComponentDataGridViewTextBoxColumn.HeaderText = "Value";
			_valueComponentDataGridViewTextBoxColumn.DataPropertyName = "Value";
			_valueComponentDataGridViewTextBoxColumn.ReadOnly = true;
			_valueComponentDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			_valueComponentDataGridViewTextBoxColumn.FillWeight = 100F;
			
			_hasValueComponentDataGridViewCheckBoxColumn.Name = "hasValueComponentDataGridViewCheckBoxColumn";
			_hasValueComponentDataGridViewCheckBoxColumn.HeaderText = "HasValue";
			_hasValueComponentDataGridViewCheckBoxColumn.DataPropertyName = "HasValue";
			_hasValueComponentDataGridViewCheckBoxColumn.ReadOnly = true;
			_hasValueComponentDataGridViewCheckBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			
			queryComponentDataGridView.Columns.Add(_orderComponentDataGridViewTextBoxColumn);
			queryComponentDataGridView.Columns.Add(_keyComponentDataGridViewTextBoxColumn);
			queryComponentDataGridView.Columns.Add(_valueComponentDataGridViewTextBoxColumn);
			queryComponentDataGridView.Columns.Add(_hasValueComponentDataGridViewCheckBoxColumn);
			
			queryComponentDataGridView.DataSource = _queryItemList;
        }
		
		private void ResetForm()
		{
			urlTextBox.Text = "";
			urlErrorLabel.Visible = true;
			urlErrorLabel.Text = "URL is empty.";
			hostCheckBox.Checked = false;
			pathBuildTextBox.Text = "";
			queryCheckBox.Checked = false;
			fragmentBuildCheckBox.Checked = false;
			schemeComponentTextBox.Text = "";
			userNameComponentTextBox.Text = "";
			passwordComponentTextBox.Text = "";
			hostComponentTextBox.Text = "";
			portComponentTextBox.Text = "";
			portComponentDescLabel.Text = "(default)";
			queryNotIncludedLabel.Text = "Not included.";
			fragmentComponentTextBox.Text = "";
			parentDirButton.Enabled = false;
		}

		private void LoadFallback(string text)
		{
			if (text == null || text.Trim().Length == 0)
			{
				ResetForm();
				return;
			}
			
			if (urlErrorLabel.Visible)
			{
				urlErrorLabel.Text = "";
				urlErrorLabel.Visible = false;
			}
			urlTextBox.Text = text;
			
			string scheme = null, userName = null, password = null, host = null, path = null, query = null, fragment = null;
			int? port = null;
			
			int index = text.IndexOfAny(new char[] { ':', '@', '/', '\\', '?', '#' });
			
			if (index < 0)
			{
				Load(scheme, userName, password, host, port, text, query, fragment);
				return;
			}
			
			if (text[index] == '/' || text[index] == '\\' || text[index] == '?' || text[index] == '#')
				path = text;
			else if (text[index] == '@')
			{
				userName = text.Substring(0, index);
				text = text.Substring(index + 1);
				index = text.IndexOfAny(new char[] { ':', '@', '/', '\\', '?', '#' });
				if (index < 0)
				{
					Load(scheme, userName, password, host, port, text, query, fragment);
					return;
				}
				if (text[index] == '?' || text[index] == '#')
					path = text;
				else if (text[index] == '/' || text[index] == '\\')
				{
					path = text;
					index = path.IndexOfAny(new char[] { '?', '#' });
				}
				else if (text[index] == ':')
				{
					host = text.Substring(0, index);
					text = text.Substring(index + 1);
					// text@text:text
					index = text.IndexOfAny(new char[] { '/', '\\', '?', '#' });
					string ps;
					if (index < 0)
						ps = text;
					else
					{
						ps = text.Substring(0, index);
						path = text.Substring(index);
						index = path.IndexOfAny(new char[] { '?', '#' });
					}
				}
				else
				{
					path = text;
					index = path.IndexOfAny(new char[] { '?', '#' });
				}
			}
			else
			{
				scheme = text.Substring(0, index);
				text = text.Substring(index + 1);
				if (text.Length > 1 && text.StartsWith("//"))
					text = text.Substring(3);
				index = text.IndexOfAny(new char[] { ':', '@', '/', '\\', '?', '#' });
				if (index < 0)
				{
					Load(scheme, userName, password, host, port, text, query, fragment);
					return;
				}
				
				if (text[index] == '?' || text[index] == '#')
					path = text;
				else if (text[index] == '/' || text[index] == '\\')
				{
					path = text;
					index = path.IndexOfAny(new char[] { '?', '#' });
				}
				else if (text[index] == ':')
				{
					host = text.Substring(0, index);
					text = text.Substring(index + 1);
					index = text.IndexOfAny(new char[] { '@', ':', '/', '\\', '?', '#' });
					string ps;
					if (index < 0)
						ps = text;
					else if (text[index] == '@')
					{
						userName = host;
						host = null;
						password = text.Substring(0, index);
						text = text.Substring(index + 1);
						index = text.IndexOfAny(new char[] { '@', ':', '/', '\\', '?', '#' });
						if (index < 0)
							host = text;
						else if (text[index] == ':')
						{
							host = text.Substring(0, index);
							text = text.Substring(index + 1);
							index = text.IndexOfAny(new char[] { '@', ':', '/', '\\', '?', '#' });
							if (index < 0)
								ps = text;
							else
							{
								ps = text.Substring(0, index);
								path = text.Substring(index);
								index = path.IndexOfAny(new char[] { '?', '#' });
							}
						}
					}
					else
					{
						ps = text.Substring(0, index);
						path = text.Substring(index);
						index = path.IndexOfAny(new char[] { '?', '#' });
					}
				}
				else
				{
					path = text;
					index = text.IndexOfAny(new char[] { '?', '#' });
				}
			}
			
			if (ps != null)
			{
				int pn;
				if (ps.Length > 0 && Int32.TryParse(ps, out pn))
					port = pn;
				else
				{
					if (path == null)
						path = ":" + ps;
					else
					{
						if (index >= 0)
							index += ps.Length + 1;
						path = ":" + ps + path;
					}
				}
			}
			
			if (index >= 0)
			{
				if (path[index] == '#')
					fragment = path.Substring(index + 1);
				else
				{
					query = path.Substring(index + 1);
					int index = query.IndexOf('#');
					if (index >= 0)
					{
						fragment = query.Substring(index + 1);
						query = query.Substring(0, index);
					}
				}
				path = path.Substring(0, index);
			}
			
			Load(scheme, userName, password, host, port, path, query, fragment);
		}

		private void Load(string scheme, string userName, string password, string host, int? port, string path, string query, string fragment)
		{
			if (!String.IsNullOrEmpty(scheme))
				scheme = Uri.UnescapeDataString(scheme);
			if (!String.IsNullOrEmpty(userName))
				userName = Uri.UnescapeDataString(userName);
			if (!String.IsNullOrEmpty(password))
				userName = Uri.UnescapeDataString(password);
			if (!String.IsNullOrEmpty(host))
				userName = Uri.UnescapeDataString(host);
			if (!String.IsNullOrEmpty(fragment))
				userName = Uri.UnescapeDataString(fragment);
			
			if (scheme != null || userName != null || password != null || host != null || port.HasValue)
			{
				hostCheckBox.Checked = true;
				schemeBuildTextBox.Text = (scheme == null) ? "" : scheme;
				hostBuildTextBox.Text = (host == null) ? "" : host;
				if (password != null)
				{
					userNameCheckBox.Checked = true;
					passwordCheckBox.Checked = true;
					userNameBuildTextBox.Text = (userName == null) ? "" : userName;
					passwordBuildTextBox.Text = password;
				}
				else if (userName != null)
				{
					userNameCheckBox.Checked = true;
					userNameBuildTextBox.Text = userName;
					if (password == null)
						passwordCheckBox.Checked = false;
					else
					{
						passwordCheckBox.Checked = true;
						passwordBuildTextBox.Text = password;
					}
				}
				else
					userNameCheckBox.Checked = false;
				if (port.HasValue)
				{
					portCheckBox.Checked = true;
					portBuildTextBox.Text = port.Value.ToString();
					portMessageLabel.Text = "";
					portComponentDescLabel.Text = "";
					portMessageLabel.Visible = false;
					portComponentDescLabel.Visible = false;
				}
				else
				{
					portCheckBox.Checked = false;
					if (String.IsNullOrEmpty(scheme))
					{
						portMessageLabel.Text = "";
						portComponentDescLabel.Text = "";
						portMessageLabel.Visible = false;
						portComponentDescLabel.Visible = false;
					}
					else
					{
						UriBuilder uriBuilder = new UriBuilder();
						try { uriBuilder.Scheme = scheme; } catch { }
						
						if (String.IsNullOrEmpty(host))
							uriBuilder.Host = "localhost";
						else
							try { uriBuilder.Host = host; } catch { uriBuilder.Host = "localhost" }
						try
						{
							if (uriBuilder.Uri.Port > 0)
							{
								portBuildTextBox.Text = uriBuilder.Uri.Port.ToString();
								portMessageLabel.Visible = true;
								portMessageLabel.Text = "(default)";
								portComponentDescLabel.Visible = true;
								portComponentDescLabel.Text = "(default)";
							}
							else
							{
								portMessageLabel.Text = "";
								portComponentDescLabel.Text = "";
								portMessageLabel.Visible = false;
								portComponentDescLabel.Visible = false;
							}
						}
						catch
						{
							portMessageLabel.Text = "";
							portComponentDescLabel.Text = "";
							portMessageLabel.Visible = false;
							portComponentDescLabel.Visible = false;
						}
					}
				}
			}
			else
				hostCheckBox.Checked = false;
			
			if (fragment == null)
				fragmentBuildCheckBox.Checked = false;
			else
			{
				fragmentBuildCheckBox.Checked = true;
				fragmentBuildTextBox.Text = Uri.UnescapeDataString(fragment);
			}
			
			_segmentsList.Clear();
			_queryItemList.Clear();
			
			int order = 0;
			if (path != null)
			{
				pathBuildTextBox.Text = path;
				if (path.Length == 0)
					_segmentsList.Add(new UriSegment("", 0));
				else
				{
					int startIndex = 0;
					char[] pathSeparators = new char[] { '/', '\\', ':' };
					for (int index = path.IndexOfAny(pathSeparators, startIndex); index >= 0; index = path.IndexOfAny(pathSeparators, startIndex))
					{
						_segmentsList.Add(new UriSegment(Uri.UnescapeDataString(path.Substring(startIndex, (index - startIndex) + 1)), order));
						startIndex = index + 1;
						order++;
					}
					if (startIndex < path.Length)
						_segmentsList.Add(new UriSegment(Uri.UnescapeDataString(path.Substring(startIndex)), order));
				}
			}
			else
				pathBuildTextBox.Text = "";
			
			if (query == null)
			{
				queryCheckBox.Checked = false;
				return;
			}
			
			queryCheckBox.Checked = true;
			
			order = 0;
			foreach (string[] kvp in query.Split('&').Select(s => s.Split('=', 2)))
			{
				if (kvp.Length == 2)
					_queryItemList.Add(new UriQueryParam(Uri.UnescapeDataString(kvp[0]), Uri.UnescapeDataString(kvp[1]), order));
				else
					_queryItemList.Add(new UriQueryParam(Uri.UnescapeDataString(kvp[0]), order));
				order++;
			}
		}

		private void Load(string text)
		{
			if (text == null || text.Trim().Length == 0)
			{
				ResetForm();
				return;
			}
			Uri uri;
			if (Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri))
				Load(uri);
			else
				LoadFallback(text);
		}
		
		private void Load(Uri uri)
		{
			if (uri == null)
			{
				ResetForm();
				return;
			}
			
			if (uri.IsAbsoluteUri)
			{
				Load(new UriBuilder(uri), false);
				return;
			}
			
			Uri u;
			string text = uri.ToString();
			if (!text.StartsWith("/"))
				text = "/" + text;
			if (Uri.TryCreate("http://localhost" + text, UriKind.Absolute))
				Load(new UriBuilder(u), true);
			else
				LoadFallback(uri.ToString());
		}
		
		private void Load(UriBuilder uriBuilder, bool asRelative)
		{
			if (uriBuilder == null)
			{
				ResetForm();
				return;
			}
			
			if (urlErrorLabel.Visible)
			{
				urlErrorLabel.Text = "";
				urlErrorLabel.Visible = false;
			}
			
			string scheme = null, userName = null, password = null, host = null, path = uriBuilder.Path, query = null, fragment = null;
			int? port = null;
			
			if (!String.IsNullOrEmpty(uriBuilder.Query))
			{
				query = uriBuilder.Query;
				if (query[0] == '?')
					query = query.Substring(1);
			}
			
			if (!String.IsNullOrEmpty(uriBuilder.Fragment))
			{
				fragment = uriBuilder.Fragment;
				if (fragment[0] == '?')
					fragment = fragment.Substring(1);
			}
			
			if (!asRelative)
			{
				scheme = uriBuilder.Scheme;
				if (!String.IsNullOrEmpty(uriBuilder.Uri.UserInfo))
				{
					userName = uriBuilder.UserName;
					if (!String.IsNullOrEmpty(uriBuilder.Password) || uriBuilder.Uri.UserInfo.Contains(':'))
						password = uriBuilder.Password;
				}
				host = uriBuilder.Host;
				if (!uriBuilder.Uri.IsDefaultPort)
					port = uriBuilder.Uri.Port;
			}
			
			Load(scheme, userName, password, host, port, path, query, fragment);
		}
		
        private void mainTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
			if (mainTabControl.SelectedTab == null)
				return;
			if (ReferenceEquals(mainTabControl.SelectedTab, parseTabPage))
			{
				string text = urlTextBox.Text;
				if (text.Trim().Length > 0)
					Load(text);
				else
				{
					urlErrorLabel.Visible = true;
					urlErrorLabel.Text = "URL is empty.";
					e.Cancel = true;
				}
			}
			else if (ReferenceEquals(mainTabControl.SelectedTab, buildTabPage))
			{
				// TODO: Try to build URL
			}
        }

        private void buildButton_Click(object sender, EventArgs e)
        {

        }

        private void parseButton_Click(object sender, EventArgs e)
        {

        }

        private void userNameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (userNameCheckBox.Checked)
			{
				userNameBuildTextBox.Visible = true;
				authSeparatorLabel.Visible = true;
			}
			else
			{
				userNameBuildTextBox.Text = "";
				passwordCheckBox.Checked = false;
				userNameBuildTextBox.Visible = false;
				authSeparatorLabel.Visible = false;
			}
        }

        private void passwordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (passwordCheckBox.Checked)
			{
				passwordBuildTextBox.Visible = true;
				passwordSeparatorLabel.Visible = true;
			}
			else
			{
				passwordBuildTextBox.Text = "";
				passwordBuildTextBox.Visible = false;
				passwordSeparatorLabel.Visible = false;
			}
        }

        private void hostCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (hostCheckBox.Checked)
			{
				schemeBuildTextBox.Visible = true;
				schemeSeparatorLabel.Visible = true;
				hostBuildTextBox.Visible = true;
				portSeparatorLabel.Visible = true;
				portBuildTextBox.Visible = true;
				portMessageLabel.Visible = (portMessageLabel.Text.Length > 0);
			}
			else
			{
				hostBuildTextBox.Text = "";
				userNameCheckBox.Checked = false;
				userNameCheckBox.Checked = false;
				portCheckBox.Checked = false;
				schemeBuildTextBox.Visible = false;
				schemeSeparatorLabel.Visible = false;
				hostBuildTextBox.Visible = false;
				portSeparatorLabel.Visible = false;
				portMessageLabel.Visible = false;
			}
        }

        private void portCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (portCheckBox.Checked)
			{
				portBuildTextBox.ReadOnly = false;
				portMessageLabel.Visible = (portMessageLabel.Text.Length > 0);
			}
			else
			{
				portBuildTextBox.ReadOnly = true;
				portMessageLabel.Visible = false;
			}
        }

        private void queryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (queryCheckBox.Checked)
				querySplitContainer.Visible = false;
			else
			{
				querySplitContainer.Visible = false;
				_queryItemList.Clear();
			}
        }

        private void queryBuildDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void addQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void saveQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void queryItemValueCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void moveUpQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void moveDownQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void addQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void insertQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void deleteQueryItemButton_Click(object sender, EventArgs e)
        {

        }

        private void fragmentBuildCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			if (fragmentBuildCheckBox.Checked)
				fragmentBuildTextBox.Visible = true;
			else
			{
				fragmentBuildTextBox.Text = "";
				fragmentBuildTextBox.Visible = false;
			}
        }

        private void buildUrlButton_Click(object sender, EventArgs e)
        {

        }

        private void parentDirButton_Click(object sender, EventArgs e)
        {

        }
    }
}