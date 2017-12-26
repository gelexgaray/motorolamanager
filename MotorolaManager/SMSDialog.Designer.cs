/*
 * Creado por SharpDevelop.
 * Usuario: Gorka
 * Fecha: 22/03/2009
 * Hora: 17:40
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
namespace MotorolaManager
{
	partial class SMSDialog
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
			this.tbTo = new System.Windows.Forms.TextBox();
			this.tbBody = new System.Windows.Forms.TextBox();
			this.lTo = new System.Windows.Forms.Label();
			this.lBody = new System.Windows.Forms.Label();
			this.lCharCounter = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.bSend = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.tlpForm.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpForm
			// 
			this.tlpForm.ColumnCount = 3;
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.06849F));
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.9315F));
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
			this.tlpForm.Controls.Add(this.tbTo, 1, 1);
			this.tlpForm.Controls.Add(this.tbBody, 1, 2);
			this.tlpForm.Controls.Add(this.lTo, 0, 1);
			this.tlpForm.Controls.Add(this.lBody, 0, 2);
			this.tlpForm.Controls.Add(this.lCharCounter, 1, 0);
			this.tlpForm.Controls.Add(this.panel1, 1, 3);
			this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpForm.Location = new System.Drawing.Point(0, 0);
			this.tlpForm.Name = "tlpForm";
			this.tlpForm.RowCount = 4;
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.90226F));
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.90226F));
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.81955F));
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
			this.tlpForm.Size = new System.Drawing.Size(292, 266);
			this.tlpForm.TabIndex = 0;
			// 
			// tbTo
			// 
			this.tbTo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbTo.Location = new System.Drawing.Point(41, 27);
			this.tbTo.Name = "tbTo";
			this.tbTo.Size = new System.Drawing.Size(211, 20);
			this.tbTo.TabIndex = 0;
			this.tbTo.TextChanged += new System.EventHandler(this.TbToTextChanged);
			this.tbTo.Validating += new System.ComponentModel.CancelEventHandler(this.TbToValidating);
			// 
			// tbBody
			// 
			this.tbBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbBody.Location = new System.Drawing.Point(41, 51);
			this.tbBody.Multiline = true;
			this.tbBody.Name = "tbBody";
			this.tbBody.Size = new System.Drawing.Size(211, 172);
			this.tbBody.TabIndex = 1;
			this.tbBody.TextChanged += new System.EventHandler(this.TbBodyTextChanged);
			// 
			// lTo
			// 
			this.lTo.Location = new System.Drawing.Point(3, 24);
			this.lTo.Name = "lTo";
			this.lTo.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lTo.Size = new System.Drawing.Size(32, 23);
			this.lTo.TabIndex = 2;
			this.lTo.Text = "To:";
			// 
			// lBody
			// 
			this.lBody.Location = new System.Drawing.Point(3, 48);
			this.lBody.Name = "lBody";
			this.lBody.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.lBody.Size = new System.Drawing.Size(32, 23);
			this.lBody.TabIndex = 3;
			this.lBody.Text = "Body:";
			// 
			// lCharCounter
			// 
			this.lCharCounter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lCharCounter.Location = new System.Drawing.Point(41, 0);
			this.lCharCounter.Name = "lCharCounter";
			this.lCharCounter.Size = new System.Drawing.Size(211, 24);
			this.lCharCounter.TabIndex = 4;
			this.lCharCounter.Text = "160 characters left";
			this.lCharCounter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bSend);
			this.panel1.Controls.Add(this.bCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(38, 226);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(217, 40);
			this.panel1.TabIndex = 5;
			// 
			// bSend
			// 
			this.bSend.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bSend.Enabled = false;
			this.bSend.Location = new System.Drawing.Point(60, 0);
			this.bSend.Name = "bSend";
			this.bSend.Size = new System.Drawing.Size(75, 23);
			this.bSend.TabIndex = 2;
			this.bSend.Text = "Send";
			this.bSend.UseVisualStyleBackColor = true;
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(141, 0);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 23);
			this.bCancel.TabIndex = 3;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// SMSDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.ControlBox = false;
			this.Controls.Add(this.tlpForm);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SMSDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New SMS";
			this.Load += new System.EventHandler(this.SMSDialogLoad);
			this.tlpForm.ResumeLayout(false);
			this.tlpForm.PerformLayout();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bSend;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lCharCounter;
		private System.Windows.Forms.Label lBody;
		private System.Windows.Forms.Label lTo;
		private System.Windows.Forms.TextBox tbBody;
		private System.Windows.Forms.TextBox tbTo;
		private System.Windows.Forms.TableLayoutPanel tlpForm;
	}
}
