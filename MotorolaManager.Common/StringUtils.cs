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
using System.Collections.Generic;

namespace MotorolaManager.Common
{
	/// <summary>
	/// Utilidades de manipulación de strings
	/// </summary>
	public static class StringUtils
	{
		
		/// <summary>
		/// Juego de caracteres con diacríticos soportados por el
		/// alfabeto estándar de SMSs. Representado como lista de
		/// códigos unicode enteros
		/// </summary>
		private static readonly List<char> smsSupportedDiacritics 
			= new List<char>( 
			     new char[]{
					'ñ', 'Ñ', 
					'ä', 'ü', 'ö',
					'Ä', 'Ü', 'Ö',
					'é', 'É',
					'å', 'Å',
					'à', 'è', 'ì', 'ò', 'ù'
					}
			);
		
		/// <summary>
		/// Método que elimina los caracteres diacríticos
		/// de un string, exceptuando los caracteres diacríticos
		/// soportados por el juego de caracteres estandar de los SMS
		/// </summary>
		/// <remarks>
		/// Inspirado en
		/// http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
		/// </remarks>
		/// <param name="stIn">Cadena de entrada</param>
		/// <returns>Cadena de texto con los diacríticos eliminados</returns>
		public static string RemoveDiacritics(string stIn) 
		{
			
			// Explicación:
			// Para cada caracter no excluido de la función,
			// se normaliza en forma D, se recorre el string normalizado
			// que representa el caracter, y se quitan los caracteres de categoría
			// NonSpacingMark (que representan a los diacríticos en esta forma D.
			// El string resultante, se vuelve a poner en forma C, y los caracteres
			// con acentos han pasado a la historia
			
			StringBuilder sb = new StringBuilder();
			for(int ich = 0; ich < stIn.Length; ich++) 
			{
				char c = stIn[ich];
				if(smsSupportedDiacritics.Contains(c)) sb.Append(c);
				else
				{
					string stFormD = c.ToString().Normalize(NormalizationForm.FormD);
					for( int jch = 0; jch < stFormD.Length; jch ++)
					{
						UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD, jch);
						if(uc != UnicodeCategory.NonSpacingMark) sb.Append(stFormD[jch]);
					} // fin - for( int jch = 0; jch < sFormD.Length; jch ++)
				} // fin - else
			} // fin - for(int ich = 0; ich < stIn.Length; ich++)

			return(sb.ToString().Normalize(NormalizationForm.FormC));
		}

	}
}
