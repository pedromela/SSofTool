using Newtonsoft.Json;
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

namespace SSofTool
{

	public partial class Form1 : Form
	{
		private Manager manager;
		private string filename = null;
		private string fileout = null;
		public Form1()
		{
			InitializeComponent();
			manager = new Manager();
		}

		public Form1(string input)
		{
			//InitializeComponent();
			manager = new Manager();
			filename = input;
			//string[] toks = filename.Split('.');
			fileout = filename.Substring(0, filename.Length - 4) + "output.json";
			LoadJson(filename);
			//LoadCode();
			LoadStackCMD();
		}

		private void Init() {

		}

		public void LoadJson(string file)
		{
			
			using (StreamReader r = new StreamReader(file))
			{
				string json = r.ReadToEnd();
				if(string.IsNullOrEmpty(json))
				{
					Console.WriteLine("Couldn't load Json");
				}
				//Console.WriteLine("=========================================================================");
				//Console.WriteLine("Json :");
				//Console.WriteLine(json);
				//Console.WriteLine("=========================================================================");

				dynamic array = JsonConvert.DeserializeObject(json);
				foreach (dynamic item in array)
				{
					//Console.WriteLine("=========================================================================");
					//Console.WriteLine("Item: ");
					//Console.WriteLine(item.Name);
					Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)item.Value;
					Function f = new Function(jObject);
					manager.n_instructions += f.Ninstructions;
					manager.AddFunction(item.Name, f);
					//Console.WriteLine("=========================================================================");

				}
			}

		}

		private void LoadCode()
		{
			//Console.WriteLine(manager.RenderCode());
			Code.Text = manager.RenderCode();
		}

		private void LoadStack()
		{
			Stack.Rows.Clear();
			//stack.
			//Stack.ClearSelection();
			
			object[] pars = { "av", "ac" };
			manager.f = manager.GetFunction("main");
			manager.PushRetAddr("_MAINRET");
			Dictionary<int, char> stack = manager.Stack();
			ripLabel.Text = manager.GetRegister("rip");
			int pointer = 0;
			for(pointer = 0; stack.ContainsKey(pointer); pointer++) {
				pars[0] = (0xFFFFFFFF - pointer).ToString("X8");
				pars[1] = stack[pointer];
				Stack.Rows.Add(pars);
			}
			//foreach (var item in stack)
			//{
			//	//pars[0] = item.Key;
			//	pars[0] = (0xFFFFFFFF - item.Key).ToString("X8");
			//	pars[1] = item.Value;
			//	Stack.Rows.Add(pars);
			//}
			manager.WriteJson(fileout);
		}

		private void LoadStackCMD()
		{
			manager.f = manager.GetFunction("main");
			manager.PushRetAddr("_MAINRET");
			Dictionary<int, char> stack = manager.Stack();
			manager.WriteJson(fileout);
		}

		private void Clear()
		{
			manager.Clear();
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			Clear();
			//manager.n_instructions = (int) instruction.Value;
			//instruction.Value = manager.n_instructions;
			if (string.IsNullOrEmpty(File.Text)) {
				Console.WriteLine("Couldnt load file!");
				//return;
			}

			string file = File.Text;
			
			if (string.IsNullOrEmpty(file))
			{
				LoadJson("public_tests/test15.json");
			}
			else
			{
				Console.WriteLine("Loading {0}...", file);

				LoadJson(file);
			}
			
			LoadCode();
			LoadStack();
			instruction.Value = manager.n_instructions;

		}

		private void Next_Click(object sender, EventArgs e)
		{
			
		}

		private void instruction_ValueChanged(object sender, EventArgs e)
		{

		}

		private void Prev_Click(object sender, EventArgs e)
		{
			Clear();
			manager.n_instructions = (int)instruction.Value - 1;
			//instruction.Value = manager.n_instructions;
			if (string.IsNullOrEmpty(File.Text))
			{
				Console.WriteLine("Couldnt load file!");
				//return;
			}

			LoadCode();
			LoadStack();
		}
	}
}
