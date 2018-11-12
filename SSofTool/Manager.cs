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
			cstuff = new Dictionary<string, string>(); // <address , value>
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
			registers.Add("rsi", "00000000");
			registers.Add("esi", "0000");
			registers.Add("eax", "0000");
			registers.Add("rax", "00000000");
			registers.Add("rdx", "00000000");
			registers.Add("rip", "00000000");
			registers.Add("rbp", "FFFFFFFF");
			registers.Add("rsp", "FFFFFFFF");
		}

		private static Random random = new Random();
		public string RandomString(int length)
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
		public static string AutoComplete(string str, int size)
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


		private string GetString(string addr, int n)
		{
			//Console.WriteLine("addr_Val : " + addr);
			long addr_val = ParseToPointer(addr);
			//Console.WriteLine("addr_Val : " + addr_val);
			string str = "";
			while (n > 0)
			{
				n--;
				str += stack[(int) addr_val];
				addr_val--;
			}
			return str;
		}

		private string GetString(string addr)
		{
			//Console.WriteLine("addr_Val : " + addr);
			long addr_val = ParseToPointer(addr);
			//Console.WriteLine("addr_Val : " + addr_val);
			string str = "";
			while(stack.ContainsKey((int) addr_val)) // refatorizar tudo pra long , cast pra int temporario
			{
				if(stack[(int)addr_val] != '#')
				{
					str += stack[(int )addr_val];
				}
				else
				{
					str += stack[(int)addr_val];
					break;
				}
				addr_val--;
			}
			return str;
		}

		public long ParseToPointer(string addr)
		{

			int len = addr.Length;
			if (len == 8)
			{
			}
			return 0xFFFFFFFF - Convert.ToInt64(addr, 16);

			//else if(len == 16) {
			//	return Convert.ToInt64(addr, 16);
			//}
			return 0;
		}

		public int ParseAddr(string addr)
		{
			int len = addr.Length;
			if (len == 8)
			{
			}
			return Convert.ToInt32(addr, 16);

			//else if(len == 16) {
			//	return Convert.ToInt64(addr, 16);
			//}
			return 0;
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
		public string SubReg(string reg, int value)
		{
			return (Convert.ToInt32(registers[reg],16) - value).ToString("X8");
		}

		public Dictionary<int, char> Stack(string name)
		{
			Function f = functions_dict[name];
			Frame frame;

			foreach (Instruction instr in f.GetInstructions())
			{
				SetRip(instr);
				//Console.WriteLine("rdx : " + registers["rdx"]);
				//Console.WriteLine("rdi : " + registers["rdi"]);
				Console.WriteLine("rbp : " + registers["rbp"]);
				Console.WriteLine("rsp : " + registers["rsp"]);

				Console.WriteLine("{0}", instr.op);
				//Console.WriteLine("{0}", pointer);

				switch (instr.op)
				{
					case "sub":
						if(instr.args.Length == 2)
						{
							if (instr.args[0].ToString().Equals("rsp"))
							{
								int intValue = Convert.ToInt32(instr.args[1].ToString(), 16);
								frame = new Frame(pointer, pointer + intValue - 1);
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
								if ((tokens[0].Substring(1) + tokens[1]).Equals("WORDPTR")) // caso em que o primeiro argumento é do tipo WORD PTR [addr]
								{
									string x = arg1.Substring(10);
									if (x.Contains("rbp"))
									{
										string addr = x;
										x = x.Trim('[', ']');
										string[] args = x.Split('-');
										if (args.Length == 2) // do tipo rbp - x
										{
											//Console.WriteLine("buff1 addr: " + args[1]);

											int intValue = Convert.ToInt32(args[1], 16);
											string value = instr.args[1].ToString();
											if (!string.IsNullOrEmpty(value))
											{
												string reg = SubReg("rbp", intValue);
												if (f.HasVariable(x))
												{
													//Console.WriteLine("buff1 : " + reg);
													if (cstuff.ContainsKey(reg))
													{
														cstuff[reg] = x;
													}
													else
													{
														cstuff.Add(reg, x);
													}
												}
												frame = frames.First();
												int i = frame.start + intValue - 1;

												string ToWrite = "";

												if (instr.args[1].ToString().StartsWith("0x"))
												{
													int n = 0;
													if (arg1.First() == 'Q')
													{
														n = 8;
													}
													if (arg1.First() == 'D')
													{
														n = 4;
													}
													ToWrite = AutoComplete(value.Substring(2), n);
												} else if(registers.ContainsKey(value))
												{
													ToWrite = registers[value];
												}
												foreach (char c in ToWrite)
												{
													stack[i] = c;
													i--;
												}
											}
										}
									}
								}
							}
							else // caso em que o primeiro argumento é um registo
							{
								foreach (var r in registers)
								{
									if (arg1.Equals(r.Key)) // achar o primeiro registo
									{
										if (instr.args.Length == 3) // 3 args -> tem comentario do tipo # 601040 <stdin@@GLIBC_2.2.5>
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
										else if(instr.args.Length == 2) // 2  args 
										{
											string arg = instr.args[1].ToString();

											string[] toks = instr.args[1].ToString().Split(' '); //arg.Split(' ');
											string aux = ParseHex(instr.args[1].ToString(), r.Value.Length);

											if (aux != null) //mov reg1, 0x00...
											{
												registers[r.Key] = aux;
											}
											else if(toks.Length == 3) // mov reg, WORD PTR [addr]
											{
												//string addr = arg1.Substring(4);
												if (toks[2].First() == '[' && toks[2].Last() == ']') 
												{

													string addr = toks[2].Trim('[',']');
													string[] args = addr.Split('-');

													if (args.Length == 2) // 
													{
														int intValue = Convert.ToInt32(args[1].Substring(2), 16);
														if (!string.IsNullOrEmpty(instr.args[1].ToString()))
														{
															string reg = SubReg(args[0], intValue);
															//Console.WriteLine("buff1 addr: " + addr);
															int n = 0;
															if (toks[0].First() == 'Q')
															{
																n = 8;
															}
															else if (toks[0].First() == 'D')
															{
																n = 4;
															}
															//Console.WriteLine("mov {0}  , {1}", instr.args[0].ToString(), reg);
															//Console.WriteLine("mov {0}  , {1}", instr.args[0].ToString()), instr.args[1].ToString());

															registers[instr.args[0].ToString()] = reg;
														}
													}


												}
											}
											else // mov reg1, reg2
											{
												foreach (var reg in registers)
												{
													if (arg.Equals(reg.Key))
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
                            {																			  //Console.WriteLine("buff2 : " + SubReg("rbp", intValue));
								string x = args1.Trim('[', ']');
								if (f.HasVariable(x))
								{
									int intValue = Convert.ToInt32(split[1], 16); //Address starting from RBP to save
									string reg = SubReg("rbp", intValue);
									if (cstuff.ContainsKey(reg))
									{
										cstuff[reg] = x;
									}
									else
									{
										cstuff.Add(reg, x);
									}
									//Frame frame = frames.First();
									registers[registername] = (Convert.ToInt32(registers["rbp"], 16) - intValue).ToString("X8");
								}else
								{
									Console.WriteLine("lea : variable {0} doesnt exist.", x);

								}
							}
                        }
						break;
					case "call":
						if (instr.args.Length < 1)
						{
							Console.WriteLine("This call makes no sense");
						}
						Console.WriteLine(instr.args[0]);
						//fgets is dangerous, there's 3 arguments  that are put in registers before calling an fgets
						//the buffer (LEA'd into register)
						//the buff_len (mov'd into register)
						//the stdinstream (This is accessed via mov [rip-"code"])
						string buffstart = registers["rdi"];
						int bufflen = Convert.ToInt32(registers["esi"], 16);
						string rip = registers["rip"];
						string rdx = registers["rdx"];
						string input;
						Console.WriteLine("default functions input address: " + rdx);
						switch (instr.args[0].ToString())
						{
							//case for dangerous function calls
							case "<fgets@plt>":
								if(cstuff.ContainsKey(rdx))
								{
									//Console.WriteLine("input: " + cstuff[input]);

									if (cstuff[rdx].Equals("<stdin@@GLIBC_2.2.5>"))
									{
										if (cstuff.ContainsKey(buffstart))
										{
											//Console.WriteLine("cstuff[buffstart] : " + cstuff[buffstart]);
											Variable v = f.GetVariable(cstuff[buffstart]);
											if (v != null)
											{
												int varmaxlen = v.bytes;
												input = RandomString(bufflen);
												frame = frames.First();
												int i;
												for (i = 0; i < bufflen; i++)
												{
													if (i < varmaxlen)
													{
														if (stack.ContainsKey(frame.end - i))
														{
															stack[frame.end - i] = input[i];
															//Console.WriteLine("line {0}, pointer {1}", instr.address, frame.end - i);

														}
														else
														{
															Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, frame.end - i);
														}
													}
													else
													{
														Console.WriteLine("OVERFLOW : fgets CANT RIDE OUTSIDE VARIABLE BAUNDARIES var {0}, pointer {1}", v.name, frame.end - i);
													}
												}

												stack[i < varmaxlen ? frame.end - i : frame.end - varmaxlen] = '#';
											}else
											{
												Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
											}
										}
										else
										{
											Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
										}
									}
								}

								break;
							case "<strcpy@plt>":
								//Console.WriteLine("{0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);

								if (cstuff.ContainsKey(buffstart))
								{
									//Console.WriteLine("buffstart : " + buffstart);
									//Console.WriteLine("cstuff[buffstart] : " + cstuff[buffstart]);
									//Console.WriteLine("input : " + input);

									Variable v = f.GetVariable(cstuff[buffstart]);
									if (v != null)
									{
										int varmaxlen = v.bytes;
										input = GetString(rdx);
										frame = frames.First();
										//Console.WriteLine("frame end : " + frame.end +" , bytes : " + v.bytes);
										Console.WriteLine("strcpy input : " + input);
										//int var_size = f.GetVariable().bytes;
										for (int i = 0; i < input.Length; i++)
										{
											if (i < varmaxlen)
											{
												if (stack.ContainsKey(frame.end - i))
												{
													stack[frame.end - i] = input[i];
													//Console.WriteLine("line {0}, pointer {1}", instr.address, frame.end - i);

												}
												else
												{
													Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, frame.end - i);
												}
											}
											else
											{
												Console.WriteLine("OVERFLOW : strcpy CANT RIDE OUTSIDE VARIABLE BAUNDARIES var {0}, pointer {1}", v.name, frame.end - i);
											}
										}
									}
									else
									{
										Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
									}
								}
								else
								{
									Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
								}
								break;
							default:
								//Console.WriteLine("function : {0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);
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
