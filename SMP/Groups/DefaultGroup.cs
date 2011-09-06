/*
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

//just used for testing-ish groups and to atleast semi-enable group-specific actions i.e help and other commented sections
namespace SMP
{
	public class DefaultGroup : Group
	{
		public DefaultGroup ()
		{
			Name = "Default";
			//IsDefaultGroup = true;
			CanBuild = true;
			Prefix = "";
			Suffix = "";
			GroupColor = Color.Gray;
			// temp till a better permission node system is in place
            foreach (Command c in Command.all.All())
			{
				this.PermissionList.Add(c.PermissionNode);
			}
			GroupList.Add(this);
			Group.DefaultGroup = this;
		}
	}
}
