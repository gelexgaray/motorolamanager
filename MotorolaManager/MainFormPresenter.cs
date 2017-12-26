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

using MotorolaManager.Converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MotorolaManager.Common;
using MotorolaManager.MotorolaIO;

namespace MotorolaManager
{
	/// <summary>
	/// Clase Presenter asociada al formulario MainForm (patrón MVP)
	/// </summary>
	public class MainFormPresenter
	{
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="form"></param>
		public MainFormPresenter( MainForm form )
		{
			_form = form;
		}
		
		/// <summary>
		/// Referencia al formulario sobre el que se presentan los
		/// datos del modelo
		/// </summary>
		private MainForm _form;
		
		/// <summary>
		/// Interfaz a utilizar para acceder al teléfono
		/// </summary>
		private static ATInterface _phoneInterface;

		/// <summary>
		/// Puerto de comunicaciones a utilizar para conectarse al móvil
		/// </summary>
		internal static string ComPort
		{
			set
			{
				_comPort = value;
			}
		}
		private static string _comPort;
		
		/// <summary>
		/// Indica si el teléfono debe ser reiniciado al salir de la aplicación
		/// </summary>
		internal static bool MustRebootOnExit
		{
			get {return _mustRebootOnExit;	}
			set { _mustRebootOnExit = value; }
		}
		private static bool _mustRebootOnExit;
		
		/// <summary>
		/// Propiedad que establece si estamos conectados al teléfono
		/// </summary>
		internal static bool Connected
		{
			set
			{
				// Conexión
				if( value )
				{
					if( null == _phoneInterface )
					{
						if( string.IsNullOrEmpty(_comPort )) throw new InvalidOperationException("Unable to connect. Please select COM Port first");
						
						// Crear la interfaz
						_phoneInterface = new ATInterface(
							_comPort,
							ATInterface.GuessSpeed(),
							ATInterface.GuessParity(),
							ATInterface.GuessDataBits(),
							ATInterface.GuessStopBits()
						);
					}
					_phoneInterface.Open();
				}
				
				// Desconexión
				else if (null != _phoneInterface)
				{
					_phoneInterface.Close();
					_phoneInterface.Dispose();
					_phoneInterface = null;
				}

				_connected = value;
			}
			get
			{
				return _connected;
			}
		}
		private static bool _connected;
		
		/// <summary>
		/// Contador de errores en el grid de contactos
		/// </summary>
		internal int PhonebookGridErrorCounter
		{
			get
			{
				return _phonebookGridErrorCounter;
			}
			private set
			{
				_phonebookGridErrorCounter = value;
				bool hasErrors = (0 == value);
				_form.EnableExport = hasErrors;
				_form.EnableWrite = hasErrors;
			}
		}
		private int _phonebookGridErrorCounter;
		
		/// <summary>
		/// Vuelve a leer los contactos de la SIM
		/// </summary>
		internal void ReadSIMContacts()
		{
			// Seleccionar la agenda de la SIM
			_phoneInterface.SelectSIMPhoneBook();

			// Cargar todos los contactos en el DataSet de contactos
			IList<PhonebookEntry> pbEntryList = _phoneInterface.ReadAllPhonebookEntries();
			_form.LoadPhoneBookEntryListIntoGrid(pbEntryList);

			// Actualizar la capacidad de la SIM
			_form.PhonebookCapacity = _phoneInterface.HigherPhonebookIndex - _phoneInterface.LowerPhonebookIndex + 1;
			_form.PhonebookUsedEntries = _form.PhonebookGrid.Rows.Count;

			// Actualizar la barra de estado
			_form.UpdateStatusLabel();
		}
		
		/// <summary>
		/// Graba todos los contactos del grid en el teléfono
		/// </summary>
		internal void WriteGridContacts()
		{
			// Señalizar que se ha modificado la SIM, y que por tanto se debe reiniciar el teléfono al salir de la aplicación
			MustRebootOnExit = true; 
			
			foreach (DataGridViewRow dgvr in _form.PhonebookGrid.Rows)
			{
				// Si la celda tiene el flag de modificada, modificarla
				object isModifiedCell = dgvr.Cells[MainFormConstants.IsModifiedColumnIndex].Value;
				if (null != isModifiedCell && true == (bool)isModifiedCell)
				{
					// Determinar la posición del contacto en la SIM, si es que está establecida
					int? simPosition = (int?)dgvr.Cells[MainFormConstants.PositionColumnIndex].Value;

					// BORRADO
					object mustBeDeletedCell = dgvr.Cells[MainFormConstants.MustBeDeletedColumnIndex].Value;
					if (null != mustBeDeletedCell && true == (bool)mustBeDeletedCell)
					{
						if (simPosition.HasValue) _phoneInterface.RemoveEntry(simPosition.Value);
					}
					// MODIFICACION
					else if (simPosition.HasValue)
					{
						PhonebookEntry pbEntry = new PhonebookEntry(
							simPosition.Value,
							(string)dgvr.Cells[MainFormConstants.NameColumnIndex].Value,
							(string)dgvr.Cells[MainFormConstants.NumberColumnIndex].Value
						);
						_phoneInterface.WritePhonebookEntry(pbEntry);
					}
					// ALTA
					else
					{
						PhonebookEntry pbEntry = new PhonebookEntry(
							(string)dgvr.Cells[MainFormConstants.NameColumnIndex].Value,
							(string)dgvr.Cells[MainFormConstants.NumberColumnIndex].Value
						);
						_phoneInterface.WritePhonebookEntry(pbEntry);
					}
				}
			}
		}
		
