using Microsoft.Win32;
using Patcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace PatcherGUI
{
	public partial class MainForm : Form
	{
		private bool GameLoaded = false;
		private string GameFile = null;
		private Panel mainPanel;
		private Button searchGame, browse, exportLibrary, exportPublic, startPatching, startGame;
		private TextBox gamePath, log;
		private ListView patchesList, scriptsList;
		public MainForm()
		{
			InitializeComponent();
			mainPanel = new Panel();
			mainPanel.Size = this.ClientSize;
			this.Controls.Add(mainPanel);

			searchGame = new Button();
			searchGame.Text = "搜索游戏";
			searchGame.Location = new Point(0, 5);
			searchGame.Size = new Size(70, 30);
			searchGame.Click += (sender, e) =>
			{
				SearchGame();
				if (!GameLoaded)
				{
					MessageBox.Show("未找到游戏位置，请手动选择");
				}
				else
				{
					gamePath.Text = GameFile;
				}
			};
			this.mainPanel.Controls.Add(searchGame);
			gamePath = new TextBox();
			gamePath.AutoSize = false;
			gamePath.Font = new Font(SystemFonts.DefaultFont.Name, 16);
			gamePath.Location = new Point(75, 5);
			gamePath.Size = new Size(480, 30);
			this.mainPanel.Controls.Add(gamePath);
			browse = new Button();
			browse.Location = new Point(560, 5);
			browse.Size = new Size(40, 30);
			browse.Text = "...";
			browse.Click += (sender, e) =>
			{
				OpenFileDialog fd = new OpenFileDialog();
				fd.Multiselect = false;
				fd.Filter = "可执行文件(*.exe)|*.exe";
				if (fd.ShowDialog() == DialogResult.OK)
				{
					gamePath.Text = fd.FileName;
					GameFile = fd.FileName;
					GameLoaded = true;
				}
			};
			this.mainPanel.Controls.Add(browse);

			patchesList = new ListView();
			patchesList.Location = new Point(0, 40);
			patchesList.Size = new Size(300, 150);
			patchesList.View = View.List;
			patchesList.CheckBoxes = true;
			patchesList.BeginUpdate();
			if (Directory.Exists(@".\Patches"))
			{
				var PBase = patchesList.Items.Add(@".\PBase.dll", @".\PBase.dll", 0);
				PBase.Checked = true;

				string[] files = Directory.GetFiles(@".\Patches");
				foreach (var s in files)
				{
					if (s.EndsWith(".dll"))
					{
						patchesList.Items.Add(s, s, 0).Checked = true;
					}
				}
			}
			patchesList.ItemCheck += (sender, e) =>
			{
				if (patchesList.Items[e.Index].Text == @".\PBase.dll")
				{
					e.NewValue = CheckState.Checked;
				}
			};
			patchesList.EndUpdate();
			this.mainPanel.Controls.Add(patchesList);

			scriptsList = new ListView();
			scriptsList.Location = new Point(300, 40);
			scriptsList.Size = new Size(300, 150);
			scriptsList.View = View.List;
			scriptsList.CheckBoxes = true;
			scriptsList.BeginUpdate();
			if (Directory.Exists(@".\Patches"))
			{

				string[] files = Directory.GetFiles(@".\Patches");
				foreach (var s in files)
				{
					if (s.EndsWith(".py"))
					{
						scriptsList.Items.Add(s, s, 0).Checked = true;
					}
				}
			}
			scriptsList.EndUpdate();
			this.mainPanel.Controls.Add(scriptsList);


			log = new TextBox();
			log.Multiline = true;
			log.ScrollBars = ScrollBars.Vertical;
			log.KeyPress += (sender, e) =>
			{
				e.Handled = true;
			};
			log.Location = new Point(0, 190);
			log.Size = new Size(this.ClientSize.Width, 180);
			this.mainPanel.Controls.Add(log);

			exportLibrary = new Button();
			exportLibrary.Text = "导出资源";
			exportLibrary.Location = new Point(0, 370);
			exportLibrary.Size = new Size(150, 30);
			exportLibrary.Click += (sender, e) =>
			{
				if (!GameLoaded)
				{
					MessageBox.Show("还没有选择游戏文件");
					return;
				}
				PatchTool.ExportTerrariaLibrary(GameFile);
				Console.WriteLine("Library exported");
			};
			this.mainPanel.Controls.Add(exportLibrary);

			exportPublic = new Button();
			exportPublic.Text = "导出Public.exe";
			exportPublic.Location = new Point(150, 370);
			exportPublic.Size = new Size(150, 30);
			exportPublic.Click += (sender, e) =>
			{
				if (!GameLoaded)
				{
					MessageBox.Show("还没有选择游戏文件");
					return;
				}
				PatchTool.ExportPublic(GameFile);
				Console.WriteLine("Public.exe exported");
			};
			this.mainPanel.Controls.Add(exportPublic);


			startPatching = new Button();
			startPatching.Text = "应用补丁";
			startPatching.Location = new Point(300, 370);
			startPatching.Size = new Size(150, 30);
			startPatching.Click += (sender, e) =>
			{
				log.Text = "";
				if (!GameLoaded)
				{
					MessageBox.Show("还没有选择游戏文件");
					return;
				}
				IList<string> checkedPatches = new List<string>();
				IList<string> checkedScripts = new List<string>();
				foreach (var i in patchesList.CheckedItems)
				{
					ListViewItem l = (ListViewItem)i;
					checkedPatches.Add(l.Text);
				}
				foreach (var i in scriptsList.CheckedItems)
				{
					ListViewItem l = (ListViewItem)i;
					checkedScripts.Add(l.Text);
				}
				ThreadPool.QueueUserWorkItem((o) =>
				{
					var begin = DateTime.Now;
					try
					{
						this.mainPanel.Enabled = false;
						new PatchTool().Patch(GameFile, "Patched.exe", checkedPatches.ToArray(), checkedScripts.ToArray());
						GC.Collect();
						this.mainPanel.Enabled = true;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.GetType() + ":" + ex.Message + "\n" + ex.StackTrace);
					}
					finally
					{
						var end = DateTime.Now;
						var time = end - begin;
						Console.WriteLine("Finished all in:" + time);
						File.WriteAllText(@".\log.txt", log.Text);
						this.mainPanel.Enabled = true;
					}
				}, null);
			};
			this.mainPanel.Controls.Add(startPatching);

			startGame = new Button();
			startGame.Text = "启动Patched.exe";
			startGame.Location = new Point(450, 370);
			startGame.Size = new Size(150, 30);
			startGame.Click += (sender, e) =>
			{
				if (!GameLoaded)
				{
					MessageBox.Show("还没有选择游戏文件");
					return;
				}
				ProcessStartInfo psi = new ProcessStartInfo("./Patched.exe");
				psi.UseShellExecute = false;
				Process.Start(psi);
			};
			this.mainPanel.Controls.Add(startGame);

			Console.SetOut(new TextBoxWriter(log));
		}
		public void SearchGame()
		{
			string reg = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Re-Logic\\Terraria", "Install_Path", "");
			string file = Path.Combine(reg, "Terraria.exe");
			if (reg != null && File.Exists(file))
			{
				GameLoaded = true;
				GameFile = file;
			}
		}
	}
}
