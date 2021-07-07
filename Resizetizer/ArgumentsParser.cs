using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Resizetizer
{
	public static class ArgumentsParser
	{
		public static List<string> GetArguments(string source)
		{
			var results = new List<string>();

			try
			{
				var arg = new StringBuilder();

				int t = source.Length;

				for (int i = 0; i < t; i++)
				{
					char c = source[i];

					if (c == '"' || c == '\'')
					{
						arg.Append(c);
						
						char end = c;

						for (i++; i < t; i++)
						{
							c = source[i];

							if (c == end)
							{
								arg.Append(c);
								break;
							}
							arg.Append(c);
						}
					}
					else if (c == ' ')
					{
						if (arg.Length > 0)
						{
							results.Add (arg.ToString());
							arg.Length = 0;
						}
					}
					else
						arg.Append(c);
				}
				if (arg.Length > 0)
				{
					results.Add (arg.ToString());
					arg.Length = 0;
				}
			}
			catch
			{
			}

			return results;
		}
	}
}
