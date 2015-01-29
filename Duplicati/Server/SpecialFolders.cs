﻿//  Copyright (C) 2011, Kenneth Skovhede

//  http://www.hexad.dk, opensource@hexad.dk
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using System.Linq;
using System.Collections.Generic;

namespace Duplicati.Server
{
    public static class SpecialFolders
    {
        public static readonly Serializable.TreeNode[] Nodes;
        private static readonly Dictionary<string, string> PathMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, string> DisplayMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        
        public static string ExpandEnvironmentVariables(string path)
        {
            foreach(var n in Nodes)
                path = path.Replace(n.id, n.resolvedpath);
            return System.Environment.ExpandEnvironmentVariables(path);
        }
        
        public static string TranslateToPath(string str) 
        {
            string res;
            if (PathMap.TryGetValue(str, out res))
                return res;
            
            return null;
        }

        public static string TranslateToDisplayString(string str) 
        {
            string res;
            if (DisplayMap.TryGetValue(str, out res))
                return res;
            
            return null;
        }

        private static void TryAdd(List<Serializable.TreeNode> lst, System.Environment.SpecialFolder folder, string id, string display)
        {
            try
            {
                TryAdd(lst, System.Environment.GetFolderPath(folder), id, display);
            }
            catch
            {
            }
        }

        private static void TryAdd(List<Serializable.TreeNode> lst, string folder, string id, string display)
        {
            try
            {
                if (System.IO.Path.IsPathRooted(folder) && System.IO.Directory.Exists(folder))
                {
                    lst.Add(new Serializable.TreeNode() {
                        id = id,
                        text = display,
                        leaf = false,
                        iconCls = "x-tree-icon-special",
                        resolvedpath = folder
                    });
                    
                    PathMap[id] = folder;
                    DisplayMap[id] = display;
                }
            }
            catch
            {
            }
        }
        
        static SpecialFolders()
        {
            var lst = new List<Serializable.TreeNode>();
            
            if (!Library.Utility.Utility.IsClientLinux)
            {
                TryAdd(lst, Environment.SpecialFolder.MyDocuments, "%MY_DOCUMENTS%", "My Documents");
                TryAdd(lst, Environment.SpecialFolder.MyMusic, "%MY_MUSIC%", "My Music");
                TryAdd(lst, Environment.SpecialFolder.MyPictures, "%MY_PICTURES%", "My Pictures");
                TryAdd(lst, Environment.SpecialFolder.MyVideos, "%MY_VIDEOS%", "My Videos");
                TryAdd(lst, Environment.SpecialFolder.DesktopDirectory, "%DESKTOP%", "Desktop");
                TryAdd(lst, Environment.SpecialFolder.ApplicationData, "%APPDATA%", "Application Data");
            } else {
                TryAdd(lst, Environment.SpecialFolder.MyDocuments, "%MY_DOCUMENTS%", "My Documents");
                TryAdd(lst, Environment.SpecialFolder.MyMusic, "%MY_MUSIC%", "My Music");
                TryAdd(lst, Environment.SpecialFolder.MyPictures, "%MY_PICTURES%", "My Pictures");
                TryAdd(lst, Environment.SpecialFolder.DesktopDirectory, "%DESKTOP%", "Desktop");
            }
            
            TryAdd(lst, Library.Utility.Utility.IsClientLinux ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), "%HOME%", "Home");
            
            Nodes = lst.ToArray();
        }

        internal static Dictionary<string, string> GetSourceNames(Serialization.Interface.IBackup backup)
        {
            var systemIO = Duplicati.Library.Snapshots.SnapshotUtility.SystemIO;

            return backup.Sources.Distinct().Select(x => {
                    var sp = SpecialFolders.TranslateToDisplayString(x);
                    if (sp != null)
                        return new KeyValuePair<string, string>(x, sp);

                    x = SpecialFolders.ExpandEnvironmentVariables(x);
                    try {
                        var nx = x;
                        if (nx.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                            nx = nx.Substring(0, nx.Length - 1);
                        var n = systemIO.PathGetFileName(nx);
                        if (!string.IsNullOrWhiteSpace(n))
                            return new KeyValuePair<string, string>(x, n);
                    } catch {
                    }

                    if (x.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) && x.Length > 1)
                        return new KeyValuePair<string, string>(x, x.Substring(0, x.Length - 1).Substring(x.Substring(0, x.Length - 1).LastIndexOf("/") + 1));
                    else
                        return new KeyValuePair<string, string>(x, x);

                }).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}

