using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSofTool
{
	class Variable
	{
		public int bytes;
		public string type;
		public string name;
		public string address;
		public string stackAddr;

		public Variable() { }
		public Variable(int _bytes, string _type, string _name, string _address) {
			bytes = _bytes;
			type = _type;
			name = _name;
			address = _address;
		}


	}
}