		/// <summary>
		/// Importar los contactos desde el archivo indicado
		/// </summary>
		/// <param name="filename">Nombre del archivo de contactos</param>
		internal void ImportContactsFromFile(string filename )
		{
			StreamReader sr = null;
			try{
				sr = new StreamReader(filename);
				IList<PhonebookEntry> pbEntryCollection = CsvConverter.ToPhonebookEntryCollection(sr);

				// Vaciar el grid
				_form.PhonebookGrid.Rows.Clear();

				// Cargar todos los contactos del archivo
				foreach (PhonebookEntry entry in pbEntryCollection) {
					_form.PhonebookGrid.Rows.Add( entry.Position, entry.Name, entry.Number, true, false);
				}

				// TODO: Indicar que venimos de una importación, y que por lo tanto todos los contactos de la tarjeta deben eliminarse antes de proceder a la escritura de los nuevos contactos

				// Indicar que los datos han cambiado
				_form.DataChanged = true;

				// Actualizar el contador de entradas usadas
				_form.PhonebookUsedEntries = pbEntryCollection.Count;
				_form.UpdateStatusLabel();
			}
			finally
			{
				if(null != sr) sr.Close();
			}
		}
		
		/// <summary>
		/// Exporta los contactos al archivo indicado
		/// </summary>
		/// <param name="filename">Nombre del archivo sobre el que se exportarán todos los contactos</param>
		internal void ExportContactsToFile(string filename)
		{
			StreamWriter sw = null;
			try
			{
				// Crear un StreamWriter sobre ese archivo
				sw = new StreamWriter(filename);

				// Convertir el grid en entradas de tipo PhonebookEntry
				Collection<PhonebookEntry> pbList = GridToPhonebookEntryCollection(_form.PhonebookGrid.Rows);

				// Convertir la lista de PhonebookEntry en string CSV
				string textToExport = CsvConverter.ToCsvString(pbList);

				// Volcar el archivo CSV en el StreamWriter
				sw.Write(textToExport);
			}
			finally
			{
				if( null != sw ) sw.Close();
			}
		}

		/// <summary>
		/// Borra los contactos seleccionados en el grid
		/// </summary>
		internal void DeleteSelectedContacts()
		{
			// Si hay una línea que se está editando, debería finalizarse la edición antes de continuar
			_form.PhonebookGrid.EndEdit();

			_form.DataChanged = true;

			// Si no hay celda en curso, o la celda es nueva, no se puede borrar!
			if (null != _form.PhonebookGrid.CurrentRow)
			{
				// Nuevo estado en el que tenemos que dejar las filas seleccionadas es el estado contrario
				// al de la celda en la que está el cursor.
				object newStatusObject = _form.PhonebookGrid.CurrentRow.Cells[MainFormConstants.MustBeDeletedColumnIndex].Value;
				bool newStatus = (null == newStatusObject) ? true : !(bool)newStatusObject;

				// Marcar para borrar las celdas seleccionadas
				foreach (DataGridViewRow row in _form.PhonebookGrid.SelectedRows)
				{
					_form.SetDeleteStatus(row, newStatus);
					
					// Si borramos celdas con errores, los descontamos del contador de errores
					if( true == newStatus)
					{
						foreach (DataGridViewCell cell in row.Cells)
							if (!string.IsNullOrEmpty(cell.ErrorText)) PhonebookGridErrorCounter--;
					}
				}

				// Marcar para borrar la celda actual, si es que no estaba dentro de la selección
				if (!_form.PhonebookGrid.CurrentRow.Selected)
					_form.SetDeleteStatus(_form.PhonebookGrid.CurrentRow, newStatus);
			}

			// Actualizar la barra de estado para reflejar los cambios
			_form.UpdateStatusLabel();
		}
		
		/// <summary>
		/// Inserta un nuevo contacto en el grid de contactos
		/// </summary>
		internal void InsertNewContact()
		{
			int rowNumber = _form.PhonebookGrid.Rows.Add();
			_form.PhonebookGrid.FirstDisplayedScrollingRowIndex = rowNumber;
			_form.PhonebookGrid.CurrentCell = _form.PhonebookGrid.Rows[rowNumber].Cells[MainFormConstants.NameColumnIndex];

			// Empezar a editar la celda recién creada
			_form.PhonebookUsedEntries++;

			// Actualizar la barra de estado
			_form.UpdateStatusLabel();
		}

