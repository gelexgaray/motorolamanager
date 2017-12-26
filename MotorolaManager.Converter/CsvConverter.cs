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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MotorolaManager.Common;

namespace MotorolaManager.Converter
{
	/// <summary>
	/// Clase que permite convertir una lista de PhonebookEntry a un stream de texto
	/// separado por comas y viceversa
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CSV")]
	public static class CsvConverter
	{
		/// <summary>
		/// Convertir un Nombre, Teléfono, Posición a una línea de texto
		/// </summary>
		/// <param name="pbEntry">PhonebookEntry a convertir en texto</param>
		/// <returns>Línea de texto que representa al PhonebookEntry</returns>
		public static string ToCsvString( PhonebookEntry pbEntry )
		{
			return
				String.Format(CultureInfo.InvariantCulture, "\"{0}\",\"{1}\",{2}",
				              pbEntry.Name,
				              pbEntry.Number,
				              pbEntry.Position);
		}
		
		/// <summary>
		/// Convertir una colección de PhonebookEntry a múltiples líneas de texto
		/// </summary>
		/// <param name="pbEntryCollection">Colección de entradas PhonebookEntry</param>
		/// <returns>Conjunto de líneas de texto con el Phonebook</returns>
		public static string ToCsvString( Collection<PhonebookEntry> pbEntryCollection)
		{
			StringBuilder sb = new StringBuilder();
			foreach( PhonebookEntry pbEntry in pbEntryCollection )
			{
				sb.AppendLine( ToCsvString( pbEntry ));
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Convertir una línea de texto a PhonebookEntry
		/// </summary>
		/// <param name="line">Línea de texto a convertir</param>
		/// <returns>PhonebookEntry si el texto contiene una entrada, o NULL en caso contrario</returns>
		public static PhonebookEntry ToPhonebookEntry( string line )
		{
			PhonebookEntry pbEntry = null;
			Regex r = new Regex("\"(.+)\"\\s*,\\s*\"(.+)\"\\s*,\\s*(\\d+)");
			Match m = r.Match(line);
			if (m.Success)
			{
				// Construir el objeto
				pbEntry = new PhonebookEntry(
					int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture), // Posición
					m.Groups[1].Value, // Nombre
					m.Groups[2].Value); // Teléfono
			}
			return pbEntry;

		}
		
		/// <summary>
		/// Convertir un stream a una colección de PhonebookEntry
		/// </summary>
		/// <param name="sReader">Stream con el texto a convertir</param>
		/// <returns>Lista con todos los PhonebookEntry válidos encontrados</returns>
		public static IList<PhonebookEntry> ToPhonebookEntryCollection( TextReader sReader)
		{
			string line = sReader.ReadLine();
			List<PhonebookEntry> pbCollection = new List<PhonebookEntry>();
			while( null != line )
			{
				PhonebookEntry pbEntry = ToPhonebookEntry( line );
				if( null != pbEntry) pbCollection.Add( pbEntry );
				line = sReader.ReadLine();
			}
			sReader.Close();
			return (IList<PhonebookEntry>) pbCollection;
		}
	}
}
