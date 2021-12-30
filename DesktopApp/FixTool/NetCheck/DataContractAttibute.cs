using System;
using System.Collections.Generic;
using System.Text;

namespace NetCheck
{
	public class DataContractAttribute : Attribute
	{
	}

	public class IgnoreDataMemberAttribute : Attribute
	{
	}

	public class DataMemberAttribute : Attribute
	{
		public string Name { get; set; }
		public int Order { get; set; }
	}
}
