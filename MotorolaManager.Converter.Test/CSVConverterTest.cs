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
using System.Collections.ObjectModel;
using System.IO.Ports;
using MotorolaManager.Common;
using MotorolaManager.Converter;
using System.IO;

namespace MotorolaManager.Converter.Test
{
	/// <summary>
	/// Tests unitarios para el CSVConverter
	/// </summary>
	[TestFixture]
	public class CSVConverterTest
	{
		/// <summary>
		/// Test para ToString (PhonebookEntry)
		/// </summary>
		[Test]
		public void ToCsvStringTest()
		{
			PhonebookEntry pbEntry = new PhonebookEntry( 0, "UTName", "000" );
			string result = CsvConverter.ToCsvString(pbEntry);
			
			Assert.AreEqual( "\"UTName\",\"000\",0", result);
		}
		
		/// <summary>
		/// Test para ToString (Collection de PhonebookEntry)
		/// </summary>
		[Test]
		public void ToCSVStringTest2()
		{
			PhonebookEntry pbEntry = new PhonebookEntry( 0, "UTName", "000" );
			Collection<PhonebookEntry> pbList = new Collection<PhonebookEntry>();
			pbList.Add( pbEntry );
			pbList.Add( pbEntry );
			
			string result = CsvConverter.ToCsvString(pbList);
			
			// Correcto si hay más de un retorno de carro
			Assert.AreNotEqual(result.LastIndexOf( '\n', 0 ), result.IndexOf( '\n', 0 ));
		}
		
		/// <summary>
		/// Test para ToPhonebookEntry (string)
		/// </summary>
		[Test]
		public void ToPhonebookEntryTest1()
		{
			PhonebookEntry pbEntry = CsvConverter.ToPhonebookEntry("\"UTName\",\"000\",0");
			
			Assert.AreEqual( "UTName", pbEntry.Name);
			Assert.AreEqual( "000", pbEntry.Number);
			Assert.AreEqual( 0, pbEntry.Position);
		}
		
		/// <summary>
		/// Test para ToPhonebookEntry (string). 
		/// Conversión si la cadena de entrada tiene espacios intercalados entre campos
		/// </summary>
		[Test]
		public void ToPhonebookEntryTest2()
		{
			PhonebookEntry pbEntry = CsvConverter.ToPhonebookEntry("  \"UTName\" ,  \"000\" ,0");
			
			Assert.AreEqual( "UTName", pbEntry.Name);
			Assert.AreEqual( "000", pbEntry.Number);
			Assert.AreEqual( 0, pbEntry.Position);
		}
		
		/// <summary>
		/// Test para ToPhonebookEntry (string)
		/// Conversión erronea
		/// </summary>
		[Test]
		public void ToPhonebookEntryTest3()
		{
			PhonebookEntry pbEntry = CsvConverter.ToPhonebookEntry( string.Empty );
			
			Assert.IsNull(pbEntry);
		}
		
		/// <summary>
		/// Test para ToPhonebookEntryCollection (string)
		/// Conversión erronea
		/// </summary>
		[Test]
		public void ToPhonebookEntryCollectionTest()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine( "\"UTName\",\"000\",0" );
			sb.AppendLine( "\"UTName\",\"000\",0" );
			sb.AppendLine( "INVALID ITEM TO BE DISCARDED" );
			StringReader stringReader = new StringReader( sb.ToString());
				
			IList<PhonebookEntry> pbEntryCollection = CsvConverter.ToPhonebookEntryCollection( stringReader );
			
			Assert.AreEqual(2, pbEntryCollection.Count);
		}	
		
	}
}
