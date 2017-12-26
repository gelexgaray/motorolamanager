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

// Descomentar el siguiente define para que el test envíe mensajes sms de test
//#define _TESTSMS
		
using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO.Ports;
using MotorolaManager.MotorolaIO;
using MotorolaManager.Common;

namespace MotorolaManager.MotorolaIO.Test
{
	/// <summary>
	/// Tests unitarios para la clase de interfaz por comandos AT
	/// con teléfonos Motorola
	///</summary>
	[TestFixture]
	public class ATInterfaceTest
	{
		// Constantes para los test de envío de SMS: configurar para
		// un número de teléfono y cuerpo de mensaje que no tenga coste.
		const string _smsTestPhoneNumber = "470";
		const string _smsTestBody = "INFO DOMINGOS";
		
		// Variables que almacenan los parámetros de la conexión
		private static string _portName;
		private static int _speed;
		private static Parity _parity;
		private static int _dataBits;
		private static StopBits _stopBits;

		/// <summary>
		/// Entrada de la SIM que se usará para hacer las pruebas
		/// </summary>
		private static int _testPhonebookNumber;

		/// <summary>
		/// Objeto que se usa a lo largo de los tests
		/// </summary>
		private static ATInterface _atInterface;

		/// <summary>
		/// Inicialización de la clase
		/// </summary>
		[TestFixtureSetUp]
		public void ClassInitialize()
		{
			// Deducir los parámetros de la conexión
			_portName = ATInterface.GuessPortName();
			_speed = ATInterface.GuessSpeed();
			_parity = ATInterface.GuessParity();
			_dataBits = ATInterface.GuessDataBits();
			_stopBits = ATInterface.GuessStopBits();

			// Instanciar el objeto a probar
			_atInterface = new ATInterface(_portName, _speed, _parity, _dataBits, _stopBits);

			// Obtener la última posición disponible en la memoria: será la posición usada para las pruebas
			// (para minimizar la probabilidad de machacar una posición válida)
			_testPhonebookNumber = _atInterface.HigherPhonebookIndex;
		}

		/// <summary>
		/// Limpieza
		/// </summary>
		[TestFixtureTearDown]
		public void ClassCleanup()
		{
			// Borrar la entrada que se usa en los tests y destruir el objeto
			_atInterface.Open();
			_atInterface.RemoveEntry(_testPhonebookNumber);
			_atInterface.Close();
			_atInterface.Dispose();
		}

		/// <summary>
		/// Inicialización previa a cada test
		/// </summary>
		[SetUp]
		public void TestInitialize()
		{
			_atInterface.Open();
		}
		
		/// <summary>
		/// Finalizador posterior a cada test
		/// </summary>
		[TearDown]
		public void TestCleanup()
		{
			_atInterface.Close();
		}

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
		/// A test for ATInterface (string, int, Parity, int)
		///</summary>
		[Test]
		public void ConstructorTest()
		{
			Assert.IsNotNull(_atInterface);
		}

		/// <summary>
		/// A test for ATInterface (string, int, Parity, int)
		///</summary>
		[Test]
		[ExpectedException(typeof(TypeInitializationException))]
		public void ConstructorExceptionTest()
		{
			// Coger un número de puerto no válido
			string invalidPortName = "COM1";
			if (_portName == invalidPortName) invalidPortName = "COM2";

			// Probar la inicialización
			ATInterface atInterface = new ATInterface(invalidPortName, _speed, _parity, _dataBits, StopBits.One);
			atInterface.Dispose();
		}

		/// <summary>
		///A test for SelectSIMPhoneBook ()
		///</summary>
		[Test]
		public void SelectSIMPhoneBookTest()
		{
			int phonebookEntries = _atInterface.SelectSIMPhoneBook();
			Assert.IsTrue(phonebookEntries > 0, "No se ha seleccionar la lista de contactos de la SIM");
		}

		/// <summary>
		///A test for HigherPhonebookIndex
		///</summary>
		[Test]
		public void HigherPhonebookIndexTest()
		{
			int phonebookIndex = _atInterface.HigherPhonebookIndex;
			Assert.IsTrue(phonebookIndex > 0, "No se ha seleccionar la lista de contactos de la SIM");
		}

		/// <summary>
		///A test for LowerPhonebookIndex
		///</summary>
		[Test]
		public void LowerPhonebookIndexTest()
		{
			int phonebookIndex = _atInterface.LowerPhonebookIndex;
			Assert.IsTrue(phonebookIndex > 0, "No se ha seleccionar la lista de contactos de la SIM");
		}

		/// <summary>
		///A test for AddEntry (int, string, string)
		///</summary>
		[Test]
		public void WriteEntryLocalNumberTest()
		{
			PhonebookEntry pbEntry = new PhonebookEntry(_testPhonebookNumber, "UTContact", "34000000");
			bool result = _atInterface.WritePhonebookEntry(pbEntry);
			Assert.IsTrue(result);
		}

