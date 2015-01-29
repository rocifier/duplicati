#region Disclaimer / License
// Copyright (C) 2011, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicati.Library.Utility
{
    /// <summary>
    /// Common class for parsing commandline options
    /// </summary>
    public static class CommandLineParser
    {
        /// <summary>
        /// Reads a list of command line arguments, and removes the options passed,
        /// and returns them in a dictionary. Options are expected to be in the format
        /// --name=value or --name, both value and name may be enclosed in double quotes
        /// </summary>
        /// <returns>The parsed list of commandline options</returns>
        /// <param name='args'>The commandline arguments</param>
        public static Dictionary<string, string> ExtractOptions(List<string> args, Func<string, string, bool> parserCallback = null)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string key = null;
                    string value = null;
                    if (args[i].IndexOf("=") > 0)
                    {
                        key = args[i].Substring(0, args[i].IndexOf("="));
                        value = args[i].Substring(args[i].IndexOf("=") + 1);
                    }
                    else
                        key = args[i];

                    //Skip the leading --
                    key = key.Substring(2).ToLower();
                    if (!string.IsNullOrEmpty(value) && value.Length > 1 && value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2);

                    //Last argument overwrites the current
					if (parserCallback == null || parserCallback(key, value))
	                    options[key] = value;

                    args.RemoveAt(i);
                    i--;
                }
            }

            return options;
        }
    }
}
