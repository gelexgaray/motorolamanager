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
using MotorolaManager.Common;

namespace MotorolaManager.MotorolaIO
{
	/// <summary>
	/// Clase que permite manejar un movil Motorola con soporte para comandos AT por el puerto serie
	/// indicado. Desarrollado para el W375, pero extensible a cualquier telefono movil Motorola
	/// y muy probablemente a cualquier modem GSM.
	/// </summary>
	public class ATInterface : IDisposable
	{
		#region Métodos estáticos para deducir la configuración de la comunicación
		/// <summary>
		/// Obtiene la lista de puertos candidatos a tener conectado el teléfono 
		/// </summary>
		/// <returns>
		/// Lista de puertos, en orden ascendente
		/// </returns>
		public static string [] GetPortNames()
		{
			// Cuidado, que MONO sobre Linux no coge los puertos ACM con el GetPortNames.
			// asi que tenemos que hacer un apañito
			int p = (int)Environment.OSVersion.Platform;
			if (p == 4 || p == 128) {
				// Purtos estándar
				List<string> serial_ports = new List<string>();
				serial_ports.AddRange(SerialPort.GetPortNames());
				
				// Añadimos los puertos ACM
				string[] ttys = System.IO.Directory.GetFiles("/dev/", "ttyACM*");
				serial_ports.AddRange( ttys );
				
				return serial_ports.ToArray();
			}
			else return SerialPort.GetPortNames();

		}
		
		/// <summary>
		/// Método estático que busca el dispositivo a lo largo de los puertos
		/// del sistema, y devuelve el nombre del puerto más probable en el que se
		/// encuentra instalado el dispositivo
		/// </summary>
		/// <returns>Nombre del puerto COM en el que probablemente se encuentra el dispositivo</returns>
		public static string GuessPortName()
		{
			// Obtener todos los puertos com del PC
			string[] names = GetPortNames();

			// El puerto COM al que se conecta el teléfono, se crea dinámicamente.
			// Lo más probable es que sea el último puerto COM disponible en el PC
			if( names.Length > 0)
				return names[names.Length - 1];
			else return string.Empty;
		}

		/// <summary>
		/// Devuelve la velocidad típica para la comunicación con este tipo de dispositivo
		/// </summary>
		/// <returns>Velocidad preferida por el dispositivo</returns>
		public static int GuessSpeed()
		{
			return 115200;
		}

		/// <summary>
		/// Devuelve la paridad típica de este tipo de dispositivos
		/// </summary>
		/// <returns>Paridad por defecto</returns>
		public static Parity GuessParity()
		{
			return Parity.Odd; // Par
		}

		/// <summary>
		/// Infiere los bits de datos típicos para este tipo de dispositivo
		/// </summary>
		/// <returns>Bits de datos por defecto</returns>
		public static int GuessDataBits()
		{
			return 8;
		}

		/// <summary>
		/// Infiere los bits de parada típicos de este tipo de dispositivos
		/// </summary>
		/// <returns>Bits de parada por defecto</returns>
		public static StopBits GuessStopBits()
		{
			return StopBits.One;
		}

		#endregion

		/// <summary>
		/// Código con el que se marcan los números locales en las entradas de la
		/// libreta de teléfonos de la SIM
		/// </summary>
		private const int _localNumberSimCode = 129;

		/// <summary>
		/// Código con el que se marcan los números internacionales en las entradas
		/// de la libreta de teléfonos de la SIM
		/// </summary>
		private const int _internationalNumberSimCode = 145;

		/// <summary>
		/// Puerto serie usado durante las comunicaciones
		/// </summary>
		private SerialPort _sp;

		/// <summary>
		/// El menor indice de acceso a la agenda de la SIM disponible en la tarjeta insertada
		/// </summary>
		public int LowerPhonebookIndex
		{
			get { return _lowerPhonebookIndex; }
		}
		private int _lowerPhonebookIndex;

		/// <summary>
		/// El mayor indice de acceso a la agenda de la SIM disponible en la tarjeta insertada
		/// </summary>
		public int HigherPhonebookIndex
		{
			get { return _higherPhonebookIndex; }
		}
		private int _higherPhonebookIndex;

