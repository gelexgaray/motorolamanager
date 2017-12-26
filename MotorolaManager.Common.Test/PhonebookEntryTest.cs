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

using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
namespace MotorolaManager.Common.Test
{
	/// <summary>
	/// Tests para la clase PhonebookEntry
	///</summary>
	[TestFixture]
	public class PhonebookEntryTest
	{

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for IsInternational
		///</summary>
		[Test]
		public void IsInternationalTest()
		{
			PhonebookEntry target = new PhonebookEntry("UTName", "+34000");
			Assert.IsTrue(target.IsInternational);
		}

		/// <summary>
		///A test for Name
		///</summary>
		[Test]
		public void NameTest()
		{
			PhonebookEntry target = new PhonebookEntry("UTName", "+34000");
			Assert.AreEqual("UTName", target.Name);
		}

		/// <summary>
		///A test for Number
		///</summary>
		[Test]
		public void NumberTest()
		{
			PhonebookEntry target = new PhonebookEntry("UTName", "+34000");
			Assert.AreEqual("+34000", target.Number);
		}

		/// <summary>
		///A test for Position
		///</summary>
		[Test]
		public void PositionTest()
		{
			PhonebookEntry target = new PhonebookEntry(1, "UTName", "+34000");
			Assert.AreEqual(1, target.Position);
		}

		/// <summary>
		///A test for Position
		///</summary>
		[Test]
		public void PositionTest1()
		{
			PhonebookEntry target = new PhonebookEntry("UTName", "+34000");
			Assert.IsNull(target.Position);
		}

	}


}