		/// <summary>
		///A test for AddEntry (int, string, string)
		///</summary>
		[Test]
		public void WriteEntryInternationalNumberTest()
		{
			PhonebookEntry pbEntry = new PhonebookEntry(_testPhonebookNumber, "UTContact", "+34000000");
			bool result = _atInterface.WritePhonebookEntry(pbEntry);
			Assert.IsTrue(result);
		}

		/// <summary>
		///A test for RemoveEntry (int)
		///</summary>
		[Test]
		public void RemoveEntryTest()
		{
			WriteEntryLocalNumberTest();
			bool result = _atInterface.RemoveEntry(_testPhonebookNumber);
			Assert.IsTrue(result);
		}

		/// <summary>
		///A test for ReadEntry (int)
		///</summary>
		[Test]
		public void ReadEntryTest()
		{
			WriteEntryLocalNumberTest();
			
			// Leer el valor previamente escrito
			PhonebookEntry pbEntryOut = _atInterface.ReadPhonebookEntry( _atInterface.HigherPhonebookIndex );

			// Debe contener el contacto válido
			Assert.IsNotNull(pbEntryOut);
			Assert.AreEqual( false, pbEntryOut.IsInternational);
			Assert.AreEqual("UTContact", pbEntryOut.Name);
		}
		
		/// <summary>
		/// A test for ReadEntry (int)
		/// Número de teléfono internacional
		///</summary>
		[Test]
		public void ReadEntryIntNumberTest()
		{
			WriteEntryLocalNumberTest();
			
			// Leer toda la agenda
			IList<PhonebookEntry> result = _atInterface.ReadAllPhonebookEntries();
			
			// Debe contener el contacto válido
			Assert.IsNotNull(result);
			Assert.IsTrue( result.Count > 0 );
		}
		
		/// <summary>
		/// Rest para ReadAllPhonebookEntries
		/// Número de teléfono internacional
		///</summary>
		[Test]
		public void ReadAllPhonebookEntriesTest()
		{
			WriteEntryInternationalNumberTest();
			
			// Leer el valor previamente escrito
			PhonebookEntry pbEntryOut = _atInterface.ReadPhonebookEntry( _atInterface.HigherPhonebookIndex );

			// Debe contener el contacto válido
			Assert.IsNotNull(pbEntryOut);
			Assert.AreEqual( true, pbEntryOut.IsInternational);
			Assert.AreEqual("UTContact", pbEntryOut.Name);
		}

		/// <summary>
		///A test for GuessDataBits ()
		///</summary>
		[Test]
		public void GuessDataBitsTest()
		{
			int actual = MotorolaManager.MotorolaIO.ATInterface.GuessDataBits();
			Assert.AreEqual(_dataBits, actual);
		}

		/// <summary>
		///A test for GuessParity ()
		///</summary>
		[Test]
		public void GuessParityTest()
		{
			Parity actual = MotorolaManager.MotorolaIO.ATInterface.GuessParity();
			Assert.AreEqual(_parity, actual);
		}

		/// <summary>
		///A test for GuessPortName ()
		///</summary>
		[Test]
		public void GuessPortNameTest()
		{
			string actual = MotorolaManager.MotorolaIO.ATInterface.GuessPortName();
			Assert.AreEqual(_portName, actual);
		}

		/// <summary>
		///A test for GuessSpeed ()
		///</summary>
		[Test]
		public void GuessSpeedTest()
		{
			int actual = MotorolaManager.MotorolaIO.ATInterface.GuessSpeed();
			Assert.AreEqual(_speed, actual);
		}

		/// <summary>
		///A test for GuessStopBits ()
		///</summary>
		[Test]
		public void GuessStopBitsTest()
		{
			StopBits actual = MotorolaManager.MotorolaIO.ATInterface.GuessStopBits();
			Assert.AreEqual(_stopBits, actual);
		}
		
		/// <summary>
		/// Testea la apertura sobre un puerto ya abierto
		///</summary>
		[Test]
		public void OpenOnOpenTest()
		{
			_atInterface.Open();
		}
		
		/// <summary>
		/// Testea el cierre sobre un puerto ya cerrado
		///</summary>
		[Test]
		public void CloseOnCloseTest()
		{
			_atInterface.Close();
		}
		
		/// <summary>
		/// Testea el envío de SMSs
		///</summary>
		[Test]
		public void SendSMSTest()
		{
			#if _TESTSMS
			
				PhonebookEntry pbEntry = new PhonebookEntry("UTContact", _smsTestPhoneNumber);
				bool response = _atInterface.SendSMS(pbEntry, _smsTestBody);
				Assert.IsTrue(response);
				
			#else
			
				Assert.Ignore(
					"El test no está configurado para la prueba de envio de SMSs. "
					+ "Establezca las constantes de configuración de envio de SMSs"
				);
			#endif
			
		}
	}
}
