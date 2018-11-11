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
        private Dictionary<string,string> registers;
		private Dictionary<string, string> cstuff;

		public int n_instructions = 0;
		
		/*auxliary stuff*/
		private Dictionary<int, char> stack;
		private int pointer;
		private Stack<Frame> frames;

		public Manager()
		{
			cstuff = new Dictionary<string, string>(); 
			functions = new List<Function>();
			functions_dict = new Dictionary<string, Function>();
            registers = new Dictionary<string, string>();
			stack = new Dictionary<int, char>();
			frames = new Stack<Frame>();
			pointer = 0;
			InitializeRegisters();
		}

		public void InitializeRegisters()
		{
			registers.Add("rdi", "00000000");
			registers.Add("esi", "0000");
			registers.Add("eax", "0000");
			registers.Add("rax", "00000000");
			registers.Add("rdx", "00000000");
			registers.Add("rip", "00000000");
			registers.Add("rbp", "FFFFFFFF");
			registers.Add("rsp", "FFFFFFFF");
		}

		private static Random random = new Random();
		public static string RandomString(int length)
		{
			const string chars = "0123456789ABCDEF";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
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
			cstuff.Clear();
			registers.Clear();
			stack.Clear();
			frames.Clear();
			pointer = 0;
			n_instructions = 0;
			InitializeRegisters();
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
		public string GetRegister(string name)
		{
			if(registers.ContainsKey(name))
			{
				return registers[name];
			}
			return null;
		}

		public void SetRip(Instruction instr)
		{
			registers["rip"] = AutoComplete(instr.address, 8);
			//n_instructions++;
		}
		public string AutoComplete(string str, int size)
		{
			int len = str.Length;
			if(len < size)
			{
				for(int i = 0; i < size - len; i++)
				{

					str = '0' + str; // += '0';
				}
			}
			return str;
		}

		public string ParseHex(string str, int size)
		{
			str = str.Trim();
			if (str.StartsWith("0x"))
			{
				string hex = str.Substring(2);
				return AutoComplete(hex, size);
			}
			else
			{
				return null;
			}
		}

		public Dictionary<int, char> Stack(string name)
		{
			Function f = functions_dict[name];
			
			foreach (Instruction instr in f.GetInstructions())
			{
				SetRip(instr);
				//Console.WriteLine("rsp : " + registers["rsp"]);
				//Console.WriteLine("rbp : " + registers["rbp"]);

				Console.WriteLine("{0}", instr.op);
				Console.WriteLine("{0}", pointer);

				switch (instr.op)
				{
					case "sub":
						if(instr.args.Length == 2)
						{
							if (instr.args[0].ToString().Equals("rsp"))
							{
								int intValue = Convert.ToInt32(instr.args[1].ToString(), 16);
								Frame frame = new Frame(pointer, pointer + intValue - 1);
								frames.Push(frame);
								for (int i = 0; i < intValue; i++)
								{
									//s.Push('0');
									stack.Add(pointer, '0');
									pointer++;
								}
								//registers["rsp"] = (Convert.ToInt32(registers["rbp"], 16) - intValue).ToString("X8");
								registers["rsp"] = (0xFFFFFFFF - pointer).ToString("X8");

								//Console.WriteLine("limites: {0} - {1} ", frame.start, frame.end);
								//Console.WriteLine((Convert.ToInt32(registers["rbp"],16) - intValue).ToString("X8"));
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
								int len = registers["rbp"].Length;
								while (len != 8)
								{
									stack.Add(pointer, '0');
									pointer++;
									len++;
								}
								foreach (char c in registers["rbp"])
								{
									if (stack.ContainsKey(pointer))
									{
										Console.WriteLine("bad pointer : " + pointer + " , c : " + c);
									}else
									{
										//Console.WriteLine("pointer : " + pointer + " , c : " + c);

										stack.Add(pointer, c);
										pointer++;

									}
								}
								registers["rsp"] = (0xFFFFFFFF - pointer).ToString("X8");
							}
						}
						break;
					case "mov":
						if (instr.args.Length > 1)
						{
								
							string arg1 = instr.args[0].ToString();
							string[] tokens = arg1.Split(' ');
							if (tokens.Length == 3)
							{
								if ((tokens[0].Substring(1) + tokens[1]).Equals("WORDPTR"))
								{
									string x = arg1.Substring(10);
									if (x.Contains("rbp"))
									{

										x = x.Trim('[', ']');
										string[] args = x.Split('-');
										if (args.Length == 2)
										{
											int intValue = Convert.ToInt32(args[1], 16);
											if (!string.IsNullOrEmpty(instr.args[1].ToString()))
											{
												Frame frame = frames.First();
												int i = frame.start + intValue - 1;
												int n = 0;
												if (instr.args[1].ToString().First() == 'Q')
												{
													n = 8;
												}
												if (instr.args[1].ToString().First() == 'D')
												{
													n = 4;
												}
												string number = AutoComplete(instr.args[1].ToString().Substring(2), n);
												Console.WriteLine("AQUI : " + number);
												foreach (char c in number)
												{
													stack[i] = c;
													i--;
												}
											}
										}
									}
								}
							}
							else
							{
								foreach (var r in registers)
								{
									if (arg1.Equals(r.Key))
									{
										if (instr.args.Length == 3)
										{
											string[] toks = instr.args[2].ToString().Split(' ');
											int n = 0;
											if(instr.args[1].ToString().First() == 'Q')
											{
												n = 8;
											}
											if (instr.args[1].ToString().First() == 'D')
											{
												n = 4;
											}
											if (toks.Length == 3)
											{
												instr.args[instr.args.Count() - 1] = toks[1];
												toks[1] = AutoComplete(toks[1], n);
												
												registers[r.Key] = toks[1];
												cstuff.Add(toks[1], toks[2]);
											}
										}
										if(instr.args.Length == 2)
										{

											string aux = ParseHex(instr.args[1].ToString(), r.Value.Length);

											if (aux != null)
											{
												registers[r.Key] = aux;
											}
											else
											{
												foreach (var reg in registers)
												{
													if (instr.args[1].ToString().Equals(reg.Key))
													{
														registers[r.Key] = registers[reg.Key];
														break;
													}
												}
											}
										}
										break;
									}
								}
							}
						}
						break;
					case "lea":
                        //Loads address in register
                            
                        if (instr.args.Length == 2)
                        {
                                
                            string registername = instr.args[0].ToString(); //Register we're saving address in
                            string args1 = instr.args[1].ToString(); // Something in shape of [rbp-0x50]
                            string address = args1.Trim('[', ']'); // address = rbp-0x50
                            string[] split = address.Split('-');
                            if (split.Length == 2 && split[0] == "rbp")
                            {
                                int intValue = Convert.ToInt32(split[1], 16); //Address starting from RBP to save
                                Frame frame = frames.First();
                                registers[registername] = (Convert.ToInt32(registers["rbp"], 16) - intValue).ToString("X8");
                                //Console.WriteLine("HELLO, REGISTER {0} CHARGED WITH ADDRESS {1}", registername, registers[registername]);
                            }
                        }
						break;
					case "call":
						if (instr.args.Length < 1)
						{
							Console.WriteLine("This call makes no sense");
						}

						//fgets is dangerous, there's 3 arguments  that are put in registers before calling an fgets
						//the buffer (LEA'd into register)
						//the buff_len (mov'd into register)
						//the stdinstream (This is accessed via mov [rip-"code"])
						string buffstart = registers["rdi"];
						int bufflen = Convert.ToInt32(registers["esi"], 16);
						string rip = registers["rip"];
						string input = registers["rdx"];
						switch (instr.args[0].ToString())
						{
							//case for dangerous function calls
							case "<fgets@plt>":
								if(cstuff.ContainsKey(input))
								{
									//Console.WriteLine("input: " + cstuff[input]);

									if (cstuff[input].Equals("<stdin@@GLIBC_2.2.5>"))
									{
										input = RandomString(bufflen);
										Frame frame = frames.First();
										Console.WriteLine("Frame end : " + frame.end);

										for (int i = 0; i < bufflen; i++)
										{
											if (stack.ContainsKey(frame.end - i))
											{
												stack[frame.end - i] = input[i];
												Console.WriteLine("line {0}, pointer {1}", instr.address, frame.end - i);

											}
											else
											{
												Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, frame.end - i);
											}
										}
									}
								}

								break;
							case "<strcpy@plt>":
								//Console.WriteLine("{0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);
								if (cstuff.ContainsKey(input))
								{
									//Console.WriteLine("input: " + cstuff[input]);

									if (cstuff[input].Equals("<stdin@@GLIBC_2.2.5>"))
									{
										input = RandomString(bufflen);
										Frame frame = frames.First();
										Console.WriteLine("Frame end : " + frame.end);
										for (int i = 0; i < bufflen; i++)
										{
											if (stack.ContainsKey(frame.end - i))
											{
												stack[frame.end - i] = input[i];
											}else
											{
												Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, frame.end - i);
											}
										}
									}
								}

								break;
							default:
								Console.WriteLine("function : {0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);
								name = instr.args[0].ToString().Trim('<', '>');
								if (functions_dict.ContainsKey(name))
								{
									//pointer++;
									stack.Concat(Stack(name));
									frames.Pop();
								}
								break;
						}
						break;

                            
					case "leave":

						break;
					case "ret":

						break;
					default:
						break;
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
    public class Register
    {
        private string name;
        private int value;

        public Register(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
