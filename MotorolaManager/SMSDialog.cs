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
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MotorolaManager.Common;

namespace MotorolaManager
{
	/// <summary>
	/// Formulario de envío de SMSs
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SMS")]
	public partial class SMSDialog : Form
	{
		/// <summary>
		/// Título de la ventana cuando el SMS va dirigido a un contacto conocido 
		/// </summary>
		private const string _titleKnownContact = "New SMS for {0}";
		
		/// <summary> 
		/// Título de la ventana cuando el SMS va dirigido a un número desconocido
		/// </summary>
		private const string _titleUnknownContact = "New SMS";
		
		/// <summary>
		/// Número máximo de caracteres permitido 
		/// </summary>
		private const int _maxChars = 160;
		
		/// <summary>
		/// Flag que indica si el número del destinatario es válido 
		/// </summary>
		private bool _isPhoneNumberValid = false;
		
		/// <summary>
		///Constructor 
		/// </summary>
		public SMSDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Establecer el contador de caracteres y el título del diálogo
			UpdateCharCounter();
			UpdateTitle();
		}
		
		/// <value>
		/// Número de caracteres que todavía se pueden escribir en el SMS.
		/// </value>
		int RemainingChars
		{
			get
			{ 
				return _maxChars - tbBody.Text.Length;
			}
		}
		
		/// <summary>
		/// Obtiene / establece el destinatario del SMS
		/// </summary>
		public PhonebookEntry PBEntry
		{
			get{ 
				if( _pbEntry.Number != tbTo.Text ) return new PhonebookEntry( string.Empty, tbTo.Text );
				else return _pbEntry;
			}
			set{
				if( null == value ) throw new ArgumentNullException( "value" );
				
				_pbEntry = value;
				
				// Meter los datos del contacto en el diálogo
				tbTo.Text = _pbEntry.Number;
				UpdateTitle();
			}
		}

		private PhonebookEntry _pbEntry = new PhonebookEntry( string.Empty, string.Empty);
		
		/// <summary>
		/// Obtiene el texto del mensaje a enviar
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SMS")]
		public string SMSBody
		{
			get
			{
				return StringUtils.RemoveDiacritics( tbBody.Text );
			}
		}
		
		/// <summary>
		/// Establece el título del diálogo en función
		/// de las propiedades del contacto seleccionado
		/// </summary>
		private void UpdateTitle()
		{
			if (string.IsNullOrEmpty(PBEntry.Name) || _pbEntry.Number != tbTo.Text )
			{
				Text = _titleUnknownContact;
			}
			else
			{
				Text = string.Format(
					CultureInfo.CurrentUICulture,
					_titleKnownContact,
					PBEntry.Name);
				
			}
		}
		
		/// <summary>
		/// Actualiza el contador de caracteres disponibles
		/// </summary>
		private void UpdateCharCounter()
		{
			int charNumber = RemainingChars;
			lCharCounter.Text = String.Format(CultureInfo.CurrentUICulture, "{0} characters left", charNumber );
			if (charNumber >= 0 )
			{
				lCharCounter.ForeColor = System.Drawing.SystemColors.ControlText;
			} 
			else
			{
				lCharCounter.ForeColor = System.Drawing.Color.Red;
			}
		}
		
		/// <summary>
		/// Activa el botón de Enviar si la entrada es válida
		/// </summary>
		private void UpdateBSendStatus()
		{
			
			// Antiguamente, se controlaba el numero de caracteres a través de la propiedad
			// tbBody.MaxLength. Por desgracia, en Mono, cuando el textbox es multilinea y el
			// WordWrap está habilitado, se calcula mal el MaxLength :-(. Como Workaround, esta
			// función de validación comprueba que no se sobrepase el mayor número de caracteres
			// permitidos por SMS
			
			bSend.Enabled = _isPhoneNumberValid 
				&& RemainingChars >= 0
				&& RemainingChars < _maxChars;
		}	
		
		
		#region Manejadores de eventos
		/// <summary>
		/// Al cambiar el texto: Actualizar el contador
		/// de caracteres disponibles
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void TbBodyTextChanged(object sender, EventArgs e)
		{
			UpdateCharCounter();
			UpdateBSendStatus();
		}
		
		/// <summary>
		/// Al abrir el formulario:
		/// Poner el foco en el primer campo de texto a rellenar
		/// por el usuario
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void SMSDialogLoad(object sender, EventArgs e)
		{
			if( !string.IsNullOrEmpty(tbTo.Text))
			{
				tbTo.SelectionLength = 0;
				tbBody.Focus();
				
				// Workaround para MONO en Linux: la implementación 
				// de Winforms de Linux no lanza el evento de validación inicial, así que lo forzamos
				ValidateChildren();
			}
		}
		
		/// <summary>
		/// Validación del número de teléfono
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		void TbToValidating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Regex validNumber = new Regex( @"^([\+#*0-9])+$"); 
			if( !validNumber.IsMatch( tbTo.Text ))
			{
				errorProvider.SetError( tbTo, "Invalid number for SMS sending." );
				_isPhoneNumberValid = false;
			}
			else
			{
				errorProvider.SetError( tbTo, string.Empty );
				_isPhoneNumberValid = true;
			}
			UpdateBSendStatus();
		}
		
		/// <summary>
		/// Si el usuario modifica el destinatario, pasamos
		/// a enviar a un destinatario desconocido
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void TbToTextChanged(object sender, EventArgs e)
		{
			UpdateTitle();
		}
			
		
	}
	#endregion
	
}

