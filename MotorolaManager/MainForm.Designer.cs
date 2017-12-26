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

namespace MotorolaManager
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Security",
			"CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
			Justification =
			"Este método sólo se llama desde el constructor sin argumentos. " +
			"No veo forma alguna de que se pueda aprovechar de él un usuario " +
			"malintencionado con acceso a la clase MainForm."
		)]
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.lStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.cbPort = new System.Windows.Forms.ToolStripComboBox();
			this.bRead = new System.Windows.Forms.ToolStripButton();
			this.bWrite = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.bNew = new System.Windows.Forms.ToolStripButton();
			this.bDeleteContact = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.bImportCSV = new System.Windows.Forms.ToolStripButton();
			this.bExportCSV = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.bSMS = new System.Windows.Forms.ToolStripButton();
			this.dgvPhoneBook = new System.Windows.Forms.DataGridView();
			this.Position = new System.Windows.Forms.DataGridViewButtonColumn();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.cIsModified = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Deleted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendSMSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timerStatusMessage = new System.Windows.Forms.Timer(this.components);
			this.statusStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPhoneBook)).BeginInit();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.lStatus});
			this.statusStrip.Location = new System.Drawing.Point(0, 431);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(527, 22);
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip1";
			// 
			// lStatus
			// 
			this.lStatus.Name = "lStatus";
			this.lStatus.Size = new System.Drawing.Size(0, 17);
			this.lStatus.Click += new System.EventHandler(this.LStatusClick);
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.cbPort,
									this.bRead,
									this.bWrite,
									this.toolStripSeparator1,
									this.bNew,
									this.bDeleteContact,
									this.toolStripSeparator2,
									this.bImportCSV,
									this.bExportCSV,
									this.toolStripSeparator3,
									this.bSMS});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(527, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// cbPort
			// 
			this.cbPort.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.cbPort.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbPort.DropDownHeight = 60;
			this.cbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPort.DropDownWidth = 60;
			this.cbPort.IntegralHeight = false;
			this.cbPort.Name = "cbPort";
			this.cbPort.Size = new System.Drawing.Size(75, 25);
			this.cbPort.Sorted = true;
			this.cbPort.SelectedIndexChanged += new System.EventHandler(this.CbPortSelectedIndexChanged);
			// 
			// bRead
			// 
			this.bRead.Image = ((System.Drawing.Image)(resources.GetObject("bRead.Image")));
			this.bRead.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bRead.Name = "bRead";
			this.bRead.Size = new System.Drawing.Size(52, 22);
			this.bRead.Text = "Read";
			this.bRead.Click += new System.EventHandler(this.BReadClick);
			// 
			// bWrite
			// 
			this.bWrite.Enabled = false;
			this.bWrite.Image = ((System.Drawing.Image)(resources.GetObject("bWrite.Image")));
			this.bWrite.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bWrite.Name = "bWrite";
			this.bWrite.Size = new System.Drawing.Size(53, 22);
			this.bWrite.Text = "Write";
			this.bWrite.Click += new System.EventHandler(this.BWriteClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// bNew
			// 
			this.bNew.Enabled = false;
			this.bNew.Image = ((System.Drawing.Image)(resources.GetObject("bNew.Image")));
			this.bNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bNew.Name = "bNew";
			this.bNew.Size = new System.Drawing.Size(46, 22);
			this.bNew.Text = "Add";
			this.bNew.Click += new System.EventHandler(this.BNewClick);
			// 
			// bDeleteContact
			// 
			this.bDeleteContact.Enabled = false;
			this.bDeleteContact.Image = ((System.Drawing.Image)(resources.GetObject("bDeleteContact.Image")));
			this.bDeleteContact.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bDeleteContact.Name = "bDeleteContact";
			this.bDeleteContact.Size = new System.Drawing.Size(66, 22);
			this.bDeleteContact.Text = "Remove";
			this.bDeleteContact.Click += new System.EventHandler(this.BDeleteContactClick);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// bImportCSV
			// 
			this.bImportCSV.Enabled = false;
			this.bImportCSV.Image = ((System.Drawing.Image)(resources.GetObject("bImportCSV.Image")));
			this.bImportCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bImportCSV.Name = "bImportCSV";
			this.bImportCSV.Size = new System.Drawing.Size(59, 22);
			this.bImportCSV.Text = "Import";
			this.bImportCSV.Click += new System.EventHandler(this.BImportCSVClick);
			// 
			// bExportCSV
			// 
			this.bExportCSV.Enabled = false;
			this.bExportCSV.Image = ((System.Drawing.Image)(resources.GetObject("bExportCSV.Image")));
			this.bExportCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bExportCSV.Name = "bExportCSV";
			this.bExportCSV.Size = new System.Drawing.Size(59, 22);
			this.bExportCSV.Text = "Export";
			this.bExportCSV.Click += new System.EventHandler(this.BExportCSVClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// bSMS
			// 
			this.bSMS.Enabled = false;
			this.bSMS.Image = ((System.Drawing.Image)(resources.GetObject("bSMS.Image")));
			this.bSMS.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bSMS.Name = "bSMS";
			this.bSMS.Size = new System.Drawing.Size(47, 22);
			this.bSMS.Text = "SMS";
			this.bSMS.Click += new System.EventHandler(this.BSMSClick);
			// 
			// dgvPhoneBook
			// 
			this.dgvPhoneBook.AllowUserToAddRows = false;
			this.dgvPhoneBook.AllowUserToDeleteRows = false;
			this.dgvPhoneBook.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvPhoneBook.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.dgvPhoneBook.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
									this.Position,
									this.Column1,
									this.Column2,
									this.cIsModified,
									this.Deleted});
			this.dgvPhoneBook.ContextMenuStrip = this.contextMenuStrip;
			this.dgvPhoneBook.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPhoneBook.Location = new System.Drawing.Point(0, 25);
			this.dgvPhoneBook.Name = "dgvPhoneBook";
			this.dgvPhoneBook.RowHeadersVisible = false;
			this.dgvPhoneBook.RowHeadersWidth = 50;
			this.dgvPhoneBook.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvPhoneBook.Size = new System.Drawing.Size(527, 406);
			this.dgvPhoneBook.TabIndex = 2;
			this.dgvPhoneBook.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DgvPhoneBookCellBeginEdit);
			this.dgvPhoneBook.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvPhoneBookColumnHeaderMouseClick);
			this.dgvPhoneBook.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.DgvPhoneBookPreviewKeyDown);
			this.dgvPhoneBook.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvRowLeave);
			this.dgvPhoneBook.CurrentCellChanged += new System.EventHandler(this.DgvPhoneBookSelectionChanged);
			// 
			// Position
			// 
			this.Position.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			this.Position.DefaultCellStyle = dataGridViewCellStyle1;
			this.Position.HeaderText = "Pos";
			this.Position.Name = "Position";
			this.Position.ReadOnly = true;
			this.Position.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Position.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.Position.Width = 48;
			// 
			// Column1
			// 
			this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Column1.FillWeight = 40F;
			this.Column1.HeaderText = "Name";
			this.Column1.MaxInputLength = 14;
			this.Column1.Name = "Column1";
			this.Column1.Width = 160;
			// 
			// Column2
			// 
			this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Column2.FillWeight = 60F;
			this.Column2.HeaderText = "Number";
			this.Column2.MaxInputLength = 40;
			this.Column2.Name = "Column2";
			this.Column2.Width = 300;
			// 
			// cIsModified
			// 
			this.cIsModified.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.cIsModified.HeaderText = "Modify";
			this.cIsModified.Name = "cIsModified";
			this.cIsModified.ReadOnly = true;
			this.cIsModified.Visible = false;
			// 
			// Deleted
			// 
			this.Deleted.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.Deleted.HeaderText = "Delete";
			this.Deleted.Name = "Deleted";
			this.Deleted.Visible = false;
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyToolStripMenuItem,
									this.sendSMSToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(150, 48);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.copyToolStripMenuItem.Text = "Copy number";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
			// 
			// sendSMSToolStripMenuItem
			// 
			this.sendSMSToolStripMenuItem.Name = "sendSMSToolStripMenuItem";
			this.sendSMSToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.sendSMSToolStripMenuItem.Text = "Send SMS";
			this.sendSMSToolStripMenuItem.Click += new System.EventHandler(this.SendSMSToolStripMenuItemClick);
			// 
			// timerStatusMessage
			// 
			this.timerStatusMessage.Interval = 5000;
			this.timerStatusMessage.Tick += new System.EventHandler(this.TimerStatusMessageTick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 453);
			this.Controls.Add(this.dgvPhoneBook);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.statusStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(535, 480);
			this.Name = "MainForm";
			this.Text = "Motorola W375 SIM Phonebook Editor";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPhoneBook)).EndInit();
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem sendSMSToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton bSMS;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.Timer timerStatusMessage;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripButton bExportCSV;
		private System.Windows.Forms.ToolStripButton bImportCSV;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton bDeleteContact;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.DataGridViewCheckBoxColumn Deleted;
		private System.Windows.Forms.DataGridViewCheckBoxColumn cIsModified;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewButtonColumn Position;

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripComboBox cbPort;
		private System.Windows.Forms.DataGridView dgvPhoneBook;
		private System.Windows.Forms.ToolStripButton bRead;
		private System.Windows.Forms.ToolStripButton bWrite;
		private System.Windows.Forms.ToolStripButton bNew;
		private System.Windows.Forms.ToolStripStatusLabel lStatus;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		
	}
}