		/// <summary>
		/// Valida una fila
		/// </summary>
		/// <param name="rowIndex">Fila a validar</param>
		internal void ValidateRow(int rowIndex)
		{
			// Validar cada una de las celdas de la fila
			foreach (DataGridViewCell cell in _form.PhonebookGrid.Rows[rowIndex].Cells)
				ValidateCell(cell);

			// Actualizar la barra de estado
			_form.UpdateStatusLabel();
		}

		/// <summary>
		/// Valida el contenido de una celda
		/// </summary>
		/// <param name="cell">Celda a validar</param>
		private void ValidateCell(DataGridViewCell cell)
		{
			switch (cell.ColumnIndex)
			{
					// Validación de números de teléfono
				case MainFormConstants.NumberColumnIndex:
					ValidateCellAgainstRegularExpression(cell, @"^([\+#*pP0-9])+$");
					break;

					// Validación de nombres de contactos
				case MainFormConstants.NameColumnIndex:
					ValidateCellAgainstRegularExpression(cell, @"^([ -!|#-+|\-\.|/-~])+$");
					break;
			}
		}


		/// <summary>
		/// Valida una celda contra una expresión regular
		/// </summary>
		/// <param name="cell">Celda a validar</param>
		/// <param name="regExString">El string de la expresión regular</param>
		private void ValidateCellAgainstRegularExpression(DataGridViewCell cell, string regExString)
		{
			Regex regEx = new Regex(regExString);
			if (null != cell.Value && regEx.IsMatch(cell.Value.ToString()))
			{
				// Si la celda tenía error, decrementar el contador de errores
				if (!string.IsNullOrEmpty(_form.PhonebookGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].ErrorText))
				{
					_form.PhonebookGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].ErrorText = string.Empty;
					PhonebookGridErrorCounter--;
				}
			}
			else
			{
				// Si la celda no tenía error, incrementar el contador de errores
				if (string.IsNullOrEmpty(_form.PhonebookGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].ErrorText))
				{
					_form.PhonebookGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].ErrorText = "Invalid format";
					PhonebookGridErrorCounter++;
				}
			}
		}
		
		/// <summary>
		/// Convierte una colección de filas del grid de teléfonos en una lista
		/// de PhonebookEntry
		/// </summary>
		/// <param name="rowCollection">Colección de celdas a convertir</param>
		/// <returns>Lista de PhonebookEntry</returns>
		internal static Collection<PhonebookEntry> GridToPhonebookEntryCollection( DataGridViewRowCollection rowCollection )
		{
			Collection<PhonebookEntry> pbEntryCollection = new Collection<PhonebookEntry>();
			
			// Copiar a una lista de PhonebookEntry todas las celdas del grid
			foreach (DataGridViewRow row in rowCollection)
			{
				// Si la celda no está marcada para ser borrada, se convierte
				if( false == (bool)row.Cells[MainFormConstants.MustBeDeletedColumnIndex].Value )
				{
					pbEntryCollection.Add(
						RowToPhonebookEntry( row )
					);
				}
			}
			
			return pbEntryCollection;
		}
		
		/// <summary>
		/// Convierte una fila del grid de contactos en un contacto
		/// </summary>
		/// <param name="row">Fila a convertir</param>
		/// <returns>Contacto de tipo PhonebookEntry</returns>
		private static PhonebookEntry RowToPhonebookEntry( DataGridViewRow row )
		{
			return new PhonebookEntry(
				Int32.Parse(row.Cells[MainFormConstants.PositionColumnIndex].Value.ToString(), CultureInfo.CurrentCulture),
				(string)row.Cells[MainFormConstants.NameColumnIndex].Value,
				(string)row.Cells[MainFormConstants.NumberColumnIndex].Value
			);
		}
		
		/// <summary>
		/// Inicia el envío de un SMS
		/// </summary>
		/// <returns>True, si se ha enviado un mensaje. False si se ha cancelado la operación</returns>
		internal void SendSMS()
		{
			// Mostrar el diálogo de envío de SMS
			SMSDialog smsDialog = new SMSDialog();
			if ( _form.PhonebookGrid.SelectedRows.Count > 0 )
			{
				smsDialog.PBEntry = RowToPhonebookEntry( _form.PhonebookGrid.SelectedRows[0]);
			}
			DialogResult result = smsDialog.ShowDialog();
			
			// Si se ha cumplimentado correctamente el diálogo de envío, enviar el mensaje
			if( DialogResult.OK == result )
			{
				PhonebookEntry pbe = smsDialog.PBEntry;
				_phoneInterface.SendSMS( pbe, smsDialog.SMSBody );
	
				_form.ShowStatusMessage(
				    String.Format( 
				        CultureInfo.CurrentUICulture,
				      		"SMS sent to {0}", 
				      		string.IsNullOrEmpty(pbe.Name)?pbe.Number: pbe.Name ));
			}
		}

		/// <summary>
		/// Finaliza la éjecución de la aplicación
		/// </summary>
		internal static void ApplicationExit()
		{
			Connected = false;
			if ( MustRebootOnExit )
			{
				MessageBox.Show("Changes has been made in mobile phone.\nYou must reboot your mobile phone in order to make changes effective", "Please reboot your phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}
