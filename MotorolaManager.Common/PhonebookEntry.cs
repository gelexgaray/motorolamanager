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
using System.Collections.Generic;
using System.Text;

namespace MotorolaManager.Common
{
	/// <summary>
	/// Clase que representa una entrada en la agenda de la SIM
	/// </summary>
	public class PhonebookEntry
	{
		/// <summary>
		/// Construye una entrada en la agenda de la SIM
		/// </summary>
		/// <param name="position">Posición que debe ocupar en la agenda</param>
		/// <param name="name">Nombre del contacto</param>
		/// <param name="number">Número de teléfono del contacto</param>
		public PhonebookEntry(int position, string name, string number) 
		{
			_position = position;
			_name = name.Trim();
			_number = number.Trim();
		}

		/// <summary>
		/// Construye una entrada en la agenda de la SIM (sin posición asignada)
		/// </summary>
		/// <param name="name">Nombre del contacto</param>
		/// <param name="number">Número de teléfono del contacto</param>
		public PhonebookEntry(string name, string number)
		{
			_name = name.Trim();
			_number = number.Trim();
		}

		/// <summary>
		/// Posición que ocupa el elemento en la SIM.
		/// Si vale NULL, el elemento no tiene un código asignado en la SIM
		/// </summary>
		public int? Position
		{
			get { return _position; }
		}
		private int? _position;

		/// <summary>
		/// Nombre del contacto
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		private string _name;

		/// <summary>
		/// Número de teléfono
		/// (si el número es internacional, estará con el formato +nnn)
		/// </summary>
		public string Number
		{
			get { return _number; }
		}
		private string _number;

		/// <summary>
		/// Flag que indica si el teléfono está en formato internacional
		/// </summary>
		public bool IsInternational
		{
			get { return Number.StartsWith("+", StringComparison.CurrentCulture); }
		}
	
	}
}
