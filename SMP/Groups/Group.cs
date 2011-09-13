﻿/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Text;

/*TODO
 * tracks
 * inheritance
 * grouputils
 * Saving everything :p
 * Commands
*/

namespace SMP
{
    public class Group
    {
        public static List<Group> GroupList = new List<Group>();
        public static Group DefaultGroup;
		public static Dictionary<string, List<Group>> TracksDictionary = new Dictionary<string, List<Group>>(); //holds the all the tracks
		public List<string> Tracks = new List<string>(); //holds whatever track(s) it is a part of, used to reference Dictionary id
        public string Name;
        public bool IsDefaultGroup = false;
        public bool CanBuild = false;
        public string Prefix = "";
        public string Suffix = "";
        public string GroupColor = Color.Gray;
        public List<string> PermissionList = new List<string>();
		public List<string> InheritedPermissionList = new List<string>();
        public List<Group> InheritanceList = new List<Group>();
        public List<string> tempInheritanceList = new List<string>();

        /// <summary>
        /// Checks if a player has permission to use a command
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckPermission(Player p, String perm)
        {
			List<string> nodes = new List<string>();
			nodes.AddRange(GetParentNodes(perm));
			
			foreach(string node in nodes)
			{
				if(p.group.PermissionList.Contains("-" + node))
				   return false;
				else if(p.AdditionalPermissions.Contains("-" + node))
				   return false;
				else if(p.AdditionalPermissions.Contains(node) || p.group.PermissionList.Contains(node))
				   return true;
				else if(p.group.InheritedPermissionList.Contains("-" + node))
				   return false;
				else if(p.group.InheritedPermissionList.Contains(node))
				   return true;
			}
			
			if(p.group.PermissionList.Contains("*") || p.AdditionalPermissions.Contains("*") || p.group.InheritedPermissionList.Contains("*"))
				return true;
			
			return false;
        }

        /// <summary>
        /// Finds a group by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Group FindGroup(string name)
        {
            foreach (Group g in GroupList)
            {
                if (g.Name.ToLower() == name.ToLower())
                    return g;
            }
            return null;
        }
		
		private static List<string> GetParentNodes(string perm)
		{
			string[] nodearray = perm.Split('.');
			List<string> nodeList = new List<string>();
			
			for(int i = 0; i < nodearray.Length; i++)
			{
				StringBuilder sb = new StringBuilder("");
				
				for(int ix = 0; ix <= i; ix ++)
				{
					sb.Append(nodearray[ix] + ".");
				}
				
				sb.Remove(sb.Length - 1, 1);
				nodeList.Add(sb.ToString());
				sb.Append(".*");
				nodeList.Add(sb.ToString());
			}
			
			nodeList.Reverse();
			return nodeList;
			
		}
		
		#region LOADING/SAVING
		public static void LoadGroups()
		{
			System.Data.DataTable dt = new System.Data.DataTable();
			
			try
			{
				dt = Server.SQLiteDB.GetDataTable("SELECT * FROM Groups;");
			}
			catch{Server.Log("Something went wrong loading groups");}
			
			for(int i = 0; i < dt.Rows.Count; i++)
			{
				Group g = new Group();
				
				g.Name = dt.Rows[i]["Name"].ToString();
				
				if (dt.Rows[i]["IsDefault"].ToString() == "1")
				{
					g.IsDefaultGroup = true;
					Group.DefaultGroup = g;
				}
				
				if (dt.Rows[i]["CanBuild"].ToString() == "1")
					g.CanBuild = true;
				
				g.Prefix = dt.Rows[i]["Prefix"].ToString();
				g.Suffix = dt.Rows[i]["Suffix"].ToString();
				
				g.GroupColor = dt.Rows[i]["Color"].ToString();
				
				if (g.GroupColor.Length == 2 && g.GroupColor[0] == '%' || g.GroupColor[0] == '§' || g.GroupColor[0] == '&')
					if (Color.IsColorValid((char)g.GroupColor[1]))
					    g.GroupColor = "§" + g.GroupColor[1];
				else if (g.GroupColor.Length == 1 && Color.IsColorValid((char)g.GroupColor[0]))
				 	g.GroupColor = "§" + g.GroupColor[1];
				
				string[] perms = dt.Rows[i]["Permissions"].ToString().Replace(" ", "").Split(',');
				foreach(string s in perms)
				{
					Server.Log("S: " + s);
					
					string perm;
					if (s[0] == '-')
						perm = "-" + Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s.Substring(1) + "';");
					else
						perm = Server.SQLiteDB.ExecuteScalar("SELECT Node FROM Permission WHERE ID = '" + s + "';");
					
					
					Server.Log("Perm: " + perm);
					
					if (perm.Substring(0,1) == "-" && !g.PermissionList.Contains(perm.Substring(1)))
						g.PermissionList.Add(perm);
					else if (perm.Substring(0,1) != "-" && !g.PermissionList.Contains("-" + perm))
						g.PermissionList.Add(perm);
				}
				
				string temp = dt.Rows[i]["Inheritance"].ToString().Replace(" ", "");
				string[] inheritance = temp.Split(',');
				if (inheritance.Length >= 1)
				{
					foreach(string s in inheritance)
					{
						if (!String.IsNullOrEmpty(s))
							g.tempInheritanceList.Add(Server.SQLiteDB.ExecuteScalar("SELECT Name FROM Groups WHERE ID = '" + s + "';"));	
					}
				}
				
				Group.GroupList.Add(g);
			}
			
			foreach(Group g in Group.GroupList)
			{
				foreach(string s in g.tempInheritanceList)
				{
					Group gr = Group.FindGroup(s);
					if (gr != null)
					{
						g.InheritanceList.Add(gr);
					}
				}
			}
		}
		
		#endregion
    }
}
