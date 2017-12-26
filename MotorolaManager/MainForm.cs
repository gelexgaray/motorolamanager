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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

using MotorolaManager.Common;
using MotorolaManager.Converter;
using MotorolaManager.MotorolaIO;

namespace MotorolaManager
{
	/// <summary>
	/// Formulario principal de la aplicación
	/// </summary>
	public partial class MainForm : Form
	{
		// ICONOS: Preferiblemente usar Iconos del tema "Nuvola" de KDE
		
		/// <summary>
		/// URL con información sobre cómo conectar el movil
		/// </summary>
		const string HowtoConnectURL = "http://code.google.com/p/motorolamanager/wiki/HOWTO_Connect";
		
		/// <summary>
		/// Constructor del formulario principal
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
			
			_presenter = new MainFormPresenter( this );
		}

		#region Método Main
		/// <summary>
		/// Método Main
		/// </summary>
		[STAThread]
		public static void Main(/*string[] args*/)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Registrar eventos a nivel del objeto Application
			Application.ApplicationExit += new EventHandler(ApplicationExitEventHandler);

			Application.Run(new MainForm());
		}

		/// <summary>
		/// Al finalizar la aplicación:
		/// Asegurarnos de que el puerto queda cerrado
		/// </summary>
		/// <param name="sender">Emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private static void ApplicationExitEventHandler(object sender, EventArgs e)
		{
			MainFormPresenter.ApplicationExit();
		}
		#endregion
		
		/// <summary>
		/// Clase Presenter asociada al formulario (patrón MVP)
		/// </summary>
		private MainFormPresenter _presenter;

		#region Propiedades
		/// <summary>
		/// Capacidad disponible en la SIM
		/// </summary>
		internal int PhonebookCapacity
		{
			get {return _phoneBookCapacity;	}
			set { _phoneBookCapacity = value; }
		}
		private int _phoneBookCapacity;

		/// <summary>
		/// Número de entradas utilizadas en la SIM
		/// </summary>
		internal int PhonebookUsedEntries
		{
			get { return _phoneBookUsedEntries; }
			set
			{
				_phoneBookUsedEntries = value;
				bNew.Enabled = ( value < PhonebookCapacity );
			}
		}
		private int _phoneBookUsedEntries;

		/// <summary>
		/// Propiedad que conmuta la interfaz de usuario entre
		/// interfaz para teléfono conectado o desconectado
		/// </summary>
		private bool EnableConnectedOperations
		{
			set
			{
				dgvPhoneBook.Enabled = value;
				bNew.Enabled = value;
				bImportCSV.Enabled = value;
				bExportCSV.Enabled = value;
				bSMS.Enabled = value;
				MainFormPresenter.Connected = value;
				
				// Cambia el estado en función de si estamos o no conectados
				if( value )
				{
					lStatus.Text = String.Empty;
					lStatus.Tag = String.Empty;   
					lStatus.IsLink = false;
				} 
				else
				{
					lStatus.Text = "Click here to get help on how to connect";
					lStatus.Tag = HowtoConnectURL;   
					lStatus.IsLink = true;
				}
			}
		}
		
		/// <summary>
		/// Habilita las opciones de guardado de contactos en el movil
		/// en la interfaz de usuario
		/// </summary>
		internal bool EnableWrite
		{
			set
			{
				bWrite.Enabled = value;
			}
		}
		
		/// <summary>
		/// Habilita las opciones de exportación de contactos
		/// en la interfaz de usuario
		/// </summary>
		internal bool EnableExport
		{
			set
			{
				bExportCSV.Enabled = value;
			}
		}

		/// <summary>
		/// Propiedad que establece si han cambiado los datos editados
		/// </summary>
		internal bool DataChanged
		{
			set
			{
				bWrite.Enabled = value;
			}
		}

		/// <summary>
		/// Propiedad que establece si hay una entrada seleccionada
		/// </summary>
		private bool ContactSelected
		{
			set
			{
				bDeleteContact.Enabled = value;
			}
		}
		
		/// <summary>
		/// Obtiene el grid de edición de teléfonos
		/// </summary>
		internal DataGridView PhonebookGrid
		{
			get
			{
				return this.dgvPhoneBook;
			}
		}
		#endregion

		#region Métodos de auxiliares de interfaz
		/// <summary>
		/// Muestra un mensaje temporalmente en la barra de estado.
		/// Tras un tiempo prudencial, revierte la barra de estado a su
		/// texto original
		/// </summary>
		/// <param name="message">Mensaje que se quiere mostrar en la barra de estado</param>
		internal void ShowStatusMessage(string message)
		{
			if( timerStatusMessage.Enabled )
			{
				// Si ya se estaba mostrando otro mensaje, reseteamos el timer
				// y no guardamos el mensaje que se estaba mostrando, puesto que
				// hay que restaurar el anterior
				timerStatusMessage.Stop();
			}
			else
			{
				// Guardar la barra de estado para posteriormente restaurarla.
				_statusMessageToRevert = lStatus.Text;
			}
			lStatus.Text = message;
			timerStatusMessage.Start();
		}
		
		/// <summary>
		/// Se usa en ShowStatusMessage y TimerStatusMessageTick.
		/// Almacena el texto que tenía la barra de estado antes de que se
		/// invocase un ShowStatusMessage, y permite al
		/// TimerStatusMessageTick revertir el mensaje de estado original
		/// pasado un cierto tiempo.
		/// </summary>
		private string _statusMessageToRevert;
		
		/// <summary>
		/// Temporizador de la barra de estado finalizado. Revierte el texto de la barra
		/// de estado a su texto original
		/// </summary>
		/// <param name="sender">Emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void TimerStatusMessageTick(object sender, EventArgs e)
		{
			timerStatusMessage.Stop();
			lStatus.Text = _statusMessageToRevert;
		}
		
		/// <summary>
		/// Carga una lista de contactos en el grid de contactos
		/// </summary>
		internal void LoadPhoneBookEntryListIntoGrid(IList<PhonebookEntry> pbEntryList)
		{
			dgvPhoneBook.Rows.Clear();
			foreach (PhonebookEntry entry in pbEntryList)
			{
				dgvPhoneBook.Rows.Add(entry.Position, entry.Name, entry.Number, false, false );
			}
			DataChanged = false;
		}

		/// <summary>
		/// Invierte el estado de "borrado" del elemento indicado
		/// </summary>
		/// <param name="row">Elemento cuyo estado se quiere cambiar</param>
		/// <param name="deleteStatus">Estado en el que se quiere dejar la fila</param>
		internal void SetDeleteStatus(DataGridViewRow row, bool deleteStatus)
		{
			// Determinamos si la celda es nueva:
			bool isNewRow = (null == row.Cells[MainFormConstants.PositionColumnIndex].Value);
			if (isNewRow)
			{
				// Si la celda es nueva y la operación es de borrado, directamente borramos la celda
				if (deleteStatus)
				{
					// Borrar la fila
					dgvPhoneBook.Rows.Remove(row);
					PhonebookUsedEntries--;
				}

				// Si la operación es de "undelete", se deja la celda nueva como estaba
			}
			else
			{
				// Indicar que la celda ha sido modificada
				row.Cells[MainFormConstants.IsModifiedColumnIndex].Value = true;

				// Establecer el flag de estado de la celda
				row.Cells[MainFormConstants.MustBeDeletedColumnIndex].Value = deleteStatus;
				
				// Descontar el elemento borrado del contador de posiciones usadas
				if(deleteStatus) PhonebookUsedEntries--;
				else PhonebookUsedEntries++;

				// Poner las celdas borradas con font tachado, y las no tachadas con font regular
				foreach (DataGridViewCell cell in row.Cells)
				{
					if (deleteStatus)
					{
						cell.Style.Font = new Font(this.Font, FontStyle.Strikeout | FontStyle.Italic);
					}
					else
					{
						cell.Style.Font = null; // Restablecer al font por defecto
					}
				}
			}
		}

		/// <summary>
		/// Actualizar el texto de la barra de estado
		/// </summary>
		internal void UpdateStatusLabel()
		{
			StringBuilder sbStatus = new StringBuilder();

			// Informar de la capacidad de la SIM y de las posiciones actualmente ocupadas
			if ( MainFormPresenter.Connected)
				sbStatus.AppendFormat("{0}/{1} Contacts used.", PhonebookUsedEntries, PhonebookCapacity);

			// Informar del número de errores en el grid
			if (_presenter.PhonebookGridErrorCounter > 0)
				sbStatus.AppendFormat(" {0} Errors.", _presenter.PhonebookGridErrorCounter);

			lStatus.Text = sbStatus.ToString();
			_statusMessageToRevert = sbStatus.ToString();
			
		}
		#endregion
		
		#region Manejadores de eventos
		/// <summary>
		/// Al Cargar:
		/// Cargar la lista de puertos serie del sistema y predeterminar el
		/// puerto más adecuado al dispositivo
		/// </summary>
		private void MainFormLoad(object sender, EventArgs e)
		{
			// Obtener todos los puertos com del PC
			string[] names = ATInterface.GetPortNames();
			this.cbPort.Items.AddRange(names);

			// Seleccionar el puerto por defecto para el dispositivo configurado
			string comPort = ATInterface.GuessPortName();
			if( string.IsNullOrEmpty(comPort))
			{
				bRead.Enabled = false;
			}
			this.cbPort.Text = comPort;

			// Iniciamos en modo desconectado
			EnableConnectedOperations = false;
			DataChanged = false;
			
			// Workaround para MONO en Linux: la implementación 
			// de Winforms de Linux no lanza el evento de validación inicial, así que lo forzamos
			ValidateChildren();
		}

		/// <summary>
		/// Al seleccionar un nuevo puerto: Se invalida el objeto de comunicaciones anterior
		/// mente establecido y deshabilita los botones
		/// </summary>
		private void CbPortSelectedIndexChanged(object sender, EventArgs e)
		{
			MainFormPresenter.ComPort = cbPort.Text;
			EnableConnectedOperations = false;
			dgvPhoneBook.Rows.Clear();
		}

		/// <summary>
		/// Click en botón "Read": Conectamos con el teléfono y leemos la agenda de la SIM
		/// en el teléfono
		/// </summary>
		/// <param name="sender">Emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BReadClick(object sender, EventArgs e)
		{
			try
			{
				// Establecer el modo conectado	y leer la libreta de contactos
				EnableConnectedOperations = true;
				
				_presenter.ReadSIMContacts();
				dgvPhoneBook.Focus();
				
				// MONO Workaround: Seleccionar la fila 0 por defecto,
				// tal y como se hace en la implementación nativa Winforms
				if (dgvPhoneBook.Rows.Count > 0 )
				{
					dgvPhoneBook.CurrentCell = dgvPhoneBook[0,0];
					ContactSelected = true;
				}
				// FIN MONO Workaround
				
				// Mostrar en la barra de estado que se ha cargado con éxito
				ShowStatusMessage( "All contacts succesfully read." );
			}
			catch (TypeInitializationException)
			{
				// Capturar excepción y mostrar diálogo de error para excepciones manejadas
				MessageBox.Show("Not a compatible phone found", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Information);

				// Pasar a modo desconectado
				EnableConnectedOperations = false;
			}
		}

		/// <summary>
		/// Al hacer click en el botón de escritura:
		/// Guardar los cambios en el móvil
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BWriteClick(object sender, EventArgs e)
		{
			// Si hay una línea que se está editando, debería finalizarse la edición antes de continuar
			dgvPhoneBook.EndEdit();

			// Validamos la línea actual, por si estaba actualmente en edición
			_presenter.ValidateRow(dgvPhoneBook.CurrentRow.Index);

			if (_presenter.PhonebookGridErrorCounter > 0)
			{
				MessageBox.Show("Please correct errors before proceeding to save", "There are errors", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			else
			{
				Cursor = Cursors.WaitCursor;
				
				try
				{
					// TODO: Mostrar una barra de progreso en la barra de estado
					
					// Grabar los contactos del grid
					_presenter.WriteGridContacts();
					
					// Volver a leer los datos una vez terminado el guardado
					_presenter.ReadSIMContacts();
					
					// Mostrar en la barra de estado que se ha cargado con éxito
					ShowStatusMessage( "Address book succesfully updated." );
				}
				finally
				{
					Cursor = Cursors.Arrow;
				}
			}
		}

		/// <summary>
		/// Cuando se edita una celda:
		/// Marcamos la celda como modificada y activamos el botón de "guardar"
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void DgvPhoneBookCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			dgvPhoneBook.Rows[e.RowIndex].Cells[MainFormConstants.IsModifiedColumnIndex].Value = true;
			DataChanged = true;
		}

		/// <summary>
		/// Al hacer click en el botón de Borrar contacto:
		/// Borrar todos los contactos seleccionados
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BDeleteContactClick(object sender, System.EventArgs e)
		{
			_presenter.DeleteSelectedContacts();
			
			// MONO WORKAROUND (.NET refresca automáticamente, pero Mono no)
			dgvPhoneBook.Refresh();
		}

		/// <summary>
		/// Click en botón de añadir:
		/// Añade un nuevo contacto
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BNewClick(object sender, EventArgs e)
		{
			_presenter.InsertNewContact();
		}

		/// <summary>
		/// Al cambiar la selección
		/// Indicamos si hay o no un contacto seleccionado
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void DgvPhoneBookSelectionChanged(object sender, EventArgs e)
		{
			// Indicamos si hay o no un contacto seleccionado
			ContactSelected = (null != dgvPhoneBook.CurrentRow);
		}

		/// <summary>
		/// Al cambiar de fila: validar su contenido
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void DgvRowLeave(object sender, DataGridViewCellEventArgs e)
		{
			// Hacemos esta validación en el RowLeave en lugar de en el Validating, porque también deben validarse
			// las celdas que no han sido editadas (contenido a null)
			// Antes se hacía en el RowStateChanged, pero ese evento no funciona en el DataGridView de Mono
			if( dgvPhoneBook.Rows.Count > e.RowIndex)
 				_presenter.ValidateRow(e.RowIndex);
		}

		/// <summary>
		/// Tratamientos específicos al pulsar algunas teclas sobre el grid
		/// (no sobre una celda del grid)
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void DgvPhoneBookPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode)
			{
				// Suprimir: Borrado de contactos
				case Keys.Delete:
					_presenter.DeleteSelectedContacts();
					dgvPhoneBook.Refresh(); // WORKAROUND MONO
					break;

				// Insertar: Añadir un nuevo contacto
				case Keys.Insert:
					_presenter.InsertNewContact();
					break;
			}
		}

		/// <summary>
		/// Click en Menú contextual "Copy" del grid:
		/// Copia el número de teléfono de la primera celda seleccionada
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void CopyToolStripMenuItemClick(object sender, EventArgs e)
		{
			// No es necesario comprobar si hay items seleccionados, porque el grid
			// no permite desactivar la selección
			Clipboard.SetText( (string)dgvPhoneBook.SelectedRows[0].Cells[MainFormConstants.NumberColumnIndex].Value );
		}
		
		/// <summary>
		/// Al pulsar el botón de importar:
		/// Sale un diálogo de apertura para seleccionar el archivo a importar, se limpia
		/// el grid, y se carga el grid a partir del archivo
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BImportCSVClick(object sender, EventArgs e)
		{
			// Obtener el nombre del archivo
			OpenFileDialog od = new OpenFileDialog();
			od.Filter = "Text file|*.txt";
			
			DialogResult result = od.ShowDialog();
			if( DialogResult.OK == result )
			{
				// Importar el archivo
				try
				{
					_presenter.ImportContactsFromFile(od.FileName);
					
					// Mostrar en la barra de estado que se ha importado con éxito
					ShowStatusMessage( "Address book succesfully imported." );
				}
				// Control de errores: comprobar que se puede escribir (no está protegido, disco lleno... etc)
				catch( System.UnauthorizedAccessException ex )
				{
					MessageBox.Show( ex.Message, "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				catch( System.IO.IOException ex )
				{
					MessageBox.Show( ex.Message, "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}

			}
		}
		
		/// <summary>
		/// Al pulsar el botón de exportar:
		/// Sale un diálogo para seleccionar el archivo de destino, y se vuelca el contenido
		/// del grid a dicho archivo
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BExportCSVClick(object sender, EventArgs e)
		{
			// Obtener el nombre del archivo
			SaveFileDialog fd = new SaveFileDialog();
			fd.Filter = "Text file|*.txt";
			fd.DefaultExt = "txt";
			fd.AddExtension = true;
			fd.OverwritePrompt = true;
			DialogResult result = fd.ShowDialog();
			
			// Exportar contactos a ese archivo
			if( DialogResult.OK == result )
			{
				try
				{
					_presenter.ExportContactsToFile(fd.FileName);
					
					// Mostrar en la barra de estado que se ha exportado con éxito
					ShowStatusMessage("Address book succesfully exported.");
				}
				// Control de errores: comprobar que se puede escribir (no está protegido, disco lleno... etc)
				catch( System.UnauthorizedAccessException ex )
				{
					MessageBox.Show( ex.Message, "Export error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				catch( System.IO.IOException ex )
				{
					MessageBox.Show( ex.Message, "Export error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		/// <summary>
		/// Se usa desde DgvPhoneBookColumnHeaderMouseClick.
		/// Contiene el último criterio de ordenación del grid, para controlar si cambiamos
		/// de una ordenación descendiente a ascendiente.
		/// </summary>
		private SortOrder _previousSortOrder = SortOrder.None;
		
		/// <summary>
		/// Al hacer click sobre una columna:
		/// Si se vuelve de un criterio de ordenación descendiente a uno ascendiente,
		/// se deja el grid ordenado por posición en la SIM (permite volver a la ordenación
		/// por defecto tras ordenar por nombre o por número de teléfono)
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		void DgvPhoneBookColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// Si se presiona con el botón izquierdo sobre la columna ordenada,
			// y pasamos de orden descendiente a ascendiente, ordenar el grid por posición en la SIM.
			// De esta forma conseguimos que el click realice ciclos
			// Orden ascendente/descendente/orden por defecto
			if ( MouseButtons.Left == e.Button &&
			    dgvPhoneBook.SortedColumn.Index == e.ColumnIndex &&
			    SortOrder.Descending == _previousSortOrder &&
			    SortOrder.Ascending == dgvPhoneBook.SortOrder
			   )
			{
				this.dgvPhoneBook.Sort( dgvPhoneBook.Columns[MainFormConstants.PositionColumnIndex], ListSortDirection.Ascending);
				_previousSortOrder = SortOrder.None;
			}
			else
			{
				_previousSortOrder = dgvPhoneBook.SortOrder;
			}
		}		
		
		/// <summary>
		/// Al hacer click en el botón de envío de SMS:
		/// Enviar un SMS al contacto seleccionado, o enviar
		/// un sms a un contacto por especificar
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		private void BSMSClick(object sender, EventArgs e)
		{
			_presenter.SendSMS();
		}
		
		/// <summary>
		/// Envío de SMS desde el menú contextual
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		void SendSMSToolStripMenuItemClick(object sender, EventArgs e)
		{
			_presenter.SendSMS();
		}
		
		/// <summary>
		/// Al hacer click en la barra de estado: si contiene
		/// un link, navegar al destino
		/// </summary>
		/// <param name="sender">Objeto emisor del evento</param>
		/// <param name="e">Argumentos del evento</param>
		void LStatusClick(object sender, EventArgs e)
		{
			string url = lStatus.Tag.ToString();
			if( lStatus.IsLink && !String.IsNullOrEmpty(url) )
			{
				System.Diagnostics.Process.Start(url);
			}
		}
		#endregion

		

	}
}
