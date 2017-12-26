// Motorola W375 Manager
// Copyright (C) 2008-2009 Gorka Elexgaray
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.


using System;
using MotorolaManager.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MotorolaManager.Common.Test
{
	/// <summary>
	/// Tests para la clase StringUtils
	///</summary>
	[TestFixture]
	public class StringUtilsTest
	{
		/// <summary>
		/// Test para el método RemoveDiacritics
		/// </summary>
		[Test]
		public void RemoveDiacriticsTest()
		{
			string result = StringUtils.RemoveDiacritics( "áéíóúâêîôûÁÉÍÓÚñÑüÜa1" );
			Assert.AreEqual( "aéiouaeiouAÉIOUñÑüÜa1", result );
		}
	}
}