		// Parámetros de la comunicación
		private string _portName;
		private int _speed;
		private Parity _parity;
		private int _dataBits;
		private StopBits _stopBits;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="portName">Nombre del puerto (por ejemplo, "COM3")</param>
		/// <param name="speed">Velocidad de transmisión de datos. Tipicamente 115200</param>
		/// <param name="parity">Bit de paridad. Generalmente se usa paridad Par (Even)</param>
		/// <param name="dataBits">Bits de datos. Típicamente 8</param>
		/// <param name="stopBits">Bits de parada. Tipicamente se usa 1 (StopBits.One)</param>
		public ATInterface(string portName, int speed, Parity parity, int dataBits, StopBits stopBits)
		{
			// Cachear los parámetros de la comunicación
			_portName = portName;
			_speed = speed;
			_parity = parity;
			_dataBits = dataBits;
			_stopBits = stopBits;

			// Construir el puerto serie adecuado segun la configuracion
			BuildNewSerialPortObject();

			// Abrir el puerto
			try
			{
				_sp.Open();
			}
			catch( System.IO.IOException ex )
			{
				throw new TypeInitializationException( "MotorolaManager.MotorolaIO.ATInterface", ex);
			}

			// Inicializar la comunicacion
			bool ok = Initialize();
			if (!ok)
			{
				_sp.Close();
				_sp.Dispose();
				throw new TypeInitializationException( "MotorolaManager.MotorolaIO.ATInterface", null);
			}

			// Seleccionar la agenda de la SIM
			SelectSIMPhoneBook();
			if (0 == this._higherPhonebookIndex)
			{
				_sp.Close();
				_sp.Dispose();
				throw new TypeInitializationException( "MotorolaManager.MotorolaIO.ATInterface", null);
			}

			// Cerrar el puerto
			_sp.Close();

		}

		/// <summary>
		/// Crea un nuevo objeto SerialPort, par su uso interno por parte de la clase
		/// </summary>
		private void BuildNewSerialPortObject()
		{
			if (null != _sp)
			{
				if (_sp.IsOpen) _sp.Close();
			}

			_sp = new SerialPort(_portName, _speed, _parity, _dataBits, _stopBits);

			// Establecer timeouts
			_sp.ReadTimeout = 500;
			_sp.WriteTimeout = 500;

			// Establecer modelo de comunicacion full-duplex
			_sp.Handshake = Handshake.None;
			_sp.DtrEnable = true;
		}


		/// <summary>
		/// Inicializa la comunicacion con el dispositivo
		/// </summary>
		/// <returns>Booleano que indica si el dispositivo se ha inicializado correctamente</returns>
		private bool Initialize()
		{
			// Vaciar el buffer de lectura/escritura
			_sp.DiscardInBuffer();
			_sp.DiscardOutBuffer();

			// Ahora mandar un comando ATE0 para eliminar el eco local
			string response;
			SendWait("ATE0", out response);

			// Por ultimo, comprobar que el equipo responde con OK a un comando AT
			bool bResult = SendWait("AT", out response);

			return bResult;
		}

		/// <summary>
		/// Abre el puerto de comunicaciones.
		/// Esta operación se puede ejecutar sobre un puerto ya abierto
		/// </summary>
		public void Open()
		{
			if( !_sp.IsOpen )	_sp.Open();
		}

		/// <summary>
		/// Cierra el puerto de comunicaciones.
		/// Esta operación se puede ejecutar sobre un puerto ya cerrado
		/// </summary>
		public void Close()
		{
			if( _sp.IsOpen )
			{
				// A veces, el W375 se queda medio tonto (sobre todo al mandar sms).
				// Si en este estado cerramos el puerto, ya no se puede recuperar hasta
				// apagar y volver a encender el teléfono :-(
				// Mandamos "Enter" hasta que el teléfono devuelva un OK, y asi
				// nos aseguramos de cerrar el puerto siempre con el teléfono "consciente"
				string response;
				int maxRetries = 10;
				while( !SendWait(string.Empty, out response) && maxRetries > 0) maxRetries--;
				
				// Cerra el puerto
				_sp.Close();
			}
		}

