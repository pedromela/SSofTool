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
		public Form1()
		{
			InitializeComponent();
			manager = new Manager();
		}

		public Form1(string input)
		{
			InitializeComponent();
			manager = new Manager();
			filename = input;
		}

		private void Init() {

		}

		public void LoadJson(string file)
		{
			
			using (StreamReader r = new StreamReader("public_tests/" + file))
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
				dynamic aux_array;
				foreach (dynamic item in array)
				{
					//Console.WriteLine("=========================================================================");
					//Console.WriteLine("Item: ");
					//Console.WriteLine(item.Name);
					Newtonsoft.Json.Linq.JObject jObject = (Newtonsoft.Json.Linq.JObject)item.Value;
					Function f = new Function(jObject);
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
			object[] pars = { "av", "ac" };
			Dictionary<int, char> stack = manager.Stack();
			ripLabel.Text = manager.GetRegister("rip");
			foreach(var item in stack)
			{
				pars[0] = (0xFFFFFFFF - item.Key).ToString("X8");
				pars[1] = item.Value;
				Stack.Rows.Add(pars);

			}
		}

		private void Clear()
		{
			manager.Clear();
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			Clear();
			if (string.IsNullOrEmpty(File.Text)) {
				Console.WriteLine("Couldnt load file!");
				//return;
			}

			string file = File.Text;
			if(filename != null)
			{
				LoadJson(filename);
			} else
			{
				if (string.IsNullOrEmpty(file))
				{
					LoadJson("test01.json");
				}
				else
				{
					LoadJson(file);
				}
			}
			LoadCode();
			LoadStack();
		}
	}
}
