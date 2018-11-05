using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSofTool
{
	class Manager
	{
		private List<Function> functions;
		private Dictionary<string, Function> functions_dict;

		public Manager()
		{
			functions = new List<Function>();
			functions_dict = new Dictionary<string, Function>();
		}

		public void AddFunction(string name, Function f)
		{
			functions_dict.Add(name, f);
			functions.Add(f);
		}
		public void Clear()
		{
			functions.Clear();
			functions_dict.Clear();
		}
		public Function GetFunction(string name) {
			functions_dict.TryGetValue(name, out Function f);
			return f;
		}
		 
		public List<Function> GetFunctions()
		{
			return functions;
		}

		public string RenderCode()
		{
			string res = "";

			foreach (var pair in functions_dict)
			{
				res += pair.Key + ":\n" + pair.Value.Render();
			}
			return res;

		}
		public Dictionary<int, char> Stack()
		{
			Dictionary<int, char> stack = new Dictionary<int, char>();
			int pointer = 0;
			foreach (Function f in functions)
			{
				foreach (Instruction instr in f.GetInstructions())
				{
					switch (instr.op)
					{
						case "sub":

							break;
						case "push":
							int len = instr.args.Length;
							if(len == 1)
							{
								string value = instr.args[0].ToString();
								if (value.Equals("rbp"))
								{
									foreach (char c in instr.address)
									{
										stack.Add(pointer,c);
										pointer++;
									}
								}
							}
							break;
						case "mov":

							break;
						case "lea":

							break;
						case "call":

							break;
						case "leave":

							break;
						case "ret":

							break;
						default:
							break;
					}
				}
			}

			return stack;
		}
		public List<KeyValuePair<string, string>> RenderStack()
		{
			List<KeyValuePair<string, string>> stack = new List<KeyValuePair<string, string>>();



			return stack;
		}
	}
}
