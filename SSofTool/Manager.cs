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
			Stack<char> s = new Stack<char>();
			//Stack<string> frames
			foreach (Function f in functions)
			{
				foreach (Instruction instr in f.GetInstructions())
				{
					switch (instr.op)
					{
						case "sub":
							if(instr.args.Length == 2)
							{
								if (instr.args[0].ToString().Equals("rsp"))
								{
									Console.WriteLine("RSP: " + instr.args[1].ToString());
									int intValue = Convert.ToInt32(instr.args[1].ToString(), 16);
									for(int i = 0; i < intValue; i++)
									{
										//s.Push('0');
										stack.Add(pointer, '0');
										pointer++;
									}
									Console.WriteLine("RSP: " + intValue);
								}
							}
							break;
						case "push":
							
							int l = instr.args.Length;
							if(l == 1)
							{
								string value = instr.args[0].ToString();
								if (value.Equals("rbp"))
								{
									int len = instr.address.Length;
									while (len != 8)
									{
										s.Push('0');
										stack.Add(pointer, '0');
										pointer++;
										len++;
									}
									foreach (char c in instr.address)
									{
										s.Push(c);
										stack.Add(pointer,c);
										pointer++;
									}
								}
							}
							break;
						case "mov":
							if (instr.args.Length > 1)
							{
								Console.WriteLine("{0} {1}, {2}", instr.op, instr.args[0], instr.args[1]);

								string arg1 = instr.args[0].ToString();
								string[] tokens = arg1.Split(' ');
								if(tokens.Length == 3)
								{
									if ((tokens[0].Substring(1) + tokens[1]).Equals("WORDPTR"))
									{
										string x = arg1.Substring(10);
										if (x.Contains("rbp"))
										{
											x = x.Trim('[', ']');
											string[] args = x.Split('-');
											if(args.Length == 2)
											{
												int intValue = Convert.ToInt32(args[1], 16);
												Console.WriteLine("WORD PTR " + intValue);
											}
										}
									}
								}
								foreach(string str in tokens)
								{
									Console.WriteLine(str);
								}
							}
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