		/// <summary>
		/// Selecciona la lista de telefonos de la SIM
		/// </summary>
		/// <returns>Capacidad de la agenda de la SIM</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Naming",
			"CA1709:IdentifiersShouldBeCasedCorrectly",
			Justification = "SIM es un acrónimo universalmente aceptado"
		)]
		public int SelectSIMPhoneBook()
		{
			return SelectPhoneBook("MT");
		}

		/// <summary>
		/// Selecciona la lista de telefonos especificada por medio de su codigo GSM
		/// </summary>
		/// <returns>Capacidad de la agenda seleccionada</returns>
		private int SelectPhoneBook(string phonebookCode)
		{
			string response;

			bool bResult =
				SendWait(
					String.Format(CultureInfo.InvariantCulture, "AT+CPBS=\"{0}\"", phonebookCode),
					out response);

			if (bResult)
			{
				SendWait("AT+CPBR=?", out response);

				// Parsear la respuesta un busca del numero de telefonos disponibles
				Regex r = new Regex(@"\+CPBR: \((\d+)-(\d+)\).*");
				Match m = r.Match(response);
				if (m.Success)
				{
					_lowerPhonebookIndex = Int32.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
					_higherPhonebookIndex = Int32.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
				}
			}
			return _higherPhonebookIndex - _lowerPhonebookIndex + 1;
		}

		/// <summary>
		/// Borra una entrada de la lista de teléfonos de la SIM
		/// </summary>
		/// <param name="number">Entrada a borrar</param>
		/// <returns>True, si el comando se ha ejecutado correctamente</returns>
		public bool RemoveEntry(int number)
		{
			string response;
			return SendWait(
				String.Format(CultureInfo.InvariantCulture, "{0}={1}", "AT+CPBW", number),
				out response);
		}

		/// <summary>
		/// Añade una entrada a la lista de teléfonos de la SIM
		/// Si el PhonebookEntry tiene una posición asociada, lo añade en esa posición,
		/// sobreescribiendo cualquier entrada previamente almacenada. Si el PhonebookEntry
		/// no tiene posición asociada, añade la entrada en la primera posición libre disponible
		/// </summary>
		/// <param name="pbEntry">Entrada a añadir</param>
		/// <returns>True, si el comando se ha ejecutado correctamente</returns>
		public bool WritePhonebookEntry(PhonebookEntry pbEntry)
		{
			string command;
			string response;
			int phoneType;

			// Determinar el tipo de teléfono:
			// Teléfono internacional (va marcado con el tipo 145)
			// Teléfono local (va marcado con el tipo 129)
			phoneType = pbEntry.IsInternational ? _internationalNumberSimCode : _localNumberSimCode;

			// Construir el comando
			command = String.Format(CultureInfo.InvariantCulture,
			                        @"{0}={1},""{2}"",{3},""{4}""",
			                        "AT+CPBW",
			                        (null == pbEntry.Number) ? String.Empty : pbEntry.Position.ToString(),
			                        pbEntry.Number,
			                        phoneType,
			                        pbEntry.Name
			                       );

			// Enviar el comando por la línea serie
			return SendWait(command, out response);
		}

		/// <summary>
		/// Lee la entrada de la posición indicada
		/// </summary>
		/// <param name="position">Posición a leer</param>
		/// <returns>Entrada solicitada. Devuelve Null si la entrada no existe </returns>
		public PhonebookEntry ReadPhonebookEntry(int position)
		{
			PhonebookEntry pbEntry = null;

			string response;
			bool ok =
				SendWait(
					String.Format(CultureInfo.InvariantCulture, "{0}={1}", "AT+CPBR", position),
					out response);

			// Interpretar la respuesta
			if (ok)
			{
				pbEntry = ATInterface.ParseCPBRResponse(response);
			}
			return pbEntry;
		}

		/// <summary>
		/// Método que parsea una línea de respuesta a un comando CPBR
		/// </summary>
		/// <param name="line">Linea de respuesta al comando</param>
		/// <returns>Una entrada de libreta de direcciones, o NULL si la linea no contiene
		/// una respuesta a un comando CPBR válida</returns>
		private static PhonebookEntry ParseCPBRResponse(string line)
		{
			PhonebookEntry pbEntry = null;
			Regex r = new Regex(@"\+CPBR:\s+(\d+)\s*,\s*\""(.*)\""\s*,\s*(\d+)\s*,\s*\""(.*)\""");
			Match m = r.Match(line);
			if (m.Success)
			{
				string phoneNumber = m.Groups[2].Value;

				// Determinar si el número está en formato internacional
				int entryCode = Int32.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
				if (_internationalNumberSimCode == entryCode)
				{
					phoneNumber = String.Format(CultureInfo.InvariantCulture, "+{0}", phoneNumber);
				}

				// Construir el objeto
				pbEntry = new PhonebookEntry(
					int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), // Posición
					m.Groups[4].Value, // Nombre
					phoneNumber); // Teléfono
			}
			return pbEntry;
		}

		/// <summary>
		/// Lee todas las entradas de la agenda seleccionada
		/// </summary>
		/// <returns>Lista de entradas </returns>
		public IList<PhonebookEntry> ReadAllPhonebookEntries()
		{
			string response;
			SendWait(
				String.Format(CultureInfo.InvariantCulture, "{0}={1},{2}", "AT+CPBR", _lowerPhonebookIndex, _higherPhonebookIndex),
				out response);

			// Partir la respuesta en líneas y parsear cada una de ellas
			List<PhonebookEntry> pbList = new List<PhonebookEntry>();
			string[] responseLines = response.Split('\n');
			foreach (string line in responseLines)
			{
				PhonebookEntry pbEntry = ATInterface.ParseCPBRResponse(line);
				if (null != pbEntry) pbList.Add(pbEntry);
			}

			return pbList;
		}
		
		/// <summary>
		/// Envía un SMS al contacto indicado
		/// </summary>
		/// <param name="pbEntry">Contacto al que se le quiere enviar un SMS</param>
		/// <param name="smsBody">Cuerpo del mensaje</param>
		/// <returns>True, si el comando se ha ejecutado correctamente. False en caso contrario.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SMS")]
		public bool SendSMS( PhonebookEntry pbEntry, string smsBody )
		{
			string command;
			string response;
			const char ctrlZ = (char)26;
			
			// Preparar el teléfono para el envío de SMSs
			bool isOk = SendWait("AT+CMGF=1", out response);

			if( isOk )
			{
				// Construir el comando
				command = String.Format(CultureInfo.InvariantCulture,
				                        @"AT+CMGS=""{0}""",
				                        pbEntry.Number
				                       );

				// Enviar el comando por la línea serie
				Send(command);
				
				// Enviar el cuerpo del mensaje cuando el terminal esté preparado
				// El mensaje termina en Ctrl + Z
				command = String.Format(CultureInfo.InvariantCulture,
				                        "{0}{1}",
				                        smsBody,
				                        ctrlZ);
				Send(command);
				
				// Esto simplemente envía un avance de línea y espera respuesta
				isOk = SendWait(string.Empty, out response); 
			}
			return isOk;
		}

		/// <summary>
		/// Objeto estático para sincronizar el envío de comandos por el puerto serie,
		/// y hacer esta clase Thread Safe
		/// </summary>
		private static object _syncRoot = new object();

		/// <summary>
		/// Envia un comando AT al telefono movil y no espera la respuesta
		/// </summary>
		/// <param name="command">Comando AT</param>
		private void Send(string command)
		{
			// Enviar el comando, con un caracter de retorno y sin caracter de avance de linea
			lock (_syncRoot)
			{
				_sp.Write(String.Format(CultureInfo.InvariantCulture, "{0}\r", command));

				// Bloquear hasta que vuelva a estar disponiblr
				try
				{
					while (_sp.ReadChar() >= 0 );
				}
				// Cuando no queda nada que leer, continuamos.
				catch (System.TimeoutException)
				{
				}

			} // fin del lock (finaliza la comunicación serie)

		}

		/// <summary>
		/// Envia un comando AT al telefono movil y espera la respuesta
		/// </summary>
		/// <param name="command">Comando AT</param>
		/// <param name="response">Respuesta al comando</param>
		/// <returns>True, si el comando se ha ejecutado correctamente y false en caso contrario</returns>
		private bool SendWait(string command, out string response)
		{
			StringBuilder sbResponse = new StringBuilder();
			bool error = false;

			// Enviar el comando, con un caracter de retorno y sin caracter de avance de linea
			lock (_syncRoot)
			{
				_sp.Write(String.Format(CultureInfo.InvariantCulture, "{0}\r", command));


				// Esperar la respuesta
				try
				{
					// Recibir la respuesta linea a linea, hasta encontrar una linea que comience con OK o ERROR
					bool eos = false;
					string line;
					while (!eos)
					{
						// Leer la línea
						line = _sp.ReadLine();
						sbResponse.AppendLine(line.TrimEnd('\r'));

						// Comprobar si ha finalizado la respuesta
						error = line.StartsWith("ERROR", StringComparison.CurrentCultureIgnoreCase);
						eos = error
							|| line.StartsWith("OK", StringComparison.CurrentCultureIgnoreCase);
					}
				}
				// Control de errores: se corta la respuesta
				catch (System.TimeoutException)
				{
					error = true;
				}

			} // fin del lock (finaliza la comunicación serie)

			// Valores de retorno
			response = sbResponse.ToString(); // String de retorno
			return !error; // Lectura OK?
		}

		#region Idisposable
		/// <summary>
		/// Interfaz IDisposable
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Cierra el puerto al destruir la clase
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose( bool disposing )
		{
			if( disposing)
			{
				Close();
			}
		}
		
		/// <summary>
		/// Destructor
		/// </summary>
		~ATInterface()
		{
			Dispose(false);
		}
		#endregion
		
	}
}
